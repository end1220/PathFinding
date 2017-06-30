using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
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


public class NavMeshEditor : EditorWindow
{
	string saveFilePath = "";
	string scenePath = "";
	readonly string drawObjectName = "_NavGraphData_Gizmo";



	void OnEnable()
	{
		scenePath = EditorUtils.GetCurrentScenePath();
		saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_navgraph.asset";
	}


	void OnGUI()
	{
		float spaceSize = 3f;

		if (GUILayout.Button("Generate"))
			Generate();
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


	void Generate()
	{
		try
		{
			Convert();
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError(e.ToString());
			EditorUtility.ClearProgressBar();
		}
	}


	private void Convert()
	{
		string navmeshPath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "NavMesh.asset";
		NavMeshTriangulation navTrian = NavMesh.CalculateTriangulation();
		Mesh mesh = new Mesh();
		mesh.name = "ExportedNavMesh";
		mesh.vertices = navTrian.vertices;
		mesh.triangles = navTrian.indices;

		var go = new GameObject("666test");
		var meshFilter = go.AddComponent<MeshFilter>();
		var render = go.AddComponent<MeshRenderer>();
		meshFilter.mesh = mesh;

		/*List<Renderer> disabledObjects = new List<Renderer>();
		foreach (Renderer item in Object.FindObjectsOfType(typeof(Renderer)))
		{
			if (GameObjectUtility.AreStaticEditorFlagsSet(item.gameObject, StaticEditorFlags.NavigationStatic)
				&& !item.enabled)
			{
				disabledObjects.Add(item);
				item.enabled = true;
			}
		}

		//NavMeshBuilder.BuildNavMesh();

		disabledObjects.ForEach(obj => obj.enabled = false);

		UnityEngine.Debug.Log(string.Format("Done building navmesh, {0} objects affected.", disabledObjects.Count));*/
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
		
	}


}