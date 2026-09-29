using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	[CreateAssetMenu(menuName = "Data/Voice")]
	public class VoiceList : ScriptableObject
	{
		public List<VoiceOnCondition> VoiceOnConditions;

		public VoiceName DefaultVoice;

		public bool AllVoiceLoaded => VoiceOnConditions.Count((VoiceOnCondition x) => !x.IsLoaded) == 0;

		public VoiceName GetVoice(FeelingParams feelings)
		{
			try
			{
				return VoiceOnConditions.First((VoiceOnCondition x) => x.Atomosphere == feelings.AtomosphereName && x.ExciteRangeLower <= feelings.Excite && x.ExciteRangeUpper > feelings.Excite).Voice;
			}
			catch
			{
				return DefaultVoice;
			}
		}

		public async UniTask Start()
		{
			foreach (VoiceOnCondition voiceOnCondition in VoiceOnConditions)
			{
				await voiceOnCondition.LoadAsync();
			}
		}

		public void OnDestroy()
		{
			foreach (VoiceOnCondition voiceOnCondition in VoiceOnConditions)
			{
				voiceOnCondition.Unload();
			}
		}

		public VoiceOnCondition GetRandomVoiceOnCondition(VoiceName name, AtomosphereName atom)
		{
			try
			{
				return (from _ in VoiceOnConditions
					where _.Atomosphere == atom && _.Voice == name
					orderby Guid.NewGuid()
					select _).First();
			}
			catch (Exception)
			{
				return null;
			}
		}

		public VoiceOnCondition GetVoiceOnCondition(VoiceName name, AtomosphereName atom, int index)
		{
			try
			{
				return VoiceOnConditions.Where((VoiceOnCondition x) => x.Atomosphere == atom && x.Voice == name).ToArray()[index];
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
