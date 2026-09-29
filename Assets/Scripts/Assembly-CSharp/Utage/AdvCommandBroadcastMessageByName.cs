using UnityEngine;

namespace Utage
{
	public class AdvCommandBroadcastMessageByName : AdvCommand
	{
		private enum TargetType
		{
			Default = 0,
			UtageObject = 1,
			RenderTexture = 2
		}

		private readonly string name;

		private readonly string function;

		private readonly TargetType targetType;

		public bool IsWait { get; set; }

		public AdvEngine Engine { get; private set; }

		public AdvCommandBroadcastMessageByName(StringGridRow row)
			: base(row)
		{
			name = ParseCell<string>(AdvColumnName.Arg1);
			function = ParseCell<string>(AdvColumnName.Arg2);
			targetType = ParseCellOptional(AdvColumnName.Arg3, TargetType.Default);
		}

		public override void DoCommand(AdvEngine engine)
		{
			Engine = engine;
			GameObject gameObject = FindTarget(engine);
			if (!(gameObject == null))
			{
				gameObject.BroadcastMessage(function, this, SendMessageOptions.RequireReceiver);
			}
		}

		private GameObject FindTarget(AdvEngine engine)
		{
			GameObject gameObject = null;
			switch (targetType)
			{
			case TargetType.UtageObject:
				gameObject = engine.GraphicManager.FindObjectOrLayer(name);
				if (gameObject == null)
				{
					Debug.LogError(name + " is not found in Utage Objects");
				}
				break;
			case TargetType.RenderTexture:
			{
				AdvGraphicObject advGraphicObject = engine.GraphicManager.FindObject(name);
				if (advGraphicObject == null)
				{
					Debug.LogError(name + " is not found in Utage Objects");
				}
				else
				{
					gameObject = advGraphicObject.TargetObject.gameObject;
				}
				break;
			}
			default:
				gameObject = GameObject.Find(name);
				if (gameObject == null)
				{
					Debug.LogError(name + " is not found in current scene");
				}
				break;
			}
			return gameObject;
		}

		public override bool Wait(AdvEngine engine)
		{
			return IsWait;
		}
	}
}
