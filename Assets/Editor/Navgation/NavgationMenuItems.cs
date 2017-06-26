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
using TwGame.NavMesh;


public class NavgationMenuItems
{

	[MenuItem(@"Tools/Navgation/2D Grid")]
	public static void ShowWindow2DGrid()
	{
		EditorWindow.GetWindow(typeof(Grid2DEditor));
	}


	[MenuItem(@"Tools/Navgation/3D Grid")]
	public static void ShowWindow3DGrid()
	{
		EditorWindow.GetWindow(typeof(Graph3DEditor));
	}


	[MenuItem(@"Tools/Navgation/Nav Mesh")]
	public static void ShowWindowNavMeshEditor()
	{
		EditorWindow.GetWindow(typeof(NavMeshEditor));
	}


}