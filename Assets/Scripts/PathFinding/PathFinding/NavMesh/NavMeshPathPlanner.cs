
using System;
using System.Collections.Generic;
using AStar;


namespace PathFinding
{
	
	public class NavMeshMapPathPlanner : GraphPathPlanner
	{
		NavMeshNode startNode;
		NavMeshNode targetNode;

		private List<Int3> resultCache = new List<Int3>();


		public List<Int3> FindPath2(int start, int end)
		{
			NavMeshNode endNode = _findPath2(start, end);

			// build path points.
			resultCache.Clear();
			NavMeshNode pathNode = endNode;
			while (pathNode != null)
			{
				resultCache.Add(pathNode.position);
				pathNode = pathNode.prev as NavMeshNode;
			}

			return resultCache;
		}

		private NavMeshNode _findPath2(int start, int end)
		{
			startNode = map.GetNodeByID(start) as NavMeshNode;
			targetNode = map.GetNodeByID(end) as NavMeshNode;

			NavMeshNode endNode = DoAStar(startNode) as NavMeshNode;

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
			NavMeshNode calNode = node as NavMeshNode;
			int dist = (calNode.position - targetNode.position).costMagnitude;
			return dist;
		}

	}


}