
using System;
using System.Collections.Generic;
using UnityEngine;
using PathFinding.Graph3d;


namespace PathFinding
{
	/// <summary>
	/// 存储着3D网格稀疏图寻路数据
	/// </summary>
	public class NavGraph3DData : ScriptableObject
	{
		[HideInInspector]
		public bool bytesMode = false;// 序列化图的节点和边数据是否使用bytes

		[HideInInspector]
		public BuildConfig buildConfig;

		[HideInInspector]
		public byte[] nodeBytes = null;

		[HideInInspector]
		public byte[] edgeBytes = null;

		public int nodeCount;

		public int edgeCount;

		[HideInInspector]
		public List<Graph3DNode> nodeList = new List<Graph3DNode>();

		[HideInInspector]
		public List<Graph3DEdge> edgeList = new List<Graph3DEdge>();
		
		[NonSerialized]
		public Dictionary<int, Graph3DNode> nodeDic = new Dictionary<int, Graph3DNode>();


		public int NodeSize
		{
			get
			{
				// int : id, int3.x.y.z  = 4 * 4
				// ushort: x y z  = 3 * 2
				return 22;
			}
		}

		public int EdgeSize
		{
			get
			{	// int: from, to, cost  = 3 * 4
				return 12;
			}
		}


#if UNITY_EDITOR

		public void Init(BuildConfig cfg)
		{
			this.buildConfig = cfg;
		}


		public void AddNode(Graph3DNode node)
		{
			nodeList.Add(node);
			nodeDic.Add(node.id, node);
			nodeCount = Math.Max(nodeCount, node.id);
		}


		public void AddEdge(Graph3DEdge edge)
		{
			edgeList.Add(edge);
		}


		public void SaveBytes()
		{
			nodeCount = nodeList.Count;
			edgeCount = edgeList.Count;

			if (bytesMode)
			{
				nodeBytes = new byte[nodeList.Count * NodeSize];
				edgeBytes = new byte[edgeList.Count * EdgeSize];

				for (int i = 0; i < nodeList.Count; ++i)
				{
					var node = nodeList[i];
					int index = i * NodeSize;
					ByteTool.IntToBytes(nodeBytes, index, node.id);
					index += 4;
					ByteTool.UshortToBytes(nodeBytes, index, node.x);
					index += 2;
					ByteTool.UshortToBytes(nodeBytes, index, node.y);
					index += 2;
					ByteTool.UshortToBytes(nodeBytes, index, node.z);
					index += 2;
					ByteTool.IntToBytes(nodeBytes, index, node.worldPosition.x);
					index += 4;
					ByteTool.IntToBytes(nodeBytes, index, node.worldPosition.y);
					index += 4;
					ByteTool.IntToBytes(nodeBytes, index, node.worldPosition.z);
				}

				for (int i = 0; i < edgeList.Count; ++i)
				{
					var edge = edgeList[i];
					int index = i * EdgeSize;
					ByteTool.IntToBytes(edgeBytes, index, edge.from);
					index += 4;
					ByteTool.IntToBytes(edgeBytes, index, edge.to);
					index += 4;
					ByteTool.IntToBytes(edgeBytes, index, edge.cost);
				}

				nodeList.Clear();
				edgeList.Clear();
			}
			else
			{

			}
			
		}

#endif


		public void ReleaseMemory()
		{
			if (bytesMode)
			{
				nodeBytes = null;
				edgeBytes = null;
			}
			else
			{
				nodeList.Clear();
				edgeList.Clear();
			}
		}


		public Graph3DNode ParseNode(int index)
		{
			Graph3DNode node = new Graph3DNode();
			int bytesIndex = index * NodeSize;
			node.id = ByteTool.IntFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 4;
			node.x = ByteTool.UshortFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 2;
			node.y = ByteTool.UshortFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 2;
			node.z = ByteTool.UshortFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 2;
			node.worldPosition.x = ByteTool.IntFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 4;
			node.worldPosition.y = ByteTool.IntFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 4;
			node.worldPosition.z = ByteTool.IntFromBytes(nodeBytes, bytesIndex);

			return node;
		}

		public Graph3DEdge ParseEdge(int index)
		{
			Graph3DEdge edge = new Graph3DEdge();
			int bytesIndex = index * EdgeSize;
			edge.from = ByteTool.IntFromBytes(edgeBytes, bytesIndex);
			bytesIndex += 4;
			edge.to = ByteTool.IntFromBytes(edgeBytes, bytesIndex);
			bytesIndex += 4;
			edge.cost = ByteTool.IntFromBytes(edgeBytes, bytesIndex);
			
			return edge;
		}

	}
}
