

namespace Lite.AStar
{

	public abstract class AStarNode : TwGame.Graph.GraphNode
	{
		public int g;
		public int h;
		public int f;
		public AStarNode prev;
		public AStarNode next;	// for linked list
		public int blockValue;

		public AStarNode(int id)
		{
			_reset();
			this.id = id;
		}

		public AStarNode()
		{
			_reset();
			this.id = -1;
		}

		private void _reset()
		{
			id = -1;
			g = h = f = 0;
			prev = null;
			next = null;
			blockValue = 0;
		}

		public virtual void Reset()
		{
			_reset();
		}

	}

}