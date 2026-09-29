using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Live2D.Cubism.Core
{
	internal static class CubismLogging
	{
		private unsafe delegate void UnmanagedLogDelegate(char* message);

		private static UnmanagedLogDelegate LogDelegate { get; set; }

		[RuntimeInitializeOnLoadMethod]
		private unsafe static void Initialize()
		{
			LogDelegate = LogUnmanaged;
			csmSetLogFunction(Marshal.GetFunctionPointerForDelegate(LogDelegate));
		}

		[MonoPInvokeCallback(typeof(UnmanagedLogDelegate))]
		private unsafe static void LogUnmanaged(char* message)
		{
			string text = Marshal.PtrToStringAnsi(new IntPtr(message));
			Debug.LogFormat("[Cubism] Core: {0}.", text);
		}

		[DllImport("Live2DCubismCore")]
		private static extern void csmSetLogFunction(IntPtr logFunction);
	}
}
