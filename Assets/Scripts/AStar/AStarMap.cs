
using System;
using System.Collections;
using System.Collections.Generic;


namespace TwGame.AStar
{

	public abstract class AStarMap : TwGame.Graph.SparseGraph
	{
		public abstract int GetNeighbourNodeCount(AStarNode node);

		public abstract AStarNode GetNeighbourNode(AStarNode node, int index);
	}

}