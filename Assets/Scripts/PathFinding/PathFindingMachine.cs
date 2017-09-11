
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PathFinding
{

	public class PathFindingMachine : MonoBehaviour
	{
		public static PathFindingMachine Instance { get; private set; }

		public PathMode pathMode = PathMode.NavMesh;

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
						navgationGraph = new Grid2DGraph();
						pathPlanner = new Grid2DPathPlanner();
						break;
					case PathMode.Grid3D:
						navgationGraph = new Grid3DGraph();
						pathPlanner = new Grid3DPathPlanner();
						break;
					case PathMode.NavMesh:
						navgationGraph = new NavMeshGraph();
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
			if (navgationData != null && IsNavDataInvalid())
				navgationData.OnDrawGizmosSelected(transform);
			//if (navgationGraph is NavMeshGraph)
			//	(navgationGraph as NavMeshGraph).bbTree.OnDrawGizmos();
		}


		public bool IsNavDataInvalid()
		{
			switch (pathMode)
			{
				case PathMode.Grid2D:
					return navgationData is Grid2DNavData;
				case PathMode.Grid3D:
					return navgationData is Grid3DNavData;
				case PathMode.NavMesh:
					return navgationData is NavMeshData;
			}
			return false;
		}


		public bool FindPath(Int3 from, Int3 to, ref List<Int3> result)
		{
			bool ret = pathPlanner.FindPath(from, to, ref result);
			return ret;
		}


		public bool IsWalkable(Int3 position)
		{
			return navgationGraph.IsWalkable(position);
		}


		public Int3 GetNearestPosition(Int3 position)
		{
			return navgationGraph.GetNearestPosition(position);
		}


		public bool LineCastForMoving(ref HitInfo hit, MoveType mov)
		{
			return navgationGraph.LineCastForMoving(ref hit, mov);
		}


		/// <summary>
		/// 走路遇到障碍，让角色侧身滑过，而不是原地不动
		/// </summary>
		public Int3 SlideByObstacles(Int3 from, Int3 to, Int3 hit)
		{
			return navgationGraph.SlideByObstacles(from, to, hit);
		}


	}

}