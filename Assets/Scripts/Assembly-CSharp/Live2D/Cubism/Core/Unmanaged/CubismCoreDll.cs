using System;
using System.Runtime.InteropServices;

namespace Live2D.Cubism.Core.Unmanaged
{
	public static class CubismCoreDll
	{
		public const string DllName = "Live2DCubismCore";

		public const int AlignofMoc = 64;

		public const int AlignofModel = 16;

		public const int MocVersion_Unknown = 0;

		public const int MocVersion_30 = 1;

		public const int MocVersion_33 = 2;

		public const int MocVersion_40 = 3;

		public const int MocVersion_42 = 4;

		public const int ParameterType_Normal = 0;

		public const int ParameterType_BlendShape = 1;

		public const byte BlendAdditive = 1;

		public const byte BlendMultiplicative = 2;

		public const byte IsDoubleSided = 4;

		public const byte IsInvertedMask = 8;

		public const byte IsVisible = 1;

		public const byte VisibilityDidChange = 2;

		public const byte OpacityDidChange = 4;

		public const byte DrawOrderDidChange = 8;

		public const byte RenderOrderDidChange = 16;

		public const byte VertexPositionsDidChange = 32;

		public const byte BlendColorDidChange = 64;

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetVersion")]
		public static extern uint GetVersion();

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetLatestMocVersion")]
		public static extern uint GetLatestMocVersion();

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetMocVersion")]
		public static extern uint GetMocVersion(IntPtr moc, uint mocSize);

		[DllImport("Live2DCubismCore", EntryPoint = "csmSetLogFunction")]
		public static extern void SetLogFunction(uint handler);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetSizeofModel")]
		public static extern uint GetSizeofModel(IntPtr moc);

		[DllImport("Live2DCubismCore", EntryPoint = "csmReviveMocInPlace")]
		public static extern IntPtr ReviveMocInPlace(IntPtr memory, uint mocSize);

		[DllImport("Live2DCubismCore", EntryPoint = "csmInitializeModelInPlace")]
		public static extern IntPtr InitializeModelInPlace(IntPtr moc, IntPtr memory, uint modelSize);

		[DllImport("Live2DCubismCore", EntryPoint = "csmHasMocConsistency")]
		public static extern int HasMocConsistency(IntPtr memory, uint mocSize);

		[DllImport("Live2DCubismCore", EntryPoint = "csmUpdateModel")]
		public static extern void UpdateModel(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmReadCanvasInfo")]
		public static extern void ReadCanvasInfo(IntPtr model, IntPtr outSizeInPixels, IntPtr outOriginInPixels, IntPtr outPixelsPerUnit);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetParameterCount")]
		public static extern int GetParameterCount(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetParameterIds")]
		public unsafe static extern char** GetParameterIds(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetParameterMinimumValues")]
		public unsafe static extern float* GetParameterMinimumValues(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetParameterTypes")]
		public unsafe static extern int* GetParameterTypes(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetParameterMaximumValues")]
		public unsafe static extern float* GetParameterMaximumValues(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetParameterDefaultValues")]
		public unsafe static extern float* GetParameterDefaultValues(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetParameterValues")]
		public unsafe static extern float* GetParameterValues(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetParameterKeyCounts")]
		public unsafe static extern int* GetParameterKeyCounts(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetParameterKeyValues")]
		public unsafe static extern float** GetParameterKeyValues(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetPartCount")]
		public static extern int GetPartCount(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetPartIds")]
		public unsafe static extern char** GetPartIds(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetPartOpacities")]
		public unsafe static extern float* GetPartOpacities(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetPartParentPartIndices")]
		public unsafe static extern int* GetPartParentPartIndices(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableCount")]
		public static extern int GetDrawableCount(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableIds")]
		public unsafe static extern char** GetDrawableIds(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableConstantFlags")]
		public unsafe static extern byte* GetDrawableConstantFlags(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableDynamicFlags")]
		public unsafe static extern byte* GetDrawableDynamicFlags(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableTextureIndices")]
		public unsafe static extern int* GetDrawableTextureIndices(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableDrawOrders")]
		public unsafe static extern int* GetDrawableDrawOrders(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableRenderOrders")]
		public unsafe static extern int* GetDrawableRenderOrders(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableOpacities")]
		public unsafe static extern float* GetDrawableOpacities(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableMaskCounts")]
		public unsafe static extern int* GetDrawableMaskCounts(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableMasks")]
		public unsafe static extern int** GetDrawableMasks(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableVertexCounts")]
		public unsafe static extern int* GetDrawableVertexCounts(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableVertexPositions")]
		public unsafe static extern float** GetDrawableVertexPositions(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableVertexUvs")]
		public unsafe static extern float** GetDrawableVertexUvs(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableIndexCounts")]
		public unsafe static extern int* GetDrawableIndexCounts(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableIndices")]
		public unsafe static extern ushort** GetDrawableIndices(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmResetDrawableDynamicFlags")]
		public static extern void ResetDrawableDynamicFlags(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableMultiplyColors")]
		public unsafe static extern float* GetDrawableMultiplyColors(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableScreenColors")]
		public unsafe static extern float* GetDrawableScreenColors(IntPtr model);

		[DllImport("Live2DCubismCore", EntryPoint = "csmGetDrawableParentPartIndices")]
		public unsafe static extern int* GetDrawableParentPartIndices(IntPtr model);
	}
}
