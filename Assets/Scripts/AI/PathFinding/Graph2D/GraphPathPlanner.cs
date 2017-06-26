using System;
using System.Collections;
using System.Collections.Generic;


namespace TwGame.AStar
{
	
	public class GraphPathPlanner : AStarPathPlanner
	{
		GraphAStarNode startNode;
		GraphAStarNode targetNode;

		private List<Point2D> resultCache = new List<Point2D>();


		public List<Point2D> FindPath(int start, int end)
		{
			GraphAStarNode endNode = _findPath(start, end);

			// build path points.
			resultCache.Clear();
			GraphAStarNode pathNode = endNode;
			while (pathNode != null)
			{
				resultCache.Add(new Point2D(pathNode.x, pathNode.y));
				pathNode = pathNode.prev as GraphAStarNode;
			}
			Cleanup();
			return resultCache;
		}

		private GraphAStarNode _findPath(int start, int end)
		{
			startNode = map.GetNodeByID(start) as GraphAStarNode;
			targetNode = map.GetNodeByID(end) as GraphAStarNode;

			GraphAStarNode endNode = DoAStar(startNode) as GraphAStarNode;

			return endNode;
		}

		protected override bool CheckArrived(AStarNode node)
		{
			return node.id == targetNode.id;
		}

		protected override int CalCostG(AStarNode prevNode, AStarNode currentNode)
		{
			return prevNode.g + map.GetEdge(prevNode.id, currentNode.id).cost;
		}

		protected override int CalCostH(AStarNode node)
		{
			int dx = Math.Abs(targetNode.x - ((GraphAStarNode)node).x);
			int dy = Math.Abs(targetNode.y - ((GraphAStarNode)node).y);
			int dist = (int)(dx > dy ? 1.4f * dy + (dx - dy) : 1.4f * dx + (dy - dx));
			return dist;
		}

	}


}