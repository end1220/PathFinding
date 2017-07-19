
using AStar;


namespace PathFinding
{
	public class GraphAStarNode : AStarNode
	{
		public ushort x;
		public ushort y;

		public GraphAStarNode(int id) :
			base(id)
		{
		}

		public GraphAStarNode()
		{
		}

	}

}