using System;
using UnityEngine;

[Serializable]
public class SESet : AudioSet
{
	[SerializeField]
	private SEName[] seNames;

	public SEName[] SENames => seNames;

	public SESet()
		: base(AudioCategory.SE)
	{
	}
}
