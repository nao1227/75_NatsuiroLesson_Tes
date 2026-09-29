using System;

namespace Live2D.Cubism.Core.Unmanaged
{
	public sealed class CubismUnmanagedByteArrayView
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

		public unsafe byte this[int index]
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

		private unsafe byte* UnmanagedFixedAddress { get; set; }

		internal unsafe CubismUnmanagedByteArrayView(byte* address, int length)
		{
			UnmanagedFixedAddress = address;
			Length = length;
		}

		internal unsafe CubismUnmanagedByteArrayView(IntPtr address, int length)
		{
			UnmanagedFixedAddress = (byte*)address.ToPointer();
			Length = length;
		}

		public unsafe void Read(byte[] buffer)
		{
			byte* unmanagedFixedAddress = UnmanagedFixedAddress;
			_ = buffer.Length;
			fixed (byte* ptr = buffer)
			{
				for (int i = 0; i < Length; i++)
				{
					ptr[i] = unmanagedFixedAddress[i];
				}
			}
		}

		public unsafe void Write(byte[] buffer)
		{
			int num = buffer.Length;
			byte* unmanagedFixedAddress = UnmanagedFixedAddress;
			fixed (byte* ptr = buffer)
			{
				for (int i = 0; i < num; i++)
				{
					unmanagedFixedAddress[i] = ptr[i];
				}
			}
		}
	}
}
