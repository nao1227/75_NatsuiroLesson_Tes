using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class FreeScenarioUIPresenter : MonoBehaviour
	{
		public ScrollRect Scroll;

		public TextMeshProUGUI LabelPrefab;

		private UtageManager _utage;

		public CanvasGroup CG;

		private ScenarioLabel _label;

		private TextMeshProUGUI _selected;

		public Image Main;

		public Image Events;

		public Image Talks;

		public FreeSccenarioCategory TargetScenarioCategory;

		public NameStringData Data;

		public Image Shadow;

		private Vector3 _shadowOriginalPos;

		public Image PlayButton;

		public Image BackButton;

		private void Start()
		{
			_utage = Object.FindObjectOfType<UtageManager>();
			if (!(null == _utage))
			{
				_shadowOriginalPos = Shadow.transform.localPosition;
				SetScenarioTitles();
				(from x in Main.OnPointerClickAsObservable()
					where x.button == PointerEventData.InputButton.Left
					select x).Subscribe(delegate
				{
					TargetScenarioCategory = FreeSccenarioCategory.Main;
					SetScenarioTitles();
					Shadow.transform.localPosition = _shadowOriginalPos;
				}).AddTo(this);
				(from x in Events.OnPointerClickAsObservable()
					where x.button == PointerEventData.InputButton.Left
					select x).Subscribe(delegate
				{
					TargetScenarioCategory = FreeSccenarioCategory.Event;
					SetScenarioTitles();
					Shadow.transform.localPosition = _shadowOriginalPos + new Vector3(110f, 0f, 0f);
				}).AddTo(this);
				(from x in Talks.OnPointerClickAsObservable()
					where x.button == PointerEventData.InputButton.Left
					select x).Subscribe(delegate
				{
					TargetScenarioCategory = FreeSccenarioCategory.Talk;
					SetScenarioTitles();
					Shadow.transform.localPosition = _shadowOriginalPos + new Vector3(220f, 0f, 0f);
				}).AddTo(this);
				(from _ in PlayButton.OnPointerEnterAsObservable()
					where _label != ScenarioLabel.None
					select _).Subscribe(delegate
				{
					PlayButton.color = new Color(PlayButton.color.r, PlayButton.color.g, PlayButton.color.b, 0.5f);
				}).AddTo(this);
				PlayButton.OnPointerExitAsObservable().Subscribe(delegate
				{
					PlayButton.color = new Color(PlayButton.color.r, PlayButton.color.g, PlayButton.color.b, 0f);
				}).AddTo(this);
				(from x in PlayButton.OnPointerClickAsObservable()
					where x.button == PointerEventData.InputButton.Left
					select x).Subscribe(delegate
				{
					Play();
					PlayButton.color = new Color(PlayButton.color.r, PlayButton.color.g, PlayButton.color.b, 0f);
				}).AddTo(this);
				BackButton.OnPointerEnterAsObservable().Subscribe(delegate
				{
					BackButton.color = new Color(BackButton.color.r, BackButton.color.g, BackButton.color.b, 0.5f);
				}).AddTo(this);
				BackButton.OnPointerExitAsObservable().Subscribe(delegate
				{
					BackButton.color = new Color(BackButton.color.r, BackButton.color.g, BackButton.color.b, 0f);
				}).AddTo(this);
				(from x in BackButton.OnPointerClickAsObservable()
					where x.button == PointerEventData.InputButton.Left
					select x).Subscribe(delegate
				{
					ReturnToTitle();
					BackButton.color = new Color(BackButton.color.r, BackButton.color.g, BackButton.color.b, 0f);
				}).AddTo(this);
			}
		}

		protected void ClearScrollView()
		{
			foreach (Transform componentInChild in Scroll.content.GetComponentInChildren<Transform>())
			{
				Object.Destroy(componentInChild.gameObject);
			}
		}

		protected void SetScenarioTitles()
		{
			ClearScrollView();
			int num = 0;
			foreach (ScenarioLabel label in Data.GetScenarioLabelsByCategory(TargetScenarioCategory))
			{
				TextMeshProUGUI labelButton = Object.Instantiate(LabelPrefab, Scroll.content.transform);
				labelButton.rectTransform.anchoredPosition = new Vector2(0f, -num * 65);
				labelButton.text = StringsManager.GetScenarioLabel(label);
				(from _ in labelButton.OnPointerClickAsObservable()
					where _.button == PointerEventData.InputButton.Left
					where !_utage.IsPlaying
					select _).Subscribe(delegate
				{
					_label = label;
					if (null != _selected)
					{
						_selected.GetComponentInChildren<CanvasGroup>().alpha = 0f;
					}
					_selected = labelButton;
					_selected.GetComponentInChildren<CanvasGroup>().alpha = 1f;
				}).AddTo(this);
				num++;
			}
		}

		public async void Play()
		{
			if (_label != ScenarioLabel.None)
			{
				CG.alpha = 0f;
				CG.blocksRaycasts = false;
				await _utage.ShowUtageTextWithoutReadFlag(_label, this.GetCancellationTokenOnDestroy());
				CG.alpha = 1f;
				CG.blocksRaycasts = true;
			}
		}

		public void ReturnToTitle()
		{
			SceneManager.LoadSceneAsync(0);
		}
	}
}
