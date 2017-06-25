using System;
using System.Collections;
using System.Collections.Generic;


namespace Lite.AStar
{

	public class GraphPath3DPlanner : GraphPathPlanner
	{
		GraphAStar3DNode startNode;
		GraphAStar3DNode targetNode;

		private List<Point3D> resultCache = new List<Point3D>();


		public List<Point3D> FindPath(int start, int end)
		{
			GraphAStar3DNode endNode = _findPath(start, end);

			// build path points.
			resultCache.Clear();
			int count = 0;
			GraphAStar3DNode pathNode = endNode;
			while (pathNode != null)
			{
				count++;
				if (count > 100)
				{
					UnityEngine.Debug.LogError("GraphPath3DPlanner : wtf !!!");
					break;
				}
				resultCache.Add(new Point3D(pathNode.x, pathNode.y, pathNode.z));
				pathNode = pathNode.prev as GraphAStar3DNode;
			}
			Cleanup();
			return resultCache;
		}

		private GraphAStar3DNode _findPath(int start, int end)
		{
			startNode = map.GetNodeByID(start) as GraphAStar3DNode;
			targetNode = map.GetNodeByID(end) as GraphAStar3DNode;
			if (startNode == null || targetNode == null)
				return null;

			GraphAStar3DNode endNode = DoAStar(startNode) as GraphAStar3DNode;

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
			int dx = Math.Abs(targetNode.x - ((GraphAStar3DNode)node).x);
			int dy = Math.Abs(targetNode.y - ((GraphAStar3DNode)node).y);
			int dz = Math.Abs(targetNode.z - ((GraphAStar3DNode)node).z);
			dx *= 10;
			dy *= 10;
			dz *= 10;
			int distxz = (int)(dx > dz ? 1.4f * dz + (dx - dz) : 1.4f * dx + (dz - dx));
			int dist = distxz + dy;
			return dist;
		}

	}


}