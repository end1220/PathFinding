
using System.Collections.Generic;


namespace AStar
{
	/// <summary>
	/// astar算法运行时的节点状态
	/// </summary>
	public class ProcessingNode
	{
		public AStarNode astarNode;

		public int g;

		public int h;

		public int f;

		public ProcessingNode prev;

		public ProcessingNode next;

		public ProcessingNode()
		{
			Reset();
		}

		public void Reset()
		{
			astarNode = null;
			g = 0;
			h = 0;
			f = 0;
			prev = null;
			next = null;
		}

	}


	/// <summary>
	/// 一次搜索过程的状态
	/// </summary>
	public class AStarContext
	{
		private AStarRequest currentRequest;

		public AStarRequest Request { get { return currentRequest; } }

		public ProcessingNode openList;

		public ProcessingNode closedList;

		public AStarMap map { get { return currentRequest.map; } }

		private ProcessingNode[] nodes = new ProcessingNode[0];

		public List<AStarNode> rawPathNodeCache = new List<AStarNode>();

		public List<Int3> rawPathPoints = new List<Int3>();

		public List<AStarNode> TracingNodes = new List<AStarNode>();


		public void SetRequest(AStarRequest request)
		{
			currentRequest = request;
			openList = null;
			closedList = null;
			for (int i = 0; i < nodes.Length; ++i)
				nodes[i].Reset();
		}


		public ProcessingNode GetStartNode()
		{
			return GetNode(currentRequest.fromNode);
		}


		public ProcessingNode GetTargetNode()
		{
			return GetNode(currentRequest.toNode);
		}


		public ProcessingNode GetNode(AStarNode node)
		{
			int index = node.id;
			if (index >= nodes.Length)
			{
				int newLength = System.Math.Max(1, (int)(index * 1.5f));
				ResizeNodes(newLength);
			}
			ProcessingNode proNode = nodes[index];
			proNode.astarNode = node;
			return proNode;
		}


		public void ResizeNodes(int size)
		{
			if (nodes.Length < size)
			{
				nodes = new ProcessingNode[size];
				for (int i = 0; i < size; ++i)
					nodes[i] = new ProcessingNode();
			}
		}


		public void AddToOpenList(ProcessingNode node)
		{
			if (openList == null)
			{
				openList = node;
				node.next = null;
			}
			else
			{
				ProcessingNode prevNode = null;
				ProcessingNode curNode = openList;
				while (curNode != null)
				{
					if (node.f < curNode.f || (node.f == curNode.f && node.g < curNode.g))
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


		public void AddToClosedList(ProcessingNode node)
		{
			if (closedList == null)
			{
				closedList = node;
				node.next = null;
			}
			else
			{
				ProcessingNode prevNode = null;
				ProcessingNode curNode = closedList;
				while (curNode != null)
				{
					if (node.f < curNode.f || (node.f == curNode.f && node.g < curNode.g))
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


		public void RemoveFromOpenList(ProcessingNode node)
		{
			if (openList != null)
			{
				ProcessingNode prevNode = null;
				ProcessingNode curNode = openList;
				while (curNode != null)
				{
					if (node.astarNode.id == curNode.astarNode.id)
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


		public void RemoveFromClosedList(ProcessingNode node)
		{
			if (closedList != null)
			{
				ProcessingNode prevNode = null;
				ProcessingNode curNode = closedList;
				while (curNode != null)
				{
					if (node.astarNode.id == curNode.astarNode.id)
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


		public ProcessingNode FindInOpenList(ProcessingNode node)
		{
			if (openList == null)
			{
				return null;
			}
			else
			{
				ProcessingNode curNode = openList;
				while (curNode != null)
				{
					if (curNode.astarNode.id == node.astarNode.id)
						return curNode;

					curNode = curNode.next;
				}

				return null;
			}
		}

		public ProcessingNode FindInClosedList(ProcessingNode node)
		{
			if (closedList == null)
			{
				return null;
			}
			else
			{
				ProcessingNode curNode = closedList;
				while (curNode != null)
				{
					if (curNode.astarNode.id == node.astarNode.id)
						return curNode;

					curNode = curNode.next;
				}

				return null;
			}
		}


	}

}