
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

		[HideInInspector]
		public List<Graph3DAStarNode> nodeList = new List<Graph3DAStarNode>();

		[HideInInspector]
		public List<Graph3DAStarEdge> edgeList = new List<Graph3DAStarEdge>();

		[HideInInspector]
		public Dictionary<int, Graph3DAStarNode> nodeDic = new Dictionary<int, Graph3DAStarNode>();

		private Graph3DAStarMap graphMap = new Graph3DAStarMap();


		public void Init(BuildConfig cfg)
		{
			this.buildConfig = cfg;
		}


		public void AddNode(Graph3DAStarNode node)
		{
			nodeList.Add(node);
			nodeDic.Add(node.id, node);
			graphMap.AddNode(node);
		}


		public void AddEdge(Graph3DAStarEdge edge)
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
