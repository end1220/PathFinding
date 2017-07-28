

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


		public bool GetPortal(NavMeshNode _other, System.Collections.Generic.List<UnityEngine.Vector3> left, System.Collections.Generic.List<UnityEngine.Vector3> right, bool backwards)
		{
			int aIndex, bIndex;

			return GetPortal(_other, left, right, backwards, out aIndex, out bIndex);
		}

		public bool GetPortal(NavMeshNode _other, System.Collections.Generic.List<UnityEngine.Vector3> left, System.Collections.Generic.List<UnityEngine.Vector3> right, bool backwards, out int aIndex, out int bIndex)
		{
			aIndex = -1;
			bIndex = -1;

			//If the nodes are in different graphs, this function has no idea on how to find a shared edge.
			//if (_other.GraphIndex != GraphIndex) return false;

			// Since the nodes are in the same graph, they are both TriangleMeshNodes
			// So we don't need to care about other types of nodes
			var other = _other as NavMeshNode;

			//Get tile indices
			int tileIndex = (GetVertexIndex(0) >> RecastGraph.TileIndexOffset) & RecastGraph.TileIndexMask;
			int tileIndex2 = (other.GetVertexIndex(0) >> RecastGraph.TileIndexOffset) & RecastGraph.TileIndexMask;

			//When the nodes are in different tiles, the edges might not be completely identical
			//so another technique is needed
			//Only do this on recast graphs
			if (tileIndex != tileIndex2 && (GetNavmeshHolder(GraphIndex) is RecastGraph))
			{
				for (int i = 0; i < connections.Length; i++)
				{
					if (connections[i].GraphIndex != GraphIndex)
					{
#if !ASTAR_NO_POINT_GRAPH
						var mid = connections[i] as NodeLink3Node;
						if (mid != null && mid.GetOther(this) == other)
						{
							// We have found a node which is connected through a NodeLink3Node

							if (left != null)
							{
								mid.GetPortal(other, left, right, false);
								return true;
							}
						}
#endif
					}
				}

				//Get the tile coordinates, from them we can figure out which edge is going to be shared
				int x1, x2, z1, z2;
				int coord;
				INavmeshHolder nm = GetNavmeshHolder(GraphIndex);
				nm.GetTileCoordinates(tileIndex, out x1, out z1);
				nm.GetTileCoordinates(tileIndex2, out x2, out z2);

				if (System.Math.Abs(x1 - x2) == 1) coord = 0;
				else if (System.Math.Abs(z1 - z2) == 1) coord = 2;
				else throw new System.Exception("Tiles not adjacent (" + x1 + ", " + z1 + ") (" + x2 + ", " + z2 + ")");

				int av = GetVertexCount();
				int bv = other.GetVertexCount();

				//Try the X and Z coordinate. For one of them the coordinates should be equal for one of the two nodes' edges
				//The midpoint between the tiles is the only place where they will be equal

				int first = -1, second = -1;

				//Find the shared edge
				for (int a = 0; a < av; a++)
				{
					int va = GetVertex(a)[coord];
					for (int b = 0; b < bv; b++)
					{
						if (va == other.GetVertex((b + 1) % bv)[coord] && GetVertex((a + 1) % av)[coord] == other.GetVertex(b)[coord])
						{
							first = a;
							second = b;
							a = av;
							break;
						}
					}
				}

				aIndex = first;
				bIndex = second;

				if (first != -1)
				{
					Int3 a = GetVertex(first);
					Int3 b = GetVertex((first + 1) % av);

					//The coordinate which is not the same for the vertices
					int ocoord = coord == 2 ? 0 : 2;

					//When the nodes are in different tiles, they might not share exactly the same edge
					//so we clamp the portal to the segment of the edges which they both have.
					int mincoord = System.Math.Min(a[ocoord], b[ocoord]);
					int maxcoord = System.Math.Max(a[ocoord], b[ocoord]);

					mincoord = System.Math.Max(mincoord, System.Math.Min(other.GetVertex(second)[ocoord], other.GetVertex((second + 1) % bv)[ocoord]));
					maxcoord = System.Math.Min(maxcoord, System.Math.Max(other.GetVertex(second)[ocoord], other.GetVertex((second + 1) % bv)[ocoord]));

					if (a[ocoord] < b[ocoord])
					{
						a[ocoord] = mincoord;
						b[ocoord] = maxcoord;
					}
					else
					{
						a[ocoord] = maxcoord;
						b[ocoord] = mincoord;
					}

					if (left != null)
					{
						//All triangles should be clockwise so second is the rightmost vertex (seen from this node)
						left.Add((Vector3)a);
						right.Add((Vector3)b);
					}
					return true;
				}
			}
			else
			if (!backwards)
			{
				int first = -1;
				int second = -1;

				int av = GetVertexCount();
				int bv = other.GetVertexCount();

				/** \todo Maybe optimize with pa=av-1 instead of modulus... */
				for (int a = 0; a < av; a++)
				{
					int va = GetVertexIndex(a);
					for (int b = 0; b < bv; b++)
					{
						if (va == other.GetVertexIndex((b + 1) % bv) && GetVertexIndex((a + 1) % av) == other.GetVertexIndex(b))
						{
							first = a;
							second = b;
							a = av;
							break;
						}
					}
				}

				aIndex = first;
				bIndex = second;

				if (first != -1)
				{
					if (left != null)
					{
						//All triangles should be clockwise so second is the rightmost vertex (seen from this node)
						left.Add((Vector3)GetVertex(first));
						right.Add((Vector3)GetVertex((first + 1) % av));
					}
				}
				else
				{
					for (int i = 0; i < connections.Length; i++)
					{
						if (connections[i].GraphIndex != GraphIndex)
						{
#if !ASTAR_NO_POINT_GRAPH
							var mid = connections[i] as NodeLink3Node;
							if (mid != null && mid.GetOther(this) == other)
							{
								// We have found a node which is connected through a NodeLink3Node

								if (left != null)
								{
									mid.GetPortal(other, left, right, false);
									return true;
								}
							}
#endif
						}
					}
					return false;
				}
			}

			return true;
		}


	}

}