using UnityEngine;
using System;
using System.Collections.Generic;
using Lite.NavMesh;
using Lite.AStar;


namespace Lite.NavMesh
{
	public class NavMeshGizmo : MonoBehaviour
	{
		public NavMeshData navMeshData;

		void OnDrawGizmosSelected()
		{
			if (navMeshData == null)
				return;

			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Color defaultColor = Gizmos.color;

			// begin draw

			float a = 0.05f;
			for (int i = 0; i < navMeshData.points.Count; ++i)
			{
				Vector3 c = navMeshData.points[i];
				Gizmos.color = Color.green;
				DrawRect(c, a);
			}

			// end draw

			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
		}


		private void DrawRect(Vector3 center, float a)
		{
			var v1 = new Vector3(center.x - a, center.y, center.z - a);
			var v2 = new Vector3(center.x + a, center.y, center.z - a);
			var v3 = new Vector3(center.x + a, center.y, center.z + a);
			var v4 = new Vector3(center.x - a, center.y, center.z + a);
			Gizmos.DrawLine(v1, v2);
			Gizmos.DrawLine(v2, v3);
			Gizmos.DrawLine(v3, v4);
			Gizmos.DrawLine(v4, v1);
		}

	}

}