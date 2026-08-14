### 発見1: `_raycaster`が必須条件だった

```csharp
where _raycaster != null && null != Camera.main
```

- ManagedStart の第1引数(raycaster)に null を渡していたため、この条件で毎回弾かれ、Subscribeの中身に一切到達していなかった
- 一時的にこの条件をコメントアウトして `where null != Camera.main` に差し替えることで、「_raycasterが原因である」ことを確定できた
- 検証後は元の条件に戻し、コメントアウトで変更履歴を残す運用にした

```csharp
where _raycaster != null && null != Camera.main
//where null != Camera.main //テスト用
```

- これは仕様(安全装置)であり、バグではない。CubismRaycaster はLive2Dモデルとの連携が前提の機能のため、本来は本物のLive2Dモデルが必要
- 今回は選択肢A(本物のCubismRaycasterを用意する)を選び、次回以降の課題として残す

### 発見2: CameraManager が null だとNullReferenceExceptionになる
NullReferenceException: Object reference not set to an instance of an object
InputManager.MoveCamera (System.Threading.CancellationToken token) (at InputManager.cs:274)
- InputManagerObj の Inspector で `Camera Manager` フィールドに何も設定していなかったことが原因
- MoveCamera は MouseOn.None(何もない場所をクリックした時)の処理で呼ばれる、今回の目的そのものの処理
- 次回、OsawariCameraManagerスタブのインスタンスをInspector上でセットするところから再開

## Git: Detached HEAD の罠と復旧方法(実体験)

### 何が起きたか
- GitHub Desktopで「main」ブランチに切り替えたところ、直前にコミットしていた
  今日の実験内容(InputManagerのスタブ作成など)が Historyタブから見えなくなった
- 原因: そのコミットが「Detached HEAD」(どのブランチにも属さない孤立した状態)で
  行われていたため。ブランチを切り替えた瞬間に「行き場を失う」

### 教訓
- 作業を始める前に、必ず GitHub Desktop 上部の「Current branch」が
  意図したブランチ(通常は main)になっているか確認する習慣をつける
- Detached HEAD 状態でも、コミットしていればデータ自体は消えない
  (見えなくなるだけで、Gitの中には残っている)

### 復旧手順(実際に成功した手順)
1. コマンドプロンプトでプロジェクトフォルダに移動
   `cd C:\01_UnityGameData\75_NatsuiroLesson_Tes`
2. 操作履歴を確認(読み取り専用、安全な操作)
   `git reflog`
3. 目的のコミットID(今回は 6c4cb12)を見つける
4. そのコミットから新しいブランチを作る
   `git branch experiment-inputmanager 6c4cb12`
5. 作成したブランチに切り替える
   `git checkout experiment-inputmanager`
6. Unityで「シーンが外部で変更されました」ダイアログが出たら Reload を押す
   (今日の実験内容に切り替わる方向なので Reload でOK)

### Push(プッシュ)について
- Push = ローカルのコミット履歴を GitHub 等のオンライン上にアップロードする操作
- Push 自体はブランチを切り替える操作ではない(現在のブランチのままアップロードされるだけ)
- 未コミットの変更が残っている状態でPushしても、その変更は含まれない
- 今回は76個の変更が何か未確認のため、Pushは次回に持ち越し

## 現在の状態(2026/08/13時点)
- main ブランチ: 13日前の状態(InputManager実験前)
- experiment-inputmanager ブランチ: 今日の実験内容を含む(コミット 6c4cb12 由来)、現在ここで作業中
- Branches一覧でも `main` と `experiment-inputmanager` の2つが正しく存在することを確認済み
- 76個の変更(Live2Dアセット関連が中心)が未コミットの状態で残っている → 次回、内容を確認してから対応

## 次にやること
- InputManagerObj の Inspector で CameraManager フィールドに OsawariCameraManager スタブを設定する
- 再度クリックして MoveCamera のログが出るか確認する
- 76個の未コミット変更の中身を確認し、必要ならコミットする
- CubismRaycaster を本物として用意する方向(選択肢A)の検討