
using System;
using System.Collections.Generic;
using AStar;


namespace PathFinding
{

	public class Graph3DPathPlanner : AStarPathPlanner
	{
		Graph3DNode startNode;
		Graph3DNode targetNode;

		private List<Int3> resultCache = new List<Int3>();


		public bool FindPath3D(FixVector3 from, FixVector3 to, ref List<FixVector3> result)
		{
			Graph3DAStarMap graphMap = this.map as Graph3DAStarMap;
			var startNode = graphMap.GetNearbyWalkableNode(from);
			var endNode = graphMap.GetNearbyWalkableNode(to);
			if (startNode == null || endNode == null)
				return false;

			var points = FindPath3D(startNode.id, endNode.id);

			PathOptimizer3D.Optimize(graphMap, ref points);

			result.Clear();
			for (int i = 0; i < points.Count; ++i)
			{
				var point = points[i];
				var node = graphMap.GetNodeAt(point.x, point.y, point.z);
				result.Add(new FixVector3(node.worldPosition.x, node.worldPosition.y, node.worldPosition.z));
			}
			if (result.Count >= 2)
			{
				result[0] = from;
				result[result.Count - 1] = to;
			}
			if (result.Count >= 4)
			{
				int cellSize = graphMap.navGraphData.buildConfig.cellSize;
				if ((result[1] - from).sqrLength < cellSize * cellSize)
					result.RemoveAt(1);
				if ((result[result.Count - 2] - to).sqrLength < cellSize * cellSize)
					result.RemoveAt(result.Count - 2);
			}
			return result.Count >= 2;
		}


		public List<Int3> FindPath3D(int startId, int endId)
		{
			Graph3DNode node = _findPath(startId, endId);

			resultCache.Clear();
			while (node != null)
			{
				resultCache.Add(new Int3(node.x, node.y, node.z));
				node = node.prev as Graph3DNode;
			}
			
			return resultCache;
		}


		private Graph3DNode _findPath(int start, int end)
		{
			startNode = map.GetNode(end) as Graph3DNode;
			targetNode = map.GetNode(start) as Graph3DNode;
			if (startNode == null || targetNode == null)
				return null;

			Graph3DNode endNode = DoAStar(startNode) as Graph3DNode;

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
			int dx = Math.Abs(targetNode.x - ((Graph3DNode)node).x);
			int dy = Math.Abs(targetNode.y - ((Graph3DNode)node).y);
			int dz = Math.Abs(targetNode.z - ((Graph3DNode)node).z);
			dx *= 10;
			dy *= 10;
			dz *= 10;
			int distxz = (int)(dx > dz ? 1.4f * dz + (dx - dz) : 1.4f * dx + (dz - dx));
			int dist = distxz + dy;
			return dist;
		}

	}


}