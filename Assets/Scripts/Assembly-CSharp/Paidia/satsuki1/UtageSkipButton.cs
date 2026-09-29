using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utage;

namespace Paidia.satsuki1
{
	public class UtageSkipButton : MonoBehaviour
	{
		public AdvEngine engine;

		public Image SkipButton;

		public Image AutoButton;

		public Color EnabledColor;

		private void Start()
		{
			(from x in SkipButton.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				engine.Config.IsSkip = !engine.Config.IsSkip;
			}).AddTo(this);
			(from x in AutoButton.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				engine.Config.IsAutoBrPage = !engine.Config.IsAutoBrPage;
			}).AddTo(this);
		}

		private void Update()
		{
			SkipButton.color = (engine.Config.IsSkip ? EnabledColor : Color.white);
			AutoButton.color = (engine.Config.IsAutoBrPage ? EnabledColor : Color.white);
		}
	}
}
