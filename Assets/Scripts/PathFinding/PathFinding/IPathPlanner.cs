

using AStar;


namespace PathFinding
{
	
	public abstract class IPathPlanner : AStarPathPlanner
	{
		protected override bool CheckArrived(ProcessingNode node, AStarContext context) { return false; }

		protected override int CalCostG(ProcessingNode prevNode, ProcessingNode currentNode, AStarContext context) { return 0; }

		protected override int CalCostH(ProcessingNode node, AStarContext context) { return 0; }

		/*public virtual bool FindPath(Int3 from, Int3 to, ref List<Int3> result)
		{
			return false;
		}*/

	}


}