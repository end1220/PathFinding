
using System;
using System.Collections;
using System.Collections.Generic;
using Lite;


namespace Lite.AStar
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
					node.blockValue = (ushort)data.At(x, y);
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

		public void SetNodePassable(int x, int y, bool passable)
		{
			if (x >= 0 && x < width && y >= 0 && y <= height)
			{
				GridAStarNode node = nodes[x, y];
				node.blockValue = (ushort)(passable ? 0 : 1);
			}
		}

		public bool IsNodePassable(int x, int y)
		{
			if (x >= 0 && x < width && y >= 0 && y <= height)
			{
				GridAStarNode node = nodes[x, y];
				return node.blockValue == 0;
			}
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


		public Int2 TwVector3ToInt2(TwVector3 position)
		{
			int gridSize = navGridData.GridSize;
			int dx = position.x - navGridData.MinX;
			int dz = position.z - navGridData.MinZ;
			int x = dx / gridSize;
			int z = dz / gridSize;
			return new Int2(x, z);
		}


		public TwVector3 Int2ToTwVector3(Int2 point)
		{
			int x = navGridData.MinX + navGridData.GridSize * point.x;
			int z = navGridData.MinZ + navGridData.GridSize * point.y;
			return new TwVector3(x, 0, z);;
		}


		public bool IsPassable(TwVector3 position)
		{
			Int2 pt2d = TwVector3ToInt2(position);
			bool ret = this.IsNodePassable(pt2d.x, pt2d.y);
			return ret;
		}


		// 射线碰撞，计算起点到终点间的最远可到达点
		public TwVector3 RayCast2D(TwVector3 from, TwVector3 to)
		{
			TwVector3 blockPoint = from;
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
					x = step > 0 ? System.Math.Min(x, to.x) : System.Math.Max(x, to.x);
					Fix64 z = (Fix64)from.z + a * (Fix64)(x - from.x);

					if (!IsPassable(new TwVector3(x, 0, (int)z)))
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
					z = step > 0 ? System.Math.Min(z, to.z) : System.Math.Max(z, to.z);
					Fix64 x = (Fix64)from.x + a * (Fix64)(z - from.z);
					if (!IsPassable(new TwVector3((int)x, 0, z)))
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


	}
}

