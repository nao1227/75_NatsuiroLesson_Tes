using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class ResultUIAnimsTween : MonoBehaviour
	{
		public StatusObject StatusObject;

		public Color TextColorEnabled;

		public Color TextColorDisabled;

		public Image Background;

		public TextMeshProUGUI ExcitedCount;

		public CanvasGroup ExcitedCountTextCg;

		public TextMeshProUGUI EjaculationCount;

		public CanvasGroup EjaculationCountTextCg;

		public TextMeshProUGUI AtomosphereText;

		public CanvasGroup AtomosphereTextCg;

		public CanvasGroup BonusActionCg;

		public TextMeshProUGUI AdditionalExtacy;

		public TextMeshProUGUI EjaculateWhileExtacy;

		public TextMeshProUGUI ExtacyKiss;

		public TextMeshProUGUI HardPiston;

		public TextMeshProUGUI Kiss;

		public TextMeshProUGUI Petting;

		public TextMeshProUGUI PistonWhileGrab;

		public TextMeshProUGUI SlowPiston;

		public TextMeshProUGUI Teaser;

		public TextMeshProUGUI Undress;

		public TextMeshProUGUI WellEarnedStroke;

		public CanvasGroup FavorabilityCg;

		public TextMeshProUGUI FavExp;

		public List<Image> HeartList1;

		public CanvasGroup DevelopmentCg;

		public TextMeshProUGUI DevExp;

		public List<Image> HeartList2;

		public Image Stamp;

		public List<Sprite> StampSprites;

		public InputManager InputManager;

		public Image ClickBlockImage;

		private Sequence _nowPlaying;

		private Sequence _openingSeq;

		private Sequence _stampSeq;

		private bool _resultShown;

		private bool _isAnimating;

		private int _exCount;

		private int _ejCount;

		private int _favPoint;

		private int _devPoint;

		private void Start()
		{
			Transform transform = Background.transform;
			Vector3 localScale = transform.localScale;
			Quaternion localRotation = Stamp.transform.localRotation;
			Initialize();
			_openingSeq = DOTween.Sequence().Append(transform.DOScaleY(localScale.y, 0.3f)).Join(Background.GetComponent<CanvasGroup>().DOFade(1f, 0.1f))
				.Append(transform.DOScaleX(localScale.x, 0.2f))
				.SetId("opening");
			_stampSeq = DOTween.Sequence().Append(Stamp.DOFade(1f, 0.7f)).Join(Stamp.transform.DOScale(1f, 0.7f))
				.Join(Stamp.transform.DORotateQuaternion(localRotation, 0.7f))
				.SetEase(Ease.InQuint);
			_nowPlaying = _openingSeq;
			ClickBlockImage.raycastTarget = false;
		}

		public async UniTask OpenResult(OsawariResult result)
		{
			ClickBlockImage.raycastTarget = true;
			_exCount = result.ExciteCount;
			_ejCount = result.EjaculateCount;
			_favPoint = result.FavPoint;
			_devPoint = result.SensitivityPoint;
			int num = _favPoint + _devPoint;
			if (num <= 49)
			{
				Stamp.sprite = StampSprites[0];
			}
			else if (num <= 99)
			{
				Stamp.sprite = StampSprites[1];
			}
			else if (num <= 149)
			{
				Stamp.sprite = StampSprites[2];
			}
			else
			{
				Stamp.sprite = StampSprites[3];
			}
			AtomosphereText.text = result.AtomosphereText;
			int favorabilityLevel = StatusObject.PersistantStatus.GetFavorabilityLevel();
			int sensitivityLevel = StatusObject.PersistantStatus.GetSensitivityLevel();
			_nowPlaying = _nowPlaying.Play();
			Sequence countupseq = DOTween.Sequence().Append(ExcitedCountTextCg.DOFade(1f, 0.3f)).Join(ExcitedCountTextCg.transform.DOScale(1f, 0.3f))
				.Append(ExcitedCount.DOCounter(0, _exCount, (float)_exCount * 0.1f, addThousandsSeparator: false))
				.Append(EjaculationCountTextCg.DOFade(1f, 0.3f))
				.Join(EjaculationCountTextCg.transform.DOScale(1f, 0.3f))
				.Append(EjaculationCount.DOCounter(0, _ejCount, (float)_ejCount * 0.1f, addThousandsSeparator: false))
				.Append(AtomosphereTextCg.DOFade(1f, 0.3f))
				.Join(AtomosphereTextCg.transform.DOScale(1f, 0.3f))
				.AppendInterval(0.3f)
				.Append(AtomosphereText.DOFade(1f, 1f))
				.Join(AtomosphereText.transform.DOScale(1f, 1f));
			Sequence bonusseq = DOTween.Sequence().Append(BonusActionCg.DOFade(1f, 0.6f)).SetDelay(1f);
			Sequence sequence = DOTween.Sequence();
			for (int i = 0; i < favorabilityLevel; i++)
			{
				Image image = HeartList1[i];
				sequence.Append(image.DOFade(1f, 0.2f)).Join(image.transform.DOScale(1f, 0.2f));
			}
			sequence.SetDelay(0.1f);
			Sequence sequence2 = DOTween.Sequence();
			for (int j = 0; j < sensitivityLevel; j++)
			{
				Image image2 = HeartList2[j];
				sequence2.Append(image2.DOFade(1f, 0.2f)).Join(image2.transform.DOScale(1f, 0.2f));
			}
			sequence2.SetDelay(0.1f);
			Sequence favseq = DOTween.Sequence().Append(FavorabilityCg.DOFade(1f, 0.5f)).Append(sequence)
				.Join(FavExp.DOCounter(0, _favPoint, 0.2f * (float)favorabilityLevel, addThousandsSeparator: false))
				.SetDelay(0.5f);
			Sequence sensitivitySeq = DOTween.Sequence().Append(DevelopmentCg.DOFade(1f, 0.5f)).Append(sequence2)
				.Join(DevExp.DOCounter(0, _devPoint, 0.2f * (float)sensitivityLevel, addThousandsSeparator: false));
			List<Tween> list = new List<Tween>();
			foreach (OsawariAction achievedAction in result.GetAchievedActions())
			{
				switch (achievedAction.name)
				{
				case "TeaserAction":
					list.Add(Teaser.DOColor(Color.black, 1.5f));
					break;
				case "KissAction":
					list.Add(Kiss.DOColor(Color.black, 1.5f));
					break;
				case "PettingAction":
					list.Add(Petting.DOColor(Color.black, 1.5f));
					break;
				case "UndressAction":
					list.Add(Undress.DOColor(Color.black, 1.5f));
					break;
				case "HardPistonAction":
					list.Add(HardPiston.DOColor(Color.black, 1.5f));
					break;
				case "SlowPistonAction":
					list.Add(SlowPiston.DOColor(Color.black, 1.5f));
					break;
				case "PistonWhileBreastGrabAction":
					list.Add(PistonWhileGrab.DOColor(Color.black, 1.5f));
					break;
				case "ExtacyKissAction":
					list.Add(ExtacyKiss.DOColor(Color.black, 1.5f));
					break;
				case "EjaculateWhileExtacyAction":
					list.Add(EjaculateWhileExtacy.DOColor(Color.black, 1.5f));
					break;
				case "WellEarnedStrokeAction":
					list.Add(WellEarnedStroke.DOColor(Color.black, 1.5f));
					break;
				case "AdditionalExtacyAction":
					list.Add(AdditionalExtacy.DOColor(Color.black, 1.5f));
					break;
				}
			}
			Sequence actionTextSeq = DOTween.Sequence();
			foreach (Tween item in list)
			{
				actionTextSeq.Join(item);
			}
			_openingSeq.OnComplete(delegate
			{
				_nowPlaying = countupseq.Play();
			});
			countupseq.OnComplete(delegate
			{
				_nowPlaying = bonusseq.Play();
			});
			bonusseq.OnComplete(delegate
			{
				_nowPlaying = actionTextSeq.Play();
			});
			actionTextSeq.OnComplete(delegate
			{
				_nowPlaying = favseq.Play();
			});
			favseq.OnComplete(delegate
			{
				_nowPlaying = sensitivitySeq.Play();
			});
			sensitivitySeq.OnComplete(delegate
			{
				_nowPlaying = _stampSeq.Play();
			});
			_stampSeq.OnComplete(delegate
			{
				_resultShown = true;
			});
			_nowPlaying.Play();
			_isAnimating = true;
			try
			{
				await UniTask.WaitUntil(() => _resultShown && InputManager.IsClickingAny, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			}
			catch (OperationCanceledException ex)
			{
				throw ex;
			}
			_isAnimating = false;
		}

		private void Initialize()
		{
			Background.transform.localScale = new Vector3(0.01f, 0f, 0f);
			ExcitedCountTextCg.transform.localScale = new Vector3(1.5f, 1.5f, 1.3f);
			EjaculationCountTextCg.transform.localScale = new Vector3(1.5f, 1.5f, 1.3f);
			AtomosphereTextCg.transform.localScale = new Vector3(1.5f, 1.5f, 1.3f);
			AtomosphereText.transform.localScale = Vector3.zero;
			foreach (Image item in HeartList1)
			{
				Color color = item.color;
				item.color = new Color(color.r, color.g, color.b, 0f);
				item.transform.localScale = new Vector3(2f, 2f, 2f);
			}
			foreach (Image item2 in HeartList2)
			{
				Color color2 = item2.color;
				item2.color = new Color(color2.r, color2.g, color2.b, 0f);
				item2.transform.localScale = new Vector3(2f, 2f, 2f);
			}
			Stamp.transform.localScale = new Vector3(2f, 2f, 2f);
			Stamp.transform.localRotation = Quaternion.identity;
			_resultShown = false;
			_isAnimating = false;
		}

		private void Update()
		{
			if (InputManager.IsClickingAny && _isAnimating)
			{
				Skip();
			}
		}

		public void Skip()
		{
			_nowPlaying.Complete(withCallbacks: true);
		}

		public void Hide()
		{
			_nowPlaying.Kill();
			Initialize();
			ClickBlockImage.raycastTarget = false;
		}
	}
}
