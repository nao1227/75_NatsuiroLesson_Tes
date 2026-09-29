using UnityEngine;

namespace Utage
{
	public class HelpBoxAttribute : PropertyAttribute
	{
		public enum Type
		{
			None = 0,
			Info = 1,
			Warning = 2,
			Error = 3
		}

		public string Message { get; set; }

		public Type MessageType { get; set; }

		public HelpBoxAttribute(string message, Type type = Type.None, int order = 0)
		{
			Message = message;
			MessageType = type;
			base.order = order;
		}
	}
}
