using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/UguiBackgroundRaycastReciever")]
	public class UguiBackgroundRaycastReciever : MonoBehaviour
	{
		[SerializeField]
		private UguiBackgroundRaycaster raycaster;

		public UguiBackgroundRaycaster Raycaster
		{
			get
			{
				return this.GetComponentCacheFindIfMissing(ref raycaster);
			}
			set
			{
				raycaster = value;
			}
		}

		private void OnEnable()
		{
			Raycaster.AddTarget(base.gameObject);
		}

		private void OnDisable()
		{
			UguiBackgroundRaycaster uguiBackgroundRaycaster = Raycaster;
			if (uguiBackgroundRaycaster != null)
			{
				uguiBackgroundRaycaster.RemoveTarget(base.gameObject);
			}
		}
	}
}
