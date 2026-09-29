using System;
using System.Collections.Generic;
using System.Linq;
using Live2D.Cubism.Core;
using UnityEngine;

public class PhysicsCalculater : MonoBehaviour
{
	[Serializable]
	private class OscillatorParameter
	{
		[SerializeField]
		private float m;

		[SerializeField]
		private float k;

		[SerializeField]
		private float z;

		[SerializeField]
		private int Delay;

		private float c;

		private float _placeNext;

		private float _speedNext;

		private float _placeNow;

		private float _speedNow;

		public OscillatorParameter()
		{
			m = 4f;
			k = 4f;
			z = 1.2f;
			c = z / 2f * Mathf.Sqrt(m * k);
			_placeNow = 0f;
			_speedNow = 0f;
			_placeNext = 0f;
			_speedNext = 0f;
		}

		public float Getm()
		{
			return m;
		}

		public float Getk()
		{
			return k;
		}

		public float Getz()
		{
			return z;
		}

		public float Getc()
		{
			return c;
		}

		public int GetDelay()
		{
			return Delay;
		}

		public float GetPlaceNext()
		{
			return _placeNext;
		}

		public float GetSpeedNext()
		{
			return _speedNext;
		}

		public float GetPlaceNow()
		{
			return _placeNow;
		}

		public float GetSpeedNow()
		{
			return _speedNow;
		}

		public void SetPlaceNext(float x)
		{
			_placeNext = x;
		}

		public void SetPlaceNow(float x)
		{
			_placeNow = x;
		}

		public void SetSpeedNext(float x)
		{
			_speedNext = x;
		}

		public void SetSpeedNow(float x)
		{
			_speedNow = x;
		}

		public void Setc(float x)
		{
			c = x;
		}
	}

	[Serializable]
	private class OutputParameter
	{
		[SerializeField]
		private int ParameterNumber;

		[SerializeField]
		private int OscillatorNumber;

		[SerializeField]
		private float OutputFactor;

		private bool _isUsable;

		public OutputParameter()
		{
			ParameterNumber = 0;
			OscillatorNumber = 0;
			OutputFactor = 1f;
			_isUsable = true;
		}

		public int GetParameterNumber()
		{
			return ParameterNumber;
		}

		public int GetOscillatorNumber()
		{
			return OscillatorNumber;
		}

		public float GetOutputFactor()
		{
			return OutputFactor;
		}

		public bool GetIsUsable()
		{
			return _isUsable;
		}

		public void SetIsUsable(bool Flag)
		{
			_isUsable = Flag;
		}
	}

	public int ModelIndex;

	private CubismModel _model;

	private CubismParameter[] _parameter;

	private OsawariManager _manager;

	private float h;

	private float Fx;

	private float Fy;

	private float F_memory;

	private float F_delay;

	private int Delay_memory = 60;

	public float Threshold = 0.1f;

	private List<float> F_memorize_x = new List<float>();

	private List<float> F_memorize_y = new List<float>();

	private FPSChecker _fpschecker;

	[SerializeField]
	private bool IsAble = true;

	[SerializeField]
	private OscillatorParameter[] oscillatorParameter;

	[SerializeField]
	private OutputParameter[] outputParameter;

	public float ArmCorrectionRate = 0.5f;

	private void Start()
	{
		if (!IsAble)
		{
			return;
		}
		_fpschecker = GameObject.Find("UI_fps").GetComponent<FPSChecker>();
		_manager = UnityEngine.Object.FindObjectOfType<OsawariManager>();
		_model = GetComponent<CubismModel>();
		if (!(null == _model))
		{
			_parameter = new CubismParameter[outputParameter.Length];
			for (int i = 0; i < outputParameter.Length; i++)
			{
				_parameter[i] = _model.Parameters[outputParameter[i].GetParameterNumber()];
			}
			for (int j = 0; j < oscillatorParameter.Length; j++)
			{
				float num = oscillatorParameter[j].Getz();
				float num2 = oscillatorParameter[j].Getm();
				float num3 = oscillatorParameter[j].Getk();
				oscillatorParameter[j].Setc(num / 2f * Mathf.Sqrt(num2 * num3));
			}
			h = 0.05f;
			Fx = 0f;
			Fy = 0f;
			F_memory = 0f;
			F_delay = 0f;
		}
	}

	private float funk1(float x1, float x2, float F, float c, float k, float m)
	{
		return x2;
	}

	private float funk2(float x1, float x2, float F, float c, float k, float m)
	{
		return 0f - F - c / m * x2 - k / m * x1;
	}

	public void Enforce(float Force, bool Assymetry, float AssymetryFactor)
	{
		if (!((double)Mathf.Abs(Force) < 0.001))
		{
			if (Force > Threshold)
			{
				Force = Threshold;
			}
			else if (Force < 0f - Threshold)
			{
				Force = 0f - Threshold;
			}
			if (Assymetry && Force < 0f)
			{
				F_memory = Force * AssymetryFactor;
			}
			else
			{
				F_memory = Force;
			}
		}
	}

	public void InitialPosition(float position, int oscnumber)
	{
		oscillatorParameter[oscnumber].SetPlaceNow(position);
	}

	public void InvalidCalc(int ParameterNumber, bool Switch)
	{
		outputParameter[ParameterNumber].SetIsUsable(Switch);
	}

	public void InvalidCalcByParamNumber(int parameterNumber, bool toSwitch)
	{
		for (int i = 0; i < outputParameter.Length; i++)
		{
			if (outputParameter[i].GetParameterNumber() == parameterNumber)
			{
				InvalidCalc(i, toSwitch);
				break;
			}
		}
	}

	private void Update()
	{
		if (null == _manager || !_manager.IsLoaded || !IsAble)
		{
			return;
		}
		h = Time.deltaTime * 10f;
		if (h > 0.2f)
		{
			h = 0.2f;
		}
		Fx = F_memory * 0.7f;
		Fy = F_memory;
		F_memory = 0f;
		F_memorize_x.Add(Fx);
		F_memorize_y.Add(Fy);
		if (F_memorize_x.Count >= Delay_memory)
		{
			F_memorize_x.RemoveAt(0);
		}
		if (F_memorize_y.Count >= Delay_memory)
		{
			F_memorize_y.RemoveAt(0);
		}
		for (int i = 0; i < oscillatorParameter.Length; i++)
		{
			int num = F_memorize_x.Count - 1 - oscillatorParameter[i].GetDelay();
			float num2 = 0f;
			float num3 = 0f;
			if (num < 0)
			{
				num2 = 0f;
				num3 = 0f;
			}
			else
			{
				num2 = F_memorize_x[num];
				num3 = F_memorize_y[num];
			}
			oscillatorParameter[i].SetPlaceNext(oscillatorParameter[i].GetPlaceNow() + h * funk1(oscillatorParameter[i].GetPlaceNow(), oscillatorParameter[i].GetSpeedNow(), num2, oscillatorParameter[i].Getc(), oscillatorParameter[i].Getk(), oscillatorParameter[i].Getm()));
			oscillatorParameter[i].SetSpeedNext(oscillatorParameter[i].GetSpeedNow() + h * funk2(oscillatorParameter[i].GetPlaceNow(), oscillatorParameter[i].GetSpeedNow(), num3, oscillatorParameter[i].Getc(), oscillatorParameter[i].Getk(), oscillatorParameter[i].Getm()));
		}
		for (int j = 0; j < oscillatorParameter.Length; j++)
		{
			if (oscillatorParameter[j].GetPlaceNext() <= -1f)
			{
				oscillatorParameter[j].SetPlaceNext(-1f);
			}
			else if (oscillatorParameter[j].GetPlaceNext() >= 1f)
			{
				oscillatorParameter[j].SetPlaceNext(1f);
			}
			if (oscillatorParameter[j].GetSpeedNext() <= -1f)
			{
				oscillatorParameter[j].SetSpeedNext(-1f);
			}
			else if (oscillatorParameter[j].GetSpeedNext() >= 1f)
			{
				oscillatorParameter[j].SetSpeedNext(1f);
			}
		}
		for (int k = 0; k < outputParameter.Length; k++)
		{
			if (!outputParameter[k].GetIsUsable())
			{
				continue;
			}
			float num4 = 1f;
			if (_parameter.Count() > 11 && outputParameter[k].GetParameterNumber() == 18 && _parameter[11].name == "ParamArmLeft")
			{
				if (_manager.Preserver.GetLastValue(18) >= 1f)
				{
					num4 = ArmCorrectionRate;
				}
			}
			else if (_parameter.Count() > 12 && outputParameter[k].GetParameterNumber() == 19 && _parameter[12].name == "ParamArmRight" && _manager.Preserver.GetLastValue(19) <= -1f)
			{
				num4 = ArmCorrectionRate;
			}
			_manager.Preserver.SetCorrectionValue(outputParameter[k].GetParameterNumber(), oscillatorParameter[outputParameter[k].GetOscillatorNumber()].GetPlaceNext() * outputParameter[k].GetOutputFactor() * num4, ModelIndex);
		}
		for (int l = 0; l < oscillatorParameter.Length; l++)
		{
			oscillatorParameter[l].SetPlaceNow(oscillatorParameter[l].GetPlaceNext());
			oscillatorParameter[l].SetSpeedNow(oscillatorParameter[l].GetSpeedNext());
		}
	}
}
