
using System;


namespace AStar
{

	public abstract class AStarNode : Graph.GraphNode
	{
		[NonSerialized]
		public int g;

		[NonSerialized]
		public int h;

		[NonSerialized]
		public int f;

		[NonSerialized]
		public AStarNode prev;

		[NonSerialized]
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