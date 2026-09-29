using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Paidia.satsuki1
{
	[Serializable]
	public class RandomSE
	{
		public List<WeightedSE> SEs;

		private Dictionary<AssetReference, AudioClip> _seClips;

		public bool HasSE
		{
			get
			{
				if (SEs != null)
				{
					return SEs.Count > 0;
				}
				return false;
			}
		}

		public Tuple<AudioClip, int> GetRandomSE()
		{
			int maxExclusive = SEs.Sum((WeightedSE x) => x.Weight);
			int num = UnityEngine.Random.Range(0, maxExclusive);
			int num2 = 0;
			foreach (WeightedSE sE in SEs)
			{
				num2 += sE.Weight;
				if (num < num2)
				{
					if (!_seClips.ContainsKey(sE.SE))
					{
						return null;
					}
					return new Tuple<AudioClip, int>(_seClips[sE.SE], sE.Channel);
				}
			}
			return null;
		}

		public async UniTask LoadSE()
		{
			_seClips = new Dictionary<AssetReference, AudioClip>();
			foreach (WeightedSE se in SEs)
			{
				AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(se.SE);
				await handle.Task;
				if (handle.Status == AsyncOperationStatus.Succeeded)
				{
					if (_seClips.ContainsKey(se.SE))
					{
						_seClips[se.SE] = handle.Result;
					}
					else
					{
						_seClips.Add(se.SE, handle.Result);
					}
				}
			}
		}

		public int GetChannel(AssetReference se)
		{
			return SEs.Find((WeightedSE x) => x.SE == se).Channel;
		}
	}
}
