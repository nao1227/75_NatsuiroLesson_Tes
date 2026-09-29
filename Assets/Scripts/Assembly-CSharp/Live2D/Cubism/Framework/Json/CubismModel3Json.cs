using System;
using System.IO;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.MotionFade;
using Live2D.Cubism.Framework.MouthMovement;
using Live2D.Cubism.Framework.Physics;
using Live2D.Cubism.Framework.Pose;
using Live2D.Cubism.Framework.Raycasting;
using Live2D.Cubism.Framework.UserData;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Rendering.Masking;
using UnityEngine;

namespace Live2D.Cubism.Framework.Json
{
	[Serializable]
	public sealed class CubismModel3Json
	{
		public delegate object LoadAssetAtPathHandler(Type assetType, string assetPath);

		public delegate Material MaterialPicker(CubismModel3Json sender, CubismDrawable drawable);

		public delegate Texture2D TexturePicker(CubismModel3Json sender, CubismDrawable drawable);

		[Serializable]
		public struct SerializableFileReferences
		{
			[SerializeField]
			public string Moc;

			[SerializeField]
			public string[] Textures;

			[SerializeField]
			public string Pose;

			[SerializeField]
			public SerializableExpression[] Expressions;

			[SerializeField]
			public SerializableMotions Motions;

			[SerializeField]
			public string Physics;

			[SerializeField]
			public string UserData;

			[SerializeField]
			public string DisplayInfo;
		}

		[Serializable]
		public struct SerializableGroup
		{
			[SerializeField]
			public string Target;

			[SerializeField]
			public string Name;

			[SerializeField]
			public string[] Ids;
		}

		[Serializable]
		public struct SerializableExpression
		{
			[SerializeField]
			public string Name;

			[SerializeField]
			public string File;

			[SerializeField]
			public float FadeInTime;

			[SerializeField]
			public float FadeOutTime;
		}

		[Serializable]
		public struct SerializableMotions
		{
			[SerializeField]
			public string[] GroupNames;

			[SerializeField]
			public SerializableMotion[][] Motions;
		}

		[Serializable]
		public struct SerializableMotion
		{
			[SerializeField]
			public string File;

			[SerializeField]
			public string Sound;

			[SerializeField]
			public float FadeInTime;

			[SerializeField]
			public float FadeOutTime;
		}

		[Serializable]
		public struct SerializableHitArea
		{
			[SerializeField]
			public string Name;

			[SerializeField]
			public string Id;
		}

		[SerializeField]
		public int Version;

		[SerializeField]
		public SerializableFileReferences FileReferences;

		[SerializeField]
		public SerializableGroup[] Groups;

		[SerializeField]
		public SerializableHitArea[] HitAreas;

		[NonSerialized]
		private CubismPose3Json _pose3Json;

		[NonSerialized]
		private CubismExp3Json[] _expression3Jsons;

		[NonSerialized]
		private Texture2D[] _textures;

		public string AssetPath { get; private set; }

		private LoadAssetAtPathHandler LoadAssetAtPath { get; set; }

		public byte[] Moc3 => LoadReferencedAsset<byte[]>(FileReferences.Moc);

		public CubismPose3Json Pose3Json
		{
			get
			{
				if (_pose3Json != null)
				{
					return _pose3Json;
				}
				string pose3Json = (string.IsNullOrEmpty(FileReferences.Pose) ? null : LoadReferencedAsset<string>(FileReferences.Pose));
				_pose3Json = CubismPose3Json.LoadFrom(pose3Json);
				return _pose3Json;
			}
		}

		public CubismExp3Json[] Expression3Jsons
		{
			get
			{
				if (FileReferences.Expressions == null)
				{
					return null;
				}
				if (_expression3Jsons == null)
				{
					_expression3Jsons = new CubismExp3Json[FileReferences.Expressions.Length];
					for (int i = 0; i < _expression3Jsons.Length; i++)
					{
						string exp3Json = (string.IsNullOrEmpty(FileReferences.Expressions[i].File) ? null : LoadReferencedAsset<string>(FileReferences.Expressions[i].File));
						_expression3Jsons[i] = CubismExp3Json.LoadFrom(exp3Json);
					}
				}
				return _expression3Jsons;
			}
		}

		public string Physics3Json
		{
			get
			{
				if (!string.IsNullOrEmpty(FileReferences.Physics))
				{
					return LoadReferencedAsset<string>(FileReferences.Physics);
				}
				return null;
			}
		}

		public string UserData3Json
		{
			get
			{
				if (!string.IsNullOrEmpty(FileReferences.UserData))
				{
					return LoadReferencedAsset<string>(FileReferences.UserData);
				}
				return null;
			}
		}

		public string DisplayInfo3Json
		{
			get
			{
				if (!string.IsNullOrEmpty(FileReferences.DisplayInfo))
				{
					return LoadReferencedAsset<string>(FileReferences.DisplayInfo);
				}
				return null;
			}
		}

		public Texture2D[] Textures
		{
			get
			{
				if (_textures == null)
				{
					_textures = new Texture2D[FileReferences.Textures.Length];
					for (int i = 0; i < _textures.Length; i++)
					{
						_textures[i] = LoadReferencedAsset<Texture2D>(FileReferences.Textures[i]);
					}
				}
				return _textures;
			}
		}

		public static CubismModel3Json LoadAtPath(string assetPath)
		{
			return LoadAtPath(assetPath, BuiltinLoadAssetAtPath);
		}

		public static CubismModel3Json LoadAtPath(string assetPath, LoadAssetAtPathHandler loadAssetAtPath)
		{
			if (!(loadAssetAtPath(typeof(string), assetPath) is string text))
			{
				return null;
			}
			CubismModel3Json cubismModel3Json = JsonUtility.FromJson<CubismModel3Json>(text);
			cubismModel3Json.AssetPath = assetPath;
			cubismModel3Json.LoadAssetAtPath = loadAssetAtPath;
			Value value = CubismJsonParser.ParseFromString(text);
			if (!value.Get("FileReferences").GetMap(null).ContainsKey("Motions"))
			{
				return cubismModel3Json;
			}
			string[] array = value.Get("FileReferences").Get("Motions").KeySet()
				.ToArray();
			cubismModel3Json.FileReferences.Motions.GroupNames = array;
			int num = array.Length;
			cubismModel3Json.FileReferences.Motions.Motions = new SerializableMotion[num][];
			for (int i = 0; i < num; i++)
			{
				Value value2 = value.Get("FileReferences").Get("Motions").Get(array[i]);
				int num2 = value2.GetVector(null).ToArray().Length;
				cubismModel3Json.FileReferences.Motions.Motions[i] = new SerializableMotion[num2];
				for (int j = 0; j < num2; j++)
				{
					if (value2.Get(j).GetMap(null).ContainsKey("File"))
					{
						cubismModel3Json.FileReferences.Motions.Motions[i][j].File = value2.Get(j).Get("File").toString();
					}
					if (value2.Get(j).GetMap(null).ContainsKey("Sound"))
					{
						cubismModel3Json.FileReferences.Motions.Motions[i][j].Sound = value2.Get(j).Get("Sound").toString();
					}
					if (value2.Get(j).GetMap(null).ContainsKey("FadeInTime"))
					{
						cubismModel3Json.FileReferences.Motions.Motions[i][j].FadeInTime = value2.Get(j).Get("FadeInTime").ToFloat();
					}
					if (value2.Get(j).GetMap(null).ContainsKey("FadeOutTime"))
					{
						cubismModel3Json.FileReferences.Motions.Motions[i][j].FadeOutTime = value2.Get(j).Get("FadeOutTime").ToFloat();
					}
				}
			}
			return cubismModel3Json;
		}

		private CubismModel3Json()
		{
		}

		public CubismModel ToModel(bool shouldImportAsOriginalWorkflow = false)
		{
			return ToModel(CubismBuiltinPickers.MaterialPicker, CubismBuiltinPickers.TexturePicker, shouldImportAsOriginalWorkflow);
		}

		public CubismModel ToModel(MaterialPicker pickMaterial, TexturePicker pickTexture, bool shouldImportAsOriginalWorkflow = false)
		{
			byte[] moc = Moc3;
			if (moc == null)
			{
				return null;
			}
			CubismModel cubismModel = CubismModel.InstantiateFrom(CubismMoc.CreateFrom(moc));
			if (cubismModel == null)
			{
				return null;
			}
			cubismModel.name = Path.GetFileNameWithoutExtension(FileReferences.Moc);
			CubismRenderer[] renderers = cubismModel.gameObject.AddComponent<CubismRenderController>().Renderers;
			CubismDrawable[] drawables = cubismModel.Drawables;
			if (renderers == null || drawables == null)
			{
				return null;
			}
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].Material = pickMaterial(this, drawables[i]);
			}
			for (int j = 0; j < renderers.Length; j++)
			{
				renderers[j].MainTexture = pickTexture(this, drawables[j]);
			}
			if (HitAreas != null)
			{
				for (int k = 0; k < HitAreas.Length; k++)
				{
					for (int l = 0; l < drawables.Length; l++)
					{
						if (drawables[l].Id == HitAreas[k].Id)
						{
							drawables[l].gameObject.AddComponent<CubismHitDrawable>().Name = HitAreas[k].Name;
							drawables[l].gameObject.AddComponent<CubismRaycastable>();
							break;
						}
					}
				}
			}
			CubismDisplayInfo3Json cubismDisplayInfo3Json = CubismDisplayInfo3Json.LoadFrom(DisplayInfo3Json);
			CubismParameter[] parameters = cubismModel.Parameters;
			for (int m = 0; m < parameters.Length; m++)
			{
				if (IsParameterInGroup(parameters[m], "EyeBlink"))
				{
					if (cubismModel.gameObject.GetComponent<CubismEyeBlinkController>() == null)
					{
						cubismModel.gameObject.AddComponent<CubismEyeBlinkController>();
					}
					parameters[m].gameObject.AddComponent<CubismEyeBlinkParameter>();
				}
				if (IsParameterInGroup(parameters[m], "LipSync"))
				{
					if (cubismModel.gameObject.GetComponent<CubismMouthController>() == null)
					{
						cubismModel.gameObject.AddComponent<CubismMouthController>();
					}
					parameters[m].gameObject.AddComponent<CubismMouthParameter>();
				}
				if (cubismDisplayInfo3Json != null)
				{
					CubismDisplayInfoParameterName cubismDisplayInfoParameterName = parameters[m].gameObject.AddComponent<CubismDisplayInfoParameterName>();
					cubismDisplayInfoParameterName.Name = cubismDisplayInfo3Json.Parameters[m].Name;
					cubismDisplayInfoParameterName.DisplayName = string.Empty;
				}
			}
			if (cubismDisplayInfo3Json != null)
			{
				CubismPart[] parts = cubismModel.Parts;
				for (int n = 0; n < parts.Length; n++)
				{
					CubismDisplayInfoPartName cubismDisplayInfoPartName = parts[n].gameObject.AddComponent<CubismDisplayInfoPartName>();
					cubismDisplayInfoPartName.Name = cubismDisplayInfo3Json.Parts[n].Name;
					cubismDisplayInfoPartName.DisplayName = string.Empty;
				}
			}
			for (int num = 0; num < drawables.Length; num++)
			{
				if (drawables[num].IsMasked)
				{
					cubismModel.gameObject.AddComponent<CubismMaskController>();
					break;
				}
			}
			if (shouldImportAsOriginalWorkflow)
			{
				if (cubismModel.gameObject.GetComponent<CubismUpdateController>() == null)
				{
					cubismModel.gameObject.AddComponent<CubismUpdateController>();
				}
				if (cubismModel.gameObject.GetComponent<CubismParameterStore>() == null)
				{
					cubismModel.gameObject.AddComponent<CubismParameterStore>();
				}
				if (cubismModel.gameObject.GetComponent<CubismPoseController>() == null)
				{
					cubismModel.gameObject.AddComponent<CubismPoseController>();
				}
				if (cubismModel.gameObject.GetComponent<CubismExpressionController>() == null)
				{
					cubismModel.gameObject.AddComponent<CubismExpressionController>();
				}
				if (cubismModel.gameObject.GetComponent<CubismFadeController>() == null)
				{
					cubismModel.gameObject.AddComponent<CubismFadeController>();
				}
			}
			string physics3Json = Physics3Json;
			if (!string.IsNullOrEmpty(physics3Json))
			{
				CubismPhysics3Json cubismPhysics3Json = CubismPhysics3Json.LoadFrom(physics3Json);
				CubismPhysicsController cubismPhysicsController = cubismModel.gameObject.GetComponent<CubismPhysicsController>();
				if (cubismPhysicsController == null)
				{
					cubismPhysicsController = cubismModel.gameObject.AddComponent<CubismPhysicsController>();
				}
				cubismPhysicsController.Initialize(cubismPhysics3Json.ToRig());
			}
			string userData3Json = UserData3Json;
			if (!string.IsNullOrEmpty(userData3Json))
			{
				CubismUserDataBody[] array = CubismUserData3Json.LoadFrom(userData3Json).ToBodyArray(CubismUserDataTargetType.ArtMesh);
				for (int num2 = 0; num2 < drawables.Length; num2++)
				{
					int bodyIndexById = GetBodyIndexById(array, drawables[num2].Id);
					if (bodyIndexById >= 0)
					{
						CubismUserDataTag cubismUserDataTag = drawables[num2].gameObject.GetComponent<CubismUserDataTag>();
						if (cubismUserDataTag == null)
						{
							cubismUserDataTag = drawables[num2].gameObject.AddComponent<CubismUserDataTag>();
						}
						cubismUserDataTag.Initialize(array[bodyIndexById]);
					}
				}
			}
			if (cubismModel.gameObject.GetComponent<Animator>() == null)
			{
				cubismModel.gameObject.AddComponent<Animator>();
			}
			cubismModel.ForceUpdateNow();
			return cubismModel;
		}

		private T LoadReferencedAsset<T>(string referencedFile) where T : class
		{
			string assetPath = Path.GetDirectoryName(AssetPath) + "/" + referencedFile;
			return LoadAssetAtPath(typeof(T), assetPath) as T;
		}

		private static object BuiltinLoadAssetAtPath(Type assetType, string assetPath)
		{
			if (assetType == typeof(byte[]))
			{
				TextAsset textAsset = Resources.Load(assetPath, typeof(TextAsset)) as TextAsset;
				if (!(textAsset != null))
				{
					return null;
				}
				return textAsset.bytes;
			}
			if (assetType == typeof(string))
			{
				TextAsset textAsset2 = Resources.Load(assetPath, typeof(TextAsset)) as TextAsset;
				if (!(textAsset2 != null))
				{
					return null;
				}
				return textAsset2.text;
			}
			return Resources.Load(assetPath, assetType);
		}

		private bool IsParameterInGroup(CubismParameter parameter, string groupName)
		{
			if (Groups == null || Groups.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < Groups.Length; i++)
			{
				if (Groups[i].Name != groupName || Groups[i].Ids == null)
				{
					continue;
				}
				for (int j = 0; j < Groups[i].Ids.Length; j++)
				{
					if (Groups[i].Ids[j] == parameter.name)
					{
						return true;
					}
				}
			}
			return false;
		}

		private int GetBodyIndexById(CubismUserDataBody[] bodies, string id)
		{
			for (int i = 0; i < bodies.Length; i++)
			{
				if (bodies[i].Id == id)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
