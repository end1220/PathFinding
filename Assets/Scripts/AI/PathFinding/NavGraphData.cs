
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
	public class NavGraphData : ScriptableObject
	{
		public BuildConfig buildConfig;

		public List<GraphAStar3DNode> nodeList = new List<GraphAStar3DNode>();

		public List<GraphAStar3DEdge> edgeList = new List<GraphAStar3DEdge>();

		public Dictionary<int, GraphAStar3DNode> nodeDic = new Dictionary<int, GraphAStar3DNode>();

		private GraphAStar3DMap graphMap = new GraphAStar3DMap();


		public void Init(BuildConfig cfg)
		{
			this.buildConfig = cfg;
		}


		public void AddNode(GraphAStar3DNode node)
		{
			nodeList.Add(node);
			nodeDic.Add(node.id, node);
			graphMap.AddNode(node);
		}


		public void AddEdge(GraphAStar3DEdge edge)
		{
			edgeList.Add(edge);
			graphMap.AddEdge(edge);
		}


		public List<GraphEdge> GetEdgeList(int nodeId)
		{
			return graphMap.GetEdgeList(nodeId);
		}

	}
}
