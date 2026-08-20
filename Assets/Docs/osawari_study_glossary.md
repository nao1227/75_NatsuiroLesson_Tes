# Osawari システム学習ログ(統合版)

## 学習の進め方・基本方針

- 依存が少ない・末端のクラスから読み始める(IInputProvider → MouseInputProvider → InputManager → OsawariManager の順)
- クラス全体・メソッド全体を網羅する必要はない。「今回の目的」を1つ決めて、それに関係する最短ルートだけを辿る
- 目的の設定と道筋の提示はAIに頼ってよいが、実装・実行は自分の手で行うと理解が深まる
- テストコードは基本的に消さず、他クラスの実験でも再利用する前提で残しておく
- 解析対象の本物コードにはデバッグコードを書き込まず、観察用の別ファイル(Testerクラス)から外側だけ呼び出して検証する
- スタブは基本残す方針だが、保存容量などの制約がある場合は「一時的に作って確認後に消す」でもよい。その場合はファイルは消してもglossaryに型の構造だけ記録しておく

---

## 用語集

- **glossary(グロッサリー)**: 用語集
- **Docs(ドックス)**: 資料・書類フォルダ
- **study(スタディ)**: 学習・研究
- **Experiment(エクスペリメント)**: 実験
- **スタブ(stub)**: 本物のクラスの代わりに使う、型だけ合わせた最低限の偽物クラス。中身のロジックはほぼ無い
- **注入される依存(Injected Dependency)**: コンストラクタや引数で外から渡してもらう依存。差し替えが簡単
- **静的な依存(Static Dependency)**: コード内に直書きされた依存。差し替えできない。実験時はスタブが必須
- **コンストラクタ**: クラス名と同名・戻り値なしのメソッド。newされた瞬間に1回だけ呼ばれる初期化処理
- **列挙型(enum)**: 決まった選択肢の中から1つを表す型。switch文で複数の決まった選択肢を分岐している型はenumの可能性が高い
- **プロパティ**: 見た目は変数だが中身はメソッドのように動く処理。アクセスする(読む)たびにgetの中身が実行される

---

## 依存関係マッピングの手順

1. クラスの「持ち物」を見る(フィールド一覧)= 依存の洗い出し
2. 「誰から作られるか(注入されるか)」を見る(コンストラクタ/メソッドの引数)
3. 表にまとめる(依存先・型・注入方法)
4. メソッドの引数・戻り値の「型」も依存としてカウントする
5. インターフェースは実装を持たないため依存が少なく、末端に近い(名前が `I〜` で始まるものはインターフェースの可能性が高い、という命名規則から推測できる)
6. 使われ方(呼び出し箇所)から、インターフェースが持つメソッド一覧を逆算できる

### 命名規則から中身を予測するコツ
- `Is〜`で始まるメソッド/プロパティは基本的にboolを返す
- パスカルケースの単語区切りで英文法通りに読むと、メソッド名から中身を予測できる
- `Not`・`Or`・`And`などの語は論理演算子(!, ||, &&)のヒントになる

---

## namespace によるスタブと本物の競合回避

- スタブは最初から `namespace Stubs { }` で囲んでおく(後から競合して直すと修正箇所が増えるため)
- スタブを使うファイル側は `using Stubs;` を1行追加するだけで、Stubs名前空間の中身を全部使える
- クラスが増えても `using Stubs;` は1行のまま(個別指定不要)
- 実装したいクラスが増えたら、新しいインターフェースには新しいスタブクラスを作る。同じインターフェースの別メソッドを試したい場合は既存スタブを育てる

---

## asmdef(アセンブリ定義)によるエラーのパターン

「型が見つからない」エラーには2つの原因パターンがある。

| パターン | 見分け方 | 対処法 |
|---|---|---|
| ゲーム独自クラスが存在しない | 実験用プロジェクトにそもそもファイルが無い | スタブを自作する |
| 外部SDK/パッケージのアセンブリが参照されていない | ファイル(SDK本体)はプロジェクトに存在するのに「見つからない」と出る | 該当フォルダの`.asmdef`を選択→Inspectorの`Assembly Definition References`に追加する |

### 実例
- Live2D: `Live2D.Cubism`(ランタイム用。`.Editor`はエディタ専用なので不要)を追加
- UniRx: `UniRx`を追加
- UniTask(Cysharp): 同様の手順で追加

### 見分け方のコツ
- コンパイルログにSDK自体のファイルについてのwarning/errorが出ていれば、SDK自体はプロジェクトに認識されている証拠
- それでも「型が見つからない」エラーが出続ける場合は、asmdefの参照不足を疑う
- Projectウィンドウの検索窓に `t:asmdef` と入力すると、プロジェクト内の全asmdefファイルを一覧できる
- `warning: Using obsolete custom response file 'mcs.rsp'...` は無関係な古い警告。errorではないので無視してよい

### 新しいエラーパターン: FindObjectOfType の型制約
```
error CS0311: The type 'X' cannot be used as type parameter 'T' in 'Object.FindObjectOfType<T>()'.
There is no implicit reference conversion from 'X' to 'UnityEngine.Object'.
```
- `FindObjectOfType<T>()` は `UnityEngine.Object`(実質MonoBehaviour等)を継承した型しか扱えない
- 対応: スタブクラスに `: MonoBehaviour` を追加し、`using UnityEngine;` も追加する
- 注意: MonoBehaviour化すると `new` でインスタンス化できなくなる

---

## MouseInputProvider の実験(完了)

### 依存関係
| 依存先 | 型 | 注入方法 | 実験での対応 |
|---|---|---|---|
| _utage | UtageManager | コンストラクタ注入 | スタブ作成(MonoBehaviour化) |
| Input | Unity標準 | 静的アクセス | そのまま動く |
| SaveLoadManager.GlobalData | 静的プロパティ | 直接アクセス | スタブ自作が必要 |

### SaveLoadManager スタブ(ネストした静的プロパティの例)
```csharp
namespace Stubs
{
    public class SaveLoadManager
    {
        public static GlobalDataClass GlobalData = new GlobalDataClass();
    }

    public class GlobalDataClass
    {
        public GameOptionClass GameOption = new GameOptionClass();
        public float GetMouseSensitivityFactor() { return 1f; }
    }

    public class GameOptionClass
    {
        public int MouseButtonDecision = 0;
        public int MouseButtonAuto = 1;
        public int MouseButtonSpecial = 2;
    }
}
```

### テストコード(MouseInputTester.cs)で確認できたこと
- MonoBehaviourではない普通のクラスは `new` でその場で生成できる
- `new MouseInputProvider(null)` のように `null` を渡してもエラーにならない → nullチェックしているため(null安全設計)
- 実験の基本方針: クラスの全メソッドをテストする必要はなく、「主役のメソッド」「シンプルで結果がすぐ分かるもの」「今知りたい疑問に直結するもの」を優先して一部だけ検証すれば設計パターンは理解できる

---

## InputManager の実験(完了、MouseOn.None / MouseOn.Osawari 両ルート到達)

### 依存関係(15フィールド)

| 依存先 | 型 | 対応方針 |
|---|---|---|
| _input | IInputProvider | 既存スタブ再利用 |
| _raycaster | CubismRaycaster | asmdef参照で解決、後に本物のLive2Dモデルを導入して対応 |
| _manager | IInputTrigger | StubInputTriggerとして実装 |
| CameraManager | OsawariCameraManager | スタブ作成、コードで直接インスタンスをセット |
| CsManager | CrossSectionManager | 空スタブ(Edge/VariableSizeObjectのケースでのみ使用のため) |
| _utage | UtageManager | 既存スタブ再利用(MonoBehaviour化) |
| _messageWindowUIPresenter | MessageWindowUIPresenter | スタブ作成(MonoBehaviour化) |
| MouseOn(型) | enum | 新規作成 |
| bool系, CompositeDisposable, CancellationTokenSource, BoolReactiveProperty | 標準/UniRx機能 | 対応不要 |

### StubInputTrigger の実装
```csharp
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Raycasting;
using Paidia.satsuki1;

namespace Stubs
{
    public class StubInputTrigger : IInputTrigger
    {
        public void UpdateWhileClicked(CubismRaycastHit[] results, int hitCount, bool isFirst) { }
        public void OnMouseUpTrigger() { }
        public void OnAutoTrigger(CubismRaycastHit[] results, int hitCount) { }
        public void OnInputSpecialTrigger(CubismRaycastHit[] results, int hitCount) { }
        public bool IsClickingAtMesh(CubismRaycastHit[] results, int hitCount) => false;
        public CubismRaycaster GetCubismRaycaster() => null;
        public AbstractOsawari GetOsawariFromDrawable(CubismDrawable mesh)
        {
            return new AbstractOsawari();   // 最終的にnullからインスタンスを返す形に変更(Osawariルート到達のため)
        }
        public Scene GetScene() => null;
    }
}
```
- インターフェースを実装するクラスは全メンバーの実装が必須(1つでも欠けるとCS0535エラー)
- 同じインターフェースから、目的に応じて複数の実装クラスを作れる(本物用のOsawariManager、実験用のStubInputTrigger)。これは依存性注入(DI)の恩恵

### MouseOn (enum)
```csharp
namespace Stubs
{
    public enum MouseOn
    {
        None,
        UI,
        Edge,
        VariableSizeObject,
        Osawari
    }
}
```

### Scene スタブ
```csharp
using UniRx;

namespace Stubs
{
    public class Scene
    {
        public BoolReactiveProperty IsModalWindowOpen = new BoolReactiveProperty(false);
        public BoolReactiveProperty IsResultWindowOpen = new BoolReactiveProperty(false);
    }
}
```
- `.Value`でアクセスされている型はReactiveProperty系と推測できる
- GetScene()がnullを返すスタブにしておけば、_anyModalOpenの判定処理自体がほぼスキップされる

### AbstractOsawari スタブ(最終形、Osawariルート到達版)
```csharp
using Live2D.Cubism.Core;

namespace Stubs
{
    public class AbstractOsawari
    {
        public bool GetConstraints()
        {
            return true;   // MouseOn.None検証時はfalse、Osawariルート到達のためtrueに変更
        }

        public bool CanTouchMesh(CubismDrawable mesh)
        {
            return true;   // 同上
        }
    }
}
```

### OsawariCameraManager スタブ
```csharp
namespace Stubs
{
    public class OsawariCameraManager
    {
        public void CameraZoom(bool zoomIn) { }
        public void SetMousePos(UnityEngine.Vector3 pos) { }
        public void MoveCamera(UnityEngine.Vector3 pos)
        {
            UnityEngine.Debug.Log("MoveCamera が呼ばれた: " + pos);
        }
    }
}
```
- MonoBehaviourを継承していない普通のクラスのため、InspectorのCamera Manager欄にドラッグ&ドロップで設定できない
- 対処: InputManagerTester.cs の Start() でコードから直接インスタンスをセットする
```csharp
TargetInputManager.CameraManager = new OsawariCameraManager();
```

### InputManagerTester.cs(完成形)
```csharp
using UnityEngine;
using Paidia.satsuki1;
using Stubs;
using Live2D.Cubism.Framework.Raycasting;

public class InputManagerTester : MonoBehaviour
{
    public InputManager TargetInputManager;
    public CubismRaycaster ModelRaycaster;

    void Start()
    {
        var mouseInput = new MouseInputProvider(null);
        var stubTrigger = new StubInputTrigger();

        TargetInputManager.CameraManager = new OsawariCameraManager();
        TargetInputManager.ManagedStart(ModelRaycaster, mouseInput, stubTrigger);

        Debug.Log("InputManager を初期化しました");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("マウスクリック検知(Unity標準)");
        }
        Debug.Log("MouseOn: " + TargetInputManager.MouseOn);
    }
}
```

### _raycaster が必須条件だった発見
```csharp
where _raycaster != null && null != Camera.main
```
- ManagedStartの第1引数(raycaster)にnullを渡していると、この条件で毎回弾かれ、Subscribeの中身に一切到達しない
- これは仕様(安全装置)であり、バグではない。CubismRaycasterはLive2Dモデルとの連携が前提の機能
- 検証時は一時的にコメントアウトして原因を切り分け、検証後は元の条件に戻す運用が有効
```csharp
where _raycaster != null && null != Camera.main
//where null != Camera.main //テスト用
```

### 本物のLive2Dモデルの導入手順
1. Cubism Editorの出力データの中から `runtime` フォルダを探す(.cmo3は編集用でUnityでは使わない)
2. runtime フォルダの中身一式をコピー: [モデル名].moc3 / .model3.json / .physics3.json / .cdi3.json / [モデル名].2048フォルダ(テクスチャ) / motionフォルダ
3. Assets内の新しいフォルダにコピー(例: Assets/Live2D/Models/[モデル名])
4. Unityが自動でPrefabやマテリアルを生成する
5. 生成されたPrefabをHierarchyにドラッグ&ドロップしてシーンに配置
6. モデルにCubismRaycasterコンポーネントをAdd Componentで追加
7. InputManagerTesterのModel Raycaster欄にモデルをドラッグ&ドロップ

- 3DモデルとLive2D(2D)モデルは別物。CubismRaycasterは2D専用で3Dモデルには使えない(混同して3Dモデルを一度誤って導入した経緯あり)
- モデルが小さく粗く見える原因は主にMain CameraのOrthographic Size設定(値が小さいほどズームインして大きく見える。本物のプロジェクト参考値は0.8)
- 待機モーションの自動再生は今回未対応(CubismRaycasterでの当たり判定検証には不要と判断、優先度低)

### Raycastの動作確認(検証用の一時ログ)
```csharp
int hitCount = _raycaster.Raycast(ray, array);
Debug.Log("Raycastヒット数: " + hitCount);
```
結果:
```
Raycastヒット数: 0   (モデルの外をクリックした時)
Raycastヒット数: 1   (モデルに当たった時、成功)
```

### MouseOn.Osawari 到達までの3つの関門
```csharp
AbstractOsawari osawariFromDrawable = _manager.GetOsawariFromDrawable(cubismRaycastHit.Drawable);
if (null != osawariFromDrawable && osawariFromDrawable.GetConstraints() && osawariFromDrawable.CanTouchMesh(cubismRaycastHit.Drawable))
{
    return true;
}
```
| 条件 | 修正前 | 修正後 |
|---|---|---|
| null != osawariFromDrawable | null固定 → false | インスタンスを返す → true |
| GetConstraints() | false固定 | true固定 |
| CanTouchMesh(...) | false固定 | true固定 |

この実験で実証できたこと: InputManagerの「Osawariかどうか判定する仕組み」自体は、AbstractOsawariの中身の複雑さとは無関係に、型と戻り値さえ揃っていれば動く。「スタブは型さえ合えば動く」という原則の集大成的な確認。

### InputManager実験、最終結果(完走)

| ルート | 内容 | 状態 |
|---|---|---|
| MouseOn.None | 何もない場所をクリック→カメラ移動(MoveCamera) | 完了 |
| MouseOn.Osawari | Live2Dモデルをクリック→触る処理の入り口 | 完了 |

クリック処理の全体フロー:
```
クリック
  ↓ ①検知(Unity標準のInput.GetMouseButtonDownで確認)
InputGrab()
  ↓ ②UniRxのwhere条件を通過(_raycasterチェックは本物のCubismRaycasterで解決)
Subscribeの中身
  ↓ ③MouseOn判定(None または Osawari)
  ↓-a MouseOn.None: MoveCamera()呼び出し → CameraManager.MoveCamera(座標)
  ↓-b MouseOn.Osawari: Raycast成功→GetOsawariFromDrawableがインスタンス返却→GetConstraints/CanTouchMeshがtrue
```

---

## Osawariシステムの設計思想についての気づき

### UniRxのsourceを分離する理由(可読性・保守性)
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
- 役割の異なる条件を分けることで可読性・保守性が上がる。変数名が「見出し」の役割を果たす
- デバッグ時も、どの段階で処理が止まっているか切り分けやすくなる

### UniRxの実行タイミングの整理
- SetUpRx()自体はManagedStartから1回だけ呼ばれる「組み立てフェーズ」
- Subscribe(async x => {...})の中身は、クリックが発生するたびに何度でも実行される
- `_pressedMouseOn = _mouseOn;` はSubscribe内にあるため、クリックのたびに現在のMouseOn値を記録している

### プロパティの評価タイミング
- MouseOnはプロパティであり、「呼ばれる」というより「読まれる(アクセスされる)たびに、getの中身が実行される」
- 本物のゲームでは、クリックされた瞬間に1回だけMouseOnを読み取り、switch文で処理を振り分けている(無駄のない設計)
- MouseOn自体は「状態を調べるだけ」で何も変更しない。switch文も「調べた結果で分岐するだけ」。実際の状態変更はswitchの中で呼ばれる別メソッド(MoveCameraなど)が担う

### ロジックを外部クラスに委任する設計パターン
```csharp
[NonSerialized]
public bool IsMouseOnUI;
[NonSerialized]
public bool IsMouseOnVariableSize;
[NonSerialized]
public bool IsMouseOnEdgeOfVariableSize;
```
- これらはInputManager自身が計算せず、外部の専門クラス(UI担当、リサイズ担当など)が毎フレーム書き込みに来る想定の「�ല示板」
- 対照的にIsMouseOnOsawariPartsはInputManager自身がロジック(Raycast)を持つ。マウス入力とLive2Dモデルの当たり判定は入力処理の中枢にとって本質的な仕事だから
- 設計パターン名: 関心の分離(Separation of Concerns)、単一責任の原則(Single Responsibility Principle)、疎結合(Loose Coupling)
- public: 外部クラスから直接書き込めるようにするため
- [NonSerialized]: 毎フレーム上書きされる一時的な値であり、保存・Inspector表示する意味がないため
- メリット: 変更に強くなる(UI判定方法が変わってもInputManagerは無変更で済む)、テストしやすくなる(boolを直接セットするだけで状況を作れる、今回のスタブ実験がまさにこれ)

---

## Git操作の記録

### Detached HEAD の罠と復旧方法(実体験)

#### 何が起きたか
GitHub Desktopで「main」ブランチに切り替えたところ、直前にコミットしていた実験内容がHistoryタブから見えなくなった。原因は、そのコミットが「Detached HEAD」(どのブランチにも属さない孤立した状態)で行われていたため。ブランチを切り替えた瞬間に「行き場を失う」。

#### 教訓
- 作業を始める前に、必ずGitHub Desktop上部の「Current branch」が意図したブランチ(通常はmain)になっているか確認する習慣をつける
- Detached HEAD状態でも、コミットしていればデータ自体は消えない(見えなくなるだけで、Gitの中には残っている)

#### 復旧手順
1. コマンドプロンプトでプロジェクトフォルダに移動: `cd [プロジェクトパス]`
2. 操作履歴を確認(読み取り専用、安全な操作): `git reflog`
3. 目的のコミットIDを見つける
4. そのコミットから新しいブランチを作る: `git branch [新しいブランチ名] [コミットID]`
5. 作成したブランチに切り替える: `git checkout [新しいブランチ名]`
6. Unityで「シーンが外部で変更されました」ダイアログが出たらReloadを押す

#### GitHub DesktopとGit(コマンド)の関係
- GitHub Desktopは、Git本体を操作するための道具(GUIアプリ)。裏側は同じGitが動いている
- reflog、branch -f、push --forceのような踏み込んだ操作は、GitHub Desktopのボタンには用意されていない(あえて危険な操作を表に出さない設計)
- History(GitHub Desktop) = 「今立っている道路から見える景色」(現在のブランチから辿れるコミットのみ表示)
- reflog = 「今まで歩いた全ての足跡が記録された行動履歴」(ブランチに接続されているか関係なく全操作を記録)

### experiment ブランチを main に統合した記録

#### 目的
古い初期状態(main)よりも実験内容(experiment-inputmanager)の方を今後の基準にしたいと判断。13日前の状態はバックアップ不要と判断し、mainの中身を置き換えた。

#### 実施手順
1. 現在のブランチがexperiment-inputmanagerであることを確認
2. mainのラベル位置を強制的に付け替える: `git branch -f main experiment-inputmanager`
3. mainブランチに切り替える: `git checkout main`
4. ローカルとorigin/mainの食い違い警告が出る(Pullは押さない、混ざる危険があるため)
5. GitHub上のmainを強制的に上書き: `git push origin main --force`
6. GitHub Desktopで確認: Pull警告が消え、0 changed files / No local changesになれば完了

#### 学んだGitコマンド一覧
| コマンド | 意味 |
|---|---|
| `git reflog` | 操作履歴を全部表示する(読み取り専用、安全) |
| `git branch [新名] [コミットID]` | 指定したコミットの位置に新しいブランチを作る |
| `git checkout [ブランチ名]` | 指定したブランチに切り替える |
| `git branch -f [ブランチ名] [別ブランチ名]` | 既存ブランチの位置を強制的に付け替える |
| `git push origin [ブランチ名] --force` | リモート(GitHub)を強制的に上書きする |

#### 注意点
- `--force`は履歴を上書きする強い操作。共同作業では特に注意(今回は一人での学習用リポジトリのため問題なし)
- 作業前にCurrent branchが意図したブランチになっているか、必ず確認する習慣をつける

### シーンとコードの保存タイミングのズレに関するトラブル
- Git操作(ブランチ切り替え)の前後で、シーンファイル(.unity)上の配置(GameObjectへのアタッチ、Inspector設定)が意図せず巻き戻ることがあった
- 原因の推測: シーンファイルの保存タイミングとGit操作のタイミングがズレていた
- コード(.csファイル)自体は無事で、消えていたのはシーン上の配置のみだった
- 教訓: コード(スクリプト)とシーン(配置)は別々に保存されるため、Git操作の前後でズレることがある。作業の節目でシーンも保存する習慣が必要

---

## Tips集

### VS Codeでのコード自動整形
```
Shift + Alt + F   (Windows)
Shift + Option + F  (Mac)
```
整形後はCtrl+Sでの保存を忘れないこと。

### Unityの検索フィルタ `t:`
`t:`は「type(タイプ、種類)」の略。ファイルの種類でピンポイントに絞り込める。
| 検索 | 意味 |
|---|---|
| `t:Script` | C#スクリプトファイルだけ検索 |
| `t:Prefab` | プレハブだけ検索 |
| `t:Model` | 3Dモデル・キャラクターモデルだけ検索 |
| `t:asmdef` | アセンブリ定義ファイルだけ検索 |

### MainThreadDispatcherについて
- UniRxのObservable機能が最初に使われたタイミングで、自動的に1回だけ生成される常駐オブジェクト
- DontDestroyOnLoadに配置され、ゲーム終了までシーンをまたいで生き続ける
- 毎フレーム、登録されている全てのObservableに「Updateが来た」という通知を配る「配達員」の役割
- EveryGameObjectUpdate()が呼ばれるたびに新しく生成されるわけではない

### 発音・読み方メモ
- glossary(グロッサリー): 用語集
- Docs(ドックス): 資料フォルダ
- study(スタディ): 学習・研究
- Experiment(エクスペリメント): 実験
- IsUtageNotPlayingOrNull(イズ・ウタゲ・ノット・プレイング・オア・ヌル): 宴が再生中でない、またはnullかどうか

---

## 次にやること(候補)

- OsawariManager(本物の司令塔、依存20個以上、正規ルートだが規模が大きい)に進む
- AbstractOsawariのサブクラス群(OsawariHead、OsawariHipなど)を読む
- 今回学んだ設計パターンを、OsawariEventやEventConditionなど別クラスに当てはめて復習する
- CrossSectionManager等、今回空スタブのままにした依存の本格的な検証(MouseOn.Edge / VariableSizeObjectルート)
- 別テーマ(Live2Dの表情システムなど)に切り替える