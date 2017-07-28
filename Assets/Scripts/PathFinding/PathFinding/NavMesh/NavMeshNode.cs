

namespace PathFinding
{
	[System.Serializable]
	public class NavMeshNode : AStar.AStarNode
	{
		public Int3 v0 = Int3.zero;

		public Int3 v1 = Int3.zero;

		public Int3 v2 = Int3.zero;

		public int Penalty;

		public bool Walkable;

		public Int3 position;

		public int[] connections;

		public int[] connectionCosts;


		public NavMeshNode(int id) :
			base(id)
		{
		}


		public void UpdatePositionFromVertices()
		{
			position = (v0 + v1 + v2) * 0.333333f;
		}


		public int GetConnectionCost(int connectId)
		{
			for (int i = 0; i < connections.Length; ++i)
			{
				if (connections[i] == connectId)
					return connectionCosts[i];
			}
			return 0;
		}

	}

}