
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

		public int GetVertexCount()
		{
			return 3;
		}

		public int GetVertexIndex(Int3 vertex)
		{
			return vertex == v0 ? 0 : (vertex == v1 ? 1 : 2);
		}

		public Int3 GetVertex(int i)
		{
			return i == 0 ? v0 : (i == 1 ? v1 : v2);
		}


		public bool IsVertex(Int3 p, out int index)
		{
			index = -1;
			if (v0.IsEqualXZ(ref p))
			{
				index = 0;
			}
			else if (v1.IsEqualXZ(ref p))
			{
				index = 1;
			}
			else if (v2.IsEqualXZ(ref p))
			{
				index = 2;
			}
			return (index != -1);
		}


		public NavMeshNode GetNeighborByEdge(int edge, out int otherEdge)
		{
			otherEdge = -1;
			if (((edge < 0) || (edge > 2)) || (this.connections == null))
			{
				return null;
			}
			
			Int3 vertex0 = this.GetVertex(edge % 3);
			Int3 vertex1 = this.GetVertex((edge + 1) % 3);
			for (int i = 0; i < connections.Length; i++)
			{
				NavMeshNode node2 =  graph.GetNode(connections[i]) as NavMeshNode;
				if (node2 != null)
				{
					if ((node2.v1 == vertex0) && (node2.v0 == vertex1))
					{
						otherEdge = 0;
					}
					else if ((node2.v2 == vertex0) && (node2.v1 == vertex1))
					{
						otherEdge = 1;
					}
					else if ((node2.v0 == vertex0) && (node2.v2 == vertex1))
					{
						otherEdge = 2;
					}
					if (otherEdge != -1)
					{
						return node2;
					}
				}
			}
			return null;
		}

		public void GetPoints(out Vector3 a, out Vector3 b, out Vector3 c)
		{
			a = (Vector3)v0;
			b = (Vector3)v1;
			c = (Vector3)v2;
		}

		public void GetPoints(out Int3 a, out Int3 b, out Int3 c)
		{
			a = v0;
			b = v1;
			c = v2;
		}

		public bool GetPortal(NavMeshNode other, List<Int3> left, List<Int3> right)
		{
			int first = -1;

			int acount = 3;
			int bcount = 3;

			// find the shared edge
			for (int a = 0; a < acount; a++)
			{
				var va = GetVertex(a);
				var va2 = GetVertex((a + 1) % acount);
				for (int b = 0; b < bcount; b++)
				{
					if (va == other.GetVertex((b + 1) % bcount) && va2 == other.GetVertex(b))
					{
						first = a;
						a = acount;// to stop the outer loop
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


		public Int3 ClosestPointOnNode(Int3 p)
		{
			return Polygon.ClosestPointOnTriangle(v0, v1, v2, p);
		}


		public Vector3 ClosestPointOnNodeXZ(Vector3 p)
		{
			Vector2 closest = Polygon.ClosestPointOnTriangle(
				new Vector2(v0.x * Int3.PrecisionFactor, v0.z * Int3.PrecisionFactor),
				new Vector2(v1.x * Int3.PrecisionFactor, v1.z * Int3.PrecisionFactor),
				new Vector2(v2.x * Int3.PrecisionFactor, v2.z * Int3.PrecisionFactor),
				new Vector2(p.x, p.z)
				);

			return new Vector3(closest.x, p.y, closest.y);
		}

		public Int3 ClosestPointOnNodeXZ(Int3 p)
		{
			Int2 closest = Polygon.ClosestPointOnTriangle(
				new Int2(v0.x, v0.z),
				new Int2(v1.x, v1.z),
				new Int2(v2.x, v2.z),
				new Int2(p.x, p.z)
				);

			return new Int3(closest.x, p.y, closest.y);
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


		public bool Intersect(Int3 a, Int3 b, out Int3 va, out Int3 vb)
		{
			if (VectorMath.SegmentsIntersectXZ(v0, v1, a, b))
			{
				va = v0;
				vb = v1;
				return true;
			}
			if (VectorMath.SegmentsIntersectXZ(v1, v2, a, b))
			{
				va = v1;
				vb = v2;
				return true;
			}
			if (VectorMath.SegmentsIntersectXZ(v2, v0, a, b))
			{
				va = v2;
				vb = v0;
				return true;
			}
			va = Int3.zero;
			vb = Int3.zero;
			return false;
		}


		public int EdgeIntersect(Int3 a, Int3 b)
		{
			Int3 num;
			Int3 num2;
			Int3 num3;
			this.GetPoints(out num, out num2, out num3);
			if (VectorMath.SegmentsIntersectXZ(num, num2, a, b))
			{
				return 0;
			}
			if (VectorMath.SegmentsIntersectXZ(num2, num3, a, b))
			{
				return 1;
			}
			if (VectorMath.SegmentsIntersectXZ(num3, num, a, b))
			{
				return 2;
			}
			return -1;
		}

		private static Int3[] _staticVerts = new Int3[3];
		public int EdgeIntersect(Int3 a, Int3 b, int startEdge, int count)
		{
			Int3[] numArray = _staticVerts;
			this.GetPoints(out numArray[0], out numArray[1], out numArray[2]);
			for (int i = 0; i < count; i++)
			{
				int index = (startEdge + i) % 3;
				int num3 = (index + 1) % 3;
				if (VectorMath.SegmentsIntersectXZ(numArray[index], numArray[num3], a, b))
				{
					return index;
				}
			}
			return -1;
		}


		public int GetColinearEdge(Int3 a, Int3 b)
		{
			Int3 num;
			Int3 num2;
			Int3 num3;
			this.GetPoints(out num, out num2, out num3);
			if (VectorMath.IsColinearXZ(num, num2, a) && VectorMath.IsColinearXZ(num, num2, b))
			{
				return 0;
			}
			if (VectorMath.IsColinearXZ(num2, num3, a) && VectorMath.IsColinearXZ(num2, num3, b))
			{
				return 1;
			}
			if (VectorMath.IsColinearXZ(num3, num, a) && VectorMath.IsColinearXZ(num3, num, b))
			{
				return 2;
			}
			return -1;
		}

		public int GetColinearEdge(Int3 a, Int3 b, int startEdge, int count)
		{
			Int3[] numArray = _staticVerts;
			this.GetPoints(out numArray[0], out numArray[1], out numArray[2]);
			for (int i = 0; i < count; i++)
			{
				int index = (startEdge + i) % 3;
				int num3 = (index + 1) % 3;
				if (VectorMath.IsColinearXZ(numArray[index], numArray[num3], a) && VectorMath.IsColinearXZ(numArray[index], numArray[num3], b))
				{
					return index;
				}
			}
			return -1;
		}


	}

}