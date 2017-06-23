
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


		void Awake()
		{
			Instance = this;
			OnSceneLoaded(0);
		}


		void OnGUI()
		{
			/*if (GUI.Button(new Rect(10, 10, 40, 20), "ast"))
			{
				TwVector3 from
				List<TwVector3> result = new List<TwVector3>();
				if (FindPath(TwVector3 from, TwVector3 to, ref result))
				{

				}
			}*/
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

		public bool FindPath(TwVector3 from, TwVector3 to, ref List<TwVector3> result)
		{
			if (graphMode)
			{
				return _graphFindPath(from, to, ref result);
			}
			else
			{
				return _gridFindPath(from, to, ref result);
			}
		}


		private bool _graphFindPath(TwVector3 from, TwVector3 to, ref List<TwVector3> result)
		{
			// 坐标转化
			var start = TwVector3ToPoint3D(from);
			var end = TwVector3ToPoint3D(to);
			var startNode = graphMap.GetNodeAt(start.x, start.y, start.z);
			var endNode = graphMap.GetNodeAt(end.x, end.y, end.z);
			if (startNode == null || endNode == null)
			{
				return false;
			}
			int startId = startNode.id;
			int endId = endNode.id;
			//startId = 91;
			//endId = 8;
			var path = graphPathFinder.FindPath(startId, endId);

			// convert to TwVector3
			result.Clear();
			for (int i = 0; i < path.Count; ++i)
			{
				var node = graphMap.GetNodeAt(path[i].x, path[i].y, path[i].z);
				result.Add(new TwVector3(node.worldPosition.x, node.worldPosition.y, node.worldPosition.z));
			}
			//  modify first and last point.
			if (result.Count > 0)
			{
				result[0] = from;
				result[result.Count - 1] = to;
			}

			return result.Count > 0;
		}


		private bool _gridFindPath(TwVector3 from, TwVector3 to, ref List<TwVector3> result)
		{
			// 坐标转化
			Point2D start = TwVector3ToPoint2D(from);
			Point2D end = TwVector3ToPoint2D(to);

			var path = gridPathFinder.FindPath(start.x, start.y, end.x, end.y);

			PathOptimizer.Optimize(ref path);

			// convert to TwVector3
			result.Clear();
			for (int i = 0; i < path.Count; ++i)
			{
				result.Add(Point2DToTwVector3(path[i]));
			}
			//  modify first and last point.
			if (result.Count > 0)
			{
				result[0] = from;
				result[result.Count - 1] = to;
			}

			return result.Count > 0;
		}


		public bool IsPassable(TwVector3 position)
		{
			if (graphMode)
			{
				Point3D pt3d = TwVector3ToPoint3D(position);
				bool ret = graphMap.IsPointPassable(pt3d);
				return ret;
			}
			else
			{
				Point2D pt2d = TwVector3ToPoint2D(position);
				bool ret = gridMap.IsNodePassable(pt2d.x, pt2d.y);
				return ret;
			}
		}


		public bool IsPassable(int x, int y)
		{
			if (graphMode)
			{
				return true;
			}
			else
			{
				bool ret = gridMap.IsNodePassable(x, y);
				return ret;
			}
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

		public Point2D TwVector3ToPoint2D(TwVector3 position)
		{
			double x = ((double)position.x / TwMath.ratio_mm - navGrid.MinX) / navGrid.GridSize + 0.5f;
			double z = ((double)position.z / (double)TwMath.ratio_mm - navGrid.MinZ) / navGrid.GridSize + 0.5f;
			return new Point2D((int)x, (int)z);
		}


		public Point3D TwVector3ToPoint3D(TwVector3 position)
		{
			int cellSize = navGraph.buildConfig.cellSize;
			int dx = (int)position.x - navGraph.buildConfig.worldMinPos.x;
			int dy = (int)position.y - navGraph.buildConfig.worldMinPos.y;
			int dz = (int)position.z - navGraph.buildConfig.worldMinPos.z;
			int x = dx / cellSize/* + ((dx % cellSize) > 0 ? 1 : 0)*/;
			int y = dy / cellSize/* + ((dy % cellSize) > 0 ? 1 : 0)*/;
			int z = dz / cellSize/* + ((dz % cellSize) > 0 ? 1 : 0)*/;
			return new Point3D(x, y, z);
		}


		public GraphAStar3DNode Point3DToGraphNode(Point3D point)
		{
			var node = graphMap.GetNodeAt(point.x, point.y, point.z);
			return node;
		}


		public bool IsPointInWorldBox(Point3D point)
		{
			return graphMap.IsIndexValid(point.x, point.y, point.z);
		}

		
		public GraphAStar3DNode TwVector3ToGraphNode(TwVector3 position)
		{
			var point = TwVector3ToPoint3D(position);
			var node = graphMap.GetNodeAt(point.x, point.y, point.z);
			return node;
		}


		public Point2D Vector3ToPoint2D(Vector3 position)
		{
			float x = (position.x - navGrid.MinX) / navGrid.GridSize + 0.5f;
			float z = (position.z - navGrid.MinZ) / navGrid.GridSize + 0.5f;

			return new Point2D((int)x, (int)z);
		}


		public TwVector3 Point2DToTwVector3(Point2D point)
		{
			float fposx = navGrid.MinX + navGrid.GridSize * point.x;
			float fposz = navGrid.MinZ + navGrid.GridSize * point.y;
			var vec = new TwVector3((long)(fposx * TwMath.ratio_mm), 0, (long)(fposz * TwMath.ratio_mm));
			return vec;
		}

		public TwVector3 Point3DToTwVector3(Point3D point)
		{
			int cellSize = navGraph.buildConfig.cellSize;
			int cellRadius = cellSize / 2;
			var minPos = navGraph.buildConfig.worldMinPos;
			int x = minPos.x + cellSize * point.x + cellRadius;
			int y = minPos.y + cellSize * point.y + cellRadius;
			int z = minPos.z + cellSize * point.z + cellRadius;
			var vec = new TwVector3(x, y, z);
			return vec;
		}


		public long GetGroundHeight3D(TwVector3 position)
		{
			int stepHeight = navGraph.buildConfig.cellSize;
			float posY = TwMath.mm2m(position.y);
			TwVector3 upperPosition = new TwVector3(position.x, position.y + stepHeight, position.z);
			Ray ray = new Ray(upperPosition.ToVector3(), Vector3.down);
			RaycastHit hit;
			float distance = TwMath.mm2m(stepHeight) * 2;
			int layerMask = 1 << LayerMask.NameToLayer(AppConst.LayerTerrain) | 1 << LayerMask.NameToLayer(AppConst.LayerLink);
			if (Physics.Raycast(ray, out hit, distance, layerMask))
			{
				posY = hit.point.y;
			}
			return TwMath.m2mm(posY);
		}


		public TwVector3 CheckStairs(TwVector3 currentPosition, TwVector3 targetPosition)
		{
			GraphAStar3DNode currentNode = null;
			var p3dCur = TwVector3ToPoint3D(currentPosition);
			for (int iy = 1; iy >= -1; iy--)
			{
				var node = graphMap.GetNodeAt(p3dCur.x, p3dCur.y + iy, p3dCur.z);
				if (node != null)
				{
					currentNode = node;
					break;
				}
			}

			//var currentNode = TwVector3ToGraphNode(currentPosition);
			
			var pointTarget = TwVector3ToPoint3D(targetPosition);

			this.currentNode = currentNode;

			var targetNode = graphMap.GetPassableNeighborNodeAt(currentNode, pointTarget.x, pointTarget.z);
			if (targetNode != null)
			{
				this.targetNode = targetNode;

				long y = GetGroundHeight3D(targetPosition);
				var pos = new TwVector3(targetPosition.x, y, targetPosition.z);
				return pos;
			}

			return currentPosition;
		}


		public GraphAStar3DNode currentNode = null;
		public GraphAStar3DNode targetNode = null;
		public Point3D currentPoint3D;

		public TwVector3 LineRayCast(TwVector3 from, TwVector3 to)
		{
			if (graphMode)
			{
				currentPoint3D = TwVector3ToPoint3D(from);
				var pos = CheckStairs(from, to);
				return pos;
			}
			else
			{
				TwVector3 ret = LineRayCast2(from, to);
				return ret;
			}
		}

		// 射线碰撞，计算起点到终点间的最远可到达点
		public TwVector3 LineRayCast2(TwVector3 from, TwVector3 to)
		{
			TwVector3 blockPoint = from;
			int stepLen = Math.Min(300, (int)(BattleMap.Instance.navGrid.GridSize * TwMath.ratio_mm));
			bool blocked = false;
			
			// y = a*x + b
			Fix64 a = (Fix64)0;
			long dx = to.x - from.x;
			long dz = to.z - from.z;
			if (Math.Abs(dx) > Math.Abs(dz))
			{
				a = (Fix64)dz / (Fix64)dx;
				int step = to.x - from.x > 0 ? stepLen : -stepLen;
				for (long x = from.x + step; step > 0 ? x < to.x + step : x > to.x - step; x += step)
				{
					x = step > 0 ? System.Math.Min(x, to.x) : System.Math.Max(x, to.x);
					Fix64 z = (Fix64)from.z + a * (Fix64)(x - from.x);

					if (!IsPassable(new TwVector3(x, 0, (long)z)))
					{
						blocked = true;
						break;
					}
					
					blockPoint.Set(x, from.y, (long)z);
				}
			}
			else
			{
				a = (Fix64)dx / (Fix64)dz;
				int step = to.z - from.z > 0 ? stepLen : -stepLen;
				for (long z = from.z + step; step > 0 ? z < to.z + step : z > to.z - step; z += step)
				{
					z = step > 0 ? System.Math.Min(z, to.z) : System.Math.Max(z, to.z);
					Fix64 x = (Fix64)from.x + a * (Fix64)(z - from.z);
					if (!IsPassable(new TwVector3((long)x, 0, z)))
					{
						blocked = true;
						break;
					}

					blockPoint.Set((long)x, from.y, z);
				}
			}

			if (blockPoint != from || blocked)
				return blockPoint;
			else
				return to;
		}


	}

}