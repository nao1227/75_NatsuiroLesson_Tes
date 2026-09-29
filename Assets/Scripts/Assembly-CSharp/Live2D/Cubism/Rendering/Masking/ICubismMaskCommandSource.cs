using UnityEngine.Rendering;

namespace Live2D.Cubism.Rendering.Masking
{
	public interface ICubismMaskCommandSource
	{
		void AddToCommandBuffer(CommandBuffer buffer);
	}
}
