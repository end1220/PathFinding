
using System;
using System.Collections;
using System.Collections.Generic;


namespace Lite.AStar
{

	public abstract class AStarMap : Lite.Graph.SparseGraph
	{
		public abstract int GetNeighbourNodeCount(AStarNode node);

		public abstract AStarNode GetNeighbourNode(AStarNode node, int index);

		public virtual void RecycleNode(AStarNode node) { }

		public virtual void Cleanup() { }
	}

}