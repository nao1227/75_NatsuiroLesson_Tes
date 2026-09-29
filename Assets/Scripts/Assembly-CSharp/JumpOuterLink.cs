using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JumpOuterLink : MonoBehaviour
{
	private void Start()
	{
		(from x in GetComponent<Image>().OnPointerClickAsObservable()
			where x.button == PointerEventData.InputButton.Left
			select x).Subscribe(delegate
		{
			Application.OpenURL(new Uri("https://docs.google.com/forms/d/e/1FAIpQLScx6hkcLWXUilisUiMMcTEkiwnj8_x3bWD6P-yJ_6biMYkfmQ/viewform").AbsoluteUri);
		}).AddTo(this);
	}
}
