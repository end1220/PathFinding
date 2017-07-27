
using AStar;


namespace PathFinding
{
	public class Grid2DNode : AStarNode
	{
		public ushort x;

		public ushort y;

		public ushort blockValue;

		public byte terrainType;

		public Grid2DNode(int id) :
			base(id)
		{
			x = 0;
			y = 0;
		}
	}

}