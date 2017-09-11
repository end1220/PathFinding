

using System.Collections.Generic;
using AStar;


namespace PathFinding
{
	
	public class NavMeshPathPlanner : IPathPlanner
	{
		NavMeshNode startNode;
		NavMeshNode targetNode;

		private List<NavMeshNode> rawPathNodeCache = new List<NavMeshNode>();
		private List<Int3> rawPathCache = new List<Int3>();


		public override bool FindPath(Int3 from, Int3 to, ref List<Int3> result)
		{
			result.Clear();

			NavMeshGraph navMap = this.map as NavMeshGraph;

			var startNode = navMap.bbTree.QueryInside(to, null);
			var endNode = navMap.bbTree.QueryInside(from, null);
			if (startNode == null || endNode == null)
				return false;

			var path = FindPath(startNode, endNode);
			if (path.Count > 0)
			{
				// set the first and last point to 'from' and 'to'.
				path[0] = new Int3(from.x, from.y, from.z);
				path[path.Count - 1] = new Int3(to.x, to.y, to.z);
				// then optimize
				NavMeshPathOptimizer.Optimize(ref rawPathNodeCache, ref path);
			}

			for (int i = 0; i < path.Count; ++i)
				result.Add(new Int3(path[i].x, path[i].y, path[i].z));

			return result.Count >= 2;
		}


		private List<Int3> FindPath(NavMeshNode start, NavMeshNode end)
		{
			startNode = start;
			targetNode = end;
			NavMeshNode endNode = DoAStar(startNode) as NavMeshNode;

			rawPathNodeCache.Clear();
			rawPathCache.Clear();
			NavMeshNode pathNode = endNode;
			while (pathNode != null)
			{
				rawPathNodeCache.Add(pathNode);
				rawPathCache.Add(pathNode.position);
				pathNode = pathNode.prev as NavMeshNode;
			}

			return rawPathCache;
		}


		protected override bool CheckArrived(AStarNode node)
		{
			return node.id == targetNode.id;
		}


		protected override int CalCostG(AStarNode prevNode, AStarNode currentNode)
		{
			return prevNode.g + (prevNode as NavMeshNode).GetConnectionCost(currentNode.id);
		}


		protected override int CalCostH(AStarNode node)
		{
			NavMeshNode calNode = node as NavMeshNode;
			int dist = (calNode.position - targetNode.position).costMagnitude;
			return dist;
		}

	}


}