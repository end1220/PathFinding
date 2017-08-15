
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PathFinding
{

	public class PathFindingMachine : MonoBehaviour
	{
		public static PathFindingMachine Instance { get; private set; }

		public PathMode pathMode = PathMode.Grid2D;

		public bool EnableMultiThread;

		public INavData navgationData;

		public INavGraph navgationGraph;

		private IPathPlanner pathPlanner;


		void Awake()
		{
			Instance = this;
		}


		void Start()
		{
			try
			{
				if (navgationData == null)
				{
					Debug.LogError("Navigation data is null !");
					return;
				}
				switch (pathMode)
				{
					case PathMode.Grid2D:
						navgationGraph = new Grid2DMap();
						pathPlanner = new Grid2DPathPlanner();
						break;
					case PathMode.Grid3D:
						navgationGraph = new Grid3DGraph();
						pathPlanner = new Grid3DPathPlanner();
						break;
					case PathMode.NavMesh:
						navgationGraph = new NavMeshMap();
						pathPlanner = new NavMeshPathPlanner();
						break;
				}
				navgationGraph.Init(navgationData);
				pathPlanner.SetGraph(navgationGraph);
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
			}
		}


		private void OnDrawGizmosSelected()
		{
			if (navgationData != null)
				navgationData.OnDrawGizmosSelected(transform);
		}


		public bool FindPath(FixVector3 from, FixVector3 to, ref List<FixVector3> result)
		{
			bool ret = pathPlanner.FindPath(from, to, ref result);
			return ret;
		}

		public bool IsPassable(FixVector3 position)
		{
			return navgationGraph.IsPassable(position);
		}

		public bool IsMissileCross(FixVector3 position, int CrossType)
		{
			return (navgationGraph as Grid2DMap).IsMissileCross(position, CrossType);
		}

		public FixVector3 GetNearestForce(FixVector3 position, int step)
		{
			return pathMode == PathMode.Grid3D ? position : (navgationGraph as Grid2DMap).GetNearestForce(position, step);
		}

		public Int3 Vector3ToPoint3D(Vector3 position)
		{
			var navGrid = navgationData as Grid2DNavData;
			int x = (FixMath.m2mm(position.x) - navGrid.MinX) / navGrid.GridSize;
			int z = (FixMath.m2mm(position.z) - navGrid.MinZ) / navGrid.GridSize;

			return new Int3(x, z);
		}

		public int GetGroundHeight3D(FixVector3 position)
		{
			return (navgationGraph as Grid3DGraph).GetGroundHeight3D(position);
		}


		public FixVector3 RayCastForMoving(FixVector3 from, FixVector3 to, MoveType mov)
		{
			return navgationGraph.RayCastForMoving(from, to, mov);
		}


		/// <summary>
		/// 走路遇到障碍，让角色侧身滑过，而不是原地不动
		/// </summary>
		public FixVector3 SlideByObstacles(FixVector3 fromPos, FixVector3 oldTargetPos)
		{
			return navgationGraph.SlideByObstacles(fromPos, oldTargetPos);
		}


	}

}