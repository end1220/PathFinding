
using System;
using AStar;


namespace PathFinding
{
	public class Grid2DGraph : AStarMap, INavGraph
	{
		private const int NEIGHBOUR_COUNT = 8;
		private readonly int[] xOffset = { -1, -1, -1, 0, 1, 1, 1, 0 };
		private readonly int[] yOffset = { -1, 0, 1, 1, 1, 0, -1, -1 };

		private Grid2DNode[,] nodes;
		private int width;
		private int height;

		private int nodeIdCounter;

		Grid2DNavData navData;


		public void Init(INavData data)
		{
			navData = data as Grid2DNavData;
			nodeIdCounter = 0;
			this.width = navData.Width;
			this.height = navData.Height;

			nodes = new Grid2DNode[width, height];

			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					Grid2DNode node = new Grid2DNode(nodeIdCounter++);
					nodes[x, y] = node;
					node.x = (ushort)x;
					node.y = (ushort)y;
					node.blockValue = navData.GetMask(x, y);
					node.terrainType = navData.GetTerrain(x, y);
				}
			}
		}


		public Grid2DNode GetNode(int x, int y)
		{
			if (x >= 0 && x < width && y >= 0 && y <= height)
			{
				Grid2DNode node = nodes[x, y];
				return node;
			}
			return null;
		}

		public void SetNodePassable(int x, int y, bool passable)
		{
			Grid2DNode node = GetNode(x, y);
			if (node != null)
				node.blockValue = (ushort)(passable ? 0 : 1);
		}

		public bool IsNodePassable(int x, int y)
		{
			Grid2DNode node = GetNode(x, y);
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
				Grid2DNode gridNode = node as Grid2DNode;
				int x = gridNode.x + xOffset[index];
				int y = gridNode.y + yOffset[index];
				if (x >= 0 && x < width && y >= 0 && y < height)
				{
					Grid2DNode toNode = nodes[x, y] as Grid2DNode;
					if (IsNeighbourPassable(gridNode, toNode))
						return toNode;
				}
			}
			return null;
		}

		public Grid2DNode GetNodeByIndex(int x, int y)
		{
			if (x >=0 && x < width && y >= 0 && y < height)
			{
				return nodes[x,y] as Grid2DNode;
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

		private bool IsNeighbourPassable(Grid2DNode from, Grid2DNode to)
		{
			return (nodes[from.x, to.y].blockValue < 1 && nodes[to.x, from.y].blockValue < 1) 
				|| (from.x == to.x && from.y == to.y);
		}


		public Int2 FixVector3ToInt2(Int3 position)
		{
			int gridSize = navData.GridSize;
			int dx = position.x - navData.MinX;
			int dz = position.z - navData.MinZ;
			int x = dx / gridSize;
			int z = dz / gridSize;
			return new Int2(x, z);
		}


		public Int3 Int2ToFixVector3(Int2 point)
		{
			int x = navData.MinX + navData.GridSize * point.x;
			int z = navData.MinZ + navData.GridSize * point.y;
			return new Int3(x, 0, z);;
		}

		public bool IsMissileCross(Int3 position,int CrossType)
		{
			Int2 pt2d = FixVector3ToInt2(position);
			var node = GetNode(pt2d.x, pt2d.y);

			if(CrossType == 1)
			{
				if (node.terrainType == (byte)TerrainType.ShortWall || node.terrainType == (byte)TerrainType.Walkable)
				{
					return true;
				}
			}else if (CrossType == 2)
			{
				if (node.terrainType == (byte)TerrainType.Walkable)
					return true;
			}

			return false;
		}

		public bool IsPassable(Int3 position)
		{
			Int2 pt2d = FixVector3ToInt2(position);
			bool ret = this.IsNodePassable(pt2d.x, pt2d.y);
			return ret;
		}

		/// <summary>
		/// 有些地形在特殊情况下walkable，如墙可以闪现而过.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="mov"></param>
		/// <returns></returns>
		public bool SpecialTerrainPassable(Int3 position, MoveType mov)
		{
			Int2 pt2d = FixVector3ToInt2(position);
			var node = GetNode(pt2d.x, pt2d.y);

			if (node.terrainType == (byte)TerrainType.ShortWall)
			{
				if (mov == MoveType.ShortWall)
					return true;
			}
			if (node.terrainType == (byte)TerrainType.Walkable)
			{
				return true;
			}
			else if (node.terrainType == (byte)TerrainType.Unwalkable)
			{
				return false;
			}
			else if (node.terrainType == (byte)TerrainType.ShortWall || node.terrainType == (byte)TerrainType.TallWall)
			{
				return mov == MoveType.Blink;
			}
			return false;
		}


		public Int3 GetNearestPosition(Int3 position)
		{
			int step = 5;
			if (IsPassable(position))
				return position;
			Int2 pt2 = FixVector3ToInt2(position);
			for (int x = pt2.x; x < pt2.x + step; ++x)
			{
				for (int y = pt2.y; y < pt2.y + step; ++y)
				{
					if (IsNodePassable(x, y))
						return Int2ToFixVector3(new Int2(x, y));
				}
				for (int y = pt2.y; y > pt2.y - step; --y)
				{
					if (IsNodePassable(x, y))
						return Int2ToFixVector3(new Int2(x, y));
				}
			}
			for (int x = pt2.x; x > pt2.x - step; --x)
			{
				for (int y = pt2.y; y < pt2.y + step; ++y)
				{
					if (IsNodePassable(x, y))
						return Int2ToFixVector3(new Int2(x,y));
				}
				for (int y = pt2.y; y > pt2.y - step; --y)
				{
					if (IsNodePassable(x, y))
						return Int2ToFixVector3(new Int2(x, y));
				}
			}
			UnityEngine.Debug.LogError("Grid2dMap: GetNearestForce failed.");
			return position;
		}


		// 射线碰撞，计算起点到终点间的最远可到达点
		public bool LineCastForMoving(ref HitInfo hit, MoveType mov)
		{
			Int3 from = hit.from;
			Int3 to = hit.to;

			Int3 blockPoint = from;
			int stepLen = Math.Min(200, navData.GridSize);
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
					var tmpPos = new Int3(x, 0, (int)z);
					if (!IsPassable(tmpPos))
					{
						if (!SpecialTerrainPassable(tmpPos, mov))
						{
							blocked = true;
							break;
						}
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
					var tmpPos = new Int3((int)x, 0, z);
					if (!IsPassable(tmpPos))
					{
						if (!SpecialTerrainPassable(tmpPos, mov))
						{
							blocked = true;
							break;
						}
					}

					blockPoint.Set((int)x, from.y, z);
				}
			}

			if (blockPoint != from)
			{
				hit.hitPosition = blockPoint;
			}
			else
			{
				hit.hitPosition = to;
			}
			return blocked;
		}


		public Int3 SlideByObstacles(Int3 from, Int3 to, Int3 hit)
		{
			Int2 fromPoint = this.FixVector3ToInt2(from);
			Int2 targetPoint = this.FixVector3ToInt2(to);
			Int3 newDirection = to - from;
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

			Int3 retPosition = from + newDirection;
			return retPosition;
		}


	}
}

