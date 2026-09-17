# オサワリシステム クリック処理 - 引き継ぎメモ(第3版)

## これまでの成果(達成済み)

クリック処理パイプラインは完成し、エラーなしで動作することを確認済み。

```
Raycast当たり判定 → MouseOn:Osawari判定 → OsawariManager.UpdateWhileClicked
→ AbstractOsawari.OnClick → OnFirstClick → OnTouchEvents → OsawariEvent.InvokeEvent
→ InvokeCore(DummyTouchEvent / ScriptExecuteEventで実際に発火)
```

`OsawariManager`をLive2Dモデル(`hiyori_free_t08`)のGameObjectに移動し、`OsawariHead`と
同一GameObjectに揃える構成整理も完了済み。

本物の`ScriptExecuteEvent`(Inspectorで自由に呼び出し先を設定できる汎用イベント)を導入し、
`TestLogger.cs`という橋渡し用スクリプト経由でDebug.Logの発火まで確認済み。

## 次の目標(このチャットでやりたいこと)

**クリックで実際にLive2Dモデルが動くようにする**ことが、残っている一番の本丸。

原因は判明済み: `Stubs.HandManager`の中身が空実装だから。

```csharp
public bool IsGrabbing(global::AbstractOsawari target)
{
    return false;   // 常にfalse
}
public void Grab(HandType type, global::AbstractOsawari target) { }   // 何もしない
```

`AbstractOsawari.OnClick`内の、実際にパラメータを動かす条件はこちら:

```csharp
if (GetConstraintsCore() && (_handManager.IsGrabbing(this) || (!IsGrabbable && !IsAnimating)))
{
    UpdateParams(ConvertMovementVec3ForParams());
}
```

`IsGrabbing`が常にfalseなので、`UpdateParams`に到達する唯一の望みは
`(!IsGrabbable && !IsAnimating)`側。次にやる2択:

1. **お手軽(未検証)**: OsawariHeadの`Is Grabbable`のチェックを外す → HandManagerを直さなくても
   `UpdateParams`に到達できる可能性がある
2. **正攻法**: `HandManager.Grab`/`IsGrabbing`を実際に「今どのパーツを掴んでいるか」を
   記録・判定するよう実装する

どちらから試すか、次のチャットで相談して決める。

## 構成変更にあたっての注意点(教訓)

- `OsawariManager`と`OsawariHead`は必ず同じGameObjectに乗せる(`GetComponent<T>()`は同一
  GameObjectしか探さないため)
- 同じコンポーネントを複数のGameObjectに重複アタッチすると、Unity側の参照解決が混線し
  NullReferenceExceptionの原因になる
- コンポーネントを別GameObjectに移す時は「Copy Component」→「Paste Component As New」。
  他のコンポーネントからの直接参照(例: OsawariManagerTesterのTarget Osawari Manager)は
  貼り直しが必要になる

## クリック処理を「実際にモデルが動く」ところまで進めるための材料

`OsawariHead.cs`(本物)は、以下のメソッドで実際にLive2Dパラメータを動かす設計になっている。

```csharp
protected override void UpdateParamsCore(Vector3 move)
{
    ...
    _headY += move.y / SensitivityY;
    _headX += move.x / SensitivityX;
    _manHand.Appear(GetActiveHand());
}
```

`InitializeParams()`が参照する`ParameterNumbers`(`HeadX=0, HeadY=1`)が、実際に使っている
Live2Dモデル(hiyori_free_t08)のパラメータ番号(`角度X`=0, `角度Y`=1)と一致しているかも
要検証(Cubism公式サンプルモデルは大抵この並びだが未確認)。

## これまでの主な教訓(繰り返し出てきたパターン)

1. `Stubs`名前空間の自作クラスは`[System.Serializable]`必須。付け忘れると実行時に`null`のまま
   → `NullReferenceException`。対処はフィールド宣言に`= new ○○();`を追記。
2. `MonoBehaviour`を継承するクラスは`new`できない。`FindObjectOfType<T>()`や
   `AddComponent<T>()`で明示的に取得・生成する必要がある。
3. `GetComponent<T>()`は同一GameObject内しか探さない。
4. `Initialize()`は`ManagedStart()`内の`foreach`で呼ばれるため、`OnTouchEvents.Add(...)`は
   必ず`ManagedStart()`より**前**に行う必要がある。
5. 名前が同じクラスが「本物(namespace Paidia.satsuki1)」と「スタブ」の両方に存在すると
   型解決の衝突が起きる。旧スタブは削除するか名前空間を揃える。実際に`OsawariEvent`と
   `ActionManager`でこの衝突を踏んだ。
6. デコンパイル由来のファイルは、たまに`using`が1行抜けていることがある(例:
   `ScriptExecuteEvent.cs`に`using Stubs;`が無く、TemporaryStatus/OsawariConditionsの
   型解決エラーが出た)。同じ基底クラスを継承する、動いている他のファイルとusingを
   見比べるのが有効な対処法。
7. `Mesh`(単数、必須の担当メッシュ申告)と`TouchableMeshs`(複数、実際の判定リスト)の関係:
   `SetTouchableMeshs()`のデフォルト実装が`ManagedStart()`内で必ず`Mesh`から
   `TouchableMeshs`を作り直すため、`Mesh`が未設定だと当たり判定が消える。
8. `global::ClassName`は、名前空間の中から名前空間なし(グローバル)のクラスを明示的に
   指す書き方。同名クラスの衝突を未然に防ぐ保険として使われる。

## 主要クラスの役割

- `InputManager` : 入力を検知し、いつ何を呼ぶかの手順を管理(SetUpRxでUniRxの監視を設定)
- `OsawariManager` : ゲームロジック全体の司令塔。パーツ・周辺システムを繋ぐ
- `AbstractOsawari`(`OsawariHead`) : 実際に触られる体のパーツ本体
- `OsawariEvent`(`ScriptExecuteEvent`, `MessageEvent`等) : 条件判定・待機・演出発動を担う
  汎用フレームワーク。`Conditions`/`FlagCondition`/`ScenarioReadCondition`という3種類の
  発動条件を持てる
- `HandManager` : 「今どのパーツを掴んでいるか」の状態管理。現在は空実装で、これが
  「クリックしても動かない」の原因
- `ActionManager` : 興奮度やパーツの状態を横断的に見て、シナリオ的な演出を発動

## クラスの3分類(詳細は memory の osawari-classes.md 参照)

1. **本物**(namespace Paidia.satsuki1): AbstractOsawari, OsawariHead, OsawariManager,
   InputManager, OsawariEvent, ScriptExecuteEvent, MessageEvent, EventCondition,
   EventConditionName, ActionManager 等
2. **スタブ**(namespace Stubs): HandManager, StatusObject, OsawariCameraManager,
   SceneContextManager 等、周辺の穴埋め
3. **橋渡しダミー**(namespaceなし): SerialEvent, DummyTouchEvent, TestLogger 等

⚠️ `ActionManager`と`OsawariAction`は本物・スタブ両方に同名で存在するため要注意。

## 使用モデルについて

実機データのモデルではなく、Live2D Cubism公式サンプルモデル(hiyori_free_t08)を使用。
当たり判定は`HitArea`1個に絞って運用中(以前は全メッシュを割り当てていたが、`Mesh`
フィールド未設定が根本原因と判明し解消済み)。