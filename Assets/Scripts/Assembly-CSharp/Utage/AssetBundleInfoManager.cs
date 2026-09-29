using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/File/AssetBundleInfoManager")]
	public class AssetBundleInfoManager : MonoBehaviour
	{
		[SerializeField]
		private int retryCount = 5;

		[SerializeField]
		private float timeOut = 5f;

		[SerializeField]
		private bool useCacheManifest = true;

		[SerializeField]
		private string cacheDirectoryName = "Cache";

		[SerializeField]
		private bool cacheLoad = true;

		[SerializeField]
		private AssetFileManager assetFileManager;

		private Dictionary<string, AssetBundleInfo> dictionary = new Dictionary<string, AssetBundleInfo>(StringComparer.OrdinalIgnoreCase);

		private const string AssetBundleManifestName = "assetbundlemanifest";

		public int RetryCount
		{
			get
			{
				return retryCount;
			}
			set
			{
				retryCount = value;
			}
		}

		public int TimeOut
		{
			get
			{
				return retryCount;
			}
			set
			{
				retryCount = value;
			}
		}

		public bool UseCacheManifest
		{
			get
			{
				return useCacheManifest;
			}
			set
			{
				useCacheManifest = value;
			}
		}

		public string CacheDirectoryName
		{
			get
			{
				return cacheDirectoryName;
			}
			set
			{
				cacheDirectoryName = value;
			}
		}

		public bool CacheLoad
		{
			get
			{
				return cacheLoad;
			}
			set
			{
				cacheLoad = value;
			}
		}

		private AssetFileManager AssetFileManager => this.GetComponentCache(ref assetFileManager);

		private FileIOManager FileIOManager => AssetFileManager.FileIOManager;

		public AssetBundleInfo FindAssetBundleInfo(string path)
		{
			if (!dictionary.TryGetValue(path, out var value))
			{
				string key = FilePathUtil.ChangeExtension(path, ".asset");
				if (!dictionary.TryGetValue(key, out value))
				{
					return null;
				}
			}
			return value;
		}

		public void AddAssetBundleInfo(string resourcePath, string assetBunleUrl, int assetBunleVersion, int assetBunleSize = 0)
		{
			AddAssetBundleInfo(resourcePath, new AssetBundleInfo(assetBunleUrl, assetBunleVersion, assetBunleSize));
		}

		public void AddAssetBundleInfo(string resourcePath, string assetBunleUrl, Hash128 assetBunleHash, int assetBunleSize = 0)
		{
			AddAssetBundleInfo(resourcePath, new AssetBundleInfo(assetBunleUrl, assetBunleHash, assetBunleSize));
		}

		public void AddAssetBundleInfo(string resourcePath, string assetBundleUrl)
		{
			AddAssetBundleInfo(resourcePath, new AssetBundleInfo(assetBundleUrl));
		}

		public void AddAssetBundleManifest(string rootUrl, AssetBundleManifest manifest)
		{
			string[] allAssetBundles = manifest.GetAllAssetBundles();
			foreach (string text in allAssetBundles)
			{
				string text2 = FilePathUtil.Combine(rootUrl, text);
				if (CacheLoad)
				{
					Hash128 assetBundleHash = manifest.GetAssetBundleHash(text);
					AddAssetBundleInfo(text2, new AssetBundleInfo(text2, assetBundleHash));
				}
				else
				{
					AddAssetBundleInfo(text2, new AssetBundleInfo(text2));
				}
			}
		}

		private void AddAssetBundleInfo(string key, AssetBundleInfo info)
		{
			try
			{
				dictionary.Add(key, info);
			}
			catch
			{
				Debug.LogError(key + "is already contains in AssetBundleManger");
			}
		}

		public IEnumerator DownloadManifestAsync(string rootUrl, string relativeUrl, Action onComplete, Action onFailed)
		{
			WWWEx wWWEx = new WWWEx(FilePathUtil.ToCacheClearUrl(FilePathUtil.Combine(rootUrl, relativeUrl)));
			if (UseCacheManifest)
			{
				wWWEx.IoManager = FileIOManager;
				wWWEx.WriteLocal = true;
				wWWEx.WritePath = GetCachePath(relativeUrl);
			}
			wWWEx.OnUpdate = OnDownloadingManifest;
			wWWEx.RetryCount = retryCount;
			wWWEx.TimeOut = timeOut;
			return wWWEx.LoadAssetBundleByNameAsync("assetbundlemanifest", unloadAllLoadedObjects: false, delegate(AssetBundleManifest manifest)
			{
				AddAssetBundleManifest(rootUrl, manifest);
				if (onComplete != null)
				{
					onComplete();
				}
			}, delegate
			{
				if (onFailed != null)
				{
					onFailed();
				}
			});
		}

		private void OnDownloadingManifest(WWWEx wwwEx)
		{
		}

		public IEnumerator LoadCacheManifestAsync(string rootUrl, string relativeUrl, Action onComplete, Action onFailed)
		{
			return new WWWEx(FilePathUtil.AddFileProtocol(GetCachePath(relativeUrl)))
			{
				OnUpdate = OnDownloadingManifest,
				RetryCount = 0,
				TimeOut = 0.1f
			}.LoadAssetBundleByNameAsync("assetbundlemanifest", unloadAllLoadedObjects: false, delegate(AssetBundleManifest manifest)
			{
				AddAssetBundleManifest(rootUrl, manifest);
				if (onComplete != null)
				{
					onComplete();
				}
			}, delegate
			{
				if (onFailed != null)
				{
					onFailed();
				}
			});
		}

		private string GetCachePath(string relativeUrl)
		{
			return FilePathUtil.Combine(FileIOManagerBase.SdkTemporaryCachePath, cacheDirectoryName, relativeUrl);
		}

		public void DeleteAllCache()
		{
			FileIOManager.DeleteDirectory(FilePathUtil.Combine(FileIOManagerBase.SdkTemporaryCachePath, cacheDirectoryName) + "/");
			WrapperUnityVersion.CleanCache();
		}
	}
}
