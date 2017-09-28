
using System;
using System.Collections.Generic;
using AStar;


namespace PathFinding
{
	
	public class Grid2DPathPlanner : IPathPlanner
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


		public void FindPath(ProcessingNode start, ProcessingNode end, AStarContext context)
		{
			ProcessingNode endNode = DoAStar(context);

			Grid2DGraph graph = context.map as Grid2DGraph;
			ProcessingNode pathNode = endNode;
			while (pathNode != null)
			{
				Grid2DNode navNode = pathNode.astarNode as Grid2DNode;
				context.rawPathNodeCache.Add(navNode);
				context.rawPathPoints.Add(graph.Int2ToFixVector3(new Int2(navNode.x, navNode.y)));
				pathNode = pathNode.prev;
			}
			
		}


		protected override bool CheckArrived(ProcessingNode node, AStarContext context)
		{
			ProcessingNode targetNode = context.GetTargetNode();
			return node.astarNode.id == targetNode.astarNode.id;
		}

		protected override int CalCostG(ProcessingNode prevNode, ProcessingNode currentNode, AStarContext context)
		{
			Grid2DNode pre = (Grid2DNode)(prevNode.astarNode);
			Grid2DNode cur = (Grid2DNode)(currentNode.astarNode);
			int dx = Math.Abs(pre.x - cur.x);
			int dy = Math.Abs(pre.y - cur.y);
			int dist = dx > dy ? 14 * dy + 10 * (dx - dy) : 14 * dx + 10 * (dy - dx);
			return prevNode.g + dist;
		}

		protected override int CalCostH(ProcessingNode node, AStarContext context)
		{
			Grid2DNode endNode = context.GetTargetNode().astarNode as Grid2DNode;
			Grid2DNode nd = (Grid2DNode)(node.astarNode);
			int dx = Math.Abs(endNode.x - nd.x);
			int dy = Math.Abs(endNode.y - nd.y);
			int dist = dx > dy ? 14 * dy + 10 * (dx - dy) : 14 * dx + 10 * (dy - dx);
			return dist;
		}

	}


}