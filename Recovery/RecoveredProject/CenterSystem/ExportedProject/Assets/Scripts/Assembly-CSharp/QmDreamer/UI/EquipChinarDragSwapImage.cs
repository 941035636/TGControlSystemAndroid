using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QmDreamer.UI
{
	public class EquipChinarDragSwapImage : Toggle, IDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler
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
				SaveData();
			}
			else if (go != null && go.tag == "Good")
			{
				SetPosAndParent(base.transform, go.transform.parent);
				SaveData();
				go.transform.SetParent(topOfUiT);
				if (Math.Abs(go.transform.position.x - beginParentTransform.position.x) <= 0f)
				{
					go.transform.DOMoveY(beginParentTransform.position.y, 0.3f).OnComplete(delegate
					{
						go.transform.SetParent(beginParentTransform);
						base.transform.GetComponent<Image>().raycastTarget = true;
					}).SetEase(Ease.InOutQuint);
					return;
				}
				go.transform.DOMoveX(beginParentTransform.position.x, 0.2f).OnComplete(delegate
				{
					go.transform.DOMoveY(beginParentTransform.position.y, 0.3f).OnComplete(delegate
					{
						go.transform.SetParent(beginParentTransform);
						base.transform.GetComponent<Image>().raycastTarget = true;
					}).SetEase(Ease.InOutQuint);
				});
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

		private void SaveData()
		{
			string index = base.transform.parent.parent.parent.parent.name.Split('p')[2];
			GameResources.Instance.RegisterEquipGameDataPath(index);
			List<LightCellItemModel> equipCellItemList = EquipCellItemControl.GetInstance().GetEquipCellItemList(index);
			FileUtil.DeleteFile(FileUtil.CreateFolder(GameResources.EquipCellItemPath, "Equip"), "EquipCellItem.txt");
			for (int i = 0; i < equipCellItemList.Count; i++)
			{
				if (equipCellItemList[i].Get_CellItemName == base.transform.name)
				{
					equipCellItemList[i].Get_ParentName = base.transform.parent.name;
					base.transform.name = base.transform.parent.name.Split('l')[2];
					equipCellItemList[i].Get_CellItemName = base.transform.parent.name.Split('l')[2];
				}
				EquipCellItemControl.GetInstance().AddEquipItem(base.transform.parent.parent.parent.parent.name.Split('p')[2], equipCellItemList[i]);
			}
		}
	}
}
