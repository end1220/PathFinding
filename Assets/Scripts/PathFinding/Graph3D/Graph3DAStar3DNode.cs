
using UnityEngine;
using TwFramework;
using TwGame;


namespace TwGame.AStar
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