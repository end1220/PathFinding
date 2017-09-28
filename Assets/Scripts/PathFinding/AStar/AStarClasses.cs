
using Graph;


namespace AStar
{

	public abstract class AStarNode : GraphNode
	{
		public AStarNode(int id)
		{
			this.id = id;
		}

		public AStarNode()
		{
			id = -1;
		}
	}


	public abstract class AStarMap : SparseGraph
	{
		public abstract int GetNeighbourNodeCount(AStarNode node);

		public abstract AStarNode GetNeighbourNode(AStarNode node, int index);
	}


	public class AStarRequest
	{
		public AStarNode fromNode;

		public AStarNode toNode;

		public AStarMap map;

		public AStarPathPlanner planner;


		public void SetData(AStarNode from, AStarNode to, AStarMap map, AStarPathPlanner planner)
		{
			fromNode = from;
			toNode = to;
			this.map = map;
			this.planner = planner;
		}
	}


}