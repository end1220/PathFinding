

namespace Lite.Graph
{
	public class GraphEdge
	{
		public int from;
		public int to;
		public int cost;

		public GraphEdge(int from, int to, int cost)
		{
			this.from = from;
			this.to = to;
			this.cost = cost;
		}
	}

}