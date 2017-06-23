
using UnityEngine;
using TwGame.Graph;


namespace Lite.AStar
{
	[System.Serializable]
	public class GraphAStar3DEdge : GraphEdge
	{
		public GraphAStar3DEdge(int from, int to, int cost) :
			base(from, to, cost)
		{
		}

	}

}