using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class DebugWindow : MonoBehaviour
	{
		public CanvasGroup CG;

		public FlagContent FlagContent;

		public TMP_InputField DaysInputField;

		public TMP_InputField FavourabilityInputField;

		public TMP_InputField SensitivityInputField;

		public GameObject ScrollViewContent;

		public Image BackButton;

		private int _days;

		private int _favourability;

		private int _sensitivity;

		private bool _loaded;

		private Dictionary<FlagEnum, bool> _changedFlags;

		private int _originalDay;

		private void Start()
		{
			CG.alpha = 0f;
			CG.blocksRaycasts = false;
			CG.interactable = false;
			_changedFlags = new Dictionary<FlagEnum, bool>();
			SetRx().Forget();
		}

		private async UniTask SetRx()
		{
			_originalDay = SaveLoadManager.UnsavedData.Days;
			await UniTask.WaitUntil(() => SaveLoadManager.UnsavedData != null);
			DaysInputField.onValueChanged.AddListener(delegate(string x)
			{
				_days = Mathf.Clamp(int.Parse(x), 1, 31);
			});
			FavourabilityInputField.onValueChanged.AddListener(delegate(string x)
			{
				_favourability = int.Parse(x);
			});
			SensitivityInputField.onValueChanged.AddListener(delegate(string x)
			{
				_sensitivity = int.Parse(x);
			});
			BackButton.OnPointerClickAsObservable().Subscribe(delegate
			{
				Close();
			});
			foreach (FlagEnum flag in Enum.GetValues(typeof(FlagEnum)))
			{
				FlagContent go = UnityEngine.Object.Instantiate(FlagContent, ScrollViewContent.transform);
				go.SetName(flag.ToString());
				go.SetState(SaveLoadManager.UnsavedData.GlobalFlags.IsOn(flag));
				go.On.OnPointerClickAsObservable().Subscribe(delegate
				{
					go.SetState(state: true);
					_changedFlags[flag] = true;
				});
				go.Off.OnPointerClickAsObservable().Subscribe(delegate
				{
					go.SetState(state: false);
					_changedFlags[flag] = false;
				});
			}
			_loaded = true;
		}

		private void Close()
		{
			CG.alpha = 0f;
			CG.blocksRaycasts = false;
			CG.interactable = false;
			SaveLoadManager.UnsavedData.PersistantStatus.AddFavourability(_favourability - SaveLoadManager.UnsavedData.PersistantStatus.Favorability, debug: true);
			SaveLoadManager.UnsavedData.PersistantStatus.AddSensitivity(_sensitivity - SaveLoadManager.UnsavedData.PersistantStatus.Sensitivity, debug: true);
			foreach (KeyValuePair<FlagEnum, bool> changedFlag in _changedFlags)
			{
				SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(changedFlag.Key, changedFlag.Value);
			}
			if (SaveLoadManager.UnsavedData.Days != _days)
			{
				SaveLoadManager.UnsavedData.Days = _days - 1;
				UnityEngine.Object.FindObjectOfType<AdventureScene>().IsWaitingForReload = true;
			}
		}

		public void Open()
		{
			_changedFlags.Clear();
			CG.alpha = 1f;
			CG.blocksRaycasts = true;
			CG.interactable = true;
			DaysInputField.text = SaveLoadManager.UnsavedData.Days.ToString();
			FavourabilityInputField.text = SaveLoadManager.UnsavedData.PersistantStatus.Favorability.ToString();
			SensitivityInputField.text = SaveLoadManager.UnsavedData.PersistantStatus.Sensitivity.ToString();
		}

		private void Update()
		{
			if (!_loaded)
			{
				return;
			}
			if (Input.GetKeyDown(KeyCode.F1))
			{
				if (CG.alpha == 0f)
				{
					Open();
				}
				else
				{
					Close();
				}
			}
			if (Input.GetKeyDown(KeyCode.F2))
			{
				UnityEngine.Object.FindObjectOfType<StatusObject>()?.TemporaryStatus.Feelings.AddExciteValue(99999999);
			}
			if (Input.GetKeyDown(KeyCode.F3))
			{
				Time.timeScale = 0.1f;
			}
			if (Input.GetKeyDown(KeyCode.F4))
			{
				Time.timeScale = 1f;
			}
			if (Input.GetKeyDown(KeyCode.F6))
			{
				Debug.Log(SaveLoadManager.UnsavedData.GlobalFlags);
			}
		}
	}
}
