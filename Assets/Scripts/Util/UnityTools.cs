using System;
using Lite;
using Lite;
using UnityEngine;
using UnityEngine.UI;

namespace Lite
{

	public static class UnityTool
	{

		// 递归设置节点以及所有子节点的layer
		public static void SetLayer(GameObject go, int layer)
		{
			go.layer = layer;
			for (int i = 0; i < go.transform.childCount; ++i)
			{
				SetLayer(go.transform.GetChild(i).gameObject, layer);
			}
		}


		// 获取此处地面高度
		static public float GetGroundHeight(Vector3 position)
		{
			float posY = 0f;
			Ray ray = new Ray(new Vector3(position.x, 1000f, position.z), Vector3.down);
			RaycastHit hit;
			int layerMask = 1 << LayerMask.NameToLayer(AppConst.LayerTerrain) | 1 << LayerMask.NameToLayer(AppConst.LayerLink);
			if (Physics.Raycast(ray, out hit, 2000f, layerMask))
			{
				posY = hit.point.y;
			}
			return posY;
		}


		public static void RemoveAllChildren(GameObject obj)
		{
			for (int i = 0; i < obj.transform.childCount; i++)
			{
				GameObject go = obj.transform.GetChild(i).gameObject;
				GameObject.Destroy(go);
			}
		}

		

		public static void RotateGameObject(GameObject go,float delta)
		{
			var angle = go.transform.localRotation;
			go.transform.localRotation = Quaternion.Euler(angle.x, -delta, angle.z);

		}

		public static void GreyImage(Image img, bool grey)
		{
			if(grey)
			{
				Material newMaterial = Resources.Load("material/grey") as Material;
				img.material = newMaterial;
			}
			else
			{
				img.material = null;
			}
		}

		public static Transform FindBone(Transform tran, string bone)
		{
			if (tran.gameObject.name == bone)
			{
				return tran;
			}
			else
			{
				for (int i = 0; i < tran.transform.childCount; ++i)
				{
					var child = tran.transform.GetChild(i);
					var ret = FindBone(child, bone);
					if (ret != null)
						return ret;
				}
				return null;
			}
		}


	}


}