

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


}