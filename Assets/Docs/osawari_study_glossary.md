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
│ └── Testers/ ← テストコード(観察用)
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

