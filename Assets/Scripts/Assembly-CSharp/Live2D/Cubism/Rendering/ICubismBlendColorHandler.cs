using UnityEngine;

namespace Live2D.Cubism.Rendering
{
	public interface ICubismBlendColorHandler
	{
		void OnBlendColorDidChange(CubismRenderController controller, Color[] newColors);
	}
}
