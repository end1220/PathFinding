

namespace PathFinding
{


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
		public Int3 clampedPosition;
		/** Clamped position for the optional constrainedNode */
		public Int3 constClampedPosition;

		public NNInfo(NavMeshNode node)
		{
			this.node = node;
			constrainedNode = null;
			clampedPosition = Int3.zero;
			constClampedPosition = Int3.zero;

			UpdateInfo();
		}

		/** Sets the constrained node */
		public void SetConstrained(NavMeshNode constrainedNode, Int3 clampedPosition)
		{
			this.constrainedNode = constrainedNode;
			constClampedPosition = clampedPosition;
		}

		/** Updates #clampedPosition and #constClampedPosition from node positions */
		public void UpdateInfo()
		{
			clampedPosition = node != null ? node.position : Int3.zero;
			constClampedPosition = constrainedNode != null ? constrainedNode.position : Int3.zero;
		}

	}


}
