using System;
using Live2D.Cubism.Core;
using Serialize;

[Serializable]
public class MeshNamePair : KeyAndValue<MeshName, CubismDrawable>
{
	public MeshNamePair(MeshName key, CubismDrawable value)
		: base(key, value)
	{
	}
}
