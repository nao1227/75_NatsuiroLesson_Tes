using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariLingerie : AbstractOsawari
	{
		protected ParameterValue _lingerie;

		public int LingeriePatternCount = 4;

		protected override void AutoAnimation()
		{
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return 0;
		}

		protected override void InitializeParams()
		{
			_lingerie = new ParameterValue(parameters[ParameterName.Lingerie]);
			_lingerie = _lingerie.Update(GetLingerieValue());
		}

		protected int GetLingerieValue()
		{
			if (SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeH)
			{
				return SingletonManager<SceneContextManager>.Instance.FreeHUnderwear;
			}
			LocalData unsavedData = SaveLoadManager.UnsavedData;
			if (unsavedData.Days < 7 || unsavedData.ForceLingerieNormal)
			{
				return 0;
			}
			return (unsavedData.Days - 7) % LingeriePatternCount;
		}

		protected override void OnLateUpdate()
		{
			parameters[ParameterName.Lingerie].Value = _lingerie.Value;
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
		}

		protected override void UpdateWhileNotClicked()
		{
		}
	}
}
