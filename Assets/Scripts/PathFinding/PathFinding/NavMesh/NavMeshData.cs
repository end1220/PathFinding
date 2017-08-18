

using UnityEngine;
using System.Collections.Generic;



namespace PathFinding
{
	/// <summary>
	/// 存储着navmesh数据
	/// </summary>
	public class NavMeshData : INavData
	{
		public NavMeshNode[] nodes;

		//public List<Int3> insertPoints = new List<Int3>();

		private bool ShowMesh = true;

		private Material navmeshMaterial;
		//private Material navmeshOutlineMaterial;

		string editorAssets = "Assets/Scripts/TwAI/PathFinding/Editor/EditorAssets";

		class GizmoMeshData
		{
			public Mesh surfaceMesh;
			//public Mesh outlineMesh;
		}

		List<GizmoMeshData> gizmoMeshes = new List<GizmoMeshData>();
		private Color lineColor = new Color(0.25f, 0.25f, 0.25f);


		public override void OnDrawGizmosSelected(Transform transform)
		{
#if UNITY_EDITOR
			if (navmeshMaterial == null)
			{
				navmeshMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(editorAssets + "/Materials/Navmesh.mat");
				if (navmeshMaterial == null)
				{
					editorAssets = "Assets/Scripts/PathFinding/Editor/EditorAssets";
					navmeshMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(editorAssets + "/Materials/Navmesh.mat");
				}
				//navmeshOutlineMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(editorAssets + "/Materials/NavmeshOutline.mat");
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
					Debug.DrawLine((Vector3)node.v0, (Vector3)node.v1, lineColor);
					Debug.DrawLine((Vector3)node.v1, (Vector3)node.v2, lineColor);
					Debug.DrawLine((Vector3)node.v2, (Vector3)node.v0, lineColor);
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
					if (navmeshMaterial != null)
						navmeshMaterial.SetPass(pass);
					for (int i = 0; i < gizmoMeshes.Count; i++)
					{
						Graphics.DrawMeshNow(gizmoMeshes[i].surfaceMesh, Matrix4x4.identity);
					}
				}
			}

			/*for (int i = 0; i < insertPoints.Count; i+=3)
			{
				Debug.DrawLine(Vector3.up + (Vector3)insertPoints[i], Vector3.up + (Vector3)insertPoints[i + 1], Color.green);
				Debug.DrawLine(Vector3.up + (Vector3)insertPoints[i + 1], Vector3.up + (Vector3)insertPoints[i + 2], Color.green);
				Debug.DrawLine(Vector3.up + (Vector3)insertPoints[i + 2], Vector3.up + (Vector3)insertPoints[i], Color.green);
			}*/


			// end draw
			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
		}


		private void UpdateGizmoMeshes()
		{
			if (gizmoMeshes.Count > 0)
				return;
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
