# オサワリシステム クリック処理 - 引き継ぎメモ

## 大きな成果
クリック処理の目標は**達成済み**。
`InputManager`のクリック検知 → `MouseOn: Osawari`判定 → `OsawariManager.UpdateWhileClicked`
→ `AbstractOsawari.OnClick`→`OnFirstClick`→`OnTouchEvents`→`OsawariEvent.InvokeEvent`
→`InvokeCore`まで、パイプライン全体が動作し、テスト用の`DummyTouchEvent`が
実際に「★ダミーイベント発火した!★」とログ出力するところまで確認済み。

## 現在の残課題(唯一)
クリックすると、稀に(あるいは毎回)以下のエラーが出るが、最終的には成功する。

```
NullReferenceException(握りつぶされてStackTraceのみログ出力)
at Paidia.satsuki1.OsawariEvent.AwaitInvokeEvent (...) OsawariEvent.cs:97
```

97行目は `StatusObject.TemporaryStatus.AddExciteValue(StatusChange.Excite);`。
`StatusChange`はデフォルト値`= new StatusChange();`が入っているため、
**`StatusObject`自体が`null`である可能性が高い**。

### 最有力の仮説
`OsawariHead.OnTouchEvents`リストに、**2つの別々の`DummyTouchEvent`インスタンスが
重複登録されている**可能性がある。

- Hierarchy上に手動作成した`OnTouch`(GameObject、`DummyTouchEvent`アタッチ済み)を、
  Inspector上で`OnTouchEvents`リストにドラッグ&ドロップで登録した(可能性)
- 加えて`OsawariManagerTester.cs`のコード側でも、動的に`new GameObject(...)`で
  別のインスタンスを作って`OnTouchEvents.Add(dummyEvent)`している
- コード側で作った方だけ`dummyEvent.StatusObject = FindObjectOfType<Stubs.StatusObject>();`
  で明示的にセットしているが、Inspector経由で登録した方は`StatusObject`が未設定のまま

### 次にやること
1. Unity Editorで`OsawariHead`コンポーネントのInspectorを開き、
   `On Touch Events`リストの要素数を確認する(1個か2個か)
2. もし2個あれば、Inspector側の重複登録を削除する。
   もしくはInspector側の`OnTouch`にも`StatusObject`を手動でアサインする
3. どちらか一方の登録方法(Inspector手動 or コード動的生成)に統一するのが望ましい

## OsawariManagerTester.cs の現在の該当箇所(参考)
```csharp
var dummyEventObj = new GameObject("DummyTouchEvent");
var dummyEvent = dummyEventObj.AddComponent<DummyTouchEvent>();
dummyEvent.StatusObject = FindObjectOfType<Stubs.StatusObject>();
osawariHead.OnTouchEvents.Add(dummyEvent);

osawariHead.ManagedStart(TargetOsawariManager, System.Threading.CancellationToken.None);

osawariHead.TouchableMeshs = drawables;
```

## これまでの主な教訓(繰り返し出てきたパターン)
1. `Stubs`名前空間の自作クラスは`[System.Serializable]`必須。付け忘れると
   Inspectorでシリアライズされず実行時に`null`のまま→`NullReferenceException`。
   対処はフィールド宣言に`= new ○○();`を追記。
2. `MonoBehaviour`を継承するクラス(`StatusObject`等)は`new`できない。
   `FindObjectOfType<T>()`や`AddComponent<T>()`で明示的に取得・生成する必要がある。
3. `GetComponent<T>()`は同一GameObject内しか探さない。
   `OsawariManager`と`OsawariHead`は同じGameObjectに乗せる必要がある。
4. `Initialize()`(イベントの初期化、`_ctsForInvoke`のセットアップ)は
   `ManagedStart()`内の`foreach`で呼ばれるため、`OnTouchEvents.Add(...)`は
   必ず`ManagedStart()`より**前**に行う必要がある。
5. 名前が同じクラスが「本物(namespace Paidia.satsuki1)」と「スタブ(グローバルor別namespace)」
   の両方に存在すると型解決の衝突が起きる。古いスタブ版を導入した本物と置き換える際は、
   旧スタブファイルを削除または名前空間を揃える必要がある。

## 主要クラスの役割
- `InputManager` : 入力を検知し、いつ何を呼ぶかの手順を管理
- `OsawariManager` : ゲームロジック全体の司令塔。パーツ・周辺システムを繋ぐ
- `AbstractOsawari`(`OsawariHead`) : 実際に触られる体のパーツ本体
- `OsawariEvent`(`ScriptExecuteEvent`等) : 条件判定・待機・演出発動を担う汎用フレームワーク
- `ActionManager` : 興奮度やパーツの状態を横断的に見て、シナリオ的な演出(`OsawariAction`)を発動

## 使用モデルについて
実機データのモデルではなく、Live2D Cubism公式サンプルモデル(hiyori_free_t08)を使用。
そのため元のprefab/シーンにOsawariHeadの実データ(Mesh、ParameterNumbers等)は存在せず、
コピーではなく自作で用意している。当たり判定は本来「HitArea」という専用の
判定用メッシュ(CubismDrawable型)を使う設計だったが、実験では簡易的に全83メッシュを
`TouchableMeshs`にまとめて割り当てている。