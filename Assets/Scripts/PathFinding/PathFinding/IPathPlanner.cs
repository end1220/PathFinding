
using System.Collections.Generic;
using AStar;


namespace PathFinding
{
	
	public abstract class IPathPlanner : AStarPathPlanner
	{
		protected override bool CheckArrived(AStarNode node) { return false; }

		protected override int CalCostG(AStarNode prevNode, AStarNode currentNode) { return 0; }

		protected override int CalCostH(AStarNode node) { return 0; }

		public void SetGraph(INavGraph graph)
		{
			Setup(graph as AStarMap);
		}

		public virtual bool FindPath(Int3 from, Int3 to, ref List<Int3> result)
		{
			return false;
		}

	}


}