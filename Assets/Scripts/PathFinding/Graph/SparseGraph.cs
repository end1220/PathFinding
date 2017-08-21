
using System;
using System.Collections;
using System.Collections.Generic;


namespace Graph
{
	using NodeList = List<GraphNode>;
	using NodeTable = Dictionary<int,GraphNode>;
	using EdgeList = List<GraphEdge>;
	using EdgeListTable = Dictionary<int,List<GraphEdge>>;

	public class SparseGraph
	{
		protected NodeTable nodeTable;
		protected EdgeListTable edgeListTable;

		private int nodeIdCounter;

		public SparseGraph()
		{
			nodeTable = new NodeTable();
			edgeListTable = new EdgeListTable();
			nodeIdCounter = 0;
		}

		public int GetNodeCount()
		{
			return nodeTable.Count;
		}

		public T AddNode<T>() where T : GraphNode, new()
		{
			T node = new T();
			node.id = nodeIdCounter++;
			node.graph = this;
			nodeTable.Add(node.id, node);
			edgeListTable.Add(node.id, new EdgeList());
			return node;
		}

		public void AddNode(GraphNode node)
		{
			node.graph = this;
			nodeTable.Add(node.id, node);
			edgeListTable.Add(node.id, new EdgeList());
		}

		public void RemoveNodeByID(int id)
		{
			for (int i = 0; i < nodeTable.Count; ++i)
			{
				if (nodeTable[i].id == id)
				{
					nodeTable.Remove(id);
					EdgeList edgeList = edgeListTable[id];
					for (int j = 0; j < edgeList.Count; ++j)
					{
						GraphEdge edge = edgeList[j];
						RemoveEdge(edge.to, edge.from);
					}
					edgeList.Clear();
					edgeListTable.Remove(id);
					break;
				}
			}

		}

		public NodeList GetNodeList()
		{
			return new NodeList(nodeTable.Values);
		}

		public GraphNode GetNode(int id)
		{
			GraphNode node = null;
			nodeTable.TryGetValue(id, out node);
			return node;
		}

		public void AddEdge(int fromId, int toId, int cost)
		{
			if (IsNodeIDValid(fromId) && IsNodeIDValid(toId) && !IsEdgePresent(fromId, toId))
			{
				GraphEdge edge = new GraphEdge(fromId, toId, cost);
				edgeListTable[fromId].Add(edge);
			}
		}

		public void AddEdge(GraphEdge edge)
		{
			if (IsNodeIDValid(edge.from) && IsNodeIDValid(edge.to) && !IsEdgePresent(edge.from, edge.to))
			{
				edgeListTable[edge.from].Add(edge);
			}
		}

		public void RemoveEdge(int fromId, int toId)
		{
			if (edgeListTable.ContainsKey(fromId))
			{
				EdgeList edgeList = edgeListTable[fromId];
				for (int i = 0; i < edgeList.Count; ++i)
				{
					if (edgeList[i].to == toId)
					{
						edgeList.RemoveAt(i);
						break;
					}
				}
			}
		}

		public EdgeList GetEdgeList(int fromId)
		{
			EdgeList list = null;
			edgeListTable.TryGetValue(fromId, out list);
			return list;
		}

		public GraphEdge GetEdge(int fromId, int toId)
		{
			if (edgeListTable.ContainsKey(fromId))
			{
				EdgeList edgeList = edgeListTable[fromId];
				for (int i = 0; i < edgeList.Count; ++i)
				{
					if (edgeList[i].to == toId)
						return edgeList[i];
				}
			}
			return null;
		}

		public void SetEdgeCost(int fromId, int toId, int cost)
		{
			GraphEdge edge = GetEdge(fromId, toId);
			if (edge != null)
				edge.cost = cost;
		}

		public bool IsNodePresent(int id)
		{
			return IsNodeIDValid(id);
		}

		public bool IsEdgePresent(int fromId, int toId)
		{
			if (edgeListTable.ContainsKey(fromId))
			{
				EdgeList edgeList = edgeListTable[fromId];
				for (int i = 0; i < edgeList.Count; ++i)
				{
					if (edgeList[i].to == toId)
						return true;
				}
			}
			return false;
		}

		private bool IsNodeIDValid(int id)
		{
			return nodeTable.ContainsKey(id);
		}

	}

}