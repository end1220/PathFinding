
using System;
using System.Collections.Generic;
using AStar;


namespace PathFinding
{

	public class Grid3DPathPlanner : IPathPlanner
	{

		public override bool Process(AStarContext context)
		{
			PathFindingRequest request = context.Request as PathFindingRequest;

			ProcessingNode startNode = context.GetStartNode();
			ProcessingNode endNode = context.GetTargetNode();
			if (startNode == null || endNode == null)
				return false;

			FindPath(startNode, endNode, context);
			List<Int3> path = context.rawPathPoints;
			if (path.Count > 0)
			{
				// set the first and last point to 'from' and 'to'.
				path[0] = request.fromPosition;
				path[path.Count - 1] = request.toPosition;
				// then optimize
				NavMeshPathOptimizer.Optimize(ref context.rawPathNodeCache, ref path);
			}

			return context.rawPathPoints.Count >= 2;
		}


		private void FindPath(ProcessingNode start, ProcessingNode end, AStarContext context)
		{
			Grid3DGraph graphMap = context.map as Grid3DGraph;
			ProcessingNode endNode = DoAStar(context);

			context.rawPathNodeCache.Clear();
			context.rawPathPoints.Clear();
			ProcessingNode pathNode = endNode;
			while (pathNode != null)
			{
				Grid3DNode navNode = pathNode.astarNode as Grid3DNode;
				Grid3DNode node = graphMap.GetNodeAt(navNode.x, navNode.y, navNode.z);
				context.rawPathNodeCache.Add(navNode);
				context.rawPathPoints.Add(node.worldPosition);
				pathNode = pathNode.prev;
			}

			//var points = FindPath3D(startNode.id, endNode.id);

			/*Grid3DPathOptimizer.Optimize(graphMap, ref points);

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
				if ((result[1] - from).sqrMagnitudeLong < cellSize * cellSize)
					result.RemoveAt(1);
				if ((result[result.Count - 2] - to).sqrMagnitudeLong < cellSize * cellSize)
					result.RemoveAt(result.Count - 2);
			}
			return result.Count >= 2;*/
		}


		protected override bool CheckArrived(ProcessingNode node, AStarContext context)
		{
			ProcessingNode targetNode = context.GetTargetNode();
			return node.astarNode.id == targetNode.astarNode.id;
		}

		protected override int CalCostG(ProcessingNode prevNode, ProcessingNode currentNode, AStarContext context)
		{
			return prevNode.g + context.map.GetEdge(prevNode.astarNode.id, currentNode.astarNode.id).cost;
		}

		protected override int CalCostH(ProcessingNode node, AStarContext context)
		{
			Grid3DNode targetNode = context.GetTargetNode().astarNode as Grid3DNode;
			Grid3DNode calNode = node.astarNode as Grid3DNode;
			int dx = Math.Abs(targetNode.x - calNode.x);
			int dy = Math.Abs(targetNode.y - calNode.y);
			int dz = Math.Abs(targetNode.z - calNode.z);
			dx *= 10;
			dy *= 10;
			dz *= 10;
			int distxz = (int)(dx > dz ? 1.4f * dz + (dx - dz) : 1.4f * dx + (dz - dx));
			int dist = distxz + dy;
			return dist;
		}

	}


}