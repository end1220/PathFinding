

namespace PathFinding
{
	public class Graph2DNode : AStar.AStarNode
	{
		public ushort x;
		public ushort y;

		public Graph2DNode(int id) :
			base(id)
		{
		}

		public Graph2DNode()
		{
		}

	}

}