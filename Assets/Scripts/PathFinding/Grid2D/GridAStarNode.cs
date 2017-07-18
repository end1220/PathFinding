

namespace AStar
{
	public class GridAStarNode : AStarNode
	{
		public ushort x;

		public ushort y;

		public ushort blockValue;

		public byte terrainType;

		public GridAStarNode(int id) :
			base(id)
		{
			x = 0;
			y = 0;
		}
	}

}