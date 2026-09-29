using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utage;
using UtageExtensions;

[AddComponentMenu("Utage/TemplateUI/UtageUguiCgGallery")]
public class UtageUguiCgGallery : UguiView
{
	[SerializeField]
	private UtageUguiGallery gallery;

	public UtageUguiCgGalleryViewer CgView;

	[FormerlySerializedAs("categoryGirdPage")]
	public UguiCategoryGridPage categoryGridPage;

	private List<AdvCgGalleryData> itemDataList = new List<AdvCgGalleryData>();

	[SerializeField]
	private AdvEngine engine;

	protected bool isInit;

	public UtageUguiGallery Gallery => this.GetComponentCacheFindIfMissing(ref gallery);

	public AdvEngine Engine => this.GetComponentCacheFindIfMissing(ref engine);

	protected virtual void OnOpen()
	{
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
		categoryGridPage.Init(Engine.DataManager.SettingDataManager.TextureSetting.CreateCgGalleryCategoryList().ToArray(), OpenCurrentCategory);
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
		itemDataList = Engine.DataManager.SettingDataManager.TextureSetting.CreateCgGalleryList(Engine.SystemSaveData.GalleryData, categoryGridPage.CurrentCategory);
		categoryGridPage.OpenCurrentCategory(itemDataList.Count, CreateItem);
	}

	protected virtual void CreateItem(GameObject go, int index)
	{
		AdvCgGalleryData data = itemDataList[index];
		go.GetComponent<UtageUguiCgGalleryItem>().Init(data, OnTap);
	}

	protected virtual void OnTap(UtageUguiCgGalleryItem item)
	{
		CgView.Open(item.Data);
	}
}
