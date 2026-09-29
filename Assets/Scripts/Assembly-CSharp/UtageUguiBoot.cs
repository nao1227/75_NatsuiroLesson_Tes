using System.Collections;
using UnityEngine;
using Utage;
using UtageExtensions;

[AddComponentMenu("Utage/TemplateUI/UtageUguiBoot")]
public class UtageUguiBoot : UguiView
{
	[SerializeField]
	protected AdvEngine engine;

	public UguiFadeTextureStream fadeTextureStream;

	public UtageUguiTitle title;

	public UtageUguiLoadWait loadWait;

	public bool isWaitBoot;

	public bool isWaitDownLoad;

	public bool isWaitSplashScreen = true;

	public AdvEngine Engine => this.GetComponentCacheFindIfMissing(ref engine);

	public virtual void Start()
	{
		title.gameObject.SetActive(value: false);
		StartCoroutine(CoUpdate());
	}

	protected virtual IEnumerator CoUpdate()
	{
		if (isWaitSplashScreen)
		{
			while (!WrapperUnityVersion.IsFinishedSplashScreen())
			{
				yield return null;
			}
		}
		Open();
		if ((bool)fadeTextureStream)
		{
			fadeTextureStream.gameObject.SetActive(value: true);
			fadeTextureStream.Play();
			while (fadeTextureStream.IsPlaying)
			{
				yield return null;
			}
		}
		if (isWaitBoot)
		{
			while (Engine.IsWaitBootLoading)
			{
				yield return null;
			}
		}
		Close();
		if (isWaitDownLoad && loadWait != null)
		{
			loadWait.OpenOnBoot();
		}
		else
		{
			title.Open();
		}
	}
}
