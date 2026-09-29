using UnityEngine;

namespace Utage
{
	public interface IAdvMessageWindowCaracterCountChecker
	{
		GameObject gameObject { get; }

		string StartCheckCaracterCount();

		bool TryCheckCaracterCount(string text, out int count, out string errorString);

		void EndCheckCaracterCount(string text);
	}
}
