

namespace AStar
{
	public abstract class AStarPathPlanner
	{
		private AStarNode openList;

		private AStarNode closedList;

		protected AStarMap map;


		public void Setup(AStarMap map)
		{
			this.map = map;
		}

		protected AStarNode DoAStar(AStarNode startNode)
		{
			openList = null;
			closedList = null;

			startNode.g = 0;
			startNode.h = CalCostH(startNode);
			startNode.f = startNode.g + startNode.h;
			startNode.prev = null;
			startNode.next = null;

			AddToOpenList(startNode);

			AStarNode arriveNode = null;

			int count = 0;
			while (openList != null)
			{
				count++;
				if (count > 10000)
				{
					UnityEngine.Debug.LogError("AStarPathPlanner : too many nodes....");
					break;
				}
				AStarNode curNode = openList;
				if (CheckArrived(curNode))
				{
					arriveNode = curNode;
					break;
				}
				EvaluateAllNeighbours(curNode);
				RemoveFromOpenList(curNode);
				AddToClosedList(curNode);
			}

			return arriveNode;
		}

		protected abstract bool CheckArrived(AStarNode node);

		protected abstract int CalCostG(AStarNode prevNode, AStarNode currentNode);

		protected abstract int CalCostH(AStarNode node);

		private void EvaluateAllNeighbours(AStarNode node)
		{
			int neighbourCount = map.GetNeighbourNodeCount(node);
			for (int i = 0; i < neighbourCount; ++i)
			{
				AStarNode neighbour = map.GetNeighbourNode(node, i);
				if (neighbour != null)
					EvaluateNeighbour(node, neighbour);
			}
		}

		private void EvaluateNeighbour(AStarNode currentNode, AStarNode neighbourNode)
		{
			int g = CalCostG(currentNode, neighbourNode);
			int h = CalCostH(neighbourNode);
			int f = g + h;

			AStarNode findNode = FindInOpenList(neighbourNode);
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
				findNode = FindInClosedList(neighbourNode);
				if (findNode != null)
				{
					if (f < findNode.f)
					{
						RemoveFromClosedList(findNode);
						findNode.g = g;
						findNode.h = h;
						findNode.f = f;
						findNode.prev = currentNode;
						AddToOpenList(findNode);
					}
				}
				else
				{
					AStarNode newNode = neighbourNode;
					newNode.g = g;
					newNode.h = h;
					newNode.f = f;
					newNode.prev = currentNode;
					AddToOpenList(newNode);
				}
			}

		}

		private void AddToOpenList(AStarNode node)
		{
			if (openList == null)
			{
				openList = node;
				node.next = null;
			}
			else
			{
				AStarNode prevNode = null;
				AStarNode curNode = openList;
				while (curNode != null)
				{
					if (node.f < curNode.f)
					{
						node.next = curNode;
						if (prevNode != null)
							prevNode.next = node;
						else
							openList = node;
						break;
					}
					else if (curNode.next == null)
					{
						curNode.next = node;
						node.next = null;
						break;
					}
					prevNode = curNode;
					curNode = curNode.next;
				}
			}
		}

		private void AddToClosedList(AStarNode node)
		{
			if (closedList == null)
			{
				closedList = node;
				node.next = null;
			}
			else
			{
				AStarNode prevNode = null;
				AStarNode curNode = closedList;
				while (curNode != null)
				{
					if (node.f < curNode.f)
					{
						node.next = curNode;
						if (prevNode != null)
							prevNode.next = node;
						else
							closedList = node;
						break;
					}
					else if (curNode.next == null)
					{
						curNode.next = node;
						node.next = null;
						break;
					}
					prevNode = curNode;
					curNode = curNode.next;
				}
			}
		}

		private void RemoveFromOpenList(AStarNode node)
		{
			if (openList != null)
			{
				AStarNode prevNode = null;
				AStarNode curNode = openList;
				while (curNode != null)
				{
					if (node.id == curNode.id)
					{
						if (prevNode != null)
							prevNode.next = curNode.next;
						else
							openList = curNode.next;
						curNode.next = null;
						break;
					}
					prevNode = curNode;
					curNode = curNode.next;
				}
			}
		}

		private void RemoveFromClosedList(AStarNode node)
		{
			if (closedList != null)
			{
				AStarNode prevNode = null;
				AStarNode curNode = closedList;
				while (curNode != null)
				{
					if (node.id == curNode.id)
					{
						if (prevNode != null)
							prevNode.next = curNode.next;
						else
							closedList = curNode.next;
						curNode.next = null;
						break;
					}
					prevNode = curNode;
					curNode = curNode.next;
				}
			}
		}

		private AStarNode FindInOpenList(AStarNode node)
		{
			if (openList == null)
			{
				return null;
			}
			else
			{
				AStarNode curNode = openList;
				while (curNode != null)
				{
					if (curNode.id == node.id)
						return curNode;
					
					curNode = curNode.next;
				}

				return null;
			}
		}

		private AStarNode FindInClosedList(AStarNode node)
		{
			if (closedList == null)
			{
				return null;
			}
			else
			{
				AStarNode curNode = closedList; 
				while (curNode != null)
				{
					if (curNode.id == node.id)
						return curNode;
					
					curNode = curNode.next;
				}

				return null;
			}
		}

	}


}