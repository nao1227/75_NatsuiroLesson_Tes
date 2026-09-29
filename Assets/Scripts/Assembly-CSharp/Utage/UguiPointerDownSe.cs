using UnityEngine;
using UnityEngine.EventSystems;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/UguiPointerDownSe")]
	public class UguiPointerDownSe : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		public AudioClip se;

		public SoundPlayMode playMode;

		public void OnPointerDown(PointerEventData data)
		{
			int pointerId = data.pointerId;
			if ((uint)(pointerId - -1) <= 1u)
			{
				PlaySe(playMode, se);
			}
		}

		private void PlaySe(SoundPlayMode mode, AudioClip clip)
		{
			if (clip != null)
			{
				SoundManager instance = SoundManager.GetInstance();
				if ((bool)instance)
				{
					instance.PlaySe(clip, clip.name, mode);
				}
				else
				{
					AudioSource.PlayClipAtPoint(clip, Vector3.zero);
				}
			}
		}
	}
}
