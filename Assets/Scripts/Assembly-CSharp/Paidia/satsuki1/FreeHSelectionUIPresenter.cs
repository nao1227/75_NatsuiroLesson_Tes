using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Paidia.satsuki1
{
	public class FreeHSelectionUIPresenter : MonoBehaviour
	{
		public FreeHSelectionImage HScene1;

		public FreeHSelectionImage HScene2;

		public FreeHSelectionImage HScene3;

		public FreeHSelectionImage HScene4;

		public FreeHSelectionImage EndOfDay;

		public CanvasGroup BGCG;

		public ChoiceWindowPresenter ChoiceWindow;

		private void Start()
		{
			LocalData sd = SaveLoadManager.UnsavedData;
			sd.GlobalFlags.SkipToDay6();
			sd.Days = 99;
			sd.PersistantStatus.SetMaxLevel();
			sd.GlobalFlags.SetFlag(FlagEnum.AllowCondomPutOff, isOn: true);
			sd.GlobalFlags.SetRead(ScenarioLabel.Sub_PinkRotor);
			HScene1.OnClick.Subscribe(async delegate
			{
				_ = 1;
				try
				{
					int num = await ChoiceWindow.WaitForAnswer(new List<string> { "通常", "マイクロビキニ", "バニー服", "猫コス", "SM服" });
					switch (num)
					{
					case 1:
						num = 2;
						break;
					case 2:
						num = 1;
						break;
					}
					SingletonManager<SceneContextManager>.Instance.FreeHCloth = num;
					if (num == 0)
					{
						num = await ChoiceWindow.WaitForAnswer(new List<string> { "下着（水色）", "下着（黒）", "下着（桃）", "下着（青）" });
					}
					SingletonManager<SceneContextManager>.Instance.FreeHUnderwear = num;
					GoToHScene(SceneName.HScene1).Forget();
				}
				catch (NoChoiceException)
				{
				}
			}).AddTo(this);
			HScene2.OnClick.Subscribe(delegate
			{
				GoToHScene(SceneName.HScene2).Forget();
			}).AddTo(this);
			HScene3.OnClick.Subscribe(async delegate
			{
				_ = 1;
				try
				{
					int num = await ChoiceWindow.WaitForAnswer(new List<string> { "通常", "下着を見せる", "マイクロビキニ", "全裸" });
					if (num > 1)
					{
						num++;
					}
					SingletonManager<SceneContextManager>.Instance.FreeHCloth = num;
					if (num <= 1)
					{
						num = await ChoiceWindow.WaitForAnswer(new List<string> { "下着（水色）", "下着（黒）", "下着（桃）", "下着（青）" });
						SingletonManager<SceneContextManager>.Instance.FreeHUnderwear = num;
					}
					GoToHScene(SceneName.HScene3).Forget();
				}
				catch (NoChoiceException)
				{
				}
			}).AddTo(this);
			HScene4.OnClick.Subscribe(async delegate
			{
				try
				{
					int num = await ChoiceWindow.WaitForAnswer(new List<string> { "普通の水着", "マイクロビキニ" });
					if (num == 1)
					{
						num = 2;
					}
					SingletonManager<SceneContextManager>.Instance.FreeHCloth = num;
					GoToHScene(SceneName.HScene4).Forget();
				}
				catch (NoChoiceException)
				{
				}
			}).AddTo(this);
			EndOfDay.OnClick.Subscribe(async delegate
			{
				try
				{
					int relationship = await ChoiceWindow.WaitForAnswer(new List<string> { "生徒と先生", "ヒミツの関係", "求め合う関係", "恋人？", "ラブラブな二人" });
					sd.PersistantStatus.SetRelationship((Relationship)relationship);
					GoToHScene(SceneName.EndOfDay).Forget();
				}
				catch (NoChoiceException)
				{
				}
			}).AddTo(this);
		}

		private async UniTask GoToHScene(SceneName target)
		{
			SingletonManager<SceneContextManager>.Instance.CurrentSceneContext = SceneContext.FreeH;
			SingletonManager<SceneContextManager>.Instance.FreeHTargetScene = target;
			await SceneManager.LoadSceneAsync(1);
		}
	}
}
