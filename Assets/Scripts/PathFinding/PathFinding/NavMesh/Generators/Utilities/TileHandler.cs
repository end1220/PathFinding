
using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.ClipperLib;
using Pathfinding.Poly2Tri;

namespace Pathfinding.Util
{
	public class TileHandler
	{
		RecastGraph _graph;
		List<TileType> tileTypes = new List<TileType>();

		Clipper clipper;
		int[] cached_int_array = new int[32];
		Dictionary<Int3, int> cached_Int3_int_dict = new Dictionary<Int3, int>();
		Dictionary<Int2, int> cached_Int2_int_dict = new Dictionary<Int2, int>();

		TileType[] activeTileTypes;
		int[] activeTileRotations;
		int[] activeTileOffsets;
		bool[] reloadedInBatch;

		bool isBatching;

		public RecastGraph graph
		{
			get
			{
				return _graph;
			}
		}

		public TileHandler(RecastGraph graph)
		{
			if (graph == null) throw new ArgumentNullException("graph");
			if (graph.GetTiles() == null) throw new ArgumentException("graph has no tiles. Please scan the graph before creating a TileHandler");
			activeTileTypes = new TileType[graph.tileXCount * graph.tileZCount];
			activeTileRotations = new int[activeTileTypes.Length];
			activeTileOffsets = new int[activeTileTypes.Length];
			reloadedInBatch = new bool[activeTileTypes.Length];

			this._graph = graph;
		}

		public int GetActiveRotation(Int2 p)
		{
			return activeTileRotations[p.x + p.y * _graph.tileXCount];
		}

		/** \deprecated */
		[System.Obsolete("Use the result from RegisterTileType instead")]
		public TileType GetTileType(int index)
		{
			throw new System.Exception("This method has been deprecated. Use the result from RegisterTileType instead.");
		}

		/** \deprecated */
		[System.Obsolete("Use the result from RegisterTileType instead")]
		public int GetTileTypeCount()
		{
			throw new System.Exception("This method has been deprecated. Use the result from RegisterTileType instead.");
		}

		public class TileType
		{
			Int3[] verts;
			int[] tris;
			Int3 offset;
			int lastYOffset;
			int lastRotation;
			int width;
			int depth;

			public int Width
			{
				get
				{
					return width;
				}
			}

			public int Depth
			{
				get
				{
					return depth;
				}
			}

			/** Matrices for rotation.
			 * Each group of 4 elements is a 2x2 matrix.
			 * The XZ position is multiplied by this.
			 * So
			 * \code
			 * //A rotation by 90 degrees clockwise, second matrix in the array
			 * (5,2) * ((0, 1), (-1, 0)) = (2,-5)
			 * \endcode
			 */
			private static readonly int[] Rotations = {
				1, 0,  //Identity matrix
				0, 1,

				0, 1,
				-1, 0,

				-1, 0,
				0, -1,

				0, -1,
				1, 0
			};

			public TileType(Int3[] sourceVerts, int[] sourceTris, Int3 tileSize, Int3 centerOffset, int width = 1, int depth = 1)
			{
				if (sourceVerts == null) throw new ArgumentNullException("sourceVerts");
				if (sourceTris == null) throw new ArgumentNullException("sourceTris");

				tris = new int[sourceTris.Length];
				for (int i = 0; i < tris.Length; i++) tris[i] = sourceTris[i];

				verts = new Int3[sourceVerts.Length];

				for (int i = 0; i < sourceVerts.Length; i++)
				{
					verts[i] = sourceVerts[i] + centerOffset;
				}

				offset = tileSize / 2;
				offset.x *= width;
				offset.z *= depth;
				offset.y = 0;

				for (int i = 0; i < sourceVerts.Length; i++)
				{
					verts[i] = verts[i] + offset;
				}

				lastRotation = 0;
				lastYOffset = 0;

				this.width = width;
				this.depth = depth;
			}

			/** Create a new TileType.
			 * First all vertices of the source mesh are offseted by the \a centerOffset.
			 * The source mesh is assumed to be centered (after offsetting). Corners of the tile should be at tileSize*0.5 along all axes. When width or depth is not 1,
			 * the tileSize param should not change, but corners of the tile are assumed to lie further out.
			 *
			 * \param source The navmesh as a unity Mesh
			 * \param width The number of base tiles this tile type occupies on the x-axis
			 * \param depth The number of base tiles this tile type occupies on the z-axis
			 * \param tileSize Size of a single tile, the y-coordinate will be ignored.
			 */
			public TileType(Mesh source, Int3 tileSize, Int3 centerOffset, int width = 1, int depth = 1)
			{
				if (source == null) throw new ArgumentNullException("source");

				Vector3[] vectorVerts = source.vertices;
				tris = source.triangles;
				verts = new Int3[vectorVerts.Length];

				for (int i = 0; i < vectorVerts.Length; i++)
				{
					verts[i] = (Int3)vectorVerts[i] + centerOffset;
				}

				offset = tileSize / 2;
				offset.x *= width;
				offset.z *= depth;
				offset.y = 0;

				for (int i = 0; i < vectorVerts.Length; i++)
				{
					verts[i] = verts[i] + offset;
				}

				lastRotation = 0;
				lastYOffset = 0;

				this.width = width;
				this.depth = depth;
			}

			/** Load a tile, result given by the vert and tris array.
			 * \warning For performance and memory reasons, the returned arrays are internal arrays, so they must not be modified in any way or
			 * subsequent calls to Load may give corrupt output. The contents of the verts array is only valid until the next call to Load since
			 * different rotations and y offsets can be applied.
			 * If you need persistent arrays, please copy the returned ones.
			 */
			public void Load(out Int3[] verts, out int[] tris, int rotation, int yoffset)
			{
				//Make sure it is a number 0 <= x < 4
				rotation = ((rotation % 4) + 4) % 4;

				//Figure out relative rotation (relative to previous rotation that is, since that is still applied to the verts array)
				int tmp = rotation;
				rotation = (rotation - (lastRotation % 4) + 4) % 4;
				lastRotation = tmp;

				verts = this.verts;

				int relYOffset = yoffset - lastYOffset;
				lastYOffset = yoffset;

				if (rotation != 0 || relYOffset != 0)
				{
					for (int i = 0; i < verts.Length; i++)
					{
						Int3 op = verts[i] - offset;
						Int3 p = op;
						p.y += relYOffset;
						p.x = op.x * Rotations[rotation * 4 + 0] + op.z * Rotations[rotation * 4 + 1];
						p.z = op.x * Rotations[rotation * 4 + 2] + op.z * Rotations[rotation * 4 + 3];
						verts[i] = p + offset;
					}
				}

				tris = this.tris;
			}
		}

		/** Register that a tile can be loaded from \a source.
		 *
		 *
		 *
		 * \param centerOffset Assumes that the mesh has its pivot point at the center of the tile.
		 * If it has not, you can supply a non-zero \a centerOffset to offset all vertices.
		 *
		 * \param width width of the tile. In base tiles, not world units.
		 * \param depth depth of the tile. In base tiles, not world units.
		 * \param source Source mesh, must be readable.
		 *
		 * \returns Identifier for loading that tile type
		 */
		public TileType RegisterTileType(Mesh source, Int3 centerOffset, int width = 1, int depth = 1)
		{
			return new TileType(source, new Int3(graph.tileSizeX, 1, graph.tileSizeZ) * (Int3.Precision * graph.cellSize), centerOffset, width, depth);
		}

		public void CreateTileTypesFromGraph()
		{
			RecastGraph.NavmeshTile[] tiles = graph.GetTiles();
			if (tiles == null || tiles.Length != graph.tileXCount * graph.tileZCount)
			{
				throw new InvalidOperationException("Graph tiles are invalid (either null or number of tiles is not equal to width*depth of the graph");
			}

			for (int z = 0; z < graph.tileZCount; z++)
			{
				for (int x = 0; x < graph.tileXCount; x++)
				{
					RecastGraph.NavmeshTile tile = tiles[x + z * graph.tileXCount];
					UpdateTileType(tile);
				}
			}
		}

		void UpdateTileType(RecastGraph.NavmeshTile tile)
		{
			int x = tile.x;
			int z = tile.z;

			Bounds b = graph.GetTileBounds(x, z);
			var min = (Int3)b.min;
			Int3 size = new Int3(graph.tileSizeX, 1, graph.tileSizeZ) * (Int3.Precision * graph.cellSize);

			min += new Int3(size.x * tile.w / 2, 0, size.z * tile.d / 2);
			min = -min;

			var tp = new TileType(tile.verts, tile.tris, size, min, tile.w, tile.d);

			int index = x + z * graph.tileXCount;
			activeTileTypes[index] = tp;
			activeTileRotations[index] = 0;
			activeTileOffsets[index] = 0;
		}


		const int CUT_ALL = 0;
		const int CUT_DUAL = CUT_ALL + 1;
		const int CUT_BREAK = CUT_DUAL + 1;

		[Flags]
		public enum CutMode
		{
			CutAll = 1,
			CutDual = 2,
			CutExtra = 4
		}


		/** Refine a mesh using delaunay refinement.
		 * Loops through all pairs of neighbouring triangles and check if it would be better to flip the diagonal joining them
		 * using the delaunay criteria.
		 *
		 * Does not require triangles to be clockwise, triangles will be checked for if they are clockwise and made clockwise if not.
		 * The resulting mesh will have all triangles clockwise.
		 */
		void DelaunayRefinement(Int3[] verts, int[] tris, ref int vCount, ref int tCount, bool delaunay, bool colinear, Int3 worldOffset)
		{
			if (tCount % 3 != 0) throw new Exception("Triangle array length must be a multiple of 3");

			Dictionary<Int2, int> lookup = cached_Int2_int_dict;
			lookup.Clear();

			for (int i = 0; i < tCount; i += 3)
			{
				if (!VectorMath.IsClockwiseXZ(verts[tris[i]], verts[tris[i + 1]], verts[tris[i + 2]]))
				{
					int tmp = tris[i];
					tris[i] = tris[i + 2];
					tris[i + 2] = tmp;
				}

				lookup[new Int2(tris[i + 0], tris[i + 1])] = i + 2;
				lookup[new Int2(tris[i + 1], tris[i + 2])] = i + 0;
				lookup[new Int2(tris[i + 2], tris[i + 0])] = i + 1;
			}

			const int maxError = 3 * 3;//(int)((graph.contourMaxError*Int3.Precision)*(graph.contourMaxError*Int3.Precision));

			for (int i = 0; i < tCount; i += 3)
			{
				for (int j = 0; j < 3; j++)
				{
					int opp;
					//Debug.DrawLine ((Vector3)verts[tris[i+((j+1)%3)]], (Vector3)verts[tris[i+((j+0)%3)]], Color.yellow);

					if (lookup.TryGetValue(new Int2(tris[i + ((j + 1) % 3)], tris[i + ((j + 0) % 3)]), out opp))
					{
						Int3 po = verts[tris[i + ((j + 2) % 3)]];
						Int3 pr = verts[tris[i + ((j + 1) % 3)]];
						Int3 pl = verts[tris[i + ((j + 3) % 3)]];

						//Debug.DrawLine (pr, pl, Color.red);
						//Debug.DrawLine (po, (pl+pr)/2, Color.blue);


						Int3 popp = verts[tris[opp]];

						//continue;

						po.y = 0;
						pr.y = 0;
						pl.y = 0;
						popp.y = 0;

						bool noDelaunay = false;

						if (!VectorMath.RightOrColinearXZ(po, pl, popp) || VectorMath.RightXZ(po, pr, popp))
						{
							//Debug.DrawLine (po, popp, Color.red);
							if (colinear)
							{
								noDelaunay = true;
							}
							else
							{
								continue;
							}
						}

						if (colinear)
						{
							// Check if op - right shared - opposite in other - is colinear
							// and if the edge right-op is not shared and if the edge opposite in other - right shared is not shared
							if (VectorMath.SqrDistancePointSegmentApproximate(po, popp, pr) < maxError &&
								!lookup.ContainsKey(new Int2(tris[i + ((j + 2) % 3)], tris[i + ((j + 1) % 3)])) &&
								!lookup.ContainsKey(new Int2(tris[i + ((j + 1) % 3)], tris[opp])))
							{
								//Debug.DrawLine ((Vector3)(po+worldOffset), (Vector3)(pr+worldOffset), Color.red);
								//Debug.DrawLine ((Vector3)(pr+worldOffset), (Vector3)(popp+worldOffset), Color.blue);
								//Debug.Break();

								tCount -= 3;

								int root = (opp / 3) * 3;

								// Move right vertex to the other triangle's opposite
								tris[i + ((j + 1) % 3)] = tris[opp];

								// Move last triangle to delete
								if (root != tCount)
								{
									tris[root + 0] = tris[tCount + 0];
									tris[root + 1] = tris[tCount + 1];
									tris[root + 2] = tris[tCount + 2];
									lookup[new Int2(tris[root + 0], tris[root + 1])] = root + 2;
									lookup[new Int2(tris[root + 1], tris[root + 2])] = root + 0;
									lookup[new Int2(tris[root + 2], tris[root + 0])] = root + 1;

									tris[tCount + 0] = 0;
									tris[tCount + 1] = 0;
									tris[tCount + 2] = 0;
								}
								else
								{
									tCount += 3;
								}

								// Since the above mentioned edges are not shared, we don't need to bother updating them

								// However some need to be updated
								// left - new right (previously opp) should have opposite vertex po
								//lookup[new Int2(tris[i+((j+3)%3)],tris[i+((j+1)%3)])] = i+((j+2)%3);

								lookup[new Int2(tris[i + 0], tris[i + 1])] = i + 2;
								lookup[new Int2(tris[i + 1], tris[i + 2])] = i + 0;
								lookup[new Int2(tris[i + 2], tris[i + 0])] = i + 1;
								continue;
							}
						}
						if (delaunay && !noDelaunay)
						{
							float beta = Int3.Angle(pr - po, pl - po);
							float alpha = Int3.Angle(pr - popp, pl - popp);

							if (alpha > (2 * Mathf.PI - 2 * beta))
							{
								//Debug.DrawLine (po, popp, Color.green);
								//Denaunay condition not holding, refine please
								tris[i + ((j + 1) % 3)] = tris[opp];

								int root = (opp / 3) * 3;
								int off = opp - root;
								tris[root + ((off - 1 + 3) % 3)] = tris[i + ((j + 2) % 3)];

								lookup[new Int2(tris[i + 0], tris[i + 1])] = i + 2;
								lookup[new Int2(tris[i + 1], tris[i + 2])] = i + 0;
								lookup[new Int2(tris[i + 2], tris[i + 0])] = i + 1;

								lookup[new Int2(tris[root + 0], tris[root + 1])] = root + 2;
								lookup[new Int2(tris[root + 1], tris[root + 2])] = root + 0;
								lookup[new Int2(tris[root + 2], tris[root + 0])] = root + 1;
							}
						}
					}
				}
			}
		}

		Vector3 Point2D2V3(Poly2Tri.TriangulationPoint p)
		{
			return new Vector3((float)p.X, 0, (float)p.Y) * Int3.PrecisionFactor;
		}

		Int3 IntPoint2Int3(IntPoint p)
		{
			return new Int3((int)p.X, 0, (int)p.Y);
		}

		
		/** Returns a new array with at most length \a newLength.
		 * The array will a copy of all elements of \a arr.
		 */
		protected static T[] ShrinkArray<T>(T[] arr, int newLength)
		{
			newLength = Math.Min(newLength, arr.Length);
			var arr2 = new T[newLength];

			// Unrolling
			if (newLength % 4 == 0)
			{
				for (int i = 0; i < newLength; i += 4)
				{
					arr2[i + 0] = arr[i + 0];
					arr2[i + 1] = arr[i + 1];
					arr2[i + 2] = arr[i + 2];
					arr2[i + 3] = arr[i + 3];
				}
			}
			else if (newLength % 3 == 0)
			{
				for (int i = 0; i < newLength; i += 3)
				{
					arr2[i + 0] = arr[i + 0];
					arr2[i + 1] = arr[i + 1];
					arr2[i + 2] = arr[i + 2];
				}
			}
			else if (newLength % 2 == 0)
			{
				for (int i = 0; i < newLength; i += 2)
				{
					arr2[i + 0] = arr[i + 0];
					arr2[i + 1] = arr[i + 1];
				}
			}
			else
			{
				for (int i = 0; i < newLength; i++)
				{
					arr2[i + 0] = arr[i + 0];
				}
			}
			return arr2;
		}

		
	}
}
