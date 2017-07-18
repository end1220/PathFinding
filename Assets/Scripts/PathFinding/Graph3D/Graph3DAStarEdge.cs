
using UnityEngine;
using Graph;


namespace AStar
{
	[System.Serializable]
	public class Graph3DAStarEdge : GraphEdge
	{
		public Graph3DAStarEdge(int from, int to, int cost) :
			base(from, to, cost)
		{
		}

		public Graph3DAStarEdge() :
			base(0, 0, 0)
		{
		}

	}

}