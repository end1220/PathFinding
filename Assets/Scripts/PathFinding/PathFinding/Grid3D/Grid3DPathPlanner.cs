
using System;
using System.Collections.Generic;
using AStar;


namespace PathFinding
{

	public class Grid3DPathPlanner : IPathPlanner
	{
		Grid3DNode startNode;
		Grid3DNode targetNode;

		private List<Int3> resultCache = new List<Int3>();


		public override bool FindPath(Int3 from, Int3 to, ref List<Int3> result)
		{
			Grid3DGraph graphMap = this.map as Grid3DGraph;
			var startNode = graphMap.GetNearbyWalkableNode(from);
			var endNode = graphMap.GetNearbyWalkableNode(to);
			if (startNode == null || endNode == null)
				return false;

			var points = FindPath3D(startNode.id, endNode.id);

			Grid3DPathOptimizer.Optimize(graphMap, ref points);

			result.Clear();
			for (int i = 0; i < points.Count; ++i)
			{
				var point = points[i];
				var node = graphMap.GetNodeAt(point.x, point.y, point.z);
				result.Add(new Int3(node.worldPosition.x, node.worldPosition.y, node.worldPosition.z));
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
			Grid3DNode node = _findPath(startId, endId);

			resultCache.Clear();
			while (node != null)
			{
				resultCache.Add(new Int3(node.x, node.y, node.z));
				node = node.prev as Grid3DNode;
			}
			
			return resultCache;
		}


		private Grid3DNode _findPath(int start, int end)
		{
			startNode = map.GetNode(end) as Grid3DNode;
			targetNode = map.GetNode(start) as Grid3DNode;
			if (startNode == null || targetNode == null)
				return null;

			Grid3DNode endNode = DoAStar(startNode) as Grid3DNode;

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
			int dx = Math.Abs(targetNode.x - ((Grid3DNode)node).x);
			int dy = Math.Abs(targetNode.y - ((Grid3DNode)node).y);
			int dz = Math.Abs(targetNode.z - ((Grid3DNode)node).z);
			dx *= 10;
			dy *= 10;
			dz *= 10;
			int distxz = (int)(dx > dz ? 1.4f * dz + (dx - dz) : 1.4f * dx + (dz - dx));
			int dist = distxz + dy;
			return dist;
		}

	}


}