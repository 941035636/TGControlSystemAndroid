using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QmDreamer.UI
{
	public class ChinarDragSwapImage : Toggle, IDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler
	{
		private Transform beginParentTransform;

		private Transform topOfUiT;

		protected override void Start()
		{
			base.Start();
			topOfUiT = GameObject.Find("Canvas").transform;
		}

		public void OnBeginDrag(PointerEventData _)
		{
			if (!(base.transform.parent == topOfUiT))
			{
				beginParentTransform = base.transform.parent;
				base.transform.SetParent(topOfUiT);
			}
		}

		public void OnDrag(PointerEventData _)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
			Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, vector.z);
			base.transform.position = Camera.main.ScreenToWorldPoint(position);
			if (base.transform.GetComponent<Image>().raycastTarget)
			{
				base.transform.GetComponent<Image>().raycastTarget = false;
			}
		}

		public void OnEndDrag(PointerEventData _)
		{
			GameObject go = _.pointerCurrentRaycast.gameObject;
			if (go != null && go.tag == "Grid")
			{
				SetPosAndParent(base.transform, go.transform);
				base.transform.GetComponent<Image>().raycastTarget = true;
				SaveData(base.transform);
			}
			else if (go != null && go.tag == "Good")
			{
				SetPosAndParent(base.transform, go.transform.parent);
				go.transform.SetParent(topOfUiT);
				if (Math.Abs(go.transform.position.x - beginParentTransform.position.x) <= 0f)
				{
					go.transform.DOMoveY(beginParentTransform.position.y, 0.3f).OnComplete(delegate
					{
						go.transform.SetParent(beginParentTransform);
						SaveData(base.transform, go.transform);
						base.transform.GetComponent<Image>().raycastTarget = true;
					}).SetEase(Ease.InOutQuint);
					Debug.LogError("go:" + go.transform.name + "     transform:" + base.transform.name);
					return;
				}
				go.transform.DOMoveX(beginParentTransform.position.x, 0.2f).OnComplete(delegate
				{
					go.transform.DOMoveY(beginParentTransform.position.y, 0.3f).OnComplete(delegate
					{
						go.transform.SetParent(beginParentTransform);
						SaveData(base.transform, go.transform);
						base.transform.GetComponent<Image>().raycastTarget = true;
					}).SetEase(Ease.InOutQuint);
				});
				Debug.LogError("go:" + go.transform.name + "     transform:" + base.transform.name);
			}
			else
			{
				SetPosAndParent(base.transform, beginParentTransform);
				base.transform.GetComponent<Image>().raycastTarget = true;
			}
		}

		private void SetPosAndParent(Transform t, Transform parent)
		{
			t.SetParent(parent);
			t.position = parent.position;
		}

		private void SaveData(Transform transform, Transform gotransform)
		{
			string index = transform.parent.parent.parent.parent.name.Split('e')[2];
			GameResources.Instance.RegisterGameDataPath(index);
			List<LightCellItemModel> lightCellItemList = LightCellItemControl.GetInstance().GetLightCellItemList(index);
			FileUtil.DeleteFile(FileUtil.CreateFolder(GameResources.LightCellItemPath, "Light"), "LightCellItem.txt");
			for (int i = 0; i < lightCellItemList.Count; i++)
			{
				if (lightCellItemList[i].Get_CellItemName == transform.name)
				{
					lightCellItemList[i].Get_ParentName = transform.parent.name;
					transform.name = transform.parent.name.Split('l')[2];
					lightCellItemList[i].Get_CellItemName = transform.name;
				}
				if (lightCellItemList[i].Get_CellItemName == gotransform.name)
				{
					lightCellItemList[i].Get_ParentName = gotransform.parent.name;
					gotransform.name = gotransform.parent.name.Split('l')[2];
					lightCellItemList[i].Get_CellItemName = gotransform.name;
				}
				LightCellItemControl.GetInstance().AddLightItem(index, lightCellItemList[i]);
			}
		}

		private void SaveData(Transform transform)
		{
			string index = transform.parent.parent.parent.parent.name.Split('e')[2];
			GameResources.Instance.RegisterGameDataPath(index);
			List<LightCellItemModel> lightCellItemList = LightCellItemControl.GetInstance().GetLightCellItemList(index);
			FileUtil.DeleteFile(FileUtil.CreateFolder(GameResources.LightCellItemPath, "Light"), "LightCellItem.txt");
			for (int i = 0; i < lightCellItemList.Count; i++)
			{
				if (lightCellItemList[i].Get_CellItemName == transform.name)
				{
					lightCellItemList[i].Get_ParentName = transform.parent.name;
					transform.name = transform.parent.name.Split('l')[2];
					lightCellItemList[i].Get_CellItemName = transform.name;
				}
				LightCellItemControl.GetInstance().AddLightItem(index, lightCellItemList[i]);
			}
		}
	}
}
