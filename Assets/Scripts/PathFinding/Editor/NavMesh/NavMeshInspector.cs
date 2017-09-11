
using UnityEngine;
using UnityEditor;



namespace PathFinding
{

	public class NavMeshInspector : EditorInspector
	{
		NavMeshBuilder builder = new NavMeshBuilder();

		string saveFilePath = "Assets/{0}_navmesh.asset";

		NavMeshData navData;


		public NavMeshInspector(PathFindingMachine machine):
			base(machine)
		{
			string scenePath = GetCurrentScenePath();
			string sceneName = GetCurrentSceneName();
			saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "/" + sceneName + "_nav.asset";

			// load previous settings
			var existingAsset = AssetDatabase.LoadAssetAtPath<NavMeshData>(saveFilePath);
			if (existingAsset != null)
			{
				
			}

		}


		public override void DrawInspector()
		{
			base.DrawInspector();
		}

		public override void Clear()
		{
			AssetDatabase.DeleteAsset(saveFilePath);
		}

		public override void Bake()
		{
			try
			{
				builder.Build();

				navData = ScriptableObject.CreateInstance<NavMeshData>();
				navData.nodes = builder.nodes;
				//navData.insertPoints.AddRange(builder.newPoints);

				machine.navgationData = navData;
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
				EditorUtility.ClearProgressBar();
			}
		}

		public override void Save()
		{
			var existingAsset = AssetDatabase.LoadAssetAtPath<NavMeshData>(saveFilePath);
			if (existingAsset == null)
			{
				AssetDatabase.CreateAsset(navData, saveFilePath);
				AssetDatabase.Refresh();
				existingAsset = navData;
			}
			else
			{
				EditorUtility.CopySerialized(navData, existingAsset);
			}

			machine.navgationData = navData;

			EditorUtility.SetDirty(navData);

			Debug.Log("Saved  : " + saveFilePath);
		}


		public void ExportObj()
		{
			string exepath = EditorUtility.OpenFolderPanel("Select Folder", System.Environment.CurrentDirectory, "");
			string filename = exepath + "/" +
				System.IO.Path.GetFileNameWithoutExtension(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path) + "_NavMesh.obj";

			builder.Build();

			Vector3[] vertices = new Vector3[builder._vertices.Length];
			for (int i = 0; i < vertices.Length; ++i)
			{
				vertices[i] = (Vector3)builder._vertices[i];
			}

			using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
			{
				Mesh mesh = new Mesh();
				mesh.name = "ExportedNavMesh";
				mesh.vertices = vertices;
				mesh.triangles = builder._triangles;

				sw.Write(MeshToString(mesh));
			}
		}

		static string MeshToString(Mesh mesh)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			sb.Append("g ").Append(mesh.name).Append("\n");
			foreach (Vector3 v in mesh.vertices)
				sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
			
			sb.Append("\n");

			foreach (Vector3 v in mesh.normals)
				sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
			
			sb.Append("\n");

			foreach (Vector3 v in mesh.uv)
				sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
			
			for (int material = 0; material < mesh.subMeshCount; material++)
			{
				sb.Append("\n");
				int[] triangles = mesh.GetTriangles(material);
				for (int i = 0; i < triangles.Length; i += 3)
				{
					sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
				}
			}

			return sb.ToString();
		}

	}

}