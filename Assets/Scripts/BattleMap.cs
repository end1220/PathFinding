
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Lite;
using Lite.AStar;


namespace TwGame
{

	public class BattleMap : MonoBehaviour
	{
		public static BattleMap Instance;

		// navigation asset
		public NavigationData navGrid;

		public NavGraphData navGraph;

		// path finding
		public bool graphMode;
		private GridAStarMap gridMap;
		private GridPathPlanner gridPathFinder = new GridPathPlanner();

		private GraphAStar3DMap graphMap;
		private GraphPath3DPlanner graphPathFinder = new GraphPath3DPlanner();

		public DebugLine line;

		void Awake()
		{
			Instance = this;
			OnSceneLoaded(0);
		}

		List<Vector3> result = new List<Vector3>();
		void OnGUI()
		{
			if (GUI.Button(new Rect(10, 10, 40, 20), "ast"))
			{
				DoIt();
			}
		}

		float lastTime = 0;
		void Update()
		{
			if (Time.timeSinceLevelLoad - lastTime > 1)
			{
				lastTime = Time.timeSinceLevelLoad;
				DoIt();
			}
		}

		System.Random random = new System.Random();
		void DoIt()
		{
			int count = 40;
			int begin = 40;
			int delta = 600;

			int start = begin + random.Next(0, count) * delta + random.Next(0, count-1);
			int end = begin + random.Next(0, count) * delta + random.Next(0, count-1);
			graphFindPath(start, end, ref result);
			if (result.Count > 0)
			{
				line.ClearLines();
				line.AddLine(result.ToArray(), Color.red);
			}
		}


		public void OnSceneLoaded(int id)
		{
			try
			{
				// nav data
				if (navGrid == null && navGraph == null)
				{
					Log.Error("Navigation data is null !");
				}
				else
				{
					if (graphMode)
					{
						graphMap = new GraphAStar3DMap();
						graphMap.Init(navGraph);
						graphPathFinder.Setup(graphMap);
					}
					else
					{
						gridMap = new GridAStarMap();
						gridMap.InitMap(navGrid.Width, navGrid.Height, navGrid);
						gridPathFinder.Setup(gridMap);
					}

#if UNITY_EDITOR
					SetNavDebugDraw();
#endif
				}
			}
			catch(Exception e)
			{
				Log.Error(e.ToString());
			}
		}


		#region ---------- 寻路 ------------

		private bool graphFindPath(int startId, int endId, ref List<Vector3> result)
		{
			var path = graphPathFinder.FindPath(startId, endId);

			result.Clear();
			for (int i = 0; i < path.Count; ++i)
			{
				var node = graphMap.GetNodeAt(path[i].x, path[i].y, path[i].z);
				result.Add(node.worldPosition.ToVector3());
			}

			return result.Count > 0;
		}

		


		private void SetNavDebugDraw()
		{
			if (graphMode)
			{
				NavGraphGizmo gizmo = gameObject.GetComponent<NavGraphGizmo>();
				if (gizmo == null)
					gizmo = gameObject.AddComponent<NavGraphGizmo>();

				gizmo.cfg = navGraph.buildConfig;
				gizmo.navData = navGraph;
				gizmo.graphMap = graphMap;
			}
			else
			{
				NavigationGizmo gizmoLine = gameObject.GetComponent<NavigationGizmo>();
				if (gizmoLine == null)
					gizmoLine = gameObject.AddComponent<NavigationGizmo>();

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
		}


		#endregion

		

	}

}