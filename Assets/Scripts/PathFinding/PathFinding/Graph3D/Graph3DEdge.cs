

namespace PathFinding
{
	[System.Serializable]
	public class Graph3DEdge : Graph.GraphEdge
	{
		public Graph3DEdge(int from, int to, int cost) :
			base(from, to, cost)
		{
		}

		public Graph3DEdge() :
			base(0, 0, 0)
		{
		}

	}

}