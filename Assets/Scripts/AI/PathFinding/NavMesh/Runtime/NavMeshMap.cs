
using System;
using System.Collections;
using System.Collections.Generic;
using TwGame.Graph;
using TwGame.AStar;


namespace TwGame.NavMesh
{
	public class NavMeshMap : AStarMap
	{
		public void Init(NavMeshData navData)
		{
			var nodeList = navData.nodeList;
			for (int i = 0; i < nodeList.Count; ++i)
			{
				this.AddNode(nodeList[i]);
			}
			var edgeList = navData.edgeList;
			for (int i = 0; i < edgeList.Count; ++i)
			{
				this.AddEdge(edgeList[i]);
			}
		}

		public override int GetNeighbourNodeCount(AStarNode node)
		{
			List<GraphEdge> edgeList = GetEdgeList(node.id);
			return edgeList != null ? edgeList.Count : 0;
		}

		public override AStarNode GetNeighbourNode(AStarNode node, int index)
		{
			List<GraphEdge> edgeList = GetEdgeList(node.id);
			return GetNodeByID(edgeList[index].to) as AStarNode;
		}

	}
}

