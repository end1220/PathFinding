

namespace TwGame.AStar
{
	public class GridAStarNode : AStarNode
	{
		public int x;
		public int y;

		public int blockValue;

		public GridAStarNode(int id) :
			base(id)
		{
			x = 0;
			y = 0;
		}
	}

}