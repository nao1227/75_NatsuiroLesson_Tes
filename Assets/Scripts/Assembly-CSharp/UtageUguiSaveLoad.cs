using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;
using UtageExtensions;

[AddComponentMenu("Utage/TemplateUI/UtageUguiSaveLoad")]
public class UtageUguiSaveLoad : UguiView
{
	[SerializeField]
	protected UguiGridPage gridPage;

	protected List<AdvSaveData> itemDataList;

	[SerializeField]
	protected AdvEngine engine;

	public UtageUguiMainGame mainGame;

	public GameObject saveRoot;

	public GameObject loadRoot;

	public float waitTimeOnLoad;

	protected bool isSave;

	protected bool isInit;

	protected int lastPage;

	public virtual AdvEngine Engine => this.GetComponentCacheFindIfMissing(ref engine);

	public virtual void OpenSave(UguiView prev)
	{
		isSave = true;
		saveRoot.SetActive(value: true);
		loadRoot.SetActive(value: false);
		Open(prev);
	}

	public virtual void OpenLoad(UguiView prev)
	{
		isSave = false;
		saveRoot.SetActive(value: false);
		loadRoot.SetActive(value: true);
		Open(prev);
	}

	protected virtual void OnOpen()
	{
		isInit = false;
		gridPage.ClearItems();
		StartCoroutine(CoWaitOpen());
	}

	protected virtual void OnClose()
	{
		lastPage = gridPage.CurrentPage;
		gridPage.ClearItems();
	}

	protected virtual IEnumerator CoWaitOpen()
	{
		while (Engine.IsWaitBootLoading)
		{
			yield return null;
		}
		AdvSaveManager saveManager = Engine.SaveManager;
		saveManager.ReadAllSaveData();
		List<AdvSaveData> list = new List<AdvSaveData>();
		if (saveManager.IsAutoSave)
		{
			list.Add(saveManager.AutoSaveData);
		}
		list.AddRange(saveManager.SaveDataList);
		itemDataList = list;
		gridPage.Init(itemDataList.Count, CallBackCreateItem);
		gridPage.CreateItems(lastPage);
		isInit = true;
	}

	protected virtual void CallBackCreateItem(GameObject go, int index)
	{
		UtageUguiSaveLoadItem component = go.GetComponent<UtageUguiSaveLoadItem>();
		AdvSaveData data = itemDataList[index];
		component.Init(data, OnTap, index, isSave);
	}

	protected virtual void Update()
	{
		if (isInit && InputUtil.IsMouseRightButtonDown())
		{
			Back();
		}
	}

	public virtual void OnTap(UtageUguiSaveLoadItem item)
	{
		if (isSave)
		{
			Engine.WriteSaveData(item.Data);
			item.Refresh(isSave: true);
		}
		else if (item.Data.IsSaved)
		{
			if (waitTimeOnLoad <= 0f)
			{
				Close();
				mainGame.OpenLoadGame(item.Data);
			}
			else
			{
				mainGame.OpenLoadGame(item.Data);
				StartCoroutine(CoWaitOnLoad(item));
			}
		}
	}

	private IEnumerator CoWaitOnLoad(UtageUguiSaveLoadItem item)
	{
		StoreAndChangeCanvasGroupInput(isActive: false);
		yield return new WaitForSeconds(waitTimeOnLoad);
		RestoreCanvasGroupInput();
		Close();
	}
}
