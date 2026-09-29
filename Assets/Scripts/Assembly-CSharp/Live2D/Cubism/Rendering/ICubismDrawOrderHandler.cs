using Live2D.Cubism.Core;

namespace Live2D.Cubism.Rendering
{
	public interface ICubismDrawOrderHandler
	{
		void OnDrawOrderDidChange(CubismRenderController controller, CubismDrawable drawable, int newDrawOrder);
	}
}
