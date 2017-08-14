
using System.Collections.Generic;
using UnityEngine;



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
			//int second = -1;

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
						//second = b;
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


		public Vector3 ClosestPointOnNode(Vector3 p)
		{
			return Polygon.ClosestPointOnTriangle((Vector3)v0, (Vector3)v1, (Vector3)v2, p);
		}


		public Vector3 ClosestPointOnNodeXZ(Vector3 p)
		{
			Int3 tp1 = v0;
			Int3 tp2 = v1;
			Int3 tp3 = v2;

			Vector2 closest = Polygon.ClosestPointOnTriangle(
				new Vector2(tp1.x * Int3.PrecisionFactor, tp1.z * Int3.PrecisionFactor),
				new Vector2(tp2.x * Int3.PrecisionFactor, tp2.z * Int3.PrecisionFactor),
				new Vector2(tp3.x * Int3.PrecisionFactor, tp3.z * Int3.PrecisionFactor),
				new Vector2(p.x, p.z)
				);

			return new Vector3(closest.x, p.y, closest.y);
		}


		public bool ContainsPoint(Int3 p)
		{
			Int3 a = v0;
			Int3 b = v1;
			Int3 c = v2;

			if ((long)(b.x - a.x) * (long)(p.z - a.z) - (long)(p.x - a.x) * (long)(b.z - a.z) > 0) return false;

			if ((long)(c.x - b.x) * (long)(p.z - b.z) - (long)(p.x - b.x) * (long)(c.z - b.z) > 0) return false;

			if ((long)(a.x - c.x) * (long)(p.z - c.z) - (long)(p.x - c.x) * (long)(a.z - c.z) > 0) return false;

			return true;
		}


	}

}