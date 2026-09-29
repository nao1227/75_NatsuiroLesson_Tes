namespace Live2D.Cubism.Rendering
{
	public static class CubismSortingModeExtensionMethods
	{
		public static bool SortByDepth(this CubismSortingMode self)
		{
			if (self != CubismSortingMode.BackToFrontZ)
			{
				return self == CubismSortingMode.FrontToBackZ;
			}
			return true;
		}

		public static bool SortByOrder(this CubismSortingMode self)
		{
			return !self.SortByDepth();
		}
	}
}
