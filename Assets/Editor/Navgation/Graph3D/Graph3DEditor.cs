﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using TwGame;
using TwGame.AStar;
using TwGame.Graph;
using TwGame.AStar.NavGraph;


public class Graph3DEditor : EditorWindow
{
	Graph3DBuilder builder = new Graph3DBuilder();

	float gridSize;
	float agentHeight = 2;

	float tan_slope;

	string str_gridSize = "1";
	string str_roleHeight = "2";
	string str_slope = "45";

	string saveFilePath = "Assets/{0}_navgraph.asset";
	readonly string drawObjectName = "_NavGraphData_Gizmo";



	void OnEnable()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		string scenePath = "";
		var paths = AssetDatabase.GetAllAssetPaths();
		foreach (var v in paths)
		{
			if (Path.GetFileName(v) == sceneName + ".unity")
			{
				scenePath = v;
				break;
			}
		}
		saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_navgraph.asset";
	}


	void OnGUI()
	{
		float spaceSize = 3f;

		GUILayout.Label("This tool generates Navigation Data from current scene.", EditorStyles.largeLabel);
		GUILayout.Space(spaceSize);

		GUILayout.Label("Grid size(0.1m-1m)：", EditorStyles.boldLabel);
		str_gridSize = GUILayout.TextField(str_gridSize);
		GUILayout.Space(spaceSize);

		GUILayout.Label("Role height(m)：", EditorStyles.boldLabel);
		str_roleHeight = GUILayout.TextField(str_roleHeight);
		GUILayout.Space(spaceSize);

		GUILayout.Label("Max slope(0°-80°)：", EditorStyles.boldLabel);
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
			// parse settings
			gridSize = float.Parse(str_gridSize);
			agentHeight = float.Parse(str_roleHeight);
			float slope = int.Parse(str_slope);
			if (slope < 0 || slope > 80)
				UnityEngine.Debug.LogError("Bad slope! XD");
			tan_slope = Mathf.Tan(slope / 180.0f * Mathf.PI);

			// build
			var go = GameObject.Find("terrain/box");
			builder.Stetup(go, gridSize, agentHeight, tan_slope);
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
			gizmo.cells = builder.cells;
			gizmo.navData = builder.navData;
			gizmo.spaces = builder.subSpaces;
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