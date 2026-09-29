using System;

namespace Live2D.Cubism.Core.Unmanaged
{
	public sealed class CubismUnmanagedFloatArrayView
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

		public unsafe float this[int index]
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

		private unsafe float* UnmanagedFixedAddress { get; set; }

		internal unsafe CubismUnmanagedFloatArrayView(float* address, int length)
		{
			UnmanagedFixedAddress = address;
			Length = length;
		}

		internal unsafe CubismUnmanagedFloatArrayView(IntPtr address, int length)
		{
			UnmanagedFixedAddress = (float*)address.ToPointer();
			Length = length;
		}

		public unsafe void Read(float[] buffer)
		{
			float* unmanagedFixedAddress = UnmanagedFixedAddress;
			_ = buffer.Length;
			fixed (float* ptr = buffer)
			{
				for (int i = 0; i < Length; i++)
				{
					ptr[i] = unmanagedFixedAddress[i];
				}
			}
		}

		public unsafe void Write(float[] buffer)
		{
			int num = buffer.Length;
			float* unmanagedFixedAddress = UnmanagedFixedAddress;
			fixed (float* ptr = buffer)
			{
				for (int i = 0; i < num; i++)
				{
					unmanagedFixedAddress[i] = ptr[i];
				}
			}
		}
	}
}
