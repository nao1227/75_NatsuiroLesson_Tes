using UnityEngine;

namespace Paidia.satsuki1
{
	public class FPSChanger : MonoBehaviour
	{
		private bool _low;

		public void SwitchFPS()
		{
			if (_low)
			{
				Application.targetFrameRate = 120;
			}
			else
			{
				Application.targetFrameRate = 20;
			}
			_low = !_low;
		}
	}
}
