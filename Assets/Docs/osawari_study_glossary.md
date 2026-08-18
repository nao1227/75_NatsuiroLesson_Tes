# 2026-08-10 学習ログ: MouseInputProvider の実験

## 今回の目的
`MouseInputProvider` を実験用プロジェクトで実際に動かして検証する。

## 依存関係マッピングの手順(復習)
1. クラスの「持ち物」を見る(フィールド一覧)= 依存の洗い出し
2. 「誰から作られるか(注入されるか)」を見る(コンストラクタ/メソッドの引数)
3. 表にまとめる(依存先・型・注入方法)

## MouseInputProvider の依存関係

| 依存先 | 型 | 注入方法 | 実験での対応 |
|---|---|---|---|
| _utage | UtageManager | コンストラクタ注入 | スタブ作成 |
| Input | Unity標準 | 静的アクセス | そのまま動く |
| SaveLoadManager.GlobalData | 静的プロパティ | 直接アクセス | スタブ自作が必要 |

## 用語

- **glossary(グロッサリー)**: 用語集
- **Docs(ドックス)**: 資料・書類フォルダ
- **study(スタディ)**: 学習・研究
- **Experiment(エクスペリメント)**: 実験
- **スタブ(stub)**: 本物のクラスの代わりに使う、型だけ合わせた最低限の偽物クラス。中身のロジックはほぼ無い。撮影の「書割(はりぼて)」のようなもの。
- **注入される依存(Injected Dependency)**: コンストラクタや引数で外から渡してもらう依存。差し替えが簡単(例: `_utage`)。
- **静的な依存(Static Dependency)**: コード内に直書きされた依存。差し替えできない。実験時はスタブが必須(例: `SaveLoadManager.GlobalData`)。
- **コンストラクタ**: クラス名と同名・戻り値なしのメソッド。`new` された瞬間に1回だけ呼ばれる初期化処理。

## 実験用プロジェクトのフォルダ構成
Assets/
├── Docs/ ← 学習メモ(.md)
├── Experiment/
│ ├── Scripts/ ← 解析対象の本物コード(コピー、極力編集しない)
│ ├── Stubs/ ← スタブ置き場
│ └── Testers/ ← テストコード(観察用# 学習ログ: InputManager「クリックしたら動く処理」達成

## 今回のセッションの流れ

### 1. シーン状態の巻き戻りトラブルと復旧

前回終了時に作成した InputManagerObj・Tester(InputManagerTester用)の配置が、
シーン上から消えて MouseInputProvider 実験時の状態に戻っていた。

- 原因の推測: シーンファイルの保存タイミングとGit操作のタイミングがズレていた可能性
- 確認したこと: .cs ファイルの中身(コード)は無事だった。消えていたのは
  シーン上の「GameObjectへのアタッチ・Inspector設定」だけ
- 教訓: コード(スクリプト)とシーン(配置)は別々に保存されるため、
  Git操作の前後でズレることがある。作業の節目でシーンも保存する習慣が必要

### 復旧手順
1. 空のGameObjectを作成、名前を InputManagerObj に変更
2. InputManagerObj に InputManager.cs をアタッチ
3. 別の空のGameObjectを作成、名前を Tester に変更
4. Tester に InputManagerTester.cs をアタッチ
5. Tester の Target Input Manager 欄に InputManagerObj をドラッグ&ドロップ

これで前回と同じ状態(Subscribeの中身に到達、MoveCameraでNullReferenceException)まで再現できた。

## OsawariCameraManager の役割(復習)

InputManager内での使われ方から、以下の役割と判断:

```csharp
case MouseOn.None:
    _isInOsawari.Value = true;
    await MoveCamera(_tokenSource.Token);   // 何もない場所をクリックした時
    break;
```

```csharp
private async UniTask MoveCamera(CancellationToken token)
{
    CameraManager.SetMousePos(GetCurrentMousePosition());   // クリック時の位置を記録
    while (_pressed)
    {
        await UniTask.Yield(token);
        CameraManager.MoveCamera(GetCurrentMousePosition());  // 毎フレーム位置を送り続ける
    }
}
```

- 「クリックした瞬間の位置」を基準に「今のマウス位置」を毎フレーム送り続ける構造から、
  ドラッグの動きに応じてカメラを動かす設計だと判断(ロジック面では確定、視覚的な動作は未確認)
- マウスホイールでのズーム処理(CameraZoom)も担当している
- 今回は「呼ばれることの確認」までを目的とし、実際に画面上でカメラが動くところまでは踏み込まない方針とした

## CameraManager の null 問題を解決

OsawariCameraManager は MonoBehaviour を継承していない普通のクラスのため、
InspectorのCamera Manager欄にドラッグ&ドロップで設定することができない。

### 対処: コードで直接インスタンスをセットする

InputManagerTester.cs の Start() に1行追加:

```csharp
void Start()
{
    var mouseInput = new MouseInputProvider(null);
    var stubTrigger = new StubInputTrigger();

    TargetInputManager.CameraManager = new OsawariCameraManager();   // 追加した行
    TargetInputManager.ManagedStart(null, mouseInput, stubTrigger);

    Debug.Log("InputManager を初期化しました");
}
```

## 結果: 今回の目的「クリックしたら動く処理」を達成)
## namespace による本物とスタブの競合回避

- スタブは最初から `namespace Stubs { }` で囲んでおく(後から競合して直すと修正箇所が増えるため)
- スタブを使うファイル側は `using Stubs;` を1行追加するだけで、Stubs名前空間の中身を全部使える
- クラスが増えても `using Stubs;` は1行のまま(個別指定不要)

## テストコード(MouseInputTester.cs)で確認できたこと

- `MonoBehaviour` ではない普通のクラス(`MouseInputProvider`)は `new` でその場で生成できる
- `new MouseInputProvider(null)` のように `null` を渡してもエラーにならない → `IsUtageNotPlayingOrNull()` がnullチェックしているため(null安全設計)
- `InputGrab()` は「マウスボタン押下」かつ「Utageが再生中でない」の両方が真の間だけ `true` を返す(毎フレーム再判定、スイッチのON/OFFではない)
- `Is〜` から始まるメソッド/プロパティは基本的に `bool` を返す(命名規則)
- パスカルケースの単語区切りで英文法通りに読むと、メソッド名から中身を予測できる

## 解析・実験の進め方(今回得た教訓)

- 依存が少ない・末端のクラスから読み始める(`IInputProvider` → `MouseInputProvider` → `InputManager` → `OsawariManager` の順)
- クラス全体・メソッド全体を網羅する必要はない。「今回の目的」を1つ決めて、それに関係する最短ルートだけを辿る
- 目的の設定と道筋の提示はAIに頼ってよいが、実装・実行は自分の手で行うと理解が深まる
- テストコードは基本的に消さず、他クラスの実験でも再利用する前提で残しておく
- 解析対象の本物コード(`MouseInputProvider.cs`)にはデバッグコードを書き込まず、観察用の別ファイル(`MouseInputTester.cs`)から外側だけ呼び出して検証する

## 次にやること(候補)
- `InputManager` の依存マッピング(依存7個)
- `InputManager` 用のスタブ作成(`OsawariCameraManager`, `CrossSectionManager` など)
- 通常の push ではなく `--force` が必要(履歴が食い違っているため)
   - 実行結果: `68805fe...6c4cb12 main -> main (forced update)`
   - これで GitHub上の main も、今日までの実験内容に置き換わった

6. GitHub Desktopで確認
   - Current branch: main
   - 「Pull 1 commit from the origin remote」の警告が消えている
   - 0 changed files / No local changes
   - → ローカルとGitHub、両方の main が完全に一致した状態になった

### 今後の運用方針
- main = 今日までの InputManager 実験を含む、最新の学習内容の基準
- experiment-inputmanager ブランチも Branches 一覧には残っている(削除はしていない)
- 13日前の初期状態は、mainの書き換えにより実質的に失われた(バックアップは取らない方針で合意済み)

### 学んだGitコマンド一覧(今回のセッション全体)

| コマンド | 意味 |
|---|---|
| `git reflog` | 操作履歴を全部表示する(読み取り専用、安全) |
| `git branch [新名] [コミットID]` | 指定したコミットの位置に新しいブランチを作る |
| `git checkout [ブランチ名]` | 指定したブランチに切り替える |
| `git branch -f [ブランチ名] [別ブランチ名]` | 既存ブランチの位置を強制的に付け替える |
| `git push origin [ブランチ名] --force` | リモート(GitHub)を強制的に上書きする |

### 注意点(今後の自分へ)
- `--force` は履歴を上書きする強い操作。他の人と共同作業している場合は特に注意が必要
  (今回は一人での学習用リポジトリのため問題なし)
- 作業前に Current branch が意図したブランチになっているか、必ず確認する習慣をつける
MoveCamera が呼ばれた: (156.40, ...)
MoveCamera が呼ばれた: (90.85, ...)
NullReferenceException が解消され、MoveCameraが正常に呼ばれることを確認できた。
マウスを押している間、座標が変化しながら連続でログが出ることも確認済み(想定通りの挙動)。

## クリック処理の全体フロー(確認完了)
クリック
↓ ①検知(Unity標準のInput.GetMouseButtonDownで確認)
InputGrab()
↓ ②UniRxのwhere条件を通過(_raycasterチェックは検証後に元の仕様へ復元済み)
Subscribeの中身
↓ ③MouseOn判定 → None(何もない場所をクリック)
MoveCamera()呼び出し
↓ ④CameraManager.MoveCamera(座標) 呼び出し成功
ログ出力: MoveCamera が呼ばれた: (座標)
## Tips: VS Codeでのコード自動整形

コピペ等でインデントが崩れた際は、以下のショートカットで自動整形できる。
整形後は Ctrl+S での保存を忘れないこと。

## 現状まとめ

- InputManager の「クリックしたら動く処理(MouseOn.Noneのケース)」の検証が完了
- _raycaster が null のため MouseOn.Osawari のケース(実際にOsawariに触れる処理)は未検証のまま
- 実際に画面上でカメラが視覚的に動くところまでは未実装(必要になれば次回検討)

## 次にやること(候補)
- 本物の CubismRaycaster を用意して、MouseOn.Osawari のケースまで検証する(選択肢A、大掛かり)
- 別のクラス(OsawariManager本体など)の学習に進む
- 今回のスタブ・実験環境を土台に、別の処理を試す
- # 学習ログ: 本物のLive2Dモデル導入(MouseOn.Osawariルート検証の準備)

## 今回の目的
InputManagerの「クリックしたら動く処理」はMouseOn.Noneルートで達成済み。
残る MouseOn.Osawari ルート(実際にモデルに触れた時の処理)を検証するため、
本物のLive2Dモデルと CubismRaycaster を実験用プロジェクトに導入する準備を進めた。

## わかったこと

### 実験用プロジェクトにLive2Dの本物のモデルは入っていなかった
- Assets/Live2D/Cubism フォルダにあったのは「仕組み(SDK)」だけ
- 「実際のキャラクターのモデルデータ」は別途用意する必要があった
- 3Dモデル(SD_unitychan_humanoidなど)とLive2D(2D)モデルは別物。
  CubismRaycasterは2D(Live2D)専用で3Dモデルには使えない

### Live2Dモデルの導入手順
1. Cubism Editorの出力データの中から `runtime` フォルダを探す
   (.cmo3 は編集用プロジェクトファイルでUnityでは使わない)
2. runtime フォルダの中身一式をコピー:
   - [モデル名].moc3(モデル本体)
   - [モデル名].model3.json(設定ファイル)
   - [モデル名].physics3.json(物理演算設定)
   - [モデル名].cdi3.json(パラメータ情報)
   - [モデル名].2048/ (テクスチャフォルダ)
   - motion/ (モーションフォルダ)
3. Assets内の新しいフォルダ(例: Assets/Live2D/Models/[モデル名])に丸ごとコピー
4. Unityが自動でPrefabやマテリアルを生成する
5. 生成されたPrefabをHierarchyにドラッグ&ドロップしてシーンに配置

実際に hiyori_free_t08 モデルの導入に成功した。

### 見た目が粗く(ドット絵っぽく)見えた原因
- テクスチャ解像度は十分高い(2048x2048)だが、Game画面上での表示サイズが小さすぎたため
- 原因は Main Camera の Orthographic Size 設定だった
  - 本物のプロジェクト(参考にした別プロジェクト): Size = 0.8
  - 実験用プロジェクト: Size = 5前後(標準的なデフォルト値に近い、モデルが小さく表示される)
- Orthographic Size は「カメラが画面に映す範囲の広さ」を表す設定
  - 値が小さい = ズームインしたようにモデルが大きく見える
  - 値が大きい = ズームアウトしたようにモデルが小さく見える
- 対処: 実験用プロジェクトの Main Camera の Size を 0.8 に変更する(次回試す)

### 待機モーションについて
- デフォルトでは、モデルをシーンに置いただけでは待機モーションは自動再生されない
- 自動再生されるには CubismMotionController 等のコンポーネントと、
  起動時にモーション再生を指示するスクリプトが必要
- Live2D Cubism SDKがインポート時に自動でコンポーネント一式を付与してくれることがある
- 今回の目的(CubismRaycasterでの当たり判定検証)には待機モーションの動作は不要と判断、優先度低として保留

## 優先順位の整理(今回の判断)

| 項目 | 優先度 | 理由 |
|---|---|---|
| 待機モーションが自動再生されるか | 低 | 見た目の話で、当たり判定の検証には無関係 |
| カメラサイズの調整 | 中 | 見た目の話だが、クリックのしやすさには関わる |
| CubismRaycasterをモデルにアタッチし、InputManagerTesterから渡せるようにする | 高 | MouseOn.Osawariルート検証の本質的な作業 |

## 次にやること
1. hiyori_free_t08(または別モデル)に CubismRaycaster コンポーネントを Add Component で追加する
2. InputManagerTester.cs で、そのモデルの CubismRaycaster を取得し、
   ManagedStart(raycaster, mouseInput, stubTrigger) の第1引数に渡すよう修正する
3. Main CameraのOrthographic Sizeを0.8程度に調整する(任意、見やすさのため)
4. モデルをクリックしてみて、MouseOn が Osawari に変化するか確認する

# 学習ログ: Live2Dモデル導入とCubismRaycaster検証(完了)

## 今回の目的
InputManagerの「MouseOn.Osawariルート」を検証するため、本物のLive2Dモデルを導入し、
CubismRaycasterによる実際のクリック当たり判定を確認する。

## 実施内容

### 1. 本物のLive2Dモデルの導入
- hiyori_free_t08 モデルをAssets/Live2D/Models/hiyoriに配置
- Live2D公式サンプルモデルを使用(以前入れた3Dモデル SD_unitychan は誤りと気づき削除)

### 2. カメラサイズの調整(任意)
- Main Camera の Orthographic Size を調整し、見やすい表示に変更

### 3. CubismRaycasterのアタッチと接続
- hiyori_free_t08 に CubismRaycaster コンポーネントを追加
- InputManagerTester.cs に public CubismRaycaster ModelRaycaster フィールドを追加し、
  ManagedStart の第1引数に渡すよう修正

```csharp
public CubismRaycaster ModelRaycaster;

void Start()
{
    var mouseInput = new MouseInputProvider(null);
    var stubTrigger = new StubInputTrigger();

    TargetInputManager.CameraManager = new OsawariCameraManager();
    TargetInputManager.ManagedStart(ModelRaycaster, mouseInput, stubTrigger);

    Debug.Log("InputManager を初期化しました");
}
```

- Inspector上で Model Raycaster 欄に hiyori_free_t08 をドラッグ&ドロップして接続

### 4. Raycastの動作確認
IsMouseOnOsawariParts に一時的にログを追加して検証:

```csharp
int hitCount = _raycaster.Raycast(ray, array);
Debug.Log("Raycastヒット数: " + hitCount);
```

結果:
Raycastヒット数: 0 (モデルの外をクリックした時)
Raycastヒット数: 1 (モデルに当たった時、成功)

**本物のLive2Dモデルに対して、CubismRaycasterが正しく当たり判定を検出できることを実証できた。**

## 今回の目標は達成、MouseOn.Osawariには未到達(意図的)

Raycastは成功しているが、MouseOnは終始 None のままだった。理由:

```csharp
// StubInputTrigger.cs
public AbstractOsawari GetOsawariFromDrawable(CubismDrawable mesh) => null;
```

IsMouseOnOsawariParts が true になるには「Raycastが当たる」だけでなく
「GetOsawariFromDrawable が null 以外を返す」必要があるため、これは想定通りの挙動。

技術的には、StubInputTrigger.GetOsawariFromDrawable の返り値を null から
何らかのAbstractOsawariインスタンスに育てれば、MouseOn.Osawariまで到達させることは可能
(Drawableの検出自体はすでにできているため)。

## 今回の判断: MouseOn.Osawariへの到達は行わず、ここで一区切り

- Raycastによる当たり判定の実証(今回の目的)は達成済み
- MouseOn.Osawariまで進めるかは目的次第と判断し、今回は見送り
- 必要になれば次回、StubInputTriggerの拡張から再開できる状態

## 設計思想についての気づき: UniRxのsourceを分離する理由

InputManager.SetUpRx() 内で、なぜ以下のように source2 という中間変数に
分けているのか考察した:

```csharp
IObservable<long> source2 = from _ in Observable.EveryGameObjectUpdate()
    where _input.InputGrab() && !_pressed
    select _;

(from _ in source2
    where _manager.GetScene() == null || !_manager.GetScene().IsModalWindowOpen.Value
    where _raycaster != null && null != Camera.main
    select new { ... }).Subscribe(async x => { ... });
```

技術的には1つのwhere群にまとめることも可能だが、あえて分離されている理由:

- source2 = 「クリックが発生したという、生の入力イベント」
- その後のwhere群 = 「そのクリックを、今のゲーム状態的に受け付けていいかの判定」
- 役割の異なる条件を分けることで、可読性・保守性が上がる
- 変数名(source2)が「見出し」の役割を果たし、全部読まなくても大まかな構造を把握できる
- デバッグ時も、どの段階で処理が止まっているか切り分けやすくなる
  (実際に今回「Subscribeの中身に到達した」ログで、この構造の恩恵を体感した)

## UniRxの実行タイミングの整理

- SetUpRx() 自体は ManagedStart から1回だけ呼ばれる「組み立てフェーズ」
- Subscribe(async x => {...}) の中身は、クリックが発生するたびに何度でも実行される
- _pressedMouseOn = _mouseOn; は Subscribe内にあるため、クリックのたびに現在のMouseOn値を記録している

## プロパティの評価タイミングについて

- MouseOn はプロパティであり、「呼ばれる」というより「読まれる(アクセスされる)たびに、get の中身が実行される」
- 本物のゲームでは、クリックされた瞬間に1回だけ MouseOn を読み取り、switch文で処理を振り分けている
  (今回のテストコードのようにUpdate内で毎フレーム読む使い方とは異なる、より無駄のない設計)

## MainThreadDispatcherについて

- UniRxのObservable機能が最初に使われたタイミングで、自動的に1回だけ生成される常駐オブジェクト
- DontDestroyOnLoadに配置され、ゲーム終了までシーンをまたいで生き続ける
- 毎フレーム、登録されている全てのObservableに「Updateが来た」という通知を配る「配達員」の役割
- EveryGameObjectUpdate()が呼ばれるたびに新しく生成されるわけではない

## 次にやること(候補)
- 別のクラス(OsawariManager本体など)の学習に進む
- 必要になれば、StubInputTrigger.GetOsawariFromDrawable を拡張してMouseOn.Osawariルートまで検証する