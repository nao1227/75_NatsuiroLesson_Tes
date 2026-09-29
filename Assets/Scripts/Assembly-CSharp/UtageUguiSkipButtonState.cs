using UnityEngine;
using UnityEngine.UI;
using Utage;
using UtageExtensions;

[AddComponentMenu("Utage/TemplateUI/UtageUguiSkipButtonState")]
public class UtageUguiSkipButtonState : MonoBehaviour
{
	[SerializeField]
	protected AdvEngine engine;

	public Toggle target;

	public AdvEngine Engine => this.GetComponentCacheFindIfMissing(ref engine);

	protected virtual void Update()
	{
		if (!(target == null))
		{
			target.interactable = Engine.Page.EnableSkip();
		}
	}
}
