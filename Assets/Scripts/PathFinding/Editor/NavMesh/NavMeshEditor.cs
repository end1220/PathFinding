

using UnityEngine;
using UnityEditor;



namespace PathFinding
{

	public class NavMeshEditor : EditorWindow
	{
		NavMeshBuilder builder = new NavMeshBuilder();

		string saveFilePath = "";
		readonly string drawObjectName = "_NavMeshData_Gizmo";



		void OnEnable()
		{
			string scenePath = EditorUtils.GetCurrentScenePath();
			saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_navmesh.asset";
		}


		void OnGUI()
		{
			if (GUILayout.Button("Generate", GUILayout.Width(100), GUILayout.Height(60)))
				GenerateNav();

			GUILayout.Space(20);

			if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(60)))
				SaveToFile();
		}


		void OnDestroy()
		{
			ClearDrawGrid();

			UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
		}


		void GenerateNav()
		{
			try
			{
				builder.ImportMesh();
				builder.ScanInternal();

				navData = ScriptableObject.CreateInstance<NavMeshData>();
				navData.nodes = builder.nodes;

				DrawGrid();

				Selection.SetActiveObjectWithContext(GameObject.Find(drawObjectName), null);
			}
			catch (System.Exception e)
			{
				UnityEngine.Debug.LogError(e.ToString());
				EditorUtility.ClearProgressBar();
			}
		}


		void DrawGrid()
		{
			var go = GameObject.Find(drawObjectName);
			if (go == null)
			{
				go = new GameObject(drawObjectName);
				go.AddComponent<NavMeshGizmo>();
			}
			try
			{
				NavMeshGizmo gizmo = go.GetComponent<NavMeshGizmo>();
				gizmo.navData = navData;
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
			}

		}


		void ClearDrawGrid()
		{
			var go = GameObject.Find(drawObjectName);
			if (go != null)
			{
				DestroyImmediate(go);
			}
		}


		NavMeshData navData;
		void SaveToFile()
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

			EditorUtility.SetDirty(navData);

			Debug.Log("Saved  : " + saveFilePath);
		}


	}

}