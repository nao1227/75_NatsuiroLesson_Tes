using System.Collections.Generic;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/AdvBackLogFilter")]
	public class AdvBackLogFilter : MonoBehaviour
	{
		[SerializeField]
		private bool disable;

		public List<string> filterMessageWindowNames = new List<string>(new string[1] { "MessageWindow" });

		[SerializeField]
		protected AdvEngine engine;

		public bool Disable
		{
			get
			{
				return disable;
			}
			set
			{
				disable = value;
			}
		}

		public AdvEngine Engine => this.GetComponentCacheFindIfMissing(ref engine);

		private void Awake()
		{
			Engine.BacklogManager.OnAddPage.AddListener(OnAddPage);
		}

		private void OnAddPage(AdvBacklogManager backlogManager)
		{
			backlogManager.IgnoreLog = !filterMessageWindowNames.Contains(Engine.MessageWindowManager.CurrentWindow.Name);
		}
	}
}
