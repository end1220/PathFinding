
using System;
using System.Collections;
using System.Collections.Generic;


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


		public void InitMap(int width, int height, NavigationData data)
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
					node.x = x;
					node.y = y;
					node.blockValue = data.At(x, y);
				}
			}
		}

		public void InitMap(int width, int height, int[,] mask)
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
					node.x = x;
					node.y = y;
					node.blockValue = mask != null ? mask[x, y] : 0;
				}
			}
		}

		public void SetNodePassable(int x, int y, bool passable)
		{
			if (x >= 0 && x < width && y >= 0 && y <= height)
			{
				GridAStarNode node = nodes[x, y];
				node.blockValue = passable ? 0 : 1;
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

	}
}

