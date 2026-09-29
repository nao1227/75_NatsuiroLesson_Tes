using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public interface IAdvMessageWindowManager
	{
		GameObject gameObject { get; }

		Dictionary<string, IAdvMessageWindow> AllWindows { get; }
	}
}
