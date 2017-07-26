
using AStar;


namespace PathFinding
{
	public class TriangleNode : AStarNode
	{
		public Int3 v0 = Int3.zero;

		public Int3 v1 = Int3.zero;

		public Int3 v2 = Int3.zero;

		public int Penalty;

		public bool Walkable;

		public Int3 position;

		public TriangleNode[] connections;

		public uint[] connectionCosts;


		public TriangleNode(int id) :
			base(id)
		{
		}


		public void UpdatePositionFromVertices()
		{
			position = (v0 + v1 + v2) * 0.333333f;
		}

	}

}