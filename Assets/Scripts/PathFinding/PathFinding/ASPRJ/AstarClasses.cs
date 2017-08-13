using UnityEngine;
using Graph;



namespace PathFinding
{



	/** Nearest node constraint. Constrains which nodes will be returned by the GetNearest function */
	public class NNConstraint
	{
		/** Graphs treated as valid to search on.
		 * This is a bitmask meaning that bit 0 specifies whether or not the first graph in the graphs list should be able to be included in the search,
		 * bit 1 specifies whether or not the second graph should be included and so on.
		 * \code
		 * //Enables the first and third graphs to be included, but not the rest
		 * myNNConstraint.graphMask = (1 << 0) | (1 << 2);
		 * \endcode
		 * \note This does only affect which nodes are returned from a GetNearest call, if an invalid graph is linked to from a valid graph, it might be searched anyway.
		 *
		 * \see AstarPath.GetNearest */
		public int graphMask = -1;

		/** Only treat nodes in the area #area as suitable. Does not affect anything if #area is less than 0 (zero) */
		public bool constrainArea;

		/** Area ID to constrain to. Will not affect anything if less than 0 (zero) or if #constrainArea is false */
		public int area = -1;

		/** Only treat nodes with the walkable flag set to the same as #walkable as suitable */
		public bool constrainWalkability = true;

		/** What must the walkable flag on a node be for it to be suitable. Does not affect anything if #constrainWalkability if false */
		public bool walkable = true;

		/** if available, do an XZ check instead of checking on all axes. The RecastGraph supports this */
		public bool distanceXZ;

		/** Sets if tags should be constrained */
		public bool constrainTags = true;

		/** Nodes which have any of these tags set are suitable. This is a bitmask, i.e bit 0 indicates that tag 0 is good, bit 3 indicates tag 3 is good etc. */
		public int tags = -1;

		/** Constrain distance to node.
		 * Uses distance from AstarPath.maxNearestNodeDistance.
		 * If this is false, it will completely ignore the distance limit.
		 * \note This value is not used in this class, it is used by the AstarPath.GetNearest function.
		 */
		public bool constrainDistance = true;

		/** Returns whether or not the graph conforms to this NNConstraint's rules.
		 * Note that only the first 31 graphs are considered using this function.
		 * If the graphMask has bit 31 set (i.e the last graph possible to fit in the mask), all graphs
		 * above index 31 will also be considered suitable.
		 */
		/*public virtual bool SuitableGraph(int graphIndex, NavGraph graph)
		{
			return ((graphMask >> graphIndex) & 1) != 0;
		}*/

		/** Returns whether or not the node conforms to this NNConstraint's rules */
		public virtual bool Suitable(NavMeshNode node)
		{
			/*if (constrainWalkability && node.Walkable != walkable) return false;

			if (constrainArea && area >= 0 && node.Area != area) return false;

			if (constrainTags && ((tags >> (int)node.Tag) & 0x1) == 0) return false;*/

			return true;
		}

		/** The default NNConstraint.
		 * Equivalent to new NNConstraint ().
		 * This NNConstraint has settings which works for most, it only finds walkable nodes
		 * and it constrains distance set by A* Inspector -> Settings -> Max Nearest Node Distance */
		public static NNConstraint Default
		{
			get
			{
				return new NNConstraint();
			}
		}

		/** Returns a constraint which will not filter the results */
		public static NNConstraint None
		{
			get
			{
				var n = new NNConstraint();
				n.constrainWalkability = false;
				n.constrainArea = false;
				n.constrainTags = false;
				n.constrainDistance = false;
				n.graphMask = -1;
				return n;
			}
		}

		/** Default constructor. Equals to the property #Default */
		public NNConstraint()
		{
		}
	}


	public struct NNInfo
	{
		/** Closest node found.
		 * This node is not necessarily accepted by any NNConstraint passed.
		 * \see constrainedNode
		 */
		public NavMeshNode node;

		/** Optional to be filled in.
		 * If the search will be able to find the constrained node without any extra effort it can fill it in. */
		public NavMeshNode constrainedNode;

		/** The position clamped to the closest point on the #node.
		 */
		public Vector3 clampedPosition;
		/** Clamped position for the optional constrainedNode */
		public Vector3 constClampedPosition;

		public NNInfo(NavMeshNode node)
		{
			this.node = node;
			constrainedNode = null;
			clampedPosition = Vector3.zero;
			constClampedPosition = Vector3.zero;

			UpdateInfo();
		}

		/** Sets the constrained node */
		public void SetConstrained(NavMeshNode constrainedNode, Vector3 clampedPosition)
		{
			this.constrainedNode = constrainedNode;
			constClampedPosition = clampedPosition;
		}

		/** Updates #clampedPosition and #constClampedPosition from node positions */
		public void UpdateInfo()
		{
			clampedPosition = node != null ? (Vector3)node.position : Vector3.zero;
			constClampedPosition = constrainedNode != null ? (Vector3)constrainedNode.position : Vector3.zero;
		}

		public static explicit operator Vector3(NNInfo ob)
		{
			return ob.clampedPosition;
		}

		public static explicit operator NavMeshNode(NNInfo ob)
		{
			return ob.node;
		}

		public static explicit operator NNInfo(NavMeshNode ob)
		{
			return new NNInfo(ob);
		}
	}



	/** Integer Rectangle.
	 * Works almost like UnityEngine.Rect but with integer coordinates
	 */
	public struct IntRect
	{
		public int xmin, ymin, xmax, ymax;

		public IntRect(int xmin, int ymin, int xmax, int ymax)
		{
			this.xmin = xmin;
			this.xmax = xmax;
			this.ymin = ymin;
			this.ymax = ymax;
		}

		public bool Contains(int x, int y)
		{
			return !(x < xmin || y < ymin || x > xmax || y > ymax);
		}

		public int Width
		{
			get
			{
				return xmax - xmin + 1;
			}
		}

		public int Height
		{
			get
			{
				return ymax - ymin + 1;
			}
		}

		/** Returns if this rectangle is valid.
		 * An invalid rect could have e.g xmin > xmax.
		 * Rectamgles with a zero area area invalid.
		 */
		public bool IsValid()
		{
			return xmin <= xmax && ymin <= ymax;
		}

		public static bool operator ==(IntRect a, IntRect b)
		{
			return a.xmin == b.xmin && a.xmax == b.xmax && a.ymin == b.ymin && a.ymax == b.ymax;
		}

		public static bool operator !=(IntRect a, IntRect b)
		{
			return a.xmin != b.xmin || a.xmax != b.xmax || a.ymin != b.ymin || a.ymax != b.ymax;
		}

		public override bool Equals(System.Object _b)
		{
			var b = (IntRect)_b;

			return xmin == b.xmin && xmax == b.xmax && ymin == b.ymin && ymax == b.ymax;
		}

		public override int GetHashCode()
		{
			return xmin * 131071 ^ xmax * 3571 ^ ymin * 3109 ^ ymax * 7;
		}

		/** Returns the intersection rect between the two rects.
		 * The intersection rect is the area which is inside both rects.
		 * If the rects do not have an intersection, an invalid rect is returned.
		 * \see IsValid
		 */
		public static IntRect Intersection(IntRect a, IntRect b)
		{
			var r = new IntRect(
				System.Math.Max(a.xmin, b.xmin),
				System.Math.Max(a.ymin, b.ymin),
				System.Math.Min(a.xmax, b.xmax),
				System.Math.Min(a.ymax, b.ymax)
				);

			return r;
		}

		/** Returns if the two rectangles intersect each other
		 */
		public static bool Intersects(IntRect a, IntRect b)
		{
			return !(a.xmin > b.xmax || a.ymin > b.ymax || a.xmax < b.xmin || a.ymax < b.ymin);
		}

		/** Returns a new rect which contains both input rects.
		 * This rectangle may contain areas outside both input rects as well in some cases.
		 */
		public static IntRect Union(IntRect a, IntRect b)
		{
			var r = new IntRect(
				System.Math.Min(a.xmin, b.xmin),
				System.Math.Min(a.ymin, b.ymin),
				System.Math.Max(a.xmax, b.xmax),
				System.Math.Max(a.ymax, b.ymax)
				);

			return r;
		}

		/** Returns a new IntRect which is expanded to contain the point */
		public IntRect ExpandToContain(int x, int y)
		{
			var r = new IntRect(
				System.Math.Min(xmin, x),
				System.Math.Min(ymin, y),
				System.Math.Max(xmax, x),
				System.Math.Max(ymax, y)
				);

			return r;
		}

		/** Returns a new rect which is expanded by \a range in all directions.
		 * \param range How far to expand. Negative values are permitted.
		 */
		public IntRect Expand(int range)
		{
			return new IntRect(xmin - range,
				ymin - range,
				xmax + range,
				ymax + range
				);
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

		/** Returns a new rect rotated around the origin 90*r degrees.
		 * Ensures that a valid rect is returned.
		 */
		public IntRect Rotate(int r)
		{
			int mx1 = Rotations[r * 4 + 0];
			int mx2 = Rotations[r * 4 + 1];
			int my1 = Rotations[r * 4 + 2];
			int my2 = Rotations[r * 4 + 3];

			int p1x = mx1 * xmin + mx2 * ymin;
			int p1y = my1 * xmin + my2 * ymin;

			int p2x = mx1 * xmax + mx2 * ymax;
			int p2y = my1 * xmax + my2 * ymax;

			return new IntRect(
				System.Math.Min(p1x, p2x),
				System.Math.Min(p1y, p2y),
				System.Math.Max(p1x, p2x),
				System.Math.Max(p1y, p2y)
				);
		}

		/** Returns a new rect which is offset by the specified amount.
		 */
		public IntRect Offset(Int2 offset)
		{
			return new IntRect(xmin + offset.x, ymin + offset.y, xmax + offset.x, ymax + offset.y);
		}

		/** Returns a new rect which is offset by the specified amount.
		 */
		public IntRect Offset(int x, int y)
		{
			return new IntRect(xmin + x, ymin + y, xmax + x, ymax + y);
		}

		public override string ToString()
		{
			return "[x: " + xmin + "..." + xmax + ", y: " + ymin + "..." + ymax + "]";
		}

		/** Draws some debug lines representing the rect */
		public void DebugDraw(Matrix4x4 matrix, Color col)
		{
			Vector3 p1 = matrix.MultiplyPoint3x4(new Vector3(xmin, 0, ymin));
			Vector3 p2 = matrix.MultiplyPoint3x4(new Vector3(xmin, 0, ymax));
			Vector3 p3 = matrix.MultiplyPoint3x4(new Vector3(xmax, 0, ymax));
			Vector3 p4 = matrix.MultiplyPoint3x4(new Vector3(xmax, 0, ymin));

			Debug.DrawLine(p1, p2, col);
			Debug.DrawLine(p2, p3, col);
			Debug.DrawLine(p3, p4, col);
			Debug.DrawLine(p4, p1, col);
		}
	}


}
