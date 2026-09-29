using UniRx;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class SoundMenu : MenuParts
	{
		public Slider slider1;

		public Slider slider2;

		public Slider slider3;

		public Slider slider4;

		public override void SetUp(MainMenuPresenter presenter)
		{
			slider1.value = SaveLoadManager.GlobalData.BGMVolume;
			slider2.value = SaveLoadManager.GlobalData.SEVolume;
			slider3.value = SaveLoadManager.GlobalData.EnvironmentalSEVolume;
			slider4.value = SaveLoadManager.GlobalData.VoiceVolume;
			slider1.OnValueChangedAsObservable().Subscribe(delegate(float x)
			{
				SaveLoadManager.GlobalData.BGMVolume = x;
				SingletonManager<SoundManager>.Instance.ChangeVolume(AudioCategory.BGM, x);
			}).AddTo(this);
			slider2.OnValueChangedAsObservable().Subscribe(delegate(float x)
			{
				SaveLoadManager.GlobalData.SEVolume = x;
				SingletonManager<SoundManager>.Instance.ChangeVolume(AudioCategory.SE, x);
			}).AddTo(this);
			slider3.OnValueChangedAsObservable().Subscribe(delegate(float x)
			{
				SaveLoadManager.GlobalData.EnvironmentalSEVolume = x;
				SingletonManager<SoundManager>.Instance.ChangeVolume(AudioCategory.Environment, x);
			}).AddTo(this);
			slider4.OnValueChangedAsObservable().Subscribe(delegate(float x)
			{
				SaveLoadManager.GlobalData.VoiceVolume = x;
				SingletonManager<SoundManager>.Instance.ChangeVolume(AudioCategory.Voice, x);
			}).AddTo(this);
		}
	}
}
