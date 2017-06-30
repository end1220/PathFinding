using System;
using System.Collections;
using System.Collections.Generic;
using TwFramework;


namespace TwGame.AStar
{

	public class Graph3DPathPlanner : GraphPathPlanner
	{
		Graph3DAStarNode startNode;
		Graph3DAStarNode targetNode;

		private List<Int3> resultCache = new List<Int3>();


		public bool FindPath3D(TwVector3 from, TwVector3 to, ref List<TwVector3> result)
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
				result.Add(new TwVector3(node.worldPosition.x, node.worldPosition.y, node.worldPosition.z));
			}
			if (result.Count >= 2)
			{
				result[0] = from;
				result[result.Count - 1] = to;
			}
			if (result.Count >= 4)
			{
				int cellSize = graphMap.navGraphData.buildConfig.cellSize;
				if ((result[1] - from).sqrMagnitude < cellSize * cellSize)
					result.RemoveAt(1);
				if ((result[result.Count - 2] - to).sqrMagnitude < cellSize * cellSize)
					result.RemoveAt(result.Count - 2);
			}
			return result.Count >= 2;
		}


		public List<Int3> FindPath3D(int startId, int endId)
		{
			Graph3DAStarNode node = _findPath(startId, endId);

			resultCache.Clear();
			while (node != null)
			{
				resultCache.Add(new Int3(node.x, node.y, node.z));
				node = node.prev as Graph3DAStarNode;
			}
			
			return resultCache;
		}


		private Graph3DAStarNode _findPath(int start, int end)
		{
			startNode = map.GetNodeByID(end) as Graph3DAStarNode;
			targetNode = map.GetNodeByID(start) as Graph3DAStarNode;
			if (startNode == null || targetNode == null)
				return null;

			Graph3DAStarNode endNode = DoAStar(startNode) as Graph3DAStarNode;

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
			int dx = Math.Abs(targetNode.x - ((Graph3DAStarNode)node).x);
			int dy = Math.Abs(targetNode.y - ((Graph3DAStarNode)node).y);
			int dz = Math.Abs(targetNode.z - ((Graph3DAStarNode)node).z);
			dx *= 10;
			dy *= 10;
			dz *= 10;
			int distxz = (int)(dx > dz ? 1.4f * dz + (dx - dz) : 1.4f * dx + (dz - dx));
			int dist = distxz + dy;
			return dist;
		}

	}


}