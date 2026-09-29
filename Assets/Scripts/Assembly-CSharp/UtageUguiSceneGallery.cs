using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utage;
using UtageExtensions;

[AddComponentMenu("Utage/TemplateUI/UtageUguiSceneGallery")]
public class UtageUguiSceneGallery : UguiView
{
	[FormerlySerializedAs("categoryGirdPage")]
	public UguiCategoryGridPage categoryGridPage;

	public UtageUguiGallery gallery;

	public UtageUguiMainGame mainGame;

	[SerializeField]
	private AdvEngine engine;

	protected bool isInit;

	protected List<AdvSceneGallerySettingData> itemDataList = new List<AdvSceneGallerySettingData>();

	public UtageUguiGallery Gallery => this.GetComponentCacheFindIfMissing(ref gallery);

	public AdvEngine Engine => this.GetComponentCacheFindIfMissing(ref engine);

	protected virtual void OnEnable()
	{
		OnClose();
		OnOpen();
	}

	protected virtual void OnOpen()
	{
		ChangeBgm();
		StartCoroutine(CoWaitOpen());
	}

	protected virtual void OnClose()
	{
		categoryGridPage.Clear();
	}

	protected virtual IEnumerator CoWaitOpen()
	{
		isInit = false;
		while (Engine.IsWaitBootLoading)
		{
			yield return null;
		}
		categoryGridPage.Init(Engine.DataManager.SettingDataManager.SceneGallerySetting.CreateCategoryList().ToArray(), OpenCurrentCategory);
		isInit = true;
	}

	protected virtual void Update()
	{
		if (isInit && InputUtil.IsMouseRightButtonDown())
		{
			Gallery.Back();
		}
	}

	protected virtual void OpenCurrentCategory(UguiCategoryGridPage categoryGridPage)
	{
		itemDataList = Engine.DataManager.SettingDataManager.SceneGallerySetting.CreateGalleryDataList(categoryGridPage.CurrentCategory);
		categoryGridPage.OpenCurrentCategory(itemDataList.Count, CreateItem);
	}

	protected virtual void CreateItem(GameObject go, int index)
	{
		AdvSceneGallerySettingData data = itemDataList[index];
		go.GetComponent<UtageUguiSceneGalleryItem>().Init(data, OnTap, Engine.SystemSaveData);
	}

	protected virtual void OnTap(UtageUguiSceneGalleryItem item)
	{
		gallery.Close();
		mainGame.OpenSceneGallery(item.Data.ScenarioLabel);
	}
}
