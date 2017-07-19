
using System;
using System.Collections.Generic;
using AStar;


namespace PathFinding
{
	
	public class GridPathPlanner : AStarPathPlanner
	{
		private int startX;
		private int startY;
		private int endX;
		private int endY;
		private GridAStarNode startNode;
		private GridAStarNode targetNode;

		private List<Int2> resultCache = new List<Int2>();


		public bool FindPath(FixVector3 from, FixVector3 to, ref List<FixVector3> result)
		{
			GridAStarMap gridMap = this.map as GridAStarMap;

			Int2 start = gridMap.FixVector3ToInt2(from);
			Int2 end = gridMap.FixVector3ToInt2(to);

			var path = FindPath(start.x, start.y, end.x, end.y);

			PathOptimizer2D.Optimize(gridMap, ref path);

			result.Clear();
			for (int i = 0; i < path.Count; ++i)
			{
				result.Add(gridMap.Int2ToFixVector3(path[i]));
			}
			
			if (result.Count > 0)
			{
				result[0] = from;
				result[result.Count - 1] = to;
			}

			return result.Count >= 2;
		}


		public List<Int2> FindPath(int startX, int startY, int endX, int endY)
		{
			GridAStarNode endNode = _findPath(startX, startY, endX, endY);

			// build path points.
			resultCache.Clear();
			GridAStarNode pathNode = endNode;
			while (pathNode != null)
			{
				resultCache.Add(new Int2(pathNode.x, pathNode.y));
				pathNode = pathNode.prev as GridAStarNode;
			}
			
			return resultCache;
		}

		private GridAStarNode _findPath(int startX, int startY, int endX, int endY)
		{
			this.startX = endX;
			this.startY = endY;
			this.endX = startX;
			this.endY = startY;

			GridAStarMap gridMap = (GridAStarMap)map;
			startNode = gridMap.GetNodeByIndex(this.startX, this.startY);
			targetNode = gridMap.GetNodeByIndex(this.endX, this.endY);

			GridAStarNode endNode = DoAStar(startNode) as GridAStarNode;

			return endNode;
		}

		protected override bool CheckArrived(AStarNode node)
		{
			return node.id == targetNode.id;
		}

		protected override int CalCostG(AStarNode prevNode, AStarNode currentNode)
		{
			int dx = Math.Abs(((GridAStarNode)prevNode).x - ((GridAStarNode)currentNode).x);
			int dy = Math.Abs(((GridAStarNode)prevNode).y - ((GridAStarNode)currentNode).y);
			int dist = dx > dy ? 14 * dy + 10 * (dx - dy) : 14 * dx + 10 * (dy - dx);
			return prevNode.g + dist;
		}

		protected override int CalCostH(AStarNode node)
		{
			int dx = Math.Abs(endX - ((GridAStarNode)node).x);
			int dy = Math.Abs(endY - ((GridAStarNode)node).y);
			int dist = dx > dy ? 14 * dy + 10 * (dx - dy) : 14 * dx + 10 * (dy - dx);
			return dist;
		}

	}


}