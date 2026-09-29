using UnityEngine;

namespace Live2D.Cubism.Rendering
{
	public static class CubismBuiltinShaders
	{
		public static Shader Unlit => Shader.Find("Live2D Cubism/Unlit");

		public static Shader Mask => Shader.Find("Live2D Cubism/Mask");
	}
}
