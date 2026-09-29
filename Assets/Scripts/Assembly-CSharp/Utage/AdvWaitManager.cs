using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	internal class AdvWaitManager
	{
		private readonly List<AdvCommandWaitBase> notWaitCommandList = new List<AdvCommandWaitBase>();

		private readonly List<AdvCommandWaitBase> waitCommandList = new List<AdvCommandWaitBase>();

		private readonly List<AdvCommandWaitBase> removeCommandList = new List<AdvCommandWaitBase>();

		internal bool IsWaiting => waitCommandList.Count > 0;

		internal bool IsWaitingDefault
		{
			get
			{
				UpdateCheckWait();
				return waitCommandList.Exists((AdvCommandWaitBase x) => x.WaitType.IsWaitingCommandType());
			}
		}

		internal bool IsWaitingInputEffect
		{
			get
			{
				UpdateCheckWait();
				return waitCommandList.Exists((AdvCommandWaitBase x) => x.WaitType.IsWaitingInputType());
			}
		}

		internal bool IsWaitingPageEndEffect
		{
			get
			{
				UpdateCheckWait();
				return waitCommandList.Exists((AdvCommandWaitBase x) => x.WaitType.IsWaitingPageEndEffect());
			}
		}

		internal bool IsWaitingOnThread
		{
			get
			{
				UpdateCheckWait();
				return waitCommandList.Exists((AdvCommandWaitBase x) => x.WaitType.IsWaitingOnThreadType());
			}
		}

		internal void Clear()
		{
			ClearCommandList(notWaitCommandList);
			ClearCommandList(waitCommandList);
		}

		private void ClearCommandList(List<AdvCommandWaitBase> list)
		{
			foreach (AdvCommandWaitBase item in list)
			{
				FinalizeCommand(item);
			}
			list.Clear();
		}

		private void FinalizeCommand(AdvCommandWaitBase command)
		{
			if (command is IAdvCommandEffect advCommandEffect)
			{
				advCommandEffect.OnEffectFinalize();
			}
		}

		internal void StartCommand(AdvCommandWaitBase command)
		{
			if (command.WaitType == AdvCommandWaitType.NoWait)
			{
				notWaitCommandList.Add(command);
			}
			else
			{
				waitCommandList.Add(command);
			}
		}

		internal void CompleteCommand(AdvCommandWaitBase command)
		{
			FinalizeCommand(command);
			if (command.WaitType == AdvCommandWaitType.NoWait)
			{
				notWaitCommandList.Remove(command);
			}
			else
			{
				waitCommandList.Remove(command);
			}
		}

		private void UpdateCheckWait()
		{
			removeCommandList.Clear();
			foreach (AdvCommandWaitBase waitCommand in waitCommandList)
			{
				if (waitCommand is IAdvCommandUpdateWait advCommandUpdateWait && !advCommandUpdateWait.UpdateCheckWait())
				{
					removeCommandList.Add(waitCommand);
				}
			}
			if (removeCommandList.Count <= 0)
			{
				return;
			}
			foreach (AdvCommandWaitBase removeCommand in removeCommandList)
			{
				CompleteCommand(removeCommand);
			}
			removeCommandList.Clear();
		}

		public void SkipEffectCommand()
		{
			SkipEffectSub((AdvCommandWaitType x) => x.IsSkippableCommand());
		}

		public void SkipEffectInput()
		{
			SkipEffectSub((AdvCommandWaitType x) => x.IsSkippableInput());
		}

		public void SkipEffectPageEnd()
		{
			SkipEffectSub((AdvCommandWaitType x) => x.IsSkippable());
		}

		public void SkipEffectCommandOnWaitThread()
		{
			SkipEffectSub((AdvCommandWaitType x) => x.IsSkippableCommandOnWaitThread());
		}

		private void SkipEffectSub(Func<AdvCommandWaitType, bool> checkSkip)
		{
			if (!waitCommandList.Exists((AdvCommandWaitBase x) => checkSkip(x.WaitType)))
			{
				return;
			}
			foreach (AdvCommandWaitBase item in new List<AdvCommandWaitBase>(waitCommandList))
			{
				if (checkSkip(item.WaitType))
				{
					if (item is IAdvCommandEffect advCommandEffect)
					{
						advCommandEffect.OnEffectSkip();
						continue;
					}
					Debug.LogErrorFormat("command {0} is not skippable effect", item.Id);
				}
			}
		}
	}
}
