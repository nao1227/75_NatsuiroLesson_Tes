using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtageExtensions;

namespace Paidia.satsuki1
{
	public class MainSceneButton : MonoBehaviour
	{
		public MainSceneButtonID ButtonID;

		public Image Image;

		private Subject<MainSceneButtonID> _onClick;

		private Subject<MainSceneButtonID> _onMouseEnter;

		private Subject<MainSceneButtonID> _onMouseExit;

		private bool _isLoaded;

		public bool Enabled { get; protected set; } = true;

		public IObservable<MainSceneButtonID> OnClick => _onClick;

		public IObservable<MainSceneButtonID> OnMouseEnter => _onMouseEnter;

		public IObservable<MainSceneButtonID> OnMouseExit => _onMouseExit;

		public void ManagedStart()
		{
			_onClick = new Subject<MainSceneButtonID>();
			_onMouseEnter = new Subject<MainSceneButtonID>();
			_onMouseExit = new Subject<MainSceneButtonID>();
			(from _ in Image.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where Enabled
				select _).Subscribe(delegate
			{
				_onClick.OnNext(ButtonID);
			}).AddTo(this);
			(from _ in Image.OnPointerEnterAsObservable()
				where Enabled
				select _).Subscribe(delegate
			{
				_onMouseEnter.OnNext(ButtonID);
			}).AddTo(this);
			(from _ in Image.OnPointerExitAsObservable()
				where Enabled
				select _).Subscribe(delegate
			{
				_onMouseExit.OnNext(ButtonID);
			}).AddTo(this);
			_isLoaded = true;
		}

		public void SetEnabled(bool enabled)
		{
			Enabled = enabled;
			if (enabled)
			{
				Image.SetAlpha(1f);
			}
			else
			{
				Image.SetAlpha(0.5f);
			}
		}

		private void Update()
		{
			if (_isLoaded)
			{
				Image.color = SaveLoadManager.GlobalData.GameOption.UIColor;
				if (Enabled)
				{
					Image.SetAlpha(1f);
				}
				else
				{
					Image.SetAlpha(0.5f);
				}
			}
		}
	}
}
