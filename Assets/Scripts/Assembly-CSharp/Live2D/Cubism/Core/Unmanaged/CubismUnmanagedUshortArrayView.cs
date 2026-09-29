using System;

namespace Live2D.Cubism.Core.Unmanaged
{
	public sealed class CubismUnmanagedUshortArrayView
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

		public unsafe ushort this[int index]
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

		private unsafe ushort* UnmanagedFixedAddress { get; set; }

		internal unsafe CubismUnmanagedUshortArrayView(ushort* address, int length)
		{
			UnmanagedFixedAddress = address;
			Length = length;
		}

		internal unsafe CubismUnmanagedUshortArrayView(IntPtr address, int length)
		{
			UnmanagedFixedAddress = (ushort*)address.ToPointer();
			Length = length;
		}

		public unsafe void Read(ushort[] buffer)
		{
			ushort* unmanagedFixedAddress = UnmanagedFixedAddress;
			_ = buffer.Length;
			fixed (ushort* ptr = buffer)
			{
				for (int i = 0; i < Length; i++)
				{
					ptr[i] = unmanagedFixedAddress[i];
				}
			}
		}

		public unsafe void Write(ushort[] buffer)
		{
			int num = buffer.Length;
			ushort* unmanagedFixedAddress = UnmanagedFixedAddress;
			fixed (ushort* ptr = buffer)
			{
				for (int i = 0; i < num; i++)
				{
					unmanagedFixedAddress[i] = ptr[i];
				}
			}
		}
	}
}
