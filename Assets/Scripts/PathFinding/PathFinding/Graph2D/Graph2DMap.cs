

using System.Collections.Generic;
using AStar;
using Graph;


namespace PathFinding
{
	public class Graph2DMap : AStarMap, INavGraph
	{
		public void Init(INavData data) { }

		public bool IsPassable(FixVector3 position) { return false; }

		public FixVector3 RayCastForMoving(FixVector3 from, FixVector3 to, MoveType mov) { return from; }

		public FixVector3 SlideByObstacles(FixVector3 fromPos, FixVector3 oldTargetPos) { return oldTargetPos; }

		public override int GetNeighbourNodeCount(AStarNode node)
		{
			List<GraphEdge> edgeList = GetEdgeList(node.id);
			return edgeList != null ? edgeList.Count : 0;
		}

		public override AStarNode GetNeighbourNode(AStarNode node, int index)
		{
			List<GraphEdge> edgeList = GetEdgeList(node.id);
			return GetNode(edgeList[index].to) as AStarNode;
		}

	}
}

