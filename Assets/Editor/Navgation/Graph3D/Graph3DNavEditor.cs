using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using FixedPoint;
using AStar;
using Graph;
using AStar.NavGraph;


public class Graph3DNavEditor : EditorWindow
{
	Graph3DBuilder builder = new Graph3DBuilder();

	GameObject worldBoxObj;
	float gridSize;
	float agentHeight = 2;
	float agentRadius = 0.5f;
	float tan_slope;

	string str_gridSize = "1";
	string str_roleHeight = "2";
	string str_roleRadius = "0.5";
	string str_slope = "45";

	string saveFilePath = "Assets/{0}_navgraph.asset";
	readonly string drawObjectName = "_NavGraphData_Gizmo";



	void OnEnable()
	{
		string scenePath = EditorUtils.GetCurrentScenePath();
		saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_navgraph.asset";

		if (worldBoxObj == null)
			worldBoxObj = GameObject.Find("worldbox");
	}


	void OnGUI()
	{
		float spaceSize = 5f;
		float leftSpace = 10;
		float titleLen = 90;
		float textLen = 250;
		float buttonLen = 200;
		float buttonHeight = 40;

		GUILayout.Label("  Generating 3d graph from current scene.", EditorStyles.helpBox);
		GUILayout.Space(spaceSize);

		GUILayout.BeginHorizontal();
		GUILayout.Space(leftSpace);
		GUILayout.Label("world box", EditorStyles.label, GUILayout.Width(titleLen));
		worldBoxObj = EditorGUILayout.ObjectField(worldBoxObj, typeof(GameObject), true, GUILayout.Width(textLen+20)) as GameObject;
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
		GUILayout.Label("Role height(m)", EditorStyles.label, GUILayout.Width(titleLen));
		str_roleHeight = GUILayout.TextField(str_roleHeight, GUILayout.Width(textLen));
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
			agentRadius = float.Parse(str_roleRadius);
			float slope = int.Parse(str_slope);
			if (slope < 20 || slope > 80)
				UnityEngine.Debug.LogError("Bad slope! XD");
			tan_slope = Mathf.Tan(slope / 180.0f * Mathf.PI);

			// build
			builder.Stetup(worldBoxObj, gridSize, agentHeight, agentRadius, tan_slope);
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
			go.AddComponent<NavGraph3DGizmo>();
		}
		try
		{
			NavGraph3DGizmo gizmo = go.GetComponent<NavGraph3DGizmo>();
			gizmo.cfg = builder.cfg;
			gizmo.cells = builder.rawCells;
			gizmo.spaces = builder.subSpaces;
			gizmo.graphMap = builder.graphMap;
			gizmo.finalCells = builder.finalCells;
		}
		catch(System.Exception e)
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
		var existingAsset = AssetDatabase.LoadAssetAtPath<NavGraph3DData>(saveFilePath);
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