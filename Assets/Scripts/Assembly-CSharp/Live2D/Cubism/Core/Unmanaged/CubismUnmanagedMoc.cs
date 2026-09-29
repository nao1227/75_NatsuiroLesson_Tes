using System;

namespace Live2D.Cubism.Core.Unmanaged
{
	public sealed class CubismUnmanagedMoc
	{
		public IntPtr Ptr { get; private set; }

		public uint MocVersion { get; private set; }

		public static CubismUnmanagedMoc FromBytes(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			CubismUnmanagedMoc cubismUnmanagedMoc = new CubismUnmanagedMoc(bytes);
			if (!(cubismUnmanagedMoc.Ptr != IntPtr.Zero))
			{
				return null;
			}
			return cubismUnmanagedMoc;
		}

		public static bool HasMocConsistency(byte[] bytes)
		{
			IntPtr intPtr = CubismUnmanagedMemory.Allocate(bytes.Length, 64);
			CubismUnmanagedMemory.Write(bytes, intPtr);
			bool result = CubismCoreDll.HasMocConsistency(intPtr, (uint)bytes.Length) == 1;
			CubismUnmanagedMemory.Deallocate(intPtr);
			return result;
		}

		public void Release()
		{
			if (!(Ptr == IntPtr.Zero))
			{
				CubismUnmanagedMemory.Deallocate(Ptr);
				Ptr = IntPtr.Zero;
			}
		}

		private CubismUnmanagedMoc(byte[] bytes)
		{
			IntPtr intPtr = CubismUnmanagedMemory.Allocate(bytes.Length, 64);
			if (!(intPtr == IntPtr.Zero))
			{
				CubismUnmanagedMemory.Write(bytes, intPtr);
				Ptr = CubismCoreDll.ReviveMocInPlace(intPtr, (uint)bytes.Length);
				if (Ptr == IntPtr.Zero)
				{
					CubismUnmanagedMemory.Deallocate(intPtr);
				}
				else
				{
					MocVersion = CubismCoreDll.GetMocVersion(Ptr, (uint)bytes.Length);
				}
			}
		}
	}
}
