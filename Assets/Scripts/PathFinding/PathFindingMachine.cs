
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PathFinding
{

	public class PathFindingMachine : MonoBehaviour
	{
		public static PathFindingMachine Instance { get; private set; }

		public NavGrid2DData navGrid;
		public NavGraph3DData navGraph;
		public NavMeshData navMesh;
		public PathMode pathMode = PathMode.Grid2D;

		private Grid2DMap gridMap;
		private Grid2DPathPlanner gridPathFinder = new Grid2DPathPlanner();
		private Graph3DMap graphMap;
		private Graph3DPathPlanner graphPathFinder = new Graph3DPathPlanner();
		private NavMeshMap navMeshMap;
		private NavMeshPathPlanner navPathFinder = new NavMeshPathPlanner();


		void Awake()
		{
			Instance = this;
		}


		void Start()
		{
			try
			{
				if (navGrid == null && navGraph == null && navMesh == null)
				{
					Log.Error("Navigation data is null !");
					return;
				}
				if (pathMode == PathMode.Grid2D)
				{
					gridMap = new Grid2DMap();
					gridMap.InitMap(navGrid.Width, navGrid.Height, navGrid);
					gridPathFinder.Setup(gridMap);
				}
				else if (pathMode == PathMode.Graph3D)
				{
					graphMap = new Graph3DMap();
					graphMap.Init(navGraph);
					graphPathFinder.Setup(graphMap);
				}
				else if (pathMode == PathMode.NavMesh)
				{
					navMeshMap = new NavMeshMap();
					navMeshMap.InitMap(navMesh);
					navPathFinder.Setup(navMeshMap);
				}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}


		private void OnDrawGizmosSelected()
		{
			if (pathMode == PathMode.Grid2D && navGrid != null)
				navGrid.OnDrawGizmosSelected(transform);
			else if (pathMode == PathMode.Graph3D && navGraph != null)
				navGraph.OnDrawGizmosSelected(transform);
			else if (pathMode == PathMode.NavMesh && navMesh != null)
			{
				//if (navMeshMap != null)
				//	navMeshMap.bbTree.OnDrawGizmos();
				navMesh.OnDrawGizmosSelected(transform);
			}
		}


		public bool FindPath(FixVector3 from, FixVector3 to, ref List<FixVector3> result)
		{
			switch (pathMode)
			{
				case PathMode.Grid2D:
					return gridPathFinder.FindPath(from, to, ref result);
				case PathMode.Graph3D:
					return graphPathFinder.FindPath3D(from, to, ref result);
				case PathMode.NavMesh:
					return navPathFinder.FindPath(from, to, ref result);
			}
			return false;
		}

		public List<Int3> FindPath(int start, int end)
		{
			return navPathFinder.FindPath(start, end);
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