

namespace AStar
{

	public abstract class AStarMap : Graph.SparseGraph
	{
		public abstract int GetNeighbourNodeCount(AStarNode node);

		public abstract AStarNode GetNeighbourNode(AStarNode node, int index);
	}

}