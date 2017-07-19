
using UnityEngine;
using UnityEditor;



namespace PathFinding
{

	public class NavMeshEditor : EditorWindow
	{
		PathFinding.NavMeshBuilder builder = new PathFinding.NavMeshBuilder();

		GameObject worldBoxObj;
		float gridSize;
		float agentHeight = 2;
		float tan_slope;

		string str_gridSize = "0.5";
		string str_roleHeight = "2";
		string str_slope = "45";

		string saveFilePath = "";
		readonly string drawObjectName = "_NavMeshData_Gizmo";



		void OnEnable()
		{
			string scenePath = EditorUtils.GetCurrentScenePath();
			saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_navmesh.asset";

			if (worldBoxObj == null)
				worldBoxObj = GameObject.Find("worldbox");
		}


		void OnGUI()
		{
			float spaceSize = 3f;

			GUILayout.Label("This tool generates 3d graph from current scene.", EditorStyles.largeLabel);
			GUILayout.Space(spaceSize);

			GUILayout.Label("world box：", EditorStyles.boldLabel);
			worldBoxObj = EditorGUILayout.ObjectField(worldBoxObj, typeof(GameObject), true) as GameObject;
			GUILayout.Space(spaceSize);

			GUILayout.Label("Grid size(0.5-1.0)：", EditorStyles.boldLabel);
			str_gridSize = GUILayout.TextField(str_gridSize);
			GUILayout.Space(spaceSize);

			/*GUILayout.Label("Role height(m)：", EditorStyles.boldLabel);
			str_roleHeight = GUILayout.TextField(str_roleHeight);
			GUILayout.Space(spaceSize);*/

			GUILayout.Label("Max slope(20°-80°)：", EditorStyles.boldLabel);
			str_slope = GUILayout.TextField(str_slope);
			GUILayout.Space(spaceSize);

			if (GUILayout.Button("Generate"))
				GenerateNav();
			GUILayout.Space(spaceSize);

			GUILayout.Label("Output file：", EditorStyles.boldLabel);
			saveFilePath = GUILayout.TextField(saveFilePath);
			GUILayout.Space(spaceSize);

			if (GUILayout.Button("Save"))
				SaveToFile();
			GUILayout.Space(spaceSize);

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
				if (worldBoxObj == null)
				{
					UnityEngine.Debug.LogError("There is no world box object in the scene ? ? ?");
					return;
				}
				var render = worldBoxObj.GetComponent<MeshRenderer>();
				if (render == null)
				{
					UnityEngine.Debug.LogError("World box has no MeshRenderer");
				}

				// parse settings
				gridSize = float.Parse(str_gridSize);
				agentHeight = float.Parse(str_roleHeight);
				float slope = int.Parse(str_slope);
				if (slope < 20 || slope > 80)
					UnityEngine.Debug.LogError("Bad slope! XD");
				tan_slope = Mathf.Tan(slope / 180.0f * Mathf.PI);

				// build
				builder.Stetup(worldBoxObj, gridSize, agentHeight, tan_slope);
				builder.Build();

				// gizmo
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
				gizmo.cfg = builder.cfg;
			}
			catch (System.Exception e)
			{
				UnityEngine.Debug.LogError(e.ToString());
			}

		}


		void ClearDrawGrid()
		{
			var go = GameObject.Find(drawObjectName);
			if (go != null)
			{
				GameObject.DestroyImmediate(go);
			}
		}


		void SaveToFile()
		{
			if (builder.navData == null)
			{
				UnityEngine.Debug.LogError("You should click generate first...");
				return;
			}

			builder.navData.SaveBytes();
			var existingAsset = AssetDatabase.LoadAssetAtPath<NavMeshData>(saveFilePath);
			if (existingAsset == null)
			{
				AssetDatabase.CreateAsset(builder.navData, saveFilePath);
				AssetDatabase.Refresh();
				existingAsset = builder.navData;
			}
			else
			{
				EditorUtility.CopySerialized(builder.navData, existingAsset);
			}

			EditorUtility.SetDirty(builder.navData);

			UnityEngine.Debug.Log("Saved  : " + saveFilePath);
		}

	}

}