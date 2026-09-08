# オサワリシステム クリック処理 学習ノート

AssetRipperで吸い出したUnityゲームを解析しながら、実験用プロジェクトで少しずつコードを実装して理解を深めている記録です。「クリックしたらイベントが発火する」までの一連の処理を、実際にコードを読みながら解明した過程をまとめています。

---

## 1. クリック処理の全体像

マウスを押してから離すまで、以下の順番で処理が流れます。

```
① InputManager: マウス座標からレイ(Ray)を飛ばし、Raycastで当たり判定を計算
② OsawariManager.UpdateWhileClicked: 当たった中から対象パーツ(AbstractOsawari)を選ぶ
③ AbstractOsawari.OnClick: 選ばれたパーツ自身がクリック処理を開始
④ OnFirstClick: クリックし始めた瞬間だけ、OnTouchEventsのリストを発火させる
⑤ OsawariEvent.InvokeEvent → AwaitInvokeEvent: 条件チェック・待機・ステータス更新
⑥ InvokeCore: 実際の演出(今回はDebug.Logを出すだけのDummyTouchEvent)を実行
```

### レイを飛ばすのはInputManager、選ぶのはOsawariManager

最初「OsawariManagerがレイを飛ばしている」と勘違いしやすいところですが、正しくは:

- **InputManager**: `Camera.main.ScreenPointToRay(...)`でレイを作り、`_raycaster.Raycast(...)`で計算する(重い処理はここ)
- **OsawariManager**: InputManagerから結果(`results`, `hitCount`)を受け取るだけで、自分ではレイを飛ばさない。受け取った結果から「どのパーツが対象か」を選ぶのが役目

### 対象パーツの選び方(Priorityによる優先度判定)

複数のパーツが画面上で重なっていた場合、`Priority`(優先度)の数値が一番高いものが選ばれます。

```csharp
for (int i = 0; i < hitCount; i++)
{
    for (int j = 0; j < _targets.Count; j++)
    {
        if (_targets[j].CanTouchMesh(results[i].Drawable) && IsTouchable(_targets[j]) && num < _targets[j].Priority)
        {
            _targetOsawari = _targets[j];
            targetMesh = results[i].Drawable;
            num = _targetOsawari.Priority;
        }
    }
}
```

一度選ばれたパーツ(`_targetOsawari`)は、**マウスを離すまで固定**されます。押しっぱなしでカーソルが多少動いても、対象パーツが途中で切り替わることはありません。

### 呼び出しは「isFirst」で1回だけ/毎フレームに分かれる

```csharp
_manager.UpdateWhileClicked(results, hitCount, isFirst: true);   // クリックした瞬間、1回だけ
while (_pressed)
{
    ...
    _manager.UpdateWhileClicked(results, hitCount, isFirst: false);  // 押している間、毎フレーム
    await UniTask.Yield(token);
}
```

`isFirst: true`の1回だけ、`OnFirstClick()`が呼ばれて`OnTouchEvents`(1回限りの演出)が発火します。それ以降の毎フレーム(`isFirst: false`)では、`OnTouchEvents`は発火せず、代わりに`TriggeredEvents`(継続的にチェックされるイベント)や、実際にLive2Dパラメータを動かす`UpdateParams`が呼ばれ続けます。

### マウスを離した後の後片付け

マウスを離すと、以下の順で後片付けが行われます。

```
_pressed = false → whileループ終了 → finally節でOnMouseUpTrigger()を呼ぶ
  → _targetOsawari.OnMouseUp() → OnMouseUpEventsを発火 → _targetOsawari = null にリセット
```

これで「次にクリックした時は、また新しく対象パーツを選び直す」という状態に戻ります。

---

## 2. クラスの3つのグループ

このプロジェクトのクラスは、大きく3種類に分けられます。混同すると型エラーの原因になるので、常に「これはどのグループか」を意識するのが大事です。

### ① 本物(namespace `Paidia.satsuki1`)

元のゲームのソースコードそのまま。挙動を変えずに使う。

- `AbstractOsawari` / `OsawariHead` — パーツの基底クラスと実装
- `OsawariManager` — 司令塔
- `InputManager` / `MouseInputProvider` — 入力検知
- `OsawariEvent` / `ScriptExecuteEvent` — イベントの基底クラスと実装
- `ActionManager` — 演出発動の管理(※スタブ側にも同名あり、要注意)

### ② スタブ(namespace `Stubs`、実験用の自作)

本物が依存している「周辺クラス」の仮実装。学習を進めるための穴埋め。

- `HandManager` — 手の管理。`IsGrabbing`/`Grab`の中身はまだ未確認(次の課題)
- `StatusObject` / `TemporaryStatus` — ステータス管理(こちらはある程度機能を確認済み)
- `OsawariCameraManager` — カメラ移動。`MoveCamera`はログを出すだけで実際には動かない、`SetMousePos`/`CameraZoom`は完全に空
- `SceneContextManager` / `SingletonManager<T>` — シーン文脈・シングルトン
- その他多数(`SaveLoadManager`関連、`EventCondition`系、`ContextOsawariTargetList`など、本物クラスが要求する依存を埋めるためのもの)

### ③ 橋渡しダミー(namespaceなし、本物を継承して中身は空)

- `SerialEvent` / `HScene3ModeChangeEvent` / `DetailAnimationEvent` — 型判定(`is`)のためだけの空クラス
- `DummyTouchEvent` — 動作確認用。`InvokeCore`でDebug.Logを出すだけ

### ⚠️ 名前の衝突に注意

`ActionManager`と`OsawariAction`は、①(本物)と②(スタブ)の**両方に同名で存在**します。以前`OsawariEvent`でも同じ「本物とスタブの名前衝突」でコンパイルエラーを踏んだことがあるので、今後この2つを触る時は、必ず`namespace`(`Paidia.satsuki1`か`Stubs`か)を確認すること。

---

## 3. つまずいたポイントと解決策

### Mesh(単数)とTouchableMeshs(複数)の関係

当たり判定は`TouchableMeshs`(配列)で判定されますが、実は`Mesh`(単数フィールド)の方が本体で、`TouchableMeshs`は`Mesh`から自動生成されるだけ、という設計でした。

```csharp
protected virtual void SetTouchableMeshs()
{
    TouchableMeshs = new CubismDrawable[1] { Mesh };
}
```

このメソッドは`ManagedStart()`の中で**必ず**呼ばれます。なので`Mesh`フィールドが`None`(未設定)のままだと、Inspector上で`TouchableMeshs`にいくら`HitArea`を手動で入れておいても、Play時に`[null]`で強制的に上書きされてしまいます。

**対処法**: Inspectorの`Mesh`(単数)フィールドの方に`HitArea`を設定する。`TouchableMeshs`側は空でも自動的に再構築されるので気にしなくてよい。

- `Mesh` = 「自分が担当するメッシュはこれです」という基本の申告(必須)
- `TouchableMeshs` = 実際の判定に使われる最終リスト(デフォルトは`Mesh`から自動生成された1個)

### StatusObject未設定によるNullReferenceException

`OsawariEvent`(すべてのタッチイベントの基底クラス)は、発火時に`StatusObject.TemporaryStatus`へアクセスする設計になっています。

```csharp
StatusObject.TemporaryStatus.AddExciteValue(StatusChange.Excite);
```

`OsawariEvent`を継承するイベント(`DummyTouchEvent`など)の`Status Object`フィールドが未設定だと、ここでNREが発生します。

**対処法**: イベントが付いているGameObjectの`Status Object`フィールドに、シーンの`Status Object`をドラッグ&ドロップして設定する。

### コンポーネントの重複によるNRE

`OsawariHead`のような同一コンポーネントを、うっかり2つのGameObject(独立した`OsawariManager`用と、Live2Dモデル本体)に重複してアタッチしてしまうと、Unity側の参照解決が混線してNREの原因になります。

**対処法**: 重複しているコンポーネントを1つに整理する。`OsawariManager`と`OsawariHead`は**必ず同じGameObject**に乗せる(`GetComponent<OsawariHead>()`は同一GameObjectしか探さないため)。

コンポーネントを別のGameObjectに移す時は「Copy Component」→(移動先で)「Paste Component As New」を使うと便利。ただし、他のコンポーネントがその元のコンポーネントを直接参照していた場合(例:`OsawariManagerTester`の`Target Osawari Manager`フィールド)、参照が切れるので、新しいインスタンスに貼り直す必要がある。

### Initialize()のタイミング

```csharp
foreach (OsawariEvent onTouchEvent in OnTouchEvents)
{
    onTouchEvent.Initialize(this).Forget();
}
```

これは`ManagedStart()`の中の処理です。つまり`OnTouchEvents.Add(...)`で新しいイベントを追加するのは、必ず`ManagedStart()`が呼ばれる**前**に行う必要があります。後から追加しても、`Initialize()`が呼ばれないまま使われてしまいます。

### 名前空間の衝突(本物とスタブの同名クラス)

以前のセッションで作った、namespace指定なしのスタブ版`OsawariEvent`が、本物の`Paidia.satsuki1.OsawariEvent`と名前が衝突していました。橋渡しダミークラス(`SerialEvent`など)が旧スタブ版を継承していたため、削除後に`using`の追加と`InvokeCore`の実装が必要になりました。

---

## 4. このシステムを読む上で出てきたC#の基礎知識

### プロパティとフィールドの違い

```csharp
private MouseOn _pressedMouseOn;              // フィールド(ただの入れ物、値をそのまま保存)
private MouseOn _mouseOn { get { ... } }      // プロパティ(見た目は変数だが、呼ばれるたびに中の処理が実行される)
```

フィールドはアクセスしても何も計算せず、今入っている値がそのまま返ってきます。プロパティは`get { ... }`の中身が、アクセスするたびに毎回実行し直されます。

### public/privateと命名慣習

```csharp
public MouseOn MouseOn { get { ... } }     // 外部公開用: 先頭大文字(PascalCase)
private MouseOn _mouseOn { get { ... } }   // 内部専用: アンダースコア+先頭小文字
```

「他のクラスから使われることを想定しているかどうか」で`public`/`private`を決め、それに応じて命名スタイルを変えるのがC#の慣習。

### 否定演算子 `!`

```csharp
if (!AllowOsawari) { return; }   // 「AllowOsawariがfalseなら」という意味
```

`AllowOsawari == false`と書いても動作は同じだが、`bool`型の変数は`== false`と比較せず`!`を使うのが一般的なスタイル。`Allow〜`、`Is〜`、`Can〜`のような名前にしておくと、`!`が付いた時に自然な否定文として読める。

### 名前付き引数

```csharp
new BoolReactiveProperty(initialValue: false)
```

`initialValue`はメソッド(コンストラクタ)を定義した側が決めた引数名で、呼び出す側は変えられない。ただし名前を省略して`new BoolReactiveProperty(false)`と書くことも可能(動作は同じ)。

### LINQの `where`

```csharp
from _ in Observable.EveryGameObjectUpdate()
where _input.InputMouseRelease()
select _;
```

「〜という条件のところだけ」を通すフィルター。条件を満たさない場合は、単にそこで処理が止まり、何も後続に伝わらない。

### ReactiveProperty(UniRx)

```csharp
private BoolReactiveProperty _isInOsawari = new BoolReactiveProperty(initialValue: false);
```

値の変化を監視できる変数。値が変わった瞬間に、それを購読(Subscribe)している側へ自動的に通知が飛ぶ。毎フレームチェックし続ける(ポーリングする)代わりに、変化した時だけ反応するコードが書ける。

### try / catch / finally

```csharp
try { ... }
catch (OperationCanceledException) { ... }
finally { ... }   // tryの中で何が起きても(正常終了でも例外発生でも)必ず実行される
```

`finally`は「後片付け処理を、エラーが起きても確実に実行させたい」時に使う。

---

## 5. 今後の課題

1. **クリックで実際にLive2Dモデルが動くようにする**
   - `OsawariHead.UpdateParamsCore`がパラメータを動かす設計になっている
   - 発動条件: `GetConstraintsCore() && (_handManager.IsGrabbing(this) || (!IsGrabbable && !IsAnimating))`
   - `IsGrabbing`が`true`になるには、`OnFirstClick`内の`_handManager.Grab(hand.HandType, this)`が実行されている必要がある
   - 現在`_handManager`は`Stubs.HandManager`なので、`IsGrabbing`/`Grab`の中身が「何もしない」実装になっていないか確認が必要(未着手)

2. **パラメータ番号の一致確認**
   - `ParameterNumbers`(`HeadX=0, HeadY=1`)が、実際に使っているhiyori_free_t08モデルのパラメータ番号(角度X=0, 角度Y=1)と一致しているか要検証

3. **テスト用の便宜処理の整理**
   - `OsawariManagerTester.cs`内に残っている、コード側で動的に`DummyTouchEvent`を追加する処理は、いずれ削除してInspector設定のみに統一する