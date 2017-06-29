
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lite.Graph;
using Lite.AStar.NavGraph;


namespace Lite.AStar
{
	/// <summary>
	/// 存储着3D网格稀疏图寻路数据
	/// </summary>
	public class NavGraph3DData : ScriptableObject
	{

		[HideInInspector]
		public BuildConfig buildConfig;
		//[HideInInspector]
		public byte[] nodeBytes = null;
		public byte[] edgeBytes = null;
		public int nodeCount;
		public int edgeCount;

#if UNITY_EDITOR

		[NonSerialized]
		public List<Graph3DAStarNode> nodeList = new List<Graph3DAStarNode>();
		[NonSerialized]
		public List<Graph3DAStarEdge> edgeList = new List<Graph3DAStarEdge>();
		[NonSerialized]
		public Dictionary<int, Graph3DAStarNode> nodeDic = new Dictionary<int, Graph3DAStarNode>();

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


		public void Init(BuildConfig cfg)
		{
			this.buildConfig = cfg;
		}


		public void AddNode(Graph3DAStarNode node)
		{
			nodeList.Add(node);
			nodeDic.Add(node.id, node);
			nodeCount = Math.Max(nodeCount, node.id);
		}


		public void AddEdge(Graph3DAStarEdge edge)
		{
			edgeList.Add(edge);
		}


		public void SaveBytes()
		{
			nodeCount = nodeList.Count;
			edgeCount = edgeList.Count;
			nodeBytes = new byte[nodeList.Count * NodeSize];
			edgeBytes = new byte[edgeList.Count * EdgeSize];

			for (int i = 0; i < nodeList.Count; ++i)
			{
				var node = nodeList[i];
				int index = i * NodeSize;
				IntToBytes(nodeBytes, index, node.id);
				index += 4;
				UshortToBytes(nodeBytes, index, node.x);
				index += 2;
				UshortToBytes(nodeBytes, index, node.y);
				index += 2;
				UshortToBytes(nodeBytes, index, node.z);
				index += 2;
				IntToBytes(nodeBytes, index, node.worldPosition.x);
				index += 4;
				IntToBytes(nodeBytes, index, node.worldPosition.y);
				index += 4;
				IntToBytes(nodeBytes, index, node.worldPosition.z);
			}

			for (int i = 0; i < edgeList.Count; ++i)
			{
				var edge = edgeList[i];
				int index = i * EdgeSize;
				IntToBytes(edgeBytes, index, edge.from);
				index += 4;
				IntToBytes(edgeBytes, index, edge.to);
				index += 4;
				IntToBytes(edgeBytes, index, edge.cost);
			}
		}

#endif

		public Graph3DAStarNode ParseNode(int index)
		{
			Graph3DAStarNode node = new Graph3DAStarNode();
			int bytesIndex = index * NodeSize;
			node.id = IntFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 4;
			node.x = UshortFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 2;
			node.y = UshortFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 2;
			node.z = UshortFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 2;
			node.worldPosition.x = IntFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 4;
			node.worldPosition.y = IntFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 4;
			node.worldPosition.z = IntFromBytes(nodeBytes, bytesIndex);

			return node;
		}

		public Graph3DAStarEdge ParseEdge(int index)
		{
			Graph3DAStarEdge edge = new Graph3DAStarEdge();
			int bytesIndex = index * EdgeSize;
			edge.from = IntFromBytes(edgeBytes, bytesIndex);
			bytesIndex += 4;
			edge.to = IntFromBytes(edgeBytes, bytesIndex);
			bytesIndex += 4;
			edge.cost = IntFromBytes(edgeBytes, bytesIndex);
			
			return edge;
		}

		private void IntToBytes(byte[] bt, int index, int value)
		{
			bt[index++] = (byte)(value >> 24);
			bt[index++] = (byte)(value >> 16);
			bt[index++] = (byte)(value >> 8);
			bt[index] = (byte)(value >> 0);
		}

		private int IntFromBytes(byte[] bt, int index)
		{
			int value = 0;
			value |= (int)bt[index++] << 24;
			value |= (int)bt[index++] << 16;
			value |= (int)bt[index++] << 8;
			value |= (int)bt[index] << 0;
			return value;
		}

		private void ShortToBytes(byte[] bt, int index, short value)
		{
			bt[index++] = (byte)(value >> 8);
			bt[index] = (byte)(value >> 0);
		}

		private short ShortFromBytes(byte[] bt, int index)
		{
			short value = 0;
			value |= (short)((int)bt[index++] << 8);
			value |= (short)((int)bt[index] << 0);
			return value;
		}

		private void UshortToBytes(byte[] bt, int index, ushort value)
		{
			bt[index++] = (byte)(value >> 8);
			bt[index] = (byte)(value >> 0);
		}

		private ushort UshortFromBytes(byte[] bt, int index)
		{
			ushort value = 0;
			value |= (ushort)((int)bt[index++] << 8);
			value |= (ushort)((int)bt[index] << 0);
			return value;
		}

	}
}
