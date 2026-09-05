# オサワリシステム クリック処理 - 引き継ぎメモ(第2版)

## これまでの成果(達成済み)
クリック処理パイプラインは完成し、エラーなしで動作することを確認済み。

```
Raycast当たり判定 → MouseOn:Osawari判定 → OsawariManager.UpdateWhileClicked
→ AbstractOsawari.OnClick → OnFirstClick → OnTouchEvents → OsawariEvent.InvokeEvent
→ InvokeCore(DummyTouchEventでDebug.Logが実際に発火)
```

途中で発生した「OsawariHeadコンポーネントが2つのGameObjectに重複していた」ことによる
NullReferenceExceptionは、重複を1つに整理して解消済み。

## 次の目標(このチャットでやりたいこと)
1. **構成の整理**: 元のゲームの設計では`OsawariManager`はLive2Dモデルの
   GameObject自体にアタッチされている(解析して判明)。現状の実験プロジェクトでは
   `OsawariManager`が単独の別GameObjectになっているため、**Live2Dモデル
   (`hiyori_free_t08`)側に`OsawariManager`を移動**し、元の設計に近づけたい。
   目的は「今後の学習で構成がわからなくなる/トラブルになるのを防ぐ」ため。
2. **クリック処理の完成**: ダミーイベントの発火確認で終わらせず、実際にクリックしたら
   Live2Dモデルが動く(パラメータが変化する)ところまで完成させたい。

## 構成変更にあたっての注意点(これまでの教訓から)
- `OsawariManager`と`OsawariHead`は**必ず同じGameObject**に乗せる必要がある
  (`OsawariManagerTester.cs`の`GetComponent<OsawariHead>()`が同一GameObjectしか探さないため)。
  移動する際はセットで動かすこと。
- 同じコンポーネントを複数のGameObjectに重複してアタッチすると、Unity側の参照解決が
  混線しNullReferenceExceptionの原因になることが実際にあった。移動する際は、
  元あった場所に重複コンポーネントを残さないよう注意する。
- `InputManagerTester.cs`は`ModelRaycaster`(`CubismRaycaster`、Live2Dモデルに付属)を
  参照している。`OsawariManager`をモデル側に移動しても、この参照自体は
  変わらないはずだが、念のため確認する。
- `OsawariManagerTester.cs`の`TargetOsawariManager.Model.Drawables`など、
  `Model`(`CubismModel`)がLive2Dモデル自身を指している前提のコードが複数ある。
  `OsawariManager`をモデル上に移動した後も、`Model`フィールドの参照先が
  正しくモデル自身になっているか確認が必要。

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

これが呼ばれるための条件(`AbstractOsawari.OnClick`内):
```csharp
if (GetConstraintsCore() && (_handManager.IsGrabbing(this) || (!IsGrabbable && !IsAnimating)))
{
    UpdateParams(ConvertMovementVec3ForParams());
}
```

`_handManager.IsGrabbing(this)`が`true`になるには、`OnFirstClick()`の中の
`_handManager.Grab(hand.HandType, this);`が実行されている必要がある。
`_handManager`は現在`Stubs.HandManager`(スタブ)なので、`IsGrabbing`や`Grab`の
中身が「何もしない」実装になっていないか確認し、必要なら実装を追加する。

`InitializeParams()`(`OsawariHead.cs`)で`parameters[ParameterName.HeadY]`等を
参照しているため、`ParameterNumbers`(`Stubs.ParameterDictionary`)の
`HeadX=0, HeadY=1`という値が、実際に使っているLive2Dモデル(hiyori_free_t08)の
パラメータ番号(`角度X`=0, `角度Y`=1)と一致しているかも確認するとよい
(Cubism公式サンプルモデルは大抵この並びだが、要検証)。

## これまでの主な教訓(繰り返し出てきたパターン)
1. `Stubs`名前空間の自作クラスは`[System.Serializable]`必須。付け忘れると
   Inspectorでシリアライズされず実行時に`null`のまま→`NullReferenceException`。
   対処はフィールド宣言に`= new ○○();`を追記。
2. `MonoBehaviour`を継承するクラス(`StatusObject`等)は`new`できない。
   `FindObjectOfType<T>()`や`AddComponent<T>()`で明示的に取得・生成する必要がある。
3. `GetComponent<T>()`は同一GameObject内しか探さない。
4. `Initialize()`(イベントの初期化)は`ManagedStart()`内の`foreach`で呼ばれるため、
   `OnTouchEvents.Add(...)`は必ず`ManagedStart()`より**前**に行う必要がある。
5. 名前が同じクラスが「本物(namespace Paidia.satsuki1)」と「スタブ」の両方に
   存在すると型解決の衝突が起きる。旧スタブは削除するか名前空間を揃える。
6. 同一コンポーネントを複数のGameObjectに重複させると、Unity側の参照解決が
   混線しNREの原因になることがある。

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
`TouchableMeshs`にまとめて割り当てている(現在は`HitArea`1個に絞られている)。