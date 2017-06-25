
using UnityEngine;
using Lite;


namespace Lite.AStar
{
	[System.Serializable]
	public class GraphAStar3DNode : GraphAStarNode
	{
		public int z;

		public Int3 worldPosition;
		public bool walkable;


		public GraphAStar3DNode(int id) :
			base(id)
		{
		}

		public GraphAStar3DNode()
		{
		}

	}

}