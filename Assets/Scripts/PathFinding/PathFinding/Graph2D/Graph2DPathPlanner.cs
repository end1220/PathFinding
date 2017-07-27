using System;
using System.Collections.Generic;
using AStar;


namespace PathFinding
{
	
	public class Graph2DPathPlanner : AStarPathPlanner
	{
		Graph2DNode startNode;
		Graph2DNode targetNode;

		private List<Int3> resultCache = new List<Int3>();


		public List<Int3> FindPath(int start, int end)
		{
			Graph2DNode endNode = _findPath(start, end);

			// build path points.
			resultCache.Clear();
			Graph2DNode pathNode = endNode;
			while (pathNode != null)
			{
				resultCache.Add(new Int3(pathNode.x, pathNode.y));
				pathNode = pathNode.prev as Graph2DNode;
			}
			
			return resultCache;
		}

		private Graph2DNode _findPath(int start, int end)
		{
			startNode = map.GetNodeByID(start) as Graph2DNode;
			targetNode = map.GetNodeByID(end) as Graph2DNode;

			Graph2DNode endNode = DoAStar(startNode) as Graph2DNode;

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
			int dx = Math.Abs(targetNode.x - ((Graph2DNode)node).x);
			int dy = Math.Abs(targetNode.y - ((Graph2DNode)node).y);
			int dist = (int)(dx > dy ? 1.4f * dy + (dx - dy) : 1.4f * dx + (dy - dx));
			return dist;
		}

	}


}