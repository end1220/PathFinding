
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lite.Graph;


namespace Lite.AStar
{
	public class Graph3DAStarMap : GraphAStarMap
	{
		private NavGraph3DData navGraphData;

		private Graph3DAStarNode[, ,] nodeMatrix;
		public Graph3DAStarNode[, ,] NodeMatrix { get { return nodeMatrix; } }

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


		public bool IsNodePassable(int x, int y, int z)
		{
			if (IsIndexValid(x, y, z))
			{
				var grid = nodeMatrix[x, y, z];
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


		public long GetGroundHeight3D(TwVector3 position)
		{
			int stepHeight = navGraphData.buildConfig.agentHeightStep * navGraphData.buildConfig.cellSize;
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
			bool ret = this.IsNodePassable(pt3d.x, pt3d.y, pt3d.z);
			return ret;
		}


		public Graph3DAStarNode GetNearbyWalkableNode(TwVector3 pos)
		{
			var pt = TwVector3ToPoint3D(pos);
			var node = GetNodeAt(pt.x, pt.y, pt.z);
			if (node != null && node.walkable)
				return node;

			for (int ix = -1; ix <= 1; ix += 2)
			{
				for (int iy = -1; iy <= 1; iy += 2)
				{
					for (int iz = -1; iz <= 1; iz += 2)
					{
						int x = pt.x + ix;
						int y = pt.y + iy;
						int z = pt.z + iz;
						if (IsNodePassable(x, y, z))
						{
							node = GetNodeAt(x, y, z);
							return node;
						}
					}
				}
			}

			return null;
		}

		
		public TwVector3 RayCast3D(TwVector3 from, TwVector3 to)
		{
			int halfAgentHeightStep = Math.Max(1, navGraphData.buildConfig.agentHeightStep / 2);

			TwVector3 blockPoint = from;
			int stepLen = navGraphData.buildConfig.cellSize / 5;
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
				long lastY = from.y;
				for (long x = from.x + step; step > 0 ? x < to.x + step : x > to.x - step; x += step)
				{
					x = step > 0 ? System.Math.Min(x, to.x) : System.Math.Max(x, to.x);
					Fix64 z = (Fix64)from.z + a_xz * (Fix64)(x - from.x);
					//Fix64 y = (Fix64)from.y + a_y * (Fix64)(x - from.x);
					long y = lastY;

					// stairs
					bool passable = false;
					for (int iy = halfAgentHeightStep; iy >= -halfAgentHeightStep; iy--)
					{
						long tmpy = y + iy * navGraphData.buildConfig.cellSize;
						Point3D pt3d = TwVector3ToPoint3D(new TwVector3(x, tmpy, (long)z));
						var node = GetNodeAt(pt3d.x, pt3d.y, pt3d.z);
						if (IsNodePassable(pt3d.x, pt3d.y, pt3d.z))
						{
							y = node.worldPosition.y;
							lastY = y;
							passable = true;
							break;
						}
					}
					if (!passable)
					{
						blocked = true;
						break;
					}

					/*if (!IsPassable(new TwVector3(x, (long)y, (long)z)))
					{
						blocked = true;
						break;
					}*/

					blockPoint.Set(x, (long)y, (long)z);
				}
			}
			else
			{
				a_xz = (Fix64)dx / (Fix64)dz;
				a_y = (Fix64)dy / (Fix64)dz;
				int step = to.z - from.z > 0 ? stepLen : -stepLen;
				long lastY = from.y;
				for (long z = from.z + step; step > 0 ? z < to.z + step : z > to.z - step; z += step)
				{
					z = step > 0 ? System.Math.Min(z, to.z) : System.Math.Max(z, to.z);
					Fix64 x = (Fix64)from.x + a_xz * (Fix64)(z - from.z);
					//Fix64 y = (Fix64)from.y + a_y * (Fix64)(z - from.z);
					long y = lastY;

					// stairs
					bool passable = false;
					for (int iy = halfAgentHeightStep; iy >= -halfAgentHeightStep; iy--)
					{
						long tmpy = y + iy * navGraphData.buildConfig.cellSize;
						Point3D pt3d = TwVector3ToPoint3D(new TwVector3((long)x, tmpy, z));
						var node = GetNodeAt(pt3d.x, pt3d.y, pt3d.z);
						if (IsNodePassable(pt3d.x, pt3d.y, pt3d.z))
						{
							y = node.worldPosition.y;
							lastY = y;
							passable = true;
							break;
						}
					}
					if (!passable)
					{
						blocked = true;
						break;
					}

					/*if (!IsPassable(new TwVector3((long)x, (long)y, z)))
					{
						blocked = true;
						break;
					}*/

					blockPoint.Set((long)x, (long)y, z);
				}
			}

			TwVector3 retPos;
			if (blockPoint != from || blocked)
				retPos = blockPoint;
			else
				retPos = to;
			retPos.y = GetGroundHeight3D(retPos);
			return retPos;
		}



	}
}

