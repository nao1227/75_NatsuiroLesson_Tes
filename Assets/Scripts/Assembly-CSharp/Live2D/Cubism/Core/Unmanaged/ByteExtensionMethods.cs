namespace Live2D.Cubism.Core.Unmanaged
{
	internal static class ByteExtensionMethods
	{
		public static bool HasBlendAdditiveFlag(this byte self)
		{
			return (self & 1) == 1;
		}

		public static bool HasBlendMultiplicativeFlag(this byte self)
		{
			return (self & 2) == 2;
		}

		public static bool HasIsDoubleSidedFlag(this byte self)
		{
			return (self & 4) == 4;
		}

		public static bool HasIsInvertedMaskFlag(this byte self)
		{
			return (self & 8) == 8;
		}

		public static bool HasIsVisibleFlag(this byte self)
		{
			return (self & 1) == 1;
		}

		public static bool HasVisibilityDidChangeFlag(this byte self)
		{
			return (self & 2) == 2;
		}

		public static bool HasOpacityDidChangeFlag(this byte self)
		{
			return (self & 4) == 4;
		}

		public static bool HasDrawOrderDidChangeFlag(this byte self)
		{
			return (self & 8) == 8;
		}

		public static bool HasRenderOrderDidChangeFlag(this byte self)
		{
			return (self & 0x10) == 16;
		}

		public static bool HasVertexPositionsDidChangeFlag(this byte self)
		{
			return (self & 0x20) == 32;
		}

		public static bool HasBlendColorDidChangeFlag(this byte self)
		{
			return (self & 0x40) == 64;
		}
	}
}
