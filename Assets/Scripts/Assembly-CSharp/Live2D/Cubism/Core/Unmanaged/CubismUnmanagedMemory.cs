using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Live2D.Cubism.Core.Unmanaged
{
	internal static class CubismUnmanagedMemory
	{
		private struct AllocationItem
		{
			public IntPtr UnalignedAddress;

			public IntPtr AlignedAddress;
		}

		private static List<AllocationItem> Allocations { get; set; }

		private static bool ContainsAllocations
		{
			get
			{
				if (Allocations != null)
				{
					return Allocations.Count > 0;
				}
				return false;
			}
		}

		public static IntPtr Allocate(int size, int align)
		{
			if (Allocations == null)
			{
				Allocations = new List<AllocationItem>();
			}
			IntPtr intPtr = Marshal.AllocHGlobal(size + align);
			long num = intPtr.ToInt64() & (align - 1);
			IntPtr intPtr2 = ((num != 0L) ? new IntPtr(intPtr.ToInt64() + align - num) : intPtr);
			Allocations.Add(new AllocationItem
			{
				UnalignedAddress = intPtr,
				AlignedAddress = intPtr2
			});
			return intPtr2;
		}

		public static void Deallocate(IntPtr allocation)
		{
			if (!ContainsAllocations)
			{
				return;
			}
			for (int i = 0; i < Allocations.Count; i++)
			{
				if (!(Allocations[i].AlignedAddress != allocation))
				{
					Marshal.FreeHGlobal(Allocations[i].UnalignedAddress);
					Allocations.RemoveAt(i);
					break;
				}
			}
		}

		public static void Write(byte[] source, IntPtr destination)
		{
			Marshal.Copy(source, 0, destination, source.Length);
		}
	}
}
