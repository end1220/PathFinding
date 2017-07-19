﻿
using System;
using AStar;


namespace PathFinding
{
	public class GridAStarMap : AStarMap
	{
		private const int NEIGHBOUR_COUNT = 8;
		private readonly int[] xOffset = { -1, -1, -1, 0, 1, 1, 1, 0 };
		private readonly int[] yOffset = { -1, 0, 1, 1, 1, 0, -1, -1 };

		private GridAStarNode[,] nodes;
		private int width;
		private int height;

		private int nodeIdCounter;

		NavGrid2DData navGridData;


		public void InitMap(int width, int height, NavGrid2DData data)
		{
			navGridData = data;
			nodeIdCounter = 0;
			this.width = width;
			this.height = height;

			nodes = new GridAStarNode[width, height];

			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					GridAStarNode node = new GridAStarNode(nodeIdCounter++);
					nodes[x, y] = node;
					node.x = (ushort)x;
					node.y = (ushort)y;
					node.blockValue = data.GetMask(x, y);
					node.terrainType = data.GetTerrain(x, y);
				}
			}
		}

		public void InitMap(int width, int height, ushort[,] mask)
		{
			nodeIdCounter = 0;
			this.width = width;
			this.height = height;

			nodes = new GridAStarNode[width, height];

			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					GridAStarNode node = new GridAStarNode(nodeIdCounter++);
					nodes[x, y] = node;
					node.x = (ushort)x;
					node.y = (ushort)y;
					node.blockValue = (ushort)(mask != null ? mask[x, y] : 0);
				}
			}
		}

		public GridAStarNode GetNode(int x, int y)
		{
			if (x >= 0 && x < width && y >= 0 && y <= height)
			{
				GridAStarNode node = nodes[x, y];
				return node;
			}
			return null;
		}

		public void SetNodePassable(int x, int y, bool passable)
		{
			GridAStarNode node = GetNode(x, y);
			if (node != null)
				node.blockValue = (ushort)(passable ? 0 : 1);
		}

		public bool IsNodePassable(int x, int y)
		{
			GridAStarNode node = GetNode(x, y);
			if (node != null)
				return node.blockValue == 0;
			return false;
		}

		public override int GetNeighbourNodeCount(AStarNode node)
		{
			return NEIGHBOUR_COUNT;
		}

		public override AStarNode GetNeighbourNode(AStarNode node, int index)
		{
			if (index >= 0 && index < NEIGHBOUR_COUNT && node != null)
			{
				GridAStarNode gridNode = node as GridAStarNode;
				int x = gridNode.x + xOffset[index];
				int y = gridNode.y + yOffset[index];
				if (x >= 0 && x < width && y >= 0 && y < height)
				{
					GridAStarNode toNode = nodes[x, y] as GridAStarNode;
					if (IsNeighbourPassable(gridNode, toNode))
						return toNode;
				}
			}
			return null;
		}

		public GridAStarNode GetNodeByIndex(int x, int y)
		{
			if (x >=0 && x < width && y >= 0 && y < height)
			{
				return nodes[x,y] as GridAStarNode;
			}
			return null;
		}

		public int GetWidth()
		{
			return width;
		}

		public int GetHeight()
		{
			return height;
		}

		private bool IsNeighbourPassable(GridAStarNode from, GridAStarNode to)
		{
			return (nodes[from.x, to.y].blockValue < 1 && nodes[to.x, from.y].blockValue < 1) 
				|| (from.x == to.x && from.y == to.y);
		}


		public Int2 FixVector3ToInt2(FixVector3 position)
		{
			int gridSize = navGridData.GridSize;
			int dx = position.x - navGridData.MinX;
			int dz = position.z - navGridData.MinZ;
			int x = dx / gridSize;
			int z = dz / gridSize;
			return new Int2(x, z);
		}


		public FixVector3 Int2ToFixVector3(Int2 point)
		{
			int x = navGridData.MinX + navGridData.GridSize * point.x;
			int z = navGridData.MinZ + navGridData.GridSize * point.y;
			return new FixVector3(x, 0, z);;
		}


		public bool IsPassable(FixVector3 position)
		{
			Int2 pt2d = FixVector3ToInt2(position);
			bool ret = this.IsNodePassable(pt2d.x, pt2d.y);
			return ret;
		}


		// 射线碰撞，计算起点到终点间的最远可到达点
		public FixVector3 RayCast2DForMoving(FixVector3 from, FixVector3 to)
		{
			FixVector3 blockPoint = from;
			int stepLen = Math.Min(200, navGridData.GridSize);
			bool blocked = false;

			// y = a*x + b
			Fix64 a = (Fix64)0;
			int dx = to.x - from.x;
			int dz = to.z - from.z;
			if (Math.Abs(dx) > Math.Abs(dz))
			{
				a = (Fix64)dz / (Fix64)dx;
				int step = to.x - from.x > 0 ? stepLen : -stepLen;
				for (int x = from.x + step; step > 0 ? x < to.x + step : x > to.x - step; x += step)
				{
					x = step > 0 ? Math.Min(x, to.x) : Math.Max(x, to.x);
					Fix64 z = (Fix64)from.z + a * (Fix64)(x - from.x);
					var tmpPos = new FixVector3(x, 0, (int)z);
					if (!IsPassable(tmpPos))
					{
						blocked = true;
						break;
					}

					blockPoint.Set(x, from.y, (int)z);
				}
			}
			else
			{
				a = (Fix64)dx / (Fix64)dz;
				int step = to.z - from.z > 0 ? stepLen : -stepLen;
				for (int z = from.z + step; step > 0 ? z < to.z + step : z > to.z - step; z += step)
				{
					z = step > 0 ? Math.Min(z, to.z) : Math.Max(z, to.z);
					Fix64 x = (Fix64)from.x + a * (Fix64)(z - from.z);
					var tmpPos = new FixVector3((int)x, 0, z);
					if (!IsPassable(tmpPos))
					{
						blocked = true;
						break;
					}

					blockPoint.Set((int)x, from.y, z);
				}
			}

			if (blockPoint != from || blocked)
				return blockPoint;
			else
				return to;
		}


		public FixVector3 SlideByObstacles(FixVector3 fromPos, FixVector3 oldTargetPos)
		{
			Int2 fromPoint = this.FixVector3ToInt2(fromPos);
			Int2 targetPoint = this.FixVector3ToInt2(oldTargetPos);
			FixVector3 newDirection = oldTargetPos - fromPos;
			if (fromPoint.x == targetPoint.x)
			{
				// 去掉z方向分量
				newDirection.z = 0;
			}
			else if (fromPoint.y == targetPoint.y)
			{
				// 去掉x方向分量
				newDirection.x = 0;
			}
			else
			{
				// 选择去掉xy方向分量
				if (Math.Abs(newDirection.x) > Math.Abs(newDirection.z))
				{
					newDirection.z = 0;
				}
				else
				{
					newDirection.x = 0;
				}
			}

			FixVector3 retPosition = fromPos + newDirection;
			return retPosition;
		}


	}
}
