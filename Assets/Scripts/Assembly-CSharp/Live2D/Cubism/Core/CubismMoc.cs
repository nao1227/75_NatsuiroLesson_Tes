using Live2D.Cubism.Core.Unmanaged;
using UnityEngine;

namespace Live2D.Cubism.Core
{
	public sealed class CubismMoc : ScriptableObject
	{
		[SerializeField]
		private byte[] _bytes;

		public static uint LatestVersion => CubismCoreDll.GetLatestMocVersion();

		private byte[] Bytes
		{
			get
			{
				return _bytes;
			}
			set
			{
				_bytes = value;
			}
		}

		private CubismUnmanagedMoc UnmanagedMoc { get; set; }

		private int ReferenceCount { get; set; }

		public bool IsRevived => UnmanagedMoc != null;

		public uint Version => UnmanagedMoc.MocVersion;

		public static bool HasMocConsistency(byte[] moc3)
		{
			return CubismUnmanagedMoc.HasMocConsistency(moc3);
		}

		public static CubismMoc CreateFrom(byte[] moc3)
		{
			CubismMoc cubismMoc = ScriptableObject.CreateInstance<CubismMoc>();
			cubismMoc.Bytes = moc3;
			return cubismMoc;
		}

		public static void ResetUnmanagedMoc(CubismMoc moc)
		{
			moc.UnmanagedMoc = null;
			moc.Revive();
		}

		public CubismUnmanagedMoc AcquireUnmanagedMoc()
		{
			int referenceCount = ReferenceCount + 1;
			ReferenceCount = referenceCount;
			Revive();
			return UnmanagedMoc;
		}

		public void ReleaseUnmanagedMoc()
		{
			int referenceCount = ReferenceCount - 1;
			ReferenceCount = referenceCount;
			if (ReferenceCount == 0)
			{
				UnmanagedMoc.Release();
				UnmanagedMoc = null;
			}
			else if (ReferenceCount < 0)
			{
				ReferenceCount = 0;
			}
		}

		private void Revive()
		{
			if (!IsRevived && Bytes != null)
			{
				UnmanagedMoc = CubismUnmanagedMoc.FromBytes(Bytes);
			}
		}
	}
}
