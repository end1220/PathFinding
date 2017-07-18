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


public class NavgationMenuItems
{

	[MenuItem(@"Tools/Navgation/2D Grid")]
	public static void ShowWindow2DGrid()
	{
		Rect wr = new Rect(100, 100, 400, 300);
		var window = (Grid2DNavEditor)EditorWindow.GetWindowWithRect(typeof(Grid2DNavEditor), wr, true, "Grid2D Nav Editor");
		window.Show();
	}


	[MenuItem(@"Tools/Navgation/3D Grid")]
	public static void ShowWindow3DGrid()
	{
		Rect wr = new Rect(100, 100, 400, 300);
		var window = (Graph3DNavEditor)EditorWindow.GetWindowWithRect(typeof(Graph3DNavEditor), wr, true, "Graph3D Nav Editor");
		window.Show();
	}


	[MenuItem(@"Tools/Navgation/Nav Mesh")]
	public static void ShowWindowNavMeshEditor()
	{
		Rect wr = new Rect(100, 100, 400, 300);
		var window = (NavMeshEditor)EditorWindow.GetWindowWithRect(typeof(NavMeshEditor), wr, true, "Nav Mesh Editor");
		window.Show();
	}

}