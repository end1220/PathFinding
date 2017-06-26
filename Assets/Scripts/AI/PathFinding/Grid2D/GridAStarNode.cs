

namespace TwGame.AStar
{
	public class GridAStarNode : AStarNode
	{
		public int x;
		public int y;

		public GridAStarNode(int id) :
			base(id)
		{
			x = 0;
			y = 0;
		}
	}

}