using System;

namespace Live2D.Cubism.Core.Unmanaged
{
	public sealed class CubismUnmanagedIntArrayView
	{
		public int Length { get; private set; }

		public unsafe bool IsValid
		{
			get
			{
				if (UnmanagedFixedAddress != null)
				{
					return Length > 0;
				}
				return false;
			}
		}

		public unsafe int this[int index]
		{
			get
			{
				return UnmanagedFixedAddress[index];
			}
			set
			{
				UnmanagedFixedAddress[index] = value;
			}
		}

		private unsafe int* UnmanagedFixedAddress { get; set; }

		internal unsafe CubismUnmanagedIntArrayView(int* address, int length)
		{
			UnmanagedFixedAddress = address;
			Length = length;
		}

		internal unsafe CubismUnmanagedIntArrayView(IntPtr address, int length)
		{
			UnmanagedFixedAddress = (int*)address.ToPointer();
			Length = length;
		}

		public unsafe void Read(int[] buffer)
		{
			int* unmanagedFixedAddress = UnmanagedFixedAddress;
			_ = buffer.Length;
			fixed (int* ptr = buffer)
			{
				for (int i = 0; i < Length; i++)
				{
					ptr[i] = unmanagedFixedAddress[i];
				}
			}
		}

		public unsafe void Write(int[] buffer)
		{
			int num = buffer.Length;
			int* unmanagedFixedAddress = UnmanagedFixedAddress;
			fixed (int* ptr = buffer)
			{
				for (int i = 0; i < num; i++)
				{
					unmanagedFixedAddress[i] = ptr[i];
				}
			}
		}
	}
}
