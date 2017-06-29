using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using Lite;
using Lite.AStar;
using Lite.Graph;


public class Grid2DEditor : EditorWindow
{
	class GridInfo
	{
		public int mask;
		public Vector3 point;
		public int layer;
	}

	GameObject worldBoxObj;

	float minX;
	float minZ;
	
	float gridSize;
	int width;
	int height;
	GridInfo[,] gridList;
	float slope;
	float tan_slope;

	string str_gridSize = "0.5";
	//string str_width = "50";
	//string str_height = "50";
	string str_slope = "45";

	NavGrid2DData navigation;
	string saveFilePath = "Assets/{0}_navgrid.asset";
	readonly string drawObjectName = "_NavigationData_Gizmo";


	void OnEnable()
	{
		string scenePath = EditorUtils.GetCurrentScenePath();
		saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_navgrid.asset";

		if (worldBoxObj == null)
			worldBoxObj = GameObject.Find("worldbox");
	}


	void OnGUI()
	{
		float spaceSize = 3f;

		GUILayout.Label("This tool generates Navigation Data from current scene.", EditorStyles.largeLabel);
		GUILayout.Space(spaceSize);

		GUILayout.Label("world box：", EditorStyles.boldLabel);
		worldBoxObj = EditorGUILayout.ObjectField(worldBoxObj, typeof(GameObject), true) as GameObject;
		GUILayout.Space(spaceSize);

		/*GUILayout.Label("Width(m)：", EditorStyles.boldLabel);
		str_width = GUILayout.TextField(str_width);
		GUILayout.Space(spaceSize);

		GUILayout.Label("Height(m)：", EditorStyles.boldLabel);
		str_height = GUILayout.TextField(str_height);
		GUILayout.Space(spaceSize);*/

		GUILayout.Label("Grid size(0.5m-1m)：", EditorStyles.boldLabel);
		str_gridSize = GUILayout.TextField(str_gridSize);
		GUILayout.Space(spaceSize);

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
				UnityEngine.Debug.LogError("There is no world box object in the scene ???");
				return;
			}
			var render = worldBoxObj.GetComponent<MeshRenderer>();
			if (render == null)
			{
				UnityEngine.Debug.LogError("world box has no MeshRenderer");
			}

			gridSize = float.Parse(str_gridSize);

			var worldSize = render.bounds.size;
			width = (int)(worldSize.x / gridSize);
			height = (int)(worldSize.z / gridSize);
			var worldMin = render.bounds.min;
			minX = worldMin.x;
			minZ = worldMin.z;
			
			slope = int.Parse(str_slope);
			if (slope < 20 || slope > 80)
			{
				UnityEngine.Debug.LogError("Bad slope! XD");
				return;
			}
			tan_slope = Mathf.Tan(slope / 180.0f * Mathf.PI);

			Fill_Grid();
			DrawGrid();

			Selection.SetActiveObjectWithContext(GameObject.Find(drawObjectName), null);

		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError(e.ToString());
			EditorUtility.ClearProgressBar();
		}
	}


	void Fill_Grid()
	{
		int terrainLayer = LayerMask.NameToLayer(AppConst.LayerTerrain);
		int obstacleLayer = LayerMask.NameToLayer(AppConst.LayerObstacle);
		int linkLayer = LayerMask.NameToLayer(AppConst.LayerLink);

		gridList = new GridInfo[width, height];
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				gridList[x, y] = new GridInfo();
				gridList[x, y].mask = 0;
			}
		}

		// get hit points.
		Ray ray = new Ray();
		ray.direction = new Vector3(0, -1, 0);

		RaycastHit hit;
		int layerMask = 1 << terrainLayer
			| 1 << obstacleLayer
			| 1 << linkLayer;

		float badY = 10000;
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				// sides
				float[] sidex = new float[]
				{
					minX + gridSize * x - gridSize * 0.5f,
					minX + gridSize * x - gridSize * 0.5f,
					minX + gridSize * x + gridSize * 0.5f,
					minX + gridSize * x + gridSize * 0.5f,
				};
				float[] sidez = new float[]
				{
					minZ + gridSize * y - gridSize * 0.5f,
					minZ + gridSize * y + gridSize * 0.5f,
					minZ + gridSize * y + gridSize * 0.5f,
					minZ + gridSize * y - gridSize * 0.5f,
				};
				bool hitObstacle = false;
				for (int s = 0; s < 4; s++)
				{
					ray.origin = new Vector3(sidex[s], 500, sidez[s]);
					if (Physics.Raycast(ray, out hit, 1000, layerMask))
					{
						if (hit.collider.gameObject.layer == obstacleLayer)
						{
							hitObstacle = true;
							break;
						}
					}
				}

				// center
				float fx = minX + gridSize * x;
				float fz = minZ + gridSize * y;

				ray.origin = new Vector3(fx, 500, fz);
				float fposy = badY;
				int hitLayer = -1;
				if (Physics.Raycast(ray, out hit, 1000, layerMask))
				{
					fposy = hit.point.y;
					hitLayer = hit.collider.gameObject.layer;
				}
				gridList[x, y].point = new Vector3(fx, fposy, fz);
				gridList[x, y].layer = hitObstacle ? obstacleLayer : hitLayer;
			}
			UpdateProgress(x, width, "");
		}

		

		// calculate slope values.
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				var cp = gridList[x, y];

				if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
				{
					// not passable.
					cp.mask = 1;
				}
				else if (cp.layer == obstacleLayer)
				{
					// not passable.
					cp.mask = 1;
				}
				else
				{
					
					var xr = gridList[x + 1, y];
					var xl = gridList[x - 1, y];
					var zr = gridList[x, y + 1];
					var zl = gridList[x, y - 1];

					int xrLayer = xr.layer;
					int xlLayer = xl.layer;
					int zrLayer = zr.layer;
					int zlLayer = zl.layer;

					if (cp.point.y == badY || (xr.point.y == badY && xl.point.y == badY) || (zr.point.y == badY && zl.point.y == badY))
					{
						cp.mask = 1;
						continue;
					}
					//float tanx = Mathf.Abs((xr.point.y - xl.point.y) / (xr.point.x - xl.point.x));
					//float tanz = Mathf.Abs((zr.point.y - zl.point.y) / (zr.point.z - zl.point.z));
					float tanx = Mathf.Min(Mathf.Abs((cp.point.y - xl.point.y) / (cp.point.x - xl.point.x)), Mathf.Abs((cp.point.y - xr.point.y) / (cp.point.x - xr.point.x)));
					float tanz = Mathf.Min(Mathf.Abs((cp.point.y - zl.point.y) / (cp.point.z - zl.point.z)), Mathf.Abs((cp.point.y - zr.point.y) / (cp.point.z - zr.point.z)));
					if (xrLayer == linkLayer || xlLayer == linkLayer)
						tanx = 0;
					if (zrLayer == linkLayer || zlLayer == linkLayer)
						tanz = 0;
					float maxTan = Mathf.Max(tanx, tanz);
					cp.mask = maxTan > tan_slope ? 1 : 0;
				}
			}
			UpdateProgress(x, width, "");
		}

		NavGrid2DData nav = ScriptableObject.CreateInstance<NavGrid2DData>();
		int[,] maskList = new int[width, height];
		for (int x = 0; x < width; ++x)
			for (int y = 0; y < height; ++y)
				maskList[x, y] = gridList[x, y].mask;
		nav._setData(maskList, width, height, (int)TwMath.m2mm(gridSize), (int)TwMath.m2mm(minX), (int)TwMath.m2mm(minZ));
		navigation = nav;

		EditorUtility.ClearProgressBar();
	}


	void SaveToFile()
	{
		/*NavigationData nav = ScriptableObject.CreateInstance<NavigationData>();
		nav._setData(nodeMarkList, width, height, gridSize, minX, minZ);*/

		if (navigation == null)
		{
			UnityEngine.Debug.LogError("You should click generate first...");
			return;
		}

		var existingAsset = AssetDatabase.LoadAssetAtPath<NavGrid2DData>(saveFilePath);
		if (existingAsset == null)
		{
			AssetDatabase.CreateAsset(navigation, saveFilePath);
			AssetDatabase.Refresh();
			existingAsset = navigation;
		}
		else
		{
			EditorUtility.CopySerialized(navigation, existingAsset);
		}

		EditorUtility.SetDirty(navigation);

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

		Vector3[,] pos = new Vector3[width, height];

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				float fposx = minX + gridSize * x;
				float fposz = minZ + gridSize * y;
				pos[x,y] = new Vector3(fposx, 1, fposz);
			}
		}

		gizmoLine.SetGridPosList(navigation, pos, width, height);
	}


	void ClearDrawGrid()
	{
		var go = GameObject.Find(drawObjectName);
		if (go != null)
		{
			GameObject.DestroyImmediate(go);
		}
	}


	static void UpdateProgress(int progress, int progressMax, string desc)
	{
		string title = "Processing...[" + progress + " / " + progressMax + "]";
		float value = (float)progress / (float)progressMax;
		EditorUtility.DisplayProgressBar(title, desc, value);
	}


}