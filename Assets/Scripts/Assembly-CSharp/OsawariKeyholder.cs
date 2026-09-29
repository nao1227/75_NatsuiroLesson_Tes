using System;
using DG.Tweening;
using Paidia.satsuki1;
using UnityEngine;

public class OsawariKeyholder : OsawariWithoutHand
{
	private ParameterValue _keyholder;

	private bool _keyholderFlag;

	private bool _animating;

	protected override void AutoAnimation()
	{
	}

	protected override void InitializeParams()
	{
		_keyholder = new ParameterValue(parameters[ParameterName.KeyHolder]);
		_animating = false;
		_keyholderFlag = ((SaveLoadManager.UnsavedData.Days > 1) ? true : false);
	}

	protected override void OnLateUpdate()
	{
		SetLive2D(ParameterName.KeyHolder, _keyholder);
		SetLive2D(ParameterName.KeyHolderFlag, _keyholderFlag ? 1f : 0f);
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
	}

	protected override void UpdateWhileNotClicked()
	{
	}

	public override void OnMouseUp(bool fromCancel = false)
	{
		_animating = true;
		DOTween.Sequence().Append(DOVirtual.Float(_keyholder.Value, Math.Abs(_keyholder.Value - 1f), 1f, delegate(float x)
		{
			_keyholder = _keyholder.Update(x);
		})).OnComplete(delegate
		{
			_animating = false;
		})
			.Play();
	}

	protected override bool GetConstraintsCore()
	{
		if (_keyholderFlag)
		{
			return !_animating;
		}
		return false;
	}
}
