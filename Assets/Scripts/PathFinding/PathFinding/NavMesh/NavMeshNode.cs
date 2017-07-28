
using System.Collections.Generic;


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

		public Int3 GetVertexIndex(int i)
		{
			return i == 0 ? v0 : (i == 1 ? v1 : v2);
		}

		public Int3 GetVertex(int i)
		{
			return i == 0 ? v0 : (i == 1 ? v1 : v2);
		}

		public bool GetPortal(NavMeshNode other, List<Int3> left, List<Int3> right)
		{
			int first = -1;
			int second = -1;

			int acount = 3;
			int bcount = 3;

			/** \todo Maybe optimize with pa=av-1 instead of modulus... */
			for (int a = 0; a < acount; a++)
			{
				var va = GetVertexIndex(a);
				for (int b = 0; b < bcount; b++)
				{
					if (va == other.GetVertexIndex((b + 1) % bcount) && GetVertexIndex((a + 1) % acount) == other.GetVertexIndex(b))
					{
						first = a;
						second = b;
						a = acount;
						break;
					}
				}
			}

			if (first != -1)
			{
				//All triangles should be clockwise so second is the rightmost vertex (seen from this node)
				left.Add(GetVertex(first));
				right.Add(GetVertex((first + 1) % acount));
				return true;
			}

			return false;
		}


	}

}