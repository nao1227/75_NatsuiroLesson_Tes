# 学習ログ: AbstractOsawari本物化、OnClick〜OsawariEvent発火の検証(進行中)

## 今回のセッションの目的

`OnClick`の先、本物のサブクラス(`AbstractOsawari` / `OsawariHead`)の中身を理解し、
「`OnFirstClick()`経由で、`OnTouchEvents`の中の1つのイベント(`ScriptExecuteEvent`)が、
実際に発火(`InvokeCore`が呼ばれる)することを確認する」ことをゴールとした。

## AbstractOsawari と OsawariEvent の役割の違い(重要な理解)

- `AbstractOsawari`(例: OsawariHead) = 「体のパーツ」そのもの。触られた時の基本動作(移動・揺れなど)を担当
- `OsawariEvent`(例: ScriptExecuteEvent) = そのパーツに触れた時に起きる「できごと」。演出・フラグ管理などを担当
- `AbstractOsawari`は `OnTouchEvents` / `TriggeredEvents` / `OnMouseUpEvents` という3種類の `List<OsawariEvent>` を持ち物として保有している
- 「触れているかどうか」の判定は AbstractOsawari側、「触れた結果何が起きるか」の判定と実行は OsawariEvent側、という役割分担

```csharp
protected void OnFirstClick()
{
    _manager.AddFirstStimulus(StimulusIncrementFirstImpact);
    foreach (OsawariEvent onTouchEvent in OnTouchEvents)
    {
        if (onTouchEvent.IsFullfillCondition(_manager.TemporaryStatus, _conditions))
        {
            onTouchEvent.InvokeEvent(_manager.TemporaryStatus, _conditions).Forget();
        }
    }
    ...
}
```

## 重要な発見: スタブ戦略の限界(今回最大の教訓)

### 何が起きたか

今まで `namespace Stubs` で本物のクラスと完全に切り離す戦略を取ってきたが、
本物の `AbstractOsawari.cs`(abstract class)を導入した際、大量の型不一致エラーが発生した。

```
error CS0029: Cannot implicitly convert type 'List<Stubs.AbstractOsawari>' to 'List<AbstractOsawari>'
```

### 原因

`AbstractOsawari` は「継承階層の中心」「多数のファイルから直接参照される背骨的な型」だった。
`Stubs.AbstractOsawari`(スタブ)と本物の`AbstractOsawari`が、名前は同じでも別の型として扱われ、
本物同士(OsawariManager・OsawariHead・IInputTrigger)の連携が壊れた。

### 教訓・今後の判断基準

新しいクラスを実験に導入する前に、以下をチェックする:
1. `abstract class` か? → Yesなら要注意(継承階層の中心=背骨の可能性)
2. 他のファイルで5回以上、型として登場するか? → Yesなら要注意
→ どちらかに該当したら、スタブ化せず最初から本物を使う方針にする
→ 該当しなければ、これまで通りスタブ化してOK(Scene, HandManagerのような「末端の部品」は問題なかった)

このプロジェクトで背骨だったもの: `AbstractOsawari`
末端で問題なかったもの: `Scene`, `HandManager`, `ContextManager` など

## 今回実施した大規模な修正作業(Stubs.AbstractOsawari → 本物AbstractOsawariへの統一)

### 1. AbstractOsawari.cs / OsawariHead.cs のインポート整理
- `using Paidia.Utils;` は実験用プロジェクトに存在しないため削除
- `using Paidia.satsuki1;` は既存の名前空間なので残す(削除しないこと、混同注意)
- 両ファイルに `using Stubs;` を追加

### 2. StubInputTriggerの無効化
本物のAbstractOsawariとの型不一致が解決できないため、`StubInputTrigger.cs`全体をコメントアウトして無効化。
今回のテストではInputManagerが直接本物のOsawariManagerを使うため、StubInputTrigger自体は不要だった。

### 3. OsawariPiston / OsawariKiss / OsawariWithoutHand / OsawariDoublehanded / OsawariPants を
`namespace Stubs` の外(グローバル)に移動し、本物のAbstractOsawariを継承する形に変更。
6つの抽象メソッド(InitializeParams, AutoAnimation, UpdateWhileNotClicked, UpdateParamsCore,
GetHandParamIndex, OnLateUpdate)を、それぞれ空実装で追加する必要があった。

### 4. OsawariEvent スタブも同様に `namespace Stubs` の外に移動
`IsInvoked`, `Cancel()`, `Initialize(AbstractOsawari parent)` を追加。

### 5. HandManager, ContextOsawariTargetList など、Stubs名前空間内から
本物のAbstractOsawariを参照する箇所は `global::AbstractOsawari` という記法で明示的に指定。
(namespace Stubsの中で単に`AbstractOsawari`と書くとStubs.AbstractOsawariと解釈されてしまうため)

### 6. ParameterValue, TemporaryStatus, FeelingsClass にメソッド追加
- ParameterValue: `Update(float)`, `IsAlmostZero()`
- TemporaryStatus: `AddExciteValue(int)`
- FeelingsClass: `AddAtomosphere(int)`

### 7. 新規スタブ作成
- `HandType`(enum), `Hand`(HandType, IsGrabbing持ち), `ParameterName`(enum), `ParameterDictionary`,
  `HandParamValue`, `SpeedRange`, `OsawariBlocker`, `OsawariConditions`, `FPSChecker`, `Easing`(static),
  `PhysicsCalculater`, `FaceController`, `TimeManager`
- `SerialEvent`, `HScene3ModeChangeEvent`, `DetailAnimationEvent`(いずれも空のOsawariEvent派生)

### 8. OsawariManagerTester.cs の修正
ダミーのAbstractOsawariインスタンス生成(abstract classなのでnew不可)をやめ、
本物の OsawariHead を GetComponent 経由で取得して使う方式に変更。

```csharp
var drawables = TargetOsawariManager.Model.Drawables;
var osawariHead = TargetOsawariManager.GetComponent<OsawariHead>();
osawariHead.TouchableMeshs = drawables;
var targetList = new System.Collections.Generic.List<AbstractOsawari> { osawariHead };
TargetOsawariManager.ContextOsawariTargets = new Stubs.ContextOsawariTargetList();
TargetOsawariManager.ContextOsawariTargets.RegisterTargets(targetList);
```

事前準備として、OsawariManagerと同じGameObjectに `Add Component` で `OsawariHead` をアタッチしておく必要がある。

## 現在の到達点(コンパイルは通っている)

```
Raycastヒット数: 1        ← モデルへの当たり判定は成功
osawariFromDrawable は null か: False   ← GetOsawariFromDrawableでOsawariHeadが見つかっている
```

しかし、その先で `MouseOn: Osawari` にならず `MouseOn: None`(MoveCamera)のままになっている。
`GetConstraints()` / `CanTouchMesh()` のどちらかで弾かれている可能性が高い。

## 未解決の問題(次回、ここから再開)

`InputManager.cs` の `IsMouseOnOsawariParts` に、以下のデバッグログを追加済みのはず(要確認):

```csharp
AbstractOsawari osawariFromDrawable = _manager.GetOsawariFromDrawable(cubismRaycastHit.Drawable);
Debug.Log("osawariFromDrawable は null か: " + (osawariFromDrawable == null));
if (osawariFromDrawable != null)
{
    Debug.Log("GetConstraints: " + osawariFromDrawable.GetConstraints() + ", CanTouchMesh: " + osawariFromDrawable.CanTouchMesh(cubismRaycastHit.Drawable));
}
```

このログが実際に出力されていない(反映されていない可能性がある)。次回、まず以下を確認する:

1. 上記のログが `InputManager.cs` に正しく追加されているか再確認する
2. `AbstractOsawari.GetConstraints()`(本物)の中身に直接ログを仕込み、呼ばれているか確認する

```csharp
// AbstractOsawari.cs の GetConstraints() 内、先頭に追加する候補
public virtual bool GetConstraints()
{
    UnityEngine.Debug.Log("GetConstraints が呼ばれた");
    ...
```

3. `GetConstraints()` の中身に、`Blockers`(OsawariBlockerのList)が絡む処理がある。
   `OsawariHead`(本物)側で `Blockers` フィールドが初期化されていない(null)可能性があり、
   `NullReferenceException` が握りつぶされている、あるいは何らかの理由で `false` を返している疑いがある。
   `Blockers` を空リストで初期化する対応が必要になるかもしれない。

## Git運用メモ(このセッションでは未実施)
- 現状かなり大規模な変更(StubInputTrigger無効化、複数ファイルのnamespace変更)を行っているため、
  区切りの良いところでコミットしておくことを推奨。

## 全体の依存関係(このセッションで新たに実験用プロジェクトに導入したファイル)

```
AbstractOsawari.cs(本物、abstract class) ← 背骨、直接使うべきだった型
OsawariHead.cs(本物、AbstractOsawariの派生)
OsawariEvent.cs(スタブだが、Stubs名前空間の外に出して運用)
ScriptExecuteEvent.cs(本物、OsawariEventの派生、UnityEventを呼ぶだけのシンプルな実装)
```

## 次回の進め方(提案)

1. `GetConstraints()` に直接ログを仕込み、呼ばれているか・何が返っているか確認する
2. `Blockers` フィールドの初期化漏れを疑い、`OsawariHead`(またはAbstractOsawari)側で
   `Blockers = new List<OsawariBlocker>();` のような初期化コードが必要か確認する
3. `MouseOn: Osawari` に到達したら、`OnFirstClick()` → `OnTouchEvents` の発火まで確認する
   (ただし `OnTouchEvents` 自体も、OsawariHeadにまだ何も登録していない可能性があるため、
   テストコード側で `OsawariHead.OnTouchEvents` にダミーの `ScriptExecuteEvent` を
   1つ追加する作業が別途必要になる見込み)# 学習ログ: AbstractOsawari本物化、OnClick〜OsawariEvent発火の検証(進行中)

## 今回のセッションの目的

`OnClick`の先、本物のサブクラス(`AbstractOsawari` / `OsawariHead`)の中身を理解し、
「`OnFirstClick()`経由で、`OnTouchEvents`の中の1つのイベント(`ScriptExecuteEvent`)が、
実際に発火(`InvokeCore`が呼ばれる)することを確認する」ことをゴールとした。

## AbstractOsawari と OsawariEvent の役割の違い(重要な理解)

- `AbstractOsawari`(例: OsawariHead) = 「体のパーツ」そのもの。触られた時の基本動作(移動・揺れなど)を担当
- `OsawariEvent`(例: ScriptExecuteEvent) = そのパーツに触れた時に起きる「できごと」。演出・フラグ管理などを担当
- `AbstractOsawari`は `OnTouchEvents` / `TriggeredEvents` / `OnMouseUpEvents` という3種類の `List<OsawariEvent>` を持ち物として保有している
- 「触れているかどうか」の判定は AbstractOsawari側、「触れた結果何が起きるか」の判定と実行は OsawariEvent側、という役割分担

```csharp
protected void OnFirstClick()
{
    _manager.AddFirstStimulus(StimulusIncrementFirstImpact);
    foreach (OsawariEvent onTouchEvent in OnTouchEvents)
    {
        if (onTouchEvent.IsFullfillCondition(_manager.TemporaryStatus, _conditions))
        {
            onTouchEvent.InvokeEvent(_manager.TemporaryStatus, _conditions).Forget();
        }
    }
    ...
}
```

## 重要な発見: スタブ戦略の限界(今回最大の教訓)

### 何が起きたか

今まで `namespace Stubs` で本物のクラスと完全に切り離す戦略を取ってきたが、
本物の `AbstractOsawari.cs`(abstract class)を導入した際、大量の型不一致エラーが発生した。

```
error CS0029: Cannot implicitly convert type 'List<Stubs.AbstractOsawari>' to 'List<AbstractOsawari>'
```

### 原因

`AbstractOsawari` は「継承階層の中心」「多数のファイルから直接参照される背骨的な型」だった。
`Stubs.AbstractOsawari`(スタブ)と本物の`AbstractOsawari`が、名前は同じでも別の型として扱われ、
本物同士(OsawariManager・OsawariHead・IInputTrigger)の連携が壊れた。

### 教訓・今後の判断基準

新しいクラスを実験に導入する前に、以下をチェックする:
1. `abstract class` か? → Yesなら要注意(継承階層の中心=背骨の可能性)
2. 他のファイルで5回以上、型として登場するか? → Yesなら要注意
→ どちらかに該当したら、スタブ化せず最初から本物を使う方針にする
→ 該当しなければ、これまで通りスタブ化してOK(Scene, HandManagerのような「末端の部品」は問題なかった)

このプロジェクトで背骨だったもの: `AbstractOsawari`
末端で問題なかったもの: `Scene`, `HandManager`, `ContextManager` など

## 今回実施した大規模な修正作業(Stubs.AbstractOsawari → 本物AbstractOsawariへの統一)

### 1. AbstractOsawari.cs / OsawariHead.cs のインポート整理
- `using Paidia.Utils;` は実験用プロジェクトに存在しないため削除
- `using Paidia.satsuki1;` は既存の名前空間なので残す(削除しないこと、混同注意)
- 両ファイルに `using Stubs;` を追加

### 2. StubInputTriggerの無効化
本物のAbstractOsawariとの型不一致が解決できないため、`StubInputTrigger.cs`全体をコメントアウトして無効化。
今回のテストではInputManagerが直接本物のOsawariManagerを使うため、StubInputTrigger自体は不要だった。

### 3. OsawariPiston / OsawariKiss / OsawariWithoutHand / OsawariDoublehanded / OsawariPants を
`namespace Stubs` の外(グローバル)に移動し、本物のAbstractOsawariを継承する形に変更。
6つの抽象メソッド(InitializeParams, AutoAnimation, UpdateWhileNotClicked, UpdateParamsCore,
GetHandParamIndex, OnLateUpdate)を、それぞれ空実装で追加する必要があった。

### 4. OsawariEvent スタブも同様に `namespace Stubs` の外に移動
`IsInvoked`, `Cancel()`, `Initialize(AbstractOsawari parent)` を追加。

### 5. HandManager, ContextOsawariTargetList など、Stubs名前空間内から
本物のAbstractOsawariを参照する箇所は `global::AbstractOsawari` という記法で明示的に指定。
(namespace Stubsの中で単に`AbstractOsawari`と書くとStubs.AbstractOsawariと解釈されてしまうため)

### 6. ParameterValue, TemporaryStatus, FeelingsClass にメソッド追加
- ParameterValue: `Update(float)`, `IsAlmostZero()`
- TemporaryStatus: `AddExciteValue(int)`
- FeelingsClass: `AddAtomosphere(int)`

### 7. 新規スタブ作成
- `HandType`(enum), `Hand`(HandType, IsGrabbing持ち), `ParameterName`(enum), `ParameterDictionary`,
  `HandParamValue`, `SpeedRange`, `OsawariBlocker`, `OsawariConditions`, `FPSChecker`, `Easing`(static),
  `PhysicsCalculater`, `FaceController`, `TimeManager`
- `SerialEvent`, `HScene3ModeChangeEvent`, `DetailAnimationEvent`(いずれも空のOsawariEvent派生)

### 8. OsawariManagerTester.cs の修正
ダミーのAbstractOsawariインスタンス生成(abstract classなのでnew不可)をやめ、
本物の OsawariHead を GetComponent 経由で取得して使う方式に変更。

```csharp
var drawables = TargetOsawariManager.Model.Drawables;
var osawariHead = TargetOsawariManager.GetComponent<OsawariHead>();
osawariHead.TouchableMeshs = drawables;
var targetList = new System.Collections.Generic.List<AbstractOsawari> { osawariHead };
TargetOsawariManager.ContextOsawariTargets = new Stubs.ContextOsawariTargetList();
TargetOsawariManager.ContextOsawariTargets.RegisterTargets(targetList);
```

事前準備として、OsawariManagerと同じGameObjectに `Add Component` で `OsawariHead` をアタッチしておく必要がある。

## 現在の到達点(コンパイルは通っている)

```
Raycastヒット数: 1        ← モデルへの当たり判定は成功
osawariFromDrawable は null か: False   ← GetOsawariFromDrawableでOsawariHeadが見つかっている
```

しかし、その先で `MouseOn: Osawari` にならず `MouseOn: None`(MoveCamera)のままになっている。
`GetConstraints()` / `CanTouchMesh()` のどちらかで弾かれている可能性が高い。

## 未解決の問題(次回、ここから再開)

`InputManager.cs` の `IsMouseOnOsawariParts` に、以下のデバッグログを追加済みのはず(要確認):

```csharp
AbstractOsawari osawariFromDrawable = _manager.GetOsawariFromDrawable(cubismRaycastHit.Drawable);
Debug.Log("osawariFromDrawable は null か: " + (osawariFromDrawable == null));
if (osawariFromDrawable != null)
{
    Debug.Log("GetConstraints: " + osawariFromDrawable.GetConstraints() + ", CanTouchMesh: " + osawariFromDrawable.CanTouchMesh(cubismRaycastHit.Drawable));
}
```

このログが実際に出力されていない(反映されていない可能性がある)。次回、まず以下を確認する:

1. 上記のログが `InputManager.cs` に正しく追加されているか再確認する
2. `AbstractOsawari.GetConstraints()`(本物)の中身に直接ログを仕込み、呼ばれているか確認する

```csharp
// AbstractOsawari.cs の GetConstraints() 内、先頭に追加する候補
public virtual bool GetConstraints()
{
    UnityEngine.Debug.Log("GetConstraints が呼ばれた");
    ...
```

3. `GetConstraints()` の中身に、`Blockers`(OsawariBlockerのList)が絡む処理がある。
   `OsawariHead`(本物)側で `Blockers` フィールドが初期化されていない(null)可能性があり、
   `NullReferenceException` が握りつぶされている、あるいは何らかの理由で `false` を返している疑いがある。
   `Blockers` を空リストで初期化する対応が必要になるかもしれない。

## Git運用メモ(このセッションでは未実施)
- 現状かなり大規模な変更(StubInputTrigger無効化、複数ファイルのnamespace変更)を行っているため、
  区切りの良いところでコミットしておくことを推奨。

## 全体の依存関係(このセッションで新たに実験用プロジェクトに導入したファイル)

```
AbstractOsawari.cs(本物、abstract class) ← 背骨、直接使うべきだった型
OsawariHead.cs(本物、AbstractOsawariの派生)
OsawariEvent.cs(スタブだが、Stubs名前空間の外に出して運用)
ScriptExecuteEvent.cs(本物、OsawariEventの派生、UnityEventを呼ぶだけのシンプルな実装)
```

## 次回の進め方(提案)

1. `GetConstraints()` に直接ログを仕込み、呼ばれているか・何が返っているか確認する
2. `Blockers` フィールドの初期化漏れを疑い、`OsawariHead`(またはAbstractOsawari)側で
   `Blockers = new List<OsawariBlocker>();` のような初期化コードが必要か確認する
3. `MouseOn: Osawari` に到達したら、`OnFirstClick()` → `OnTouchEvents` の発火まで確認する
   (ただし `OnTouchEvents` 自体も、OsawariHeadにまだ何も登録していない可能性があるため、
   テストコード側で `OsawariHead.OnTouchEvents` にダミーの `ScriptExecuteEvent` を
   1つ追加する作業が別途必要になる見込み)