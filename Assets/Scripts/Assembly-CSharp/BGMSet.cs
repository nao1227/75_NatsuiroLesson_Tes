using System;
using UnityEngine;

[Serializable]
public class BGMSet : AudioSet
{
	[SerializeField]
	private BGMName[] bgmNames;

	public BGMName[] BGMNames => bgmNames;

	public BGMSet()
		: base(AudioCategory.BGM)
	{
	}
}
