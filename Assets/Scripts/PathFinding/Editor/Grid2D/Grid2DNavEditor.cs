
using UnityEngine;
using UnityEditor;



namespace PathFinding
{

	public class Grid2DNavEditor : EditorWindow
	{
		Grid2DBuilder builder = new Grid2DBuilder();

		string str_gridSize = "0.5";
		string str_roleRadius = "0.5";
		string str_slope = "45";

		int slope;

		string saveFilePath = "Assets/{0}_navgrid.asset";
		readonly string drawObjectName = "_NavigationData_Gizmo";


		void OnEnable()
		{
			string scenePath = EditorUtils.GetCurrentScenePath();
			saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_navgrid.asset";

			if (builder.worldBoxObj == null)
				builder.worldBoxObj = GameObject.Find("worldbox");
		}


		void OnGUI()
		{
			float spaceSize = 5f;
			float leftSpace = 10;
			float titleLen = 90;
			float textLen = 250;
			float buttonLen = 200;
			float buttonHeight = 40;

			GUILayout.Label("  Generating 2D Nav Grid from current scene.\n  Ensure there's a AABB world box in the scene.", EditorStyles.helpBox);
			GUILayout.Space(spaceSize * 2);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("World box", EditorStyles.label, GUILayout.Width(titleLen));
			builder.worldBoxObj = EditorGUILayout.ObjectField(builder.worldBoxObj, typeof(GameObject), true, GUILayout.Width(textLen + 20)) as GameObject;
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("Role radius(m)", EditorStyles.label, GUILayout.Width(titleLen));
			str_roleRadius = GUILayout.TextField(str_roleRadius, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("Grid size(m)", EditorStyles.label, GUILayout.Width(titleLen));
			str_gridSize = GUILayout.TextField(str_gridSize, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("Max slope(°)", EditorStyles.label, GUILayout.Width(titleLen));
			str_slope = GUILayout.TextField(str_slope, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			if (GUILayout.Button("Generate", GUILayout.Width(buttonLen), GUILayout.Height(buttonHeight)))
				GenerateNav();
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("Output file", EditorStyles.label, GUILayout.Width(titleLen));
			saveFilePath = GUILayout.TextField(saveFilePath, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			if (GUILayout.Button("Save", GUILayout.Width(buttonLen), GUILayout.Height(buttonHeight)))
				SaveToFile();
			GUILayout.EndHorizontal();
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
				if (builder.worldBoxObj == null)
				{
					UnityEngine.Debug.LogError("Build failed : There is no world box object in the scene ???");
					return;
				}
				var render = builder.worldBoxObj.GetComponent<MeshRenderer>();
				if (render == null)
				{
					UnityEngine.Debug.LogError("Build failed : world box has no MeshRenderer");
					return;
				}

				builder.gridSize = float.Parse(str_gridSize);
				builder.agentRadius = float.Parse(str_roleRadius);
				var worldSize = render.bounds.size;
				builder.width = (int)(worldSize.x / builder.gridSize);
				builder.height = (int)(worldSize.z / builder.gridSize);
				var worldMin = render.bounds.min;
				builder.minX = worldMin.x;
				builder.minZ = worldMin.z;
				slope = int.Parse(str_slope);
				builder.tan_slope = Mathf.Tan(slope / 180.0f * Mathf.PI);

				if (!CheckParamsValid())
					return;

				builder.Build();

				DrawGrid();
				Selection.SetActiveObjectWithContext(GameObject.Find(drawObjectName), null);

			}
			catch (System.Exception e)
			{
				UnityEngine.Debug.LogError(e.ToString());
				EditorUtility.ClearProgressBar();
			}
		}


		private bool CheckParamsValid()
		{
			if (builder.gridSize < 0.5f || builder.gridSize > 1.0f)
			{
				UnityEngine.Debug.LogError("Build failed : gird size should be [0.5, 1.0]");
				return false;
			}
			if (builder.agentRadius < 0.1f || builder.agentRadius > 1.0f)
			{
				UnityEngine.Debug.LogError("Build failed : agent radius should be [0.1, 1.0]");
				return false;
			}
			if (slope < 0 || slope > 60)
			{
				UnityEngine.Debug.LogError("Build failed : slope should be [0, 60].");
				return false;
			}
			return true;
		}


		void SaveToFile()
		{
			if (builder.navigation == null)
			{
				UnityEngine.Debug.LogError("You should click generate first...");
				return;
			}

			var existingAsset = AssetDatabase.LoadAssetAtPath<NavGrid2DData>(saveFilePath);
			if (existingAsset == null)
			{
				AssetDatabase.CreateAsset(builder.navigation, saveFilePath);
				AssetDatabase.Refresh();
				existingAsset = builder.navigation;
			}
			else
			{
				EditorUtility.CopySerialized(builder.navigation, existingAsset);
			}

			EditorUtility.SetDirty(builder.navigation);

			UnityEngine.Debug.Log("File saved  : " + saveFilePath);
		}


		void DrawGrid()
		{
			var go = GameObject.Find(drawObjectName);
			if (go == null)
			{
				go = new GameObject(drawObjectName);
				go.AddComponent<NavGrid2DGizmo>();
			}
			NavGrid2DGizmo gizmoLine = go.GetComponent<NavGrid2DGizmo>();
			Vector3[,] pos = new Vector3[builder.width, builder.height];
			for (int x = 0; x < builder.width; ++x)
			{
				for (int y = 0; y < builder.height; ++y)
				{
					float fposx = builder.minX + builder.gridSize * (x + 0.5f);
					float fposz = builder.minZ + builder.gridSize * (y + 0.5f);
					pos[x, y] = new Vector3(fposx, 1, fposz);
				}
			}
			gizmoLine.SetGridPosList(builder.navigation, pos, builder.width, builder.height);
		}


		void ClearDrawGrid()
		{
			var go = GameObject.Find(drawObjectName);
			if (go != null)
			{
				GameObject.DestroyImmediate(go);
			}
		}

	}

}