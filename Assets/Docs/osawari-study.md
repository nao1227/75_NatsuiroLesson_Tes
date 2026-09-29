# オサワリシステム学習 引き継ぎメモ(第4版)

## これまでの到達点(達成済み)

クリック処理パイプラインは完成し、エラーなしで動作を確認済み。

```
Raycast当たり判定 → MouseOn:Osawari判定 → OsawariManager.UpdateWhileClicked
→ AbstractOsawari.OnClick → OnFirstClick → OnTouchEvents → OsawariEvent.InvokeEvent
→ InvokeCore(ScriptExecuteEvent/MessageEvent等で実際に発火)
```

**HandManager正攻法導入が完了し、クリック→掴み判定→パラメータ更新の一連の流れが
実際のマウス移動量付きで動作することを確認済み。**

- `AbstractOsawari.OnClick`にログを仕込み、クリック時に`CanGrab: False, IsGrabbing: True`
  (Grab成功後にCanGrabを再評価した結果、正しい状態遷移)を確認
- ドラッグ検証で`UpdateParamsCore 呼ばれた: move=(-17.80, -1.30, 0.00)`という
  非ゼロの移動量付き呼び出しを確認。`OnClick → UpdateParams → UpdateParamsCore`という
  本丸の経路が実データ付きで動作することが確定

## 今回のチャットでやりたいこと(残タスク)

**画面上で実際にLive2Dモデルの頭が視覚的に動くかどうかの確認**、およびそのための
パラメータ番号の調整が本丸。

原因候補: `ParameterNumbers`(`OsawariHead`が使う、頭のX/Y軸を何番目のLive2Dパラメータに
対応させるかのテーブル)が、現状`Stubs.ParameterDictionary`のデフォルト値
`HeadX=0, HeadY=1`のままで、今使っているサンプルモデル(hiyori_free_t08)の実際の
パラメータ番号と一致しているか未検証。

**次にやること**:
1. モデル(hiyori_free_t08)のCubismParameter一覧を確認し、頭の角度X・Y
   (`ParamAngleX`・`ParamAngleY`等)が実際に何番目のパラメータか調べる
2. 一致していなければ、`OsawariHead`側のParameterNumbers設定を実モデルに合わせて修正
3. 修正後、再度クリック&ドラッグして見た目の変化を確認

その他の未完了事項:
- `HandManager.MoveHand()`内の`hand.Parameter.UnmanagedIndex`参照は、Live2D Cubism SDKの
  バージョン差で見つからず一時コメントアウト中(手のパラメータ演出のみ影響、頭部には無関係)。
  `CubismParameter.cs`側で実際に使えるプロパティ名を確認し復旧させる必要あり
- `OsawariManager.ManagedStart()`(引数なし、本来の初期化フロー)は現状のテスト環境では
  未実行。そのため`IsLoaded`がfalseのままで`OsawariManager.Update()`/`LateUpdate()`が
  丸ごとスキップされ、`AbstractOsawari.ManagedUpdate`経由の処理(`UpdateHands`による
  手のRelease処理など)が一度も呼ばれていない。結果、一度Grabすると離れない状態になっている。
  `OnMouseUp()`自体にもRelease処理は無いため、正式に`ManagedStart()`を呼ぶ対応が必要
  (ただし他の初期化処理でエラーが連鎖する可能性あり、要注意)

## 構成変更にあたっての注意点(教訓)

- `OsawariManager`と`OsawariHead`は必ず同じGameObjectに乗せる(`GetComponent<T>()`は
  同一GameObjectしか探さないため)
- 同じコンポーネントを複数のGameObjectに重複アタッチすると、Unity側の参照解決が混線し
  NullReferenceExceptionの原因になる

## HandManager導入で踏んだ落とし穴と対処(今後同じ作業をする際の参考)

1. `HandType`・`Hand`(本物、namespaceなし)を追加した際、`AbstractOsawariDependencies.cs`
   (Stubs名前空間)内に同名の`Stubs.HandType`/`Stubs.Hand`が既に存在しており名前衝突で
   コンパイルエラー。該当2ブロックを削除して解消(他のStubsクラスはそのまま残す)
2. `Stubs/HandManager.cs`内の`GetHandToUse()`が`new Hand()`(引数なし)のままだったため
   `new Hand(HandType.Right)`に修正(本物のHandは`Hand(HandType handType)`必須引数
   コンストラクタに変更されているため)
3. 本物`HandManager.cs`(namespace Paidia.satsuki1)導入時、`using Stubs;`が抜けており
   `ParameterValue`/`ValuePreserver`が見つからずエラー。追加して解消
4. テストコード(`InputManagerTester.cs`/`OsawariManagerTester.cs`)内で`new Stubs.HandManager()`
   と明示的にスタブ版を生成していた箇所を`new Paidia.satsuki1.HandManager()`に修正
5. `Stubs.ValuePreserver`が`MonoBehaviour`を継承していなかったため、本物`HandManager`の
   コンストラクタ内`FindObjectOfType<ValuePreserver>()`が失敗。`: MonoBehaviour`を追加し、
   単独ファイル`ValuePreserver.cs`に分離してシーン上のGameObjectにアタッチ
6. `CubismParameter.UnmanagedIndex`がSDKのバージョン差で見つからず、該当1行を一時
   コメントアウトして応急処置(上記「残タスク」参照)

## これまでの主な教訓(繰り返し出てきたパターン)

1. `Stubs`名前空間の自作クラスは`[System.Serializable]`必須。付け忘れると実行時に`null`のまま
   → `NullReferenceException`。対処はフィールド宣言に`= new ○○();`を追記
2. `MonoBehaviour`を継承するクラスは`new`できない。`FindObjectOfType<T>()`や
   `AddComponent<T>()`で明示的に取得・生成する必要がある。逆に元々`new`できていたクラスを
   `MonoBehaviour`化すると、既存の`new`呼び出しがコンパイルエラーになるので要注意
3. `GetComponent<T>()`は同一GameObject内しか探さない
4. `Initialize()`は`ManagedStart()`内の`foreach`で呼ばれるため、`OnTouchEvents.Add(...)`は
   必ず`ManagedStart()`より**前**に行う必要がある
5. 名前が同じクラスが「本物(namespace Paidia.satsuki1)」と「スタブ(namespace Stubs)」の
   両方に存在すると型解決の衝突が起きる。旧スタブは削除するか名前空間を揃える
6. デコンパイル由来のファイルは、たまに`using`が1行抜けていることがある
7. `Mesh`(単数、必須の担当メッシュ申告)と`TouchableMeshs`(複数、実際の判定リスト)の関係:
   `SetTouchableMeshs()`のデフォルト実装が`ManagedStart()`内で必ず`Mesh`から
   `TouchableMeshs`を作り直すため、`Mesh`が未設定だと当たり判定が消える
8. `global::ClassName`は、名前空間の中から名前空間なし(グローバル)のクラスを明示的に
   指す書き方。同名クラスの衝突を未然に防ぐ保険として使われる
9. `Unity標準のイベント関数名(OnMouseUp等)と同じ名前でシグネチャの違うメソッドを定義すると、
   「incorrect signature」という警告が出るが、これはUnityの誤検知であり実害はない
10. C#の`??=`(null合体代入演算子)は「左辺がnullの場合だけ右辺を代入する」という意味で、
    重複初期化による上書き事故を防ぐのに使われる
11. コンパイルエラーが1箇所でも残っていると、Unityは全スクリプトをコンポーネントとして
    認識できなくなり、`Add Component`検索にも一切出てこなくなる

## 主要クラスの役割

- `InputManager`:入力を検知し、いつ何を呼ぶかの手順を管理(SetUpRxでUniRxの監視を設定)
- `OsawariManager`:ゲームロジック全体の司令塔。パーツ・周辺システムを繋ぐ。
  `Update()`/`LateUpdate()`は`IsLoaded`がtrueでないと丸ごとスキップされる点に注意
- `AbstractOsawari`(`OsawariHead`):実際に触られる体のパーツ本体。クリック処理の
  中心メソッド(OnClick→OnFirstClick→UpdateParams→UpdateParamsCore)、補助メソッド
  (GetMouseMove、GetActiveHand等)、離した時のメソッド(OnMouseUp、別系統)、
  毎フレーム裏方メソッド(ManagedUpdate→UpdateHands/UpdateHandParams/UpdateFeelingParams)
  の4系統に分類できる
- `HandManager`(本物、namespace Paidia.satsuki1):MonoBehaviourではない普通のC#クラス。
  「今どのパーツをどちらの手が掴んでいるか」の状態管理。RightHand/LeftHandという
  2つのHandインスタンスを持ち、Grab/Release/IsGrabbingはほぼその委譲
- `Hand`(namespaceなし):GrabbingTarget/Parameterを持つ薄い状態保持クラス。
  `Grab(target)`で`target.GetHandParameter(HandType)`からパラメータを取得する
- `OsawariEvent`(`ScriptExecuteEvent`, `MessageEvent`, `UtageEvent`, `SerialEvent`等):
  条件判定・待機・演出発動を担う汎用フレームワーク。`SerialEvent`は複数の子イベントを
  直列実行するコンテナで、子がUtageEvent/MessageEventの場合は型チェック+キャストで
  各々の(new修飾子で隠された)固有InvokeEventを呼ぶ特殊な仕組みを持つ
- `OsawariBlocker`(本物、namespace Paidia.satsuki1):`Conditions`(BlockType×Value)
  リストを持ち、いずれか1つでも該当すればブロック。BlockType=DayBefore/DayAfter/Flag/
  FlagOff(FlagとロジックがIsOnで完全一致=恐らくバグ)/ScenarioRead/ScenarioUnread/IsDay。
  SceneContext.FreeHの時は無条件で解除される救済ルートあり。`OrBlocker`は子Blockerの
  OR条件版
- `Scene`(本物、namespace Paidia.satsuki1、abstract class : MonoBehaviour):画面
  (シーン)のライフサイクル管理基底クラス。IsModalWindowOpen/IsResultWindowOpenを
  IReadOnlyReactiveProperty型で読み取り専用公開するカプセル化の実例。導入するには
  SceneName/SoundManager/FaceAnimationController/OsawariUIPresenter等の新規スタブが
  多数必要で、HandManagerより規模が大きい作業になる見込み(気分転換で読解のみ実施、
  導入は保留中)

## 使用モデルについて

実機データのモデルではなく、Live2D Cubism公式サンプルモデル(hiyori_free_t08)を使用。
そのためOsawariHeadのMesh/ParameterNumbers等は元ゲームのデータをコピーできず、
サンプルモデルに合わせて自作で値を用意する方針。当たり判定は`HitArea`1個に絞って運用中。