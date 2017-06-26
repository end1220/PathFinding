
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwFramework;
using TwGame.Graph;


namespace TwGame.AStar
{
	public class Graph3DAStarMap : GraphAStarMap
	{
		private NavGraph3DData navGraphData;

		private Graph3DAStarNode[, ,] nodeMatrix;

		public void Init(NavGraph3DData navData)
		{
			navGraphData = navData;
			nodeMatrix = new Graph3DAStarNode[navData.buildConfig.cellCount.x, navData.buildConfig.cellCount.y, navData.buildConfig.cellCount.z];
			for (int i = 0; i < navData.nodeList.Count; ++i)
			{
				var node = navData.nodeList[i];
				nodeMatrix[node.x, node.y, node.z] = node;
				navData.nodeDic.Add(node.id, node);
				this.AddNode(node);
			}
			for (int i = 0; i < navData.edgeList.Count; ++i)
			{
				var edge = navData.edgeList[i];
				this.AddEdge(edge);
			}
		}


		public Graph3DAStarNode GetNodeAt(int x, int y, int z)
		{
			if (IsIndexValid(x, y, z))
				return nodeMatrix[x, y, z];
			return null;
		}


		public bool IsPointPassable(Point3D point)
		{
			if (IsIndexValid(point.x, point.y, point.z))
			{
				var grid = nodeMatrix[point.x, point.y, point.z];
				return grid != null && grid.walkable;
			}
			else
				return false;
		}

		public bool IsIndexValid(int x, int y, int z)
		{
			return (x >= 0 && x < navGraphData.buildConfig.cellCount.x
				&& y >= 0 && y < navGraphData.buildConfig.cellCount.y
				&& z >= 0 && z < navGraphData.buildConfig.cellCount.z);
		}


		public Graph3DAStarNode GetPassableNeighborNodeAt(Graph3DAStarNode node, int x, int z)
		{
			if (node == null)
				return null;

			var edgeList = GetEdgeList(node.id);
			for (int i = 0; i < edgeList.Count; ++i)
			{
				var edge = edgeList[i];
				var neighbor = GetNodeByID(edge.to) as Graph3DAStarNode;
				if (neighbor.walkable && neighbor.x == x && neighbor.z == z)
					return neighbor;
			}
			return null;
		}


		public long GetGroundHeight3D(TwVector3 position)
		{
			int stepHeight = navGraphData.buildConfig.cellSize;
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


		public Point3D TwVector3ToPoint3D(TwVector3 position)
		{
			int cellSize = navGraphData.buildConfig.cellSize;
			int dx = (int)position.x - navGraphData.buildConfig.worldMinPos.x;
			int dy = (int)position.y - navGraphData.buildConfig.worldMinPos.y;
			int dz = (int)position.z - navGraphData.buildConfig.worldMinPos.z;
			int x = dx / cellSize/* + ((dx % cellSize) > 0 ? 1 : 0)*/;
			int y = dy / cellSize/* + ((dy % cellSize) > 0 ? 1 : 0)*/;
			int z = dz / cellSize/* + ((dz % cellSize) > 0 ? 1 : 0)*/;
			return new Point3D(x, y, z);
		}


		public bool IsPassable(TwVector3 position)
		{
			Point3D pt3d = TwVector3ToPoint3D(position);
			bool ret = this.IsPointPassable(pt3d);
			return ret;
		}


		public TwVector3 CheckStairs(TwVector3 currentPosition, TwVector3 targetPosition)
		{
			Graph3DAStarNode currentNode = null;
			var p3dCur = this.TwVector3ToPoint3D(currentPosition);
			for (int iy = 1; iy >= -1; iy--)
			{
				var node = this.GetNodeAt(p3dCur.x, p3dCur.y + iy, p3dCur.z);
				if (node != null)
				{
					currentNode = node;
					break;
				}
			}

			var pointTarget = this.TwVector3ToPoint3D(targetPosition);
			var targetNode = this.GetPassableNeighborNodeAt(currentNode, pointTarget.x, pointTarget.z);
			if (targetNode != null)
			{
				long y = GetGroundHeight3D(targetPosition);
				var pos = new TwVector3(targetPosition.x, y, targetPosition.z);
				return pos;
			}

			return currentPosition;
		}


		public TwVector3 RayCast3D(TwVector3 from, TwVector3 to)
		{
			TwVector3 blockPoint = from;
			int stepLen = navGraphData.buildConfig.cellSize;
			bool blocked = false;

			// y = a*x + b
			Fix64 a_xz = (Fix64)0;
			Fix64 a_y = (Fix64)0;
			long dx = to.x - from.x;
			long dy = to.y - from.y;
			long dz = to.z - from.z;
			if (Math.Abs(dx) > Math.Abs(dz))
			{
				a_xz = (Fix64)dz / (Fix64)dx;
				a_y = (Fix64)dy / (Fix64)dx;
				int step = to.x - from.x > 0 ? stepLen : -stepLen;
				for (long x = from.x + step; step > 0 ? x < to.x + step : x > to.x - step; x += step)
				{
					x = step > 0 ? System.Math.Min(x, to.x) : System.Math.Max(x, to.x);
					Fix64 z = (Fix64)from.z + a_xz * (Fix64)(x - from.x);
					Fix64 y = (Fix64)from.y + a_y * (Fix64)(x - from.x);
					if (!IsPassable(new TwVector3(x, (long)y, (long)z)))
					{
						blocked = true;
						break;
					}

					blockPoint.Set(x, (long)y, (long)z);
				}
			}
			else
			{
				a_xz = (Fix64)dx / (Fix64)dz;
				a_y = (Fix64)dy / (Fix64)dz;
				int step = to.z - from.z > 0 ? stepLen : -stepLen;
				for (long z = from.z + step; step > 0 ? z < to.z + step : z > to.z - step; z += step)
				{
					z = step > 0 ? System.Math.Min(z, to.z) : System.Math.Max(z, to.z);
					Fix64 x = (Fix64)from.x + a_xz * (Fix64)(z - from.z);
					Fix64 y = (Fix64)from.y + a_y * (Fix64)(z - from.z);
					if (!IsPassable(new TwVector3((long)x, (long)y, z)))
					{
						blocked = true;
						break;
					}

					blockPoint.Set((long)x, (long)y, z);
				}
			}

			if (blockPoint != from || blocked)
				return blockPoint;
			else
				return to;
		}


	}
}

