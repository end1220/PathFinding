using System;
using System.Collections;
using System.Collections.Generic;



namespace Lite.AStar
{

	public class Graph3DPathPlanner : GraphPathPlanner
	{
		Graph3DAStarNode startNode;
		Graph3DAStarNode targetNode;

		private List<Point3D> resultCache = new List<Point3D>();


		public bool FindPath3D(TwVector3 from, TwVector3 to, ref List<TwVector3> result)
		{
			Graph3DAStarMap graphMap = this.map as Graph3DAStarMap;
			var start = graphMap.TwVector3ToPoint3D(from);
			var end = graphMap.TwVector3ToPoint3D(to);
			var startNode = graphMap.GetNodeAt(start.x, start.y, start.z);
			var endNode = graphMap.GetNodeAt(end.x, end.y, end.z);
			if (startNode == null || endNode == null)
				return false;
			
			int startId = startNode.id;
			int endId = endNode.id;
			Graph3DAStarNode node = _findPath(startNode.id, endNode.id);

			result.Clear();
			while (node != null)
			{
				result.Add(new TwVector3(node.worldPosition.x, node.worldPosition.y, node.worldPosition.z));
				node = node.prev as Graph3DAStarNode;
			}
			if (result.Count > 0)
			{
				result[0] = from;
				result[result.Count - 1] = to;
			}
			Cleanup();
			return result.Count > 0;
		}


		public List<Point3D> FindPath3D(int from, int to)
		{
			Graph3DAStarNode node = _findPath(from, to);

			resultCache.Clear();
			while (node != null)
			{
				resultCache.Add(new Point3D(node.x, node.y, node.z));
				node = node.prev as Graph3DAStarNode;
			}
			Cleanup();
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