using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/AdvITweenPlayer")]
	internal class AdvITweenPlayer : MonoBehaviour
	{
		private iTweenData data;

		private Hashtable hashTbl;

		private Action<AdvITweenPlayer> callbackComplete;

		private bool isColorSprite;

		private int count;

		private string tweenName;

		private bool isPlaying;

		private AdvEffectColor target;

		public bool IsEndlessLoop => data.IsEndlessLoop;

		public bool IsPlaying => isPlaying;

		private iTween Tween { get; set; }

		public bool IsAddType
		{
			get
			{
				iTweenType type = data.Type;
				if (type == iTweenType.MoveAdd || type == iTweenType.RotateAdd || type == iTweenType.ScaleAdd)
				{
					return true;
				}
				return false;
			}
		}

		public void Init(iTweenData data, bool isUnder2DSpace, float pixelsToUnits, float skipSpeed, bool unscaled, Action<AdvITweenPlayer> callbackComplete)
		{
			this.data = data;
			if (data.Type == iTweenType.Stop)
			{
				return;
			}
			this.callbackComplete = callbackComplete;
			data.ReInit();
			hashTbl = iTween.Hash(data.MakeHashArray());
			if (iTweenData.IsPostionType(data.Type) && (!isUnder2DSpace || !hashTbl.ContainsKey("islocal") || !(bool)hashTbl["islocal"]))
			{
				if (hashTbl.ContainsKey("x"))
				{
					hashTbl["x"] = (float)hashTbl["x"] / pixelsToUnits;
				}
				if (hashTbl.ContainsKey("y"))
				{
					hashTbl["y"] = (float)hashTbl["y"] / pixelsToUnits;
				}
				if (hashTbl.ContainsKey("z"))
				{
					hashTbl["z"] = (float)hashTbl["z"] / pixelsToUnits;
				}
			}
			if (skipSpeed > 0f)
			{
				bool flag = hashTbl.ContainsKey("speed");
				if (flag)
				{
					hashTbl["speed"] = (float)hashTbl["speed"] * skipSpeed;
				}
				if (hashTbl.ContainsKey("time"))
				{
					hashTbl["time"] = (float)hashTbl["time"] / skipSpeed;
				}
				else if (!flag)
				{
					hashTbl["time"] = 1f / skipSpeed;
				}
			}
			if (data.Type == iTweenType.ColorTo || data.Type == iTweenType.ColorFrom)
			{
				target = base.gameObject.GetComponent<AdvEffectColor>();
				if (target != null)
				{
					Color tweenColor = target.TweenColor;
					if (data.Type == iTweenType.ColorTo)
					{
						hashTbl["from"] = tweenColor;
						hashTbl["to"] = ParaseTargetColor(hashTbl, tweenColor);
					}
					else if (data.Type == iTweenType.ColorFrom)
					{
						hashTbl["from"] = ParaseTargetColor(hashTbl, tweenColor);
						hashTbl["to"] = tweenColor;
					}
					hashTbl["onupdate"] = "OnColorUpdate";
					isColorSprite = true;
				}
			}
			if (unscaled)
			{
				hashTbl["ignoretimescale"] = true;
			}
			hashTbl["oncomplete"] = "OnCompleteTween";
			hashTbl["oncompletetarget"] = base.gameObject;
			hashTbl["oncompleteparams"] = this;
			tweenName = GetHashCode().ToString();
			hashTbl["name"] = tweenName;
		}

		public void Play()
		{
			TryStoreOldTween();
			isPlaying = true;
			if (data.Type == iTweenType.Stop)
			{
				iTween.Stop(base.gameObject);
				return;
			}
			iTween[] components = GetComponents<iTween>();
			PlaySub();
			if (!IsPlaying)
			{
				return;
			}
			iTween[] components2 = GetComponents<iTween>();
			Tween = null;
			iTween[] array = components2;
			foreach (iTween iTween2 in array)
			{
				bool flag = false;
				iTween[] array2 = components;
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j] == iTween2)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Tween = iTween2;
					break;
				}
			}
			if (Tween == null)
			{
				Debug.LogError("Tween is missing");
			}
			TrySetImmediately();
		}

		private void PlaySub()
		{
			if (isColorSprite)
			{
				iTween.ValueTo(base.gameObject, hashTbl);
				return;
			}
			switch (data.Type)
			{
			case iTweenType.ColorFrom:
				iTween.ColorFrom(base.gameObject, hashTbl);
				return;
			case iTweenType.ColorTo:
				iTween.ColorTo(base.gameObject, hashTbl);
				return;
			case iTweenType.MoveAdd:
				iTween.MoveAdd(base.gameObject, hashTbl);
				return;
			case iTweenType.MoveBy:
				iTween.MoveBy(base.gameObject, hashTbl);
				return;
			case iTweenType.MoveFrom:
				iTween.MoveFrom(base.gameObject, hashTbl);
				return;
			case iTweenType.MoveTo:
				iTween.MoveTo(base.gameObject, hashTbl);
				return;
			case iTweenType.PunchPosition:
				iTween.PunchPosition(base.gameObject, hashTbl);
				return;
			case iTweenType.PunchRotation:
				iTween.PunchRotation(base.gameObject, hashTbl);
				return;
			case iTweenType.PunchScale:
				iTween.PunchScale(base.gameObject, hashTbl);
				return;
			case iTweenType.RotateAdd:
				iTween.RotateAdd(base.gameObject, hashTbl);
				return;
			case iTweenType.RotateBy:
				iTween.RotateBy(base.gameObject, hashTbl);
				return;
			case iTweenType.RotateFrom:
				iTween.RotateFrom(base.gameObject, hashTbl);
				return;
			case iTweenType.RotateTo:
				iTween.RotateTo(base.gameObject, hashTbl);
				return;
			case iTweenType.ScaleAdd:
				iTween.ScaleAdd(base.gameObject, hashTbl);
				return;
			case iTweenType.ScaleBy:
				iTween.ScaleBy(base.gameObject, hashTbl);
				return;
			case iTweenType.ScaleFrom:
				iTween.ScaleFrom(base.gameObject, hashTbl);
				return;
			case iTweenType.ScaleTo:
				iTween.ScaleTo(base.gameObject, hashTbl);
				return;
			case iTweenType.ShakePosition:
				iTween.ShakePosition(base.gameObject, hashTbl);
				return;
			case iTweenType.ShakeRotation:
				iTween.ShakeRotation(base.gameObject, hashTbl);
				return;
			case iTweenType.ShakeScale:
				iTween.ShakeScale(base.gameObject, hashTbl);
				return;
			}
			isPlaying = false;
			Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownType, data.Type.ToString()));
		}

		private bool TrySetImmediately()
		{
			if (Tween == null)
			{
				return false;
			}
			if (data.Loop != iTween.LoopType.none)
			{
				return false;
			}
			object obj = hashTbl["time"];
			if (obj == null)
			{
				return false;
			}
			if ((float)obj > 0f)
			{
				return false;
			}
			object obj2 = hashTbl["delay"];
			if (obj2 != null && (float)obj2 > 0f)
			{
				return false;
			}
			Tween.time = 0f;
			Tween.SendMessage("Start");
			Tween.SendMessage("Update");
			Tween.SendMessage("Update");
			return true;
		}

		private void Update()
		{
			if (isPlaying && Tween == null)
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private bool TryStoreOldTween()
		{
			return false;
		}

		private Color ParaseTargetColor(Hashtable hashTbl, Color color)
		{
			if (hashTbl.Contains("color"))
			{
				color = (Color)hashTbl["color"];
			}
			else
			{
				if (hashTbl.Contains("r"))
				{
					color.r = (float)hashTbl["r"];
				}
				if (hashTbl.Contains("g"))
				{
					color.g = (float)hashTbl["g"];
				}
				if (hashTbl.Contains("b"))
				{
					color.b = (float)hashTbl["b"];
				}
				if (hashTbl.Contains("a"))
				{
					color.a = (float)hashTbl["a"];
				}
			}
			if (hashTbl.Contains("alpha"))
			{
				color.a = (float)hashTbl["alpha"];
			}
			return color;
		}

		public void Cancel()
		{
			iTween.StopByName(base.gameObject, tweenName);
			isPlaying = false;
			UnityEngine.Object.Destroy(this);
		}

		private void OnDestroy()
		{
			if (callbackComplete != null)
			{
				callbackComplete(this);
			}
			callbackComplete = null;
		}

		private void OnCompleteTween(AdvITweenPlayer arg)
		{
			if (!(arg != this))
			{
				count++;
				if (count >= data.LoopCount && !IsEndlessLoop)
				{
					Cancel();
				}
			}
		}

		private void OnColorUpdate(Color color)
		{
			if (target != null)
			{
				target.TweenColor = color;
			}
		}

		public void Write(BinaryWriter writer)
		{
			data.Write(writer);
		}

		public void Read(BinaryReader reader, bool isUnder2DSpace, float pixelsToUnits, bool unscaled)
		{
			iTweenData iTweenData2 = new iTweenData(reader);
			Init(iTweenData2, isUnder2DSpace, pixelsToUnits, 1f, unscaled, null);
		}

		internal static void WriteSaveData(BinaryWriter writer, GameObject go)
		{
			AdvITweenPlayer[] components = go.GetComponents<AdvITweenPlayer>();
			int num = 0;
			AdvITweenPlayer[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsEndlessLoop)
				{
					num++;
				}
			}
			writer.Write(num);
			array = components;
			foreach (AdvITweenPlayer advITweenPlayer in array)
			{
				if (advITweenPlayer.IsEndlessLoop)
				{
					advITweenPlayer.Write(writer);
				}
			}
		}

		internal static void ReadSaveData(BinaryReader reader, GameObject go, bool isUnder2DSpace, float pixelsToUnits, bool unscaled)
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				go.AddComponent<AdvITweenPlayer>().Read(reader, isUnder2DSpace, pixelsToUnits, unscaled);
			}
		}

		public void SkipToEnd()
		{
			iTween[] components = GetComponents<iTween>();
			foreach (iTween iTween2 in components)
			{
				if (!Mathf.Approximately(iTween2.time, 0f))
				{
					iTween2.delay = 0f;
					iTween2.time = 0f;
					iTween2.SendMessage("Update");
				}
			}
		}
	}
}
