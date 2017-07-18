
using UnityEngine;
using FixedPoint;
using FixedPoint;


namespace AStar
{
	[System.Serializable]
	public class Graph3DAStarNode : GraphAStarNode
	{
		public ushort z;

		public Int3 worldPosition;

		//public bool walkable;


		public Graph3DAStarNode(int id) :
			base(id)
		{
		}

		public Graph3DAStarNode()
		{
		}

	}

}