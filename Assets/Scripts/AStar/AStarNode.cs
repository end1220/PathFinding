
using System;


namespace Lite.AStar
{

	public abstract class AStarNode : Lite.Graph.GraphNode
	{
		public int g;
		public int h;
		public int f;

		public AStarNode prev;
		public AStarNode next;

		public AStarNode(int id)
		{
			this.id = id;
		}

		public AStarNode()
		{
			this.id = -1;
		}

	}

}