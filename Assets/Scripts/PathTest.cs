﻿

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using PathFinding;


namespace FixedPoint
{
	public enum PathMode
	{
		Grid,
		Graph,
		NavMesh
	}


	public class PathTest : MonoBehaviour
	{
		public static PathTest Instance;

		// navigation asset
		public NavGrid2DData navGrid;
		public NavGraph3DData navGraph;
		public NavMeshData navMesh;

		// path finding
		public PathMode mode;
		private GridAStarMap gridMap;
		private GridPathPlanner gridPathFinder = new GridPathPlanner();

		private Graph3DAStarMap graphMap;
		private Graph3DPathPlanner graphPathFinder = new Graph3DPathPlanner();

		private NavMeshMap navmeshMap;
		private NavMeshMapPathPlanner navPathPlannner = new NavMeshMapPathPlanner();


		public DebugLine line;

		public int start = 0;
		public int end = 10;

		void Awake()
		{
			Instance = this;

			if (mode == PathMode.Graph)
			{
				graphMap = new Graph3DAStarMap();
				graphMap.Init(navGraph);
				graphPathFinder.Setup(graphMap);
			}
			else if (mode == PathMode.Grid)
			{
				gridMap = new GridAStarMap();
				gridMap.InitMap(navGrid.Width, navGrid.Height, navGrid);
				gridPathFinder.Setup(gridMap);
			}
			else if (mode == PathMode.NavMesh)
			{
				navmeshMap = new NavMeshMap();
				navmeshMap.InitMap(navMesh);
				navPathPlannner.Setup(navmeshMap);
			}

			SetNavDebugDraw();
		}


		List<Vector3> result = new List<Vector3>();
		void OnGUI()
		{
			if (GUI.Button(new Rect(10, 10, 40, 20), "ast"))
			{
				DoIt();
			}
		}

		/*float lastTime = 0;
		void Update()
		{
			if (Time.timeSinceLevelLoad - lastTime > 0.1f)
			{
				lastTime = Time.timeSinceLevelLoad;
				DoIt();
			}
		}*/

		System.Random random = new System.Random();
		void DoIt()
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();

			if (mode == PathMode.Graph)
			{
				int start = random.Next(0, 1600);
				int end = random.Next(0, 1600);
				graphFindPath(start, end, ref result);
			}
			else if (mode == PathMode.Grid)
			{
				int startx = random.Next(2, 38);
				int starty = random.Next(2, 38);
				int endx = random.Next(2, 38);
				int endy = random.Next(2, 38);
				gridFindPath(startx, starty, endx, endy, ref result);
			}
			else if (mode == PathMode.NavMesh)
			{
				navFindPath(start,end, ref result);
			}

			if (result.Count > 0)
			{
				line.ClearLines();
				line.AddLine(result.ToArray(), Color.red);
			}

			watch.Stop();
			if (result.Count > 0)
				UnityEngine.Debug.Log("time " + watch.ElapsedMilliseconds);
		}


		private bool graphFindPath(int startId, int endId, ref List<Vector3> result)
		{
			var path = graphPathFinder.FindPath3D(startId, endId);

			result.Clear();
			for (int i = 0; i < path.Count; ++i)
			{
				var node = graphMap.GetNodeAt(path[i].x, path[i].y, path[i].z);
				result.Add(node.worldPosition.ToVector3());
			}

			return result.Count > 0;
		}


		private bool gridFindPath(int startX, int startY, int endX, int endY, ref List<Vector3> result)
		{
			var path = gridPathFinder.FindPath(startX, startY, endX, endY);

			result.Clear();
			for (int i = 0; i < path.Count; ++i)
			{
				float x = navGrid.MinX + path[i].x * navGrid.GridSize;
				float y = navGrid.MinZ + path[i].y * navGrid.GridSize;
				Vector3 pos = new Vector3(x, 1, y);
				result.Add(pos);
			}

			return result.Count > 0;
		}

		private bool navFindPath(int start, int end, ref List<Vector3> result)
		{
			var path = navPathPlannner.FindPath2(start, end);

			result.Clear();
			for (int i = 0; i < path.Count; ++i)
			{
				Vector3 pos = (Vector3)path[i];
				result.Add(pos);
			}

			return result.Count > 0;
		}


		private void SetNavDebugDraw()
		{
			if (mode == PathMode.Graph)
			{
				NavGraph3DGizmo gizmo = gameObject.GetComponent<NavGraph3DGizmo>();
				if (gizmo == null)
					gizmo = gameObject.AddComponent<NavGraph3DGizmo>();

				gizmo.cfg = navGraph.buildConfig;
				gizmo.graphMap = graphMap;
			}
			else if (mode == PathMode.Grid)
			{
				NavGrid2DGizmo gizmoLine = gameObject.GetComponent<NavGrid2DGizmo>();
				if (gizmoLine == null)
					gizmoLine = gameObject.AddComponent<NavGrid2DGizmo>();

				Vector3[,] pos = new Vector3[navGrid.Width, navGrid.Height];
				for (int x = 0; x < navGrid.Width; ++x)
				{
					for (int z = 0; z < navGrid.Height; ++z)
					{
						float fposx = navGrid.MinX + navGrid.GridSize * x;
						float fposz = navGrid.MinZ + navGrid.GridSize * z;
						pos[x, z] = new Vector3(fposx, 1, fposz);
					}
				}

				gizmoLine.SetGridPosList(navGrid, pos, navGrid.Width, navGrid.Height);
			}
			else if (mode == PathMode.NavMesh)
			{
				NavMeshGizmo gizmo = gameObject.GetComponent<NavMeshGizmo>();
				if (gizmo == null)
					gizmo = gameObject.AddComponent<NavMeshGizmo>();
				gizmo.navData = navMesh;

			}
		}
		

	}

}