
using System;
using System.Collections.Generic;
using UnityEngine;
using TwFramework;


namespace PathFinding
{

	public class PathFindingMachine : MonoBehaviour
	{
		public static PathFindingMachine Instance { get; private set; }

		public NavGrid2DData navGrid;
		public NavGraph3DData navGraph;
		//public bool enable3DNavigation;
		public PathMode pathMode = PathMode.Grid2D;

		private Grid2DMap gridMap;
		private Grid2DPathPlanner gridPathFinder = new Grid2DPathPlanner();
		private Graph3DMap graphMap;
		private Graph3DPathPlanner graphPathFinder = new Graph3DPathPlanner();


		void Awake()
		{
			Instance = this;
		}


		void Start()
		{
			try
			{
				if (navGrid == null && navGraph == null)
				{
					Log.Error("Navigation data is null !");
				}
				else
				{
					if (pathMode == PathMode.Graph3D)
					{
						graphMap = new Graph3DMap();
						graphMap.Init(navGraph);
						graphPathFinder.Setup(graphMap);
					}
					else if (pathMode == PathMode.Grid2D)
					{
						gridMap = new Grid2DMap();
						gridMap.InitMap(navGrid.Width, navGrid.Height, navGrid);
						gridPathFinder.Setup(gridMap);
					}

#if UNITY_EDITOR
					SetNavDebugDraw();
#endif
				}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}


		public bool FindPath(FixVector3 from, FixVector3 to, ref List<FixVector3> result)
		{
			return pathMode == PathMode.Graph3D ? graphPathFinder.FindPath3D(from, to, ref result) : gridPathFinder.FindPath(from, to, ref result);
		}

		public bool IsPassable(FixVector3 position)
		{
			return pathMode == PathMode.Graph3D ? graphMap.IsPassable(position) : gridMap.IsPassable(position);
		}

		public bool IsMissileCross(FixVector3 position, int CrossType)
		{
			return gridMap.IsMissileCross(position, CrossType);
		}

		public FixVector3 GetNearestForce(FixVector3 position, int step)
		{
			return pathMode == PathMode.Graph3D ? position : gridMap.GetNearestForce(position, step);
		}

		private void SetNavDebugDraw()
		{
			if (pathMode == PathMode.Graph3D)
			{
				NavGraph3DGizmo gizmo = gameObject.GetComponent<NavGraph3DGizmo>();
				if (gizmo == null)
					gizmo = gameObject.AddComponent<NavGraph3DGizmo>();

				gizmo.cfg = navGraph.buildConfig;
				gizmo.graphMap = graphMap;
			}
			else
			{
				NavGrid2DGizmo gizmoLine = gameObject.GetComponent<NavGrid2DGizmo>();
				if (gizmoLine == null)
					gizmoLine = gameObject.AddComponent<NavGrid2DGizmo>();

				Vector3[,] pos = new Vector3[navGrid.Width, navGrid.Height];
				for (int x = 0; x < navGrid.Width; ++x)
				{
					for (int z = 0; z < navGrid.Height; ++z)
					{
						float fposx = FixMath.mm2m(navGrid.MinX) + FixMath.mm2m(navGrid.GridSize) * (x + 0.5f);
						float fposz = FixMath.mm2m(navGrid.MinZ) + FixMath.mm2m(navGrid.GridSize) * (z + 0.5f);
						pos[x, z] = new Vector3(fposx, 1, fposz);
					}
				}

				gizmoLine.SetGridPosList(navGrid, pos, navGrid.Width, navGrid.Height);
			}
		}


		public Int3 Vector3ToPoint3D(Vector3 position)
		{
			int x = (FixMath.m2mm(position.x) - navGrid.MinX) / navGrid.GridSize;
			int z = (FixMath.m2mm(position.z) - navGrid.MinZ) / navGrid.GridSize;

			return new Int3(x, z);
		}

		public int GetGroundHeight3D(FixVector3 position)
		{
			return graphMap.GetGroundHeight3D(position);
		}


		public FixVector3 RayCastForMoving(FixVector3 from, FixVector3 to, MoveType mov)
		{
			return pathMode == PathMode.Graph3D ? graphMap.RayCast3DForMoving(from, to) : gridMap.RayCast2DForMoving(from, to, mov);
		}


		/// <summary>
		/// 走路遇到障碍，让角色侧身滑过，而不是原地不动
		/// </summary>
		public FixVector3 SlideByObstacles(FixVector3 fromPos, FixVector3 oldTargetPos)
		{
			return pathMode == PathMode.Graph3D ? graphMap.SlideByObstacles(fromPos, oldTargetPos) : gridMap.SlideByObstacles(fromPos, oldTargetPos);
		}




	}

}