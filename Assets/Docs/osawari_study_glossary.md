# 学習ログ: InputManager をコンパイルが通る状態にする

## 今回の目的
InputManager を実験用プロジェクトで動かし、「クリックしたら動く処理」を優先して検証する準備を整える。

## StubInputTrigger の実装

IInputTrigger を実装する実験専用の代役クラス。OsawariManager(本物)の代わりに、最低限の空実装で InputManager に渡す。

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
        public AbstractOsawari GetOsawariFromDrawable(CubismDrawable mesh) => null;
        public Scene GetScene() => null;
    }
}
```

### 気づき: インターフェースを実装するクラスは全メンバーの実装が必須
- `class X : IInputTrigger` と書いた時点で、インターフェースの全8メソッドの実装が必須になる(1つでも欠けるとCS0535エラー)
- 同じインターフェースから、目的に応じて複数の実装クラスを作れる(本物用の OsawariManager、実験用の StubInputTrigger)
- これは依存性注入(DI)の恩恵そのもの。InputManager 側は「IInputTrigger を満たしていること」しか気にしないため、渡す実体を差し替えられる

## InputManager.cs 本体をプロジェクトに追加した際のエラー分類

| エラーの型 | 原因 | 対応 |
|---|---|---|
| `Cysharp`(UniTask) | asmdef参照不足 | GameScripts.asmdefにUniTaskを追加 |
| `MouseOn` | ゲーム独自の型、switch文の選択肢に使われている | enumとしてスタブ作成 |
| `OsawariCameraManager` | ゲーム独自クラス | スタブ作成(MoveCameraのみDebug.Logを仕込み、後で動作確認に使う) |
| `CrossSectionManager` | ゲーム独自クラス | 今回の目的では空スタブでよい(Edge/VariableSizeObjectのケースでのみ使用) |
| `MessageWindowUIPresenter` | ゲーム独自クラス | スタブ作成 |

## MouseOn は enum だった

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

### 見分け方のコツ
- switch文で複数の決まった選択肢(`case MouseOn.None:` 等)を分岐している型は enum の可能性が高い
- プロパティの戻り値が「状態を表す固定の選択肢」になっている場合も同様

## asmdefパターン、3件目: UniTask(Cysharp)

- GameScripts.asmdef に UniTask(Cysharp.Threading.Tasks)の参照も不足していた
- Live2D、UniRxと同じ手順(Assembly Definition References に追加)で解決

## 新しいエラーパターン: FindObjectOfType の型制約