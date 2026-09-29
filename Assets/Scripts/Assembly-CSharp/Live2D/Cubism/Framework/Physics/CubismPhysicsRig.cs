using System;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.Physics
{
	[Serializable]
	public class CubismPhysicsRig
	{
		[SerializeField]
		public CubismPhysicsSubRig[] SubRigs;

		[SerializeField]
		public Vector2 Gravity = CubismPhysics.Gravity;

		[SerializeField]
		public Vector2 Wind = CubismPhysics.Wind;

		[SerializeField]
		public float Fps;

		private float _currentRemainTime;

		[NonSerialized]
		private float[] _parametersCache;

		[NonSerialized]
		private float[] _parametersInputCache;

		public float[] ParametersCache
		{
			get
			{
				return _parametersCache;
			}
			set
			{
				_parametersCache = value;
			}
		}

		public CubismPhysicsController Controller { get; set; }

		public CubismPhysicsSubRig GetSubRig(string name)
		{
			for (int i = 0; i < SubRigs.Length; i++)
			{
				if (SubRigs[i].Name == name)
				{
					return SubRigs[i];
				}
			}
			return null;
		}

		public void Initialize()
		{
			_currentRemainTime = 0f;
			Controller.gameObject.FindCubismModel();
			_parametersCache = new float[Controller.Parameters.Length];
			_parametersInputCache = new float[Controller.Parameters.Length];
			for (int i = 0; i < SubRigs.Length; i++)
			{
				SubRigs[i].Initialize();
			}
		}

		public void Stabilization()
		{
			if (!(Controller == null))
			{
				if (_parametersCache == null)
				{
					_parametersCache = new float[Controller.Parameters.Length];
				}
				if (_parametersCache.Length < Controller.Parameters.Length)
				{
					Array.Resize(ref _parametersCache, Controller.Parameters.Length);
				}
				if (_parametersInputCache == null)
				{
					_parametersInputCache = new float[Controller.Parameters.Length];
				}
				if (_parametersInputCache.Length < Controller.Parameters.Length)
				{
					Array.Resize(ref _parametersInputCache, Controller.Parameters.Length);
				}
				for (int i = 0; i < Controller.Parameters.Length; i++)
				{
					_parametersCache[i] = Controller.Parameters[i].Value;
					_parametersInputCache[i] = _parametersCache[i];
				}
				for (int j = 0; j < SubRigs.Length; j++)
				{
					SubRigs[j].Stabilization();
				}
				Controller.gameObject.FindCubismModel().ForceUpdateNow();
			}
		}

		public void Evaluate(float deltaTime)
		{
			if (0f >= deltaTime)
			{
				return;
			}
			_currentRemainTime += deltaTime;
			if (_currentRemainTime > 5f)
			{
				_currentRemainTime = 0f;
			}
			float num = 0f;
			num = ((!(Fps > 0f)) ? deltaTime : (1f / Fps));
			if (_parametersCache == null)
			{
				_parametersCache = new float[Controller.Parameters.Length];
			}
			if (_parametersCache.Length < Controller.Parameters.Length)
			{
				Array.Resize(ref _parametersCache, Controller.Parameters.Length);
			}
			if (_parametersInputCache == null)
			{
				_parametersInputCache = new float[Controller.Parameters.Length];
			}
			if (_parametersInputCache.Length < Controller.Parameters.Length)
			{
				Array.Resize(ref _parametersInputCache, Controller.Parameters.Length);
				for (int i = 0; i < _parametersInputCache.Length; i++)
				{
					_parametersInputCache[i] = _parametersCache[i];
				}
			}
			while (_currentRemainTime >= num)
			{
				float num2 = num / _currentRemainTime;
				for (int j = 0; j < Controller.Parameters.Length; j++)
				{
					_parametersCache[j] = _parametersInputCache[j] * (1f - num2) + Controller.Parameters[j].Value * num2;
					_parametersInputCache[j] = _parametersCache[j];
				}
				for (int k = 0; k < SubRigs.Length; k++)
				{
					SubRigs[k].Evaluate(num);
				}
				_currentRemainTime -= num;
			}
			float weight = _currentRemainTime / num;
			for (int l = 0; l < SubRigs.Length; l++)
			{
				SubRigs[l].Interpolate(weight);
			}
		}
	}
}
