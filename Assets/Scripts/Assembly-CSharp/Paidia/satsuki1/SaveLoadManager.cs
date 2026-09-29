using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Paidia.Utils;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class SaveLoadManager : SingletonManager<SaveLoadManager>
	{
		public const string SAVE_DIRECTORY = "SaveData";

		private const string SAVE_FILE_NAME = "save";

		public const string EXTENSION = ".svd";

		public static Dictionary<int, LocalData> saveDatas = new Dictionary<int, LocalData>();

		private static int _currentIndex = 99;

		private static LocalData _unsavedData;

		private static MetaSaveDataList _metaSaveData;

		private static GlobalData _globalData;

		public static Texture2D ScreenShot;

		public static bool IsLoaded => saveDatas.Count > 0;

		public static LocalData UnsavedData
		{
			get
			{
				if (!HasUnsavedData)
				{
					CreateNewSaveData();
					return _unsavedData;
				}
				return _unsavedData;
			}
		}

		public static MetaSaveDataList MetaSaveData
		{
			get
			{
				if (_metaSaveData == null)
				{
					LoadOrCreateMetaSaveDataList();
				}
				return _metaSaveData;
			}
		}

		public static GlobalData GlobalData
		{
			get
			{
				if (_globalData == null)
				{
					LoadGlobalData();
				}
				return _globalData;
			}
		}

		public static bool HasUnsavedData => _unsavedData != null;

		public static void CreateNewSaveData()
		{
			_unsavedData = new LocalData(0, new PersistantStatus(), 1, new GlobalFlags());
			_unsavedData.PersistantStatus.CalcRelationship();
		}

		private static void LoadOrCreateMetaSaveDataList()
		{
			string text = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "/SaveData/";
			CreateDirectory(Path.GetDirectoryName(text));
			string text2 = text + "/Meta.svd";
			try
			{
				StreamReader streamReader = new StreamReader(new FileInfo(text2).OpenRead(), Encoding.GetEncoding("UTF-8"));
				streamReader.ReadToEnd();
				MetaSaveDataList data;
				bool num = FileAESCrypter.TryDecryptJsonFromFile<MetaSaveDataList>(out data, text2);
				streamReader.Close();
				if (!num)
				{
					Debug.LogError("Failed to decrypt MetaSaveData");
				}
				else
				{
					_metaSaveData = data;
				}
			}
			catch (Exception)
			{
				_metaSaveData = new MetaSaveDataList();
			}
		}

		public static void Load(int fileNumber = -1)
		{
			if (fileNumber == -1)
			{
				fileNumber = _currentIndex;
			}
			else if (fileNumber < 0)
			{
				fileNumber = 0;
			}
			if (saveDatas.ContainsKey(fileNumber))
			{
				_unsavedData = saveDatas[fileNumber];
			}
			string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "/SaveData/";
			CreateDirectory(Path.GetDirectoryName(path));
			string[] files = Directory.GetFiles(path, MetaSaveData.GetMetaSaveData(fileNumber).SaveDataName + ".svd");
			foreach (string text in files)
			{
				try
				{
					StreamReader streamReader = new StreamReader(new FileInfo(text).OpenRead(), Encoding.GetEncoding("UTF-8"));
					streamReader.ReadToEnd();
					LocalData data;
					bool num = FileAESCrypter.TryDecryptJsonFromFile<LocalData>(out data, text);
					streamReader.Close();
					if (!num)
					{
						Debug.LogError("Failed to decrypt LocalData");
						return;
					}
					if (data.PlayerName == null)
					{
						data.PlayerName = "主人公";
					}
					if (saveDatas.ContainsKey(data.FileNumber))
					{
						saveDatas[data.FileNumber] = data;
					}
					else
					{
						saveDatas.Add(data.FileNumber, data);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.ToString());
					Application.Quit();
				}
			}
			_currentIndex = fileNumber;
			saveDatas[fileNumber].LoadedTime = DateTime.Now;
			_unsavedData = saveDatas[fileNumber];
			_unsavedData.Playtime = MetaSaveData.GetMetaSaveData(fileNumber).PlayTime;
			foreach (object flag in Enum.GetValues(typeof(FlagEnum)))
			{
				if (_unsavedData.GlobalFlags.Flags.Where((GameFlag x) => x.Name == (FlagEnum)flag).Count() == 0)
				{
					_unsavedData.GlobalFlags.Flags.Add(new GameFlag((FlagEnum)flag));
				}
			}
			foreach (object label in Enum.GetValues(typeof(ScenarioLabel)))
			{
				if (_unsavedData.GlobalFlags.ReadLabels.Where((ReadLabel x) => x.Name == (ScenarioLabel)label).Count() == 0)
				{
					_unsavedData.GlobalFlags.ReadLabels.Add(new ReadLabel((ScenarioLabel)label));
				}
			}
		}

		public static void ClearUnsavedData()
		{
			_unsavedData = null;
		}

		public static void Save(int index)
		{
			UnsavedData.FileNumber = index;
			UtageManager utageManager = UnityEngine.Object.FindObjectOfType<UtageManager>();
			UnsavedData.IsPlayingUtage = utageManager.IsPlaying;
			if (utageManager.IsPlaying)
			{
				UnsavedData.ScenarioLabel = utageManager.GetScenarioLabel();
				UnsavedData.ScenarioTitle = utageManager.GetScenarioTitle();
				UnsavedData.PageNumber = utageManager.GetCurrentPageNumber();
			}
			JsonUtility.ToJson(UnsavedData);
			if (!_metaSaveData.HasMetaSaveData(index))
			{
				_metaSaveData.AddNewMetaData(index, UnsavedData.PersistantStatus.Relationship, _unsavedData.Playtime, _unsavedData.LoadedTime, _unsavedData.Days);
			}
			else
			{
				_metaSaveData.UpdateMetaData(index, UnsavedData.PersistantStatus.Relationship, _unsavedData.Playtime, _unsavedData.LoadedTime, _unsavedData.Days);
			}
			string text = GetCurrentPath() + _metaSaveData.GetMetaSaveData(index).SaveDataName + ".svd";
			CreateDirectory(Path.GetDirectoryName(text));
			FileAESCrypter.EncryptJsonToFile(UnsavedData, text);
			SaveMetaData(GetCurrentDirectory());
			if (index != 0 && null != ScreenShot)
			{
				string path = GetCurrentPath() + _metaSaveData.GetMetaSaveData(index).SaveDataName + ".png";
				CreateDirectory(Path.GetDirectoryName(path));
				byte[] bytes = ScreenShot.EncodeToPNG();
				File.WriteAllBytes(path, bytes);
			}
			_currentIndex = index;
		}

		public static void DeleteAllSaveData()
		{
			string currentPath = GetCurrentPath();
			CreateDirectory(Path.GetDirectoryName(currentPath));
			string[] files = Directory.GetFiles(currentPath);
			foreach (string fileName in files)
			{
				try
				{
					new FileInfo(fileName).Delete();
				}
				catch (Exception)
				{
					Application.Quit();
				}
			}
			saveDatas.Clear();
			_metaSaveData = new MetaSaveDataList();
			SaveMetaData(GetCurrentDirectory());
		}

		public static void Delete(int index)
		{
			string currentPath = GetCurrentPath();
			CreateDirectory(Path.GetDirectoryName(currentPath));
			string[] files = Directory.GetFiles(currentPath, MetaSaveData.GetMetaSaveData(index).SaveDataName + ".svd");
			foreach (string fileName in files)
			{
				try
				{
					new FileInfo(fileName).Delete();
				}
				catch (Exception)
				{
					Application.Quit();
				}
			}
			saveDatas.Remove(index);
			_metaSaveData.RemoveMetaData(index);
			SaveMetaData(GetCurrentDirectory());
		}

		public static void CreateDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		private static void SaveMetaData(string originalPath)
		{
			string savePath = originalPath + "/SaveData/Meta.svd";
			JsonUtility.ToJson(_metaSaveData);
			FileAESCrypter.EncryptJsonToFile(_metaSaveData, savePath);
		}

		private static string GetCurrentDirectory()
		{
			return AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
		}

		public static string GetCurrentPath()
		{
			return GetCurrentDirectory() + "/SaveData/";
		}

		public static void SaveGlobalData()
		{
			string savePath = GetCurrentDirectory() + "/SaveData/Global.svd";
			JsonUtility.ToJson(_globalData);
			FileAESCrypter.EncryptJsonToFile(_globalData, savePath);
		}

		public static void LoadGlobalData()
		{
			string text = GetCurrentDirectory() + "/SaveData/Global.svd";
			try
			{
				StreamReader streamReader = new StreamReader(new FileInfo(text).OpenRead(), Encoding.GetEncoding("UTF-8"));
				streamReader.ReadToEnd();
				GlobalData data;
				bool num = FileAESCrypter.TryDecryptJsonFromFile<GlobalData>(out data, text);
				streamReader.Close();
				if (!num)
				{
					Debug.LogError("Failed to decrypt GlobalData");
					return;
				}
				_globalData = data;
				if (_globalData.GameOption.UIColor == new Color(0f, 0f, 0f, 0f))
				{
					_globalData.GameOption.UIColor = Color.white;
					_globalData.GameOption.MouseButtonAuto = 2;
					_globalData.GameOption.MouseButtonDecision = 0;
					_globalData.GameOption.MouseButtonSpecial = 1;
				}
				if (_globalData.GameOption.HeartGaugeCorrection == 0f)
				{
					_globalData.GameOption.HeartGaugeCorrection = 1f;
					_globalData.GameOption.AtomosphereGaugeCorrection = 1f;
					_globalData.GameOption.EjaculationGaugeCorrection = 1f;
					_globalData.GameOption.CountEjaculationWithCondom = true;
				}
			}
			catch (Exception)
			{
				_globalData = new GlobalData();
			}
		}
	}
}
