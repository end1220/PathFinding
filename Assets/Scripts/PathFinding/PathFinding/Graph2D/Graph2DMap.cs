

using System.Collections.Generic;
using AStar;
using Graph;


namespace PathFinding
{
	public class Graph2DMap : AStarMap
	{
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

