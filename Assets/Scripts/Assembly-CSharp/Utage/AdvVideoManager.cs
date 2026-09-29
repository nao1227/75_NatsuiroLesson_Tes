using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/AdvVideoManager")]
	public class AdvVideoManager : MonoBehaviour
	{
		private class VideoInfo
		{
			public bool Cancel { get; set; }

			public bool Started { get; set; }

			public bool Canceled { get; set; }

			public VideoPlayer Player { get; set; }
		}

		private AdvEngine engine;

		private Dictionary<string, VideoInfo> videos = new Dictionary<string, VideoInfo>();

		public AdvEngine Engine => this.GetComponentCacheInParent(ref engine);

		private Dictionary<string, VideoInfo> Videos => videos;

		internal void Play(string label, string cameraName, AssetFile file, bool loop, bool cancel)
		{
			Play(label, cameraName, file.UnityObject as VideoClip, loop, cancel);
		}

		internal void Play(string label, string cameraName, VideoClip clip, bool loop, bool cancel)
		{
			VideoInfo info = new VideoInfo
			{
				Cancel = cancel
			};
			Videos.Add(label, info);
			Camera componentInChildren = Engine.EffectManager.FindTarget(AdvEffectManager.TargetType.Camera, cameraName).GetComponentInChildren<Camera>();
			GameObject obj = base.transform.AddChildGameObject(label);
			obj.layer = GetLayerFromCamera(componentInChildren);
			VideoPlayer videoPlayer = obj.AddComponent<VideoPlayer>();
			float volume = Engine.SoundManager.BgmVolume * Engine.SoundManager.MasterVolume;
			videoPlayer.SetDirectAudioVolume(0, volume);
			videoPlayer.isLooping = loop;
			videoPlayer.clip = clip;
			videoPlayer.targetCamera = componentInChildren;
			videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
			videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
			videoPlayer.Play();
			videoPlayer.started += delegate
			{
				OnStarted(info);
			};
			info.Player = videoPlayer;
		}

		private int GetLayerFromCamera(Camera camera)
		{
			int cullingMask = camera.cullingMask;
			for (int i = 0; i < 32; i++)
			{
				if ((cullingMask & (1 << i)) != 0)
				{
					return i;
				}
			}
			return 0;
		}

		private void OnStarted(VideoInfo info)
		{
			info.Started = true;
		}

		internal void Cancel(string label)
		{
			if (Videos[label].Cancel)
			{
				Videos[label].Canceled = true;
				Videos[label].Player.Stop();
			}
		}

		internal bool IsEndPlay(string label)
		{
			if (!Videos.ContainsKey(label))
			{
				return true;
			}
			if (Videos[label].Canceled)
			{
				return true;
			}
			if (!Videos[label].Started)
			{
				return false;
			}
			if (Videos[label].Player.time > 0.0)
			{
				return !Videos[label].Player.isPlaying;
			}
			return false;
		}

		internal void Complete(string label)
		{
			VideoInfo videoInfo = Videos[label];
			videoInfo.Player.targetCamera = null;
			Object.Destroy(videoInfo.Player.gameObject);
			Videos.Remove(label);
		}

		private void Update()
		{
			if (Videos.Count <= 0)
			{
				return;
			}
			foreach (KeyValuePair<string, VideoInfo> video in Videos)
			{
				VideoPlayer player = video.Value.Player;
				if (!(player == null) && player.isPlaying)
				{
					float volume = Engine.SoundManager.BgmVolume * Engine.SoundManager.MasterVolume;
					player.SetDirectAudioVolume(0, volume);
				}
			}
		}
	}
}
