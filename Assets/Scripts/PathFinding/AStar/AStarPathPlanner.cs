

namespace AStar
{
	public abstract class AStarPathPlanner
	{
		static bool enableTracing = false;

		public abstract bool Process(AStarContext handler);

		
		protected ProcessingNode DoAStar(AStarContext context)
		{
			if (enableTracing)
				context.TracingNodes.Clear();

			ProcessingNode startNode = context.GetStartNode();
			context.openList = null;
			context.closedList = null;

			startNode.g = 0;
			startNode.h = CalCostH(startNode, context);
			startNode.f = startNode.g + startNode.h;
			startNode.prev = null;
			startNode.next = null;

			context.AddToOpenList(startNode);

			ProcessingNode arriveNode = null;

			int count = 0;
			while (context.openList != null)
			{
				count++;
				if (count > 10000)
				{
					UnityEngine.Debug.LogError("AStarPathPlanner : too many nodes....");
					break;
				}
				ProcessingNode curNode = context.openList;
				if (CheckArrived(curNode, context))
				{
					arriveNode = curNode;
					break;
				}
				EvaluateAllNeighbours(curNode, context);
				context.RemoveFromOpenList(curNode);
				context.AddToClosedList(curNode);

				if (enableTracing)
					context.TracingNodes.Add(curNode.astarNode);
			}

			return arriveNode;
		}

		protected abstract bool CheckArrived(ProcessingNode node, AStarContext context);

		protected abstract int CalCostG(ProcessingNode prevNode, ProcessingNode currentNode, AStarContext context);

		protected abstract int CalCostH(ProcessingNode node, AStarContext context);

		private void EvaluateAllNeighbours(ProcessingNode node, AStarContext context)
		{
			int neighbourCount = context.map.GetNeighbourNodeCount(node.astarNode);
			for (int i = 0; i < neighbourCount; ++i)
			{
				AStarNode astarNode = context.map.GetNeighbourNode(node.astarNode, i);
				ProcessingNode neighbour = context.GetNode(astarNode);
				if (neighbour != null)
					EvaluateNeighbour(node, neighbour, context);
			}
		}

		private void EvaluateNeighbour(ProcessingNode currentNode, ProcessingNode neighbourNode, AStarContext context)
		{
			int g = CalCostG(currentNode, neighbourNode, context);
			int h = CalCostH(neighbourNode, context);
			int f = g + h;

			ProcessingNode findNode = context.FindInOpenList(neighbourNode);
			if (findNode != null)
			{
				if (f < findNode.f)
				{
					findNode.g = g;
					findNode.h = h;
					findNode.f = f;
					findNode.prev = currentNode;
				}
			}
			else
			{
				findNode = context.FindInClosedList(neighbourNode);
				if (findNode != null)
				{
					if (f < findNode.f)
					{
						context.RemoveFromClosedList(findNode);
						findNode.g = g;
						findNode.h = h;
						findNode.f = f;
						findNode.prev = currentNode;
						context.AddToOpenList(findNode);
					}
				}
				else
				{
					ProcessingNode newNode = neighbourNode;
					newNode.g = g;
					newNode.h = h;
					newNode.f = f;
					newNode.prev = currentNode;
					context.AddToOpenList(newNode);
				}
			}

		}


	}


}