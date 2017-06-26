using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using TwGame;
namespace TwGame.NavMesh
{

	public class NavMeshEditor : EditorWindow
	{
		NavMeshBuilder builder = new NavMeshBuilder();
		NavMeshData navMeshData;

		string saveFilePath;

		string str_width = "20";
		string str_height = "20";
		string str_ag_radius = "0.5";
		string str_ag_height = "2";
		string str_step_height = "0.5";
		string str_max_slope = "45";
		string str_min_area = "2";


		void OnEnable()
		{
			string sceneName = SceneManager.GetActiveScene().name;
			string scenePath = "";
			var paths = AssetDatabase.GetAllAssetPaths();
			foreach (var v in paths)
				if (Path.GetFileName(v) == sceneName + ".unity")
				{
					scenePath = v;
					break;
				}
			saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_nav.asset";

			var go = GameObject.Find(drawObjectName);
			if (go == null)
			{
				go = new GameObject(drawObjectName);
				go.AddComponent<NavMeshGizmo>();
				go.transform.position = Vector3.zero;
				go.transform.localScale = Vector3.one;
			}
		}


		void OnGUI()
		{
			float spaceSize = 3f;

			GUILayout.Label("This tool generates NavMesh Data from current scene.", EditorStyles.largeLabel);
			GUILayout.Label("Before generating, assign layer 'Terrain' to walkable objects, 'Obstacle' to not walkable objects.", EditorStyles.boldLabel);
			GUILayout.Label("After generating, apply the file to the scene.", EditorStyles.boldLabel);
			GUILayout.Space(spaceSize);

			GUILayout.Label("scene width：", EditorStyles.boldLabel);
			str_width = GUILayout.TextField(str_width);
			GUILayout.Space(spaceSize);

			GUILayout.Label("scene height：", EditorStyles.boldLabel);
			str_height = GUILayout.TextField(str_height);
			GUILayout.Space(spaceSize);

			GUILayout.Label("agent radius：", EditorStyles.boldLabel);
			str_ag_radius = GUILayout.TextField(str_ag_radius);
			GUILayout.Space(spaceSize);

			GUILayout.Label("agent height：", EditorStyles.boldLabel);
			str_ag_height = GUILayout.TextField(str_ag_height);
			GUILayout.Space(spaceSize);

			GUILayout.Label("step height：", EditorStyles.boldLabel);
			str_step_height = GUILayout.TextField(str_step_height);
			GUILayout.Space(spaceSize);

			GUILayout.Label("max slope：", EditorStyles.boldLabel);
			str_max_slope = GUILayout.TextField(str_max_slope);
			GUILayout.Space(spaceSize);

			GUILayout.Label("min area：", EditorStyles.boldLabel);
			str_min_area = GUILayout.TextField(str_min_area);
			GUILayout.Space(spaceSize);

			GUILayout.Label("Output file：", EditorStyles.boldLabel);
			saveFilePath = GUILayout.TextField(saveFilePath);
			GUILayout.Space(spaceSize);

			if (GUILayout.Button("Bake"))
				Bake();
			GUILayout.Space(spaceSize);

			if (GUILayout.Button("Clear"))
				Clear();
			GUILayout.Space(spaceSize);

			if (GUILayout.Button("Save"))
				Save();
			GUILayout.Space(spaceSize);
		}


		void OnDestroy()
		{
			ClearDraw();

			UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
		}


		void Bake()
		{
			var go = GameObject.Find(drawObjectName);

			navMeshData = ScriptableObject.CreateInstance<NavMeshData>();

			NavMeshBuilder.Settings setting = new NavMeshBuilder.Settings();
			setting.sceneWidth = float.Parse(str_width);
			setting.sceneHeight = float.Parse(str_height);
			setting.sceneMinx = go.transform.position.x - setting.sceneWidth * 0.5f;
			setting.sceneMinz = go.transform.position.z - setting.sceneHeight * 0.5f;
			setting.agentRadius = float.Parse(str_ag_radius);
			setting.agentHeight = float.Parse(str_ag_height);
			setting.stepHeight = float.Parse(str_step_height);
			setting.maxSlope = float.Parse(str_max_slope);
			setting.minArea = float.Parse(str_min_area);

			builder.Build(setting, ref navMeshData);

			RefreshGizmo();

			Selection.SetActiveObjectWithContext(GameObject.Find(drawObjectName), null);
		}

		void Clear()
		{
			navMeshData.Clear();

			RefreshGizmo();
		}

		void Save()
		{
			var existingAsset = AssetDatabase.LoadAssetAtPath<NavMeshData>(saveFilePath);
			if (existingAsset == null)
			{
				AssetDatabase.CreateAsset(navMeshData, saveFilePath);
				AssetDatabase.Refresh();
				existingAsset = navMeshData;
			}
			else
			{
				EditorUtility.CopySerialized(navMeshData, existingAsset);
			}

			EditorUtility.SetDirty(navMeshData);
		}

		void ClearDraw()
		{
			var go = GameObject.Find(drawObjectName);
			if (go != null)
				GameObject.DestroyImmediate(go);
		}

		readonly string drawObjectName = "_navMeshData_gizmo_auto_delete";
		void RefreshGizmo()
		{
			var go = GameObject.Find(drawObjectName);
			if (go == null)
			{
				go = new GameObject(drawObjectName);
				go.AddComponent<NavMeshGizmo>();
				go.transform.position = Vector3.zero;
				go.transform.localScale = Vector3.one;
			}
			var gizmoLine = go.GetComponent<NavMeshGizmo>();

			gizmoLine.navMeshData = navMeshData;
		}

	}

}