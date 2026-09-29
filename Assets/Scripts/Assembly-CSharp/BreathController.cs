using System;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Paidia.satsuki1;
using UnityEngine;

public class BreathController : MonoBehaviour
{
	private CubismModel _model;

	private CubismParameter _BreathParam;

	private CubismParameter _MouthParam;

	private CubismParameter _HeadYParam;

	private float _Breath;

	private float _DelayedBreath;

	private float _t;

	public BreathFrequency Frequency;

	private float _stoppedTime;

	private bool _flagBreath;

	private bool _playFlag;

	private SoundController _soundController;

	private TemporaryStatus _status;

	[SerializeField]
	private float DefaultFrequency = 1f;

	[SerializeField]
	private float Degree_Breath = 1f;

	[SerializeField]
	private float Degree_Mouth = 0.3f;

	[SerializeField]
	private float Degree_HeadY = 1f;

	[SerializeField]
	private float RestartTime = 1f;

	[SerializeField]
	private float Delay = 0.5f;

	[SerializeField]
	private string[] SoundName;

	public int BreathParamNum = 14;

	public int MouthParamNum = 13;

	public int HeadYParamNum = 1;

	private bool _loaded;

	public float OriginalFrequency { get; private set; }

	public void ManagedStart()
	{
		_model = GetComponent<OsawariManager>().Model;
		_BreathParam = _model.Parameters[BreathParamNum];
		_MouthParam = _model.Parameters[MouthParamNum];
		_HeadYParam = _model.Parameters[HeadYParamNum];
		_t = 0f;
		_Breath = 0f;
		_flagBreath = false;
		RestartTime = 0f;
		_stoppedTime = 0f;
		OriginalFrequency = DefaultFrequency - 1f;
		Frequency = new BreathFrequency(OriginalFrequency);
		_soundController = GetComponent<SoundController>();
		OsawariManager component = GetComponent<OsawariManager>();
		_status = component.TemporaryStatus;
		_loaded = true;
	}

	public void StopBreezing()
	{
		_flagBreath = false;
		_stoppedTime = Time.time;
		RestartTime = 0f;
	}

	public void StopBreezingTime(float t)
	{
		_flagBreath = false;
		if (t >= 0f)
		{
			RestartTime = t;
		}
		_stoppedTime = Time.time;
	}

	public void SetFrequency(float _x)
	{
		OriginalFrequency = _x;
	}

	public void ManagedUpdate()
	{
		_t += Time.deltaTime * 4f;
		if (!_flagBreath && Time.time - _stoppedTime > RestartTime)
		{
			_flagBreath = true;
			_t = 0f;
		}
		if (_flagBreath)
		{
			if (null == SingletonManager<SoundManager>.Instance || !SingletonManager<SoundManager>.Instance.Isvalid)
			{
				return;
			}
			_ = SingletonManager<SoundManager>.Instance.IsPlayingVoice;
			_Breath = 0.5f * Mathf.Sin((Frequency.GetValue() + 1f) * (_t - (float)Math.PI / 2f)) + 0.5f;
			_DelayedBreath = 0.5f * Mathf.Sin((Frequency.GetValue() + 1f) * (_t - (float)Math.PI / 2f) - Delay) + 0.5f;
		}
		else
		{
			_Breath *= 0.9f;
			_DelayedBreath *= 0.9f;
		}
		OriginalFrequency *= 0.999f;
	}

	private void LateUpdate()
	{
		if (_loaded)
		{
			_BreathParam.BlendToValue(CubismParameterBlendMode.Additive, Degree_Breath * _Breath);
			_MouthParam.BlendToValue(CubismParameterBlendMode.Additive, Degree_Mouth * _Breath);
			_ = _HeadYParam.Value;
			_HeadYParam.BlendToValue(CubismParameterBlendMode.Additive, Degree_HeadY * _DelayedBreath);
		}
	}

	private void OnDestroy()
	{
		_flagBreath = false;
	}
}
