namespace Utage
{
	public static class AdvCommandWaitTypeExtensions
	{
		public static bool IsSkippableInput(this AdvCommandWaitType target)
		{
			if (target == AdvCommandWaitType.Skippable || (uint)(target - 7) <= 1u)
			{
				return true;
			}
			return false;
		}

		public static bool IsSkippableCommand(this AdvCommandWaitType target)
		{
			if (target == AdvCommandWaitType.Skippable || target == AdvCommandWaitType.AddSkippable)
			{
				return true;
			}
			return false;
		}

		public static bool IsSkippableCommandOnWaitThread(this AdvCommandWaitType target)
		{
			if ((uint)(target - 8) <= 1u)
			{
				return true;
			}
			return false;
		}

		public static bool IsSkippable(this AdvCommandWaitType target)
		{
			if ((uint)(target - 5) <= 3u)
			{
				return true;
			}
			return false;
		}

		public static bool IsWaitingCommandType(this AdvCommandWaitType target)
		{
			switch (target)
			{
			case AdvCommandWaitType.Default:
			case AdvCommandWaitType.Add:
			case AdvCommandWaitType.Skippable:
			case AdvCommandWaitType.AddSkippable:
			case AdvCommandWaitType.SkippableOnWaitThread:
				return true;
			default:
				return false;
			}
		}

		public static bool IsWaitingInputType(this AdvCommandWaitType target)
		{
			if ((uint)(target - 2) <= 1u || (uint)(target - 7) <= 1u)
			{
				return true;
			}
			return false;
		}

		public static bool IsWaitingPageEndEffect(this AdvCommandWaitType target)
		{
			if ((uint)(target - 1) <= 2u || (uint)(target - 6) <= 2u)
			{
				return true;
			}
			return false;
		}

		public static bool IsWaitingOnThreadType(this AdvCommandWaitType target)
		{
			switch (target)
			{
			case AdvCommandWaitType.Default:
			case AdvCommandWaitType.Add:
			case AdvCommandWaitType.Skippable:
			case AdvCommandWaitType.AddSkippable:
			case AdvCommandWaitType.SkippableOnWaitThread:
				return true;
			default:
				return false;
			}
		}
	}
}
