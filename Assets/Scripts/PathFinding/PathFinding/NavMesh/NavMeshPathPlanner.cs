

using System.Collections.Generic;
using AStar;


namespace PathFinding
{
	
	public class NavMeshPathPlanner : IPathPlanner
	{
		NavMeshNode startNode;
		NavMeshNode targetNode;

		private List<NavMeshNode> resultNodeCache = new List<NavMeshNode>();
		private List<Int3> resultCache = new List<Int3>();


		public override bool FindPath(FixVector3 from, FixVector3 to, ref List<FixVector3> result)
		{
			result.Clear();

			NavMeshMap navMap = this.map as NavMeshMap;

			var startNode = navMap.bbTree.QueryInside(to.ToVector3(), null);
			var endNode = navMap.bbTree.QueryInside(from.ToVector3(), null);
			if (startNode == null || endNode == null)
				return false;

			int start = startNode.id;
			int end = endNode.id;

			var path = FindPath(start, end);
			for (int i = 0; i < path.Count; ++i)
				result.Add(new FixVector3(path[i].x, path[i].y, path[i].z));

			if (result.Count > 0)
			{
				result[0] = from;
				result[result.Count - 1] = to;
			}

			return result.Count >= 2;
		}


		public List<Int3> FindPath(int start, int end)
		{
			NavMeshNode endNode = _findPath(start, end);

			// build path points.
			resultNodeCache.Clear();
			resultCache.Clear();
			NavMeshNode pathNode = endNode;
			while (pathNode != null)
			{
				resultNodeCache.Add(pathNode);
				resultCache.Add(pathNode.position);
				pathNode = pathNode.prev as NavMeshNode;
			}

			PathOptimizerNavMesh.Optimize(ref resultNodeCache, ref resultCache);

			return resultCache;
		}

		private NavMeshNode _findPath(int start, int end)
		{
			startNode = map.GetNode(start) as NavMeshNode;
			targetNode = map.GetNode(end) as NavMeshNode;

			NavMeshNode endNode = DoAStar(startNode) as NavMeshNode;

			return endNode;
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