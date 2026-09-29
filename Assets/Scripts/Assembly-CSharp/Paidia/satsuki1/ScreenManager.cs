using System.Collections.Generic;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class ScreenManager : SingletonManager<ScreenManager>
	{
		private Camera _baseCamera;

		public Dictionary<ScreenSize, (int, int)> ScreenResolution = new Dictionary<ScreenSize, (int, int)>
		{
			{
				ScreenSize.Low,
				(1280, 720)
			},
			{
				ScreenSize.Mid_Low,
				(1600, 900)
			},
			{
				ScreenSize.Mid,
				(1920, 1080)
			},
			{
				ScreenSize.Large,
				(2560, 1440)
			}
		};

		private ScreenSize _screenSize;

		public bool IsFullScreen { get; private set; }

		public void ChangeScreenResolution(ScreenSize size)
		{
			IsFullScreen = false;
			_screenSize = size;
			Screen.SetResolution(ScreenResolution[size].Item1, ScreenResolution[size].Item2, fullscreen: false);
		}

		public void ChangeScreenResolution(int size)
		{
			ChangeScreenResolution((ScreenSize)size);
		}

		public void SetFullScreen(bool fullScreen)
		{
			IsFullScreen = fullScreen;
			if (fullScreen)
			{
				Screen.SetResolution(Screen.width, Screen.height, IsFullScreen);
			}
			else
			{
				ChangeScreenResolution(_screenSize);
			}
		}

		private void Update()
		{
			if (null == Camera.main)
			{
				return;
			}
			if (null == _baseCamera)
			{
				_baseCamera = GameObject.Find("BaseCamera")?.GetComponent<Camera>();
			}
			if (Screen.width * 9 != Screen.height * 16)
			{
				float num = (float)Screen.width / (float)Screen.height;
				if (num > 1.7777778f)
				{
					Rect rect = new Rect((1f - 1.7777778f / num) / 2f, 0f, 1.7777778f / num, 1f);
					Camera.main.rect = rect;
					if (null != _baseCamera)
					{
						_baseCamera.rect = rect;
					}
				}
				else
				{
					Rect rect2 = new Rect(0f, (1f - num / 1.7777778f) / 2f, 1f, num / 1.7777778f);
					Camera.main.rect = rect2;
					if (null != _baseCamera)
					{
						_baseCamera.rect = rect2;
					}
				}
			}
			else
			{
				Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
				if (null != _baseCamera)
				{
					_baseCamera.rect = new Rect(0f, 0f, 1f, 1f);
				}
			}
		}
	}
}
