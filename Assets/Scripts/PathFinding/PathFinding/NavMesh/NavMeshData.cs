

using UnityEngine;
using System.Collections.Generic;



namespace PathFinding
{
	/// <summary>
	/// 存储着navmesh数据
	/// </summary>
	public class NavMeshData : ScriptableObject
	{
		public NavMeshNode[] nodes;

		public bool ShowMesh = true;

		private Material navmeshMaterial;
		private Material navmeshOutlineMaterial;

		string editorAssets = "Assets/Scripts/PathFinding/Editor/EditorAssets";

		class GizmoMeshData
		{
			public Mesh surfaceMesh;
			public Mesh outlineMesh;
		}

		List<GizmoMeshData> gizmoMeshes = new List<GizmoMeshData>();


		public void OnDrawGizmosSelected(Transform transform)
		{
#if UNITY_EDITOR
			if (navmeshMaterial == null)
			{
				navmeshMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(editorAssets + "/Materials/Navmesh.mat");
				navmeshOutlineMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(editorAssets + "/Materials/NavmeshOutline.mat");
			}
#endif

			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Color defaultColor = Gizmos.color;

			// begin draw

			NavMeshNode[] nodes = this.nodes;

			for (int i = 0; i < nodes.Length; i++)
			{
				NavMeshNode node = nodes[i] as NavMeshNode;

				float a1 = VectorMath.SignedTriangleAreaTimes2XZ((Vector3)node.v0, (Vector3)node.v1, (Vector3)node.v2);

				long a2 = VectorMath.SignedTriangleAreaTimes2XZ(node.v0, node.v1, node.v2);
				if (a1 * a2 < 0) Debug.LogError(a1 + " " + a2);


				if (VectorMath.IsClockwiseXZ(node.v0, node.v1, node.v2))
				{
					Debug.DrawLine((Vector3)node.v0, (Vector3)node.v1, Color.black);
					Debug.DrawLine((Vector3)node.v1, (Vector3)node.v2, Color.black);
					Debug.DrawLine((Vector3)node.v2, (Vector3)node.v0, Color.black);
				}
				else
				{
					Debug.DrawLine((Vector3)node.v0, (Vector3)node.v1, Color.red);
					Debug.DrawLine((Vector3)node.v1, (Vector3)node.v2, Color.red);
					Debug.DrawLine((Vector3)node.v2, (Vector3)node.v0, Color.red);
				}
			}

			UpdateGizmoMeshes();

			if (ShowMesh)
			{
				for (int pass = 0; pass <= 2; pass++)
				{
					navmeshMaterial.SetPass(pass);
					for (int i = 0; i < gizmoMeshes.Count; i++)
					{
						Graphics.DrawMeshNow(gizmoMeshes[i].surfaceMesh, Matrix4x4.identity);
					}
				}
			}

			/*navmeshOutlineMaterial.SetPass(0);
			for (int i = 0; i < gizmoMeshes.Count; i++)
			{
				Graphics.DrawMeshNow(gizmoMeshes[i].outlineMesh, Matrix4x4.identity);
			}*/


			// end draw

			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
		}


		private void UpdateGizmoMeshes()
		{
			gizmoMeshes.Clear();

			NavMeshNode[] nodes = this.nodes;

			for (int i = 0; i < nodes.Length; i++)
			{
				GizmoMeshData data = new GizmoMeshData();
				gizmoMeshes.Add(data);

				var mesh = new Mesh();
				mesh.hideFlags = HideFlags.DontSave;
				mesh.vertices = new Vector3[] { (Vector3)nodes[i].v0, (Vector3)nodes[i].v1, (Vector3)nodes[i].v2 };
				mesh.triangles = new int[] { 0, 1, 2 };
				mesh.RecalculateNormals();
				data.surfaceMesh = mesh;

				/*var mesh2 = new Mesh();
				mesh2.hideFlags = HideFlags.DontSave;
				Vector3 v0 = (Vector3)nodes[i].v0;
				Vector3 v1 = (Vector3)nodes[i].v1;
				Vector3 v2 = (Vector3)nodes[i].v2;
				mesh2.vertices = new Vector3[] { v0, v0, v1, v1, v2, v2 };
				mesh2.triangles = new int[] { 0, 1, 2, 2, 3, 0, 2, 4, 5, 4, 5, 3, 0, 5, 4, 4, 1, 0 };
				mesh2.RecalculateNormals();
				data.outlineMesh = mesh2;*/

			}

		}


	}

}
