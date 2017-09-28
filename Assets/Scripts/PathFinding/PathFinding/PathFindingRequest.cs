
using AStar;


namespace PathFinding
{

	public class PathFindingRequest : AStarRequest
	{
		public Int3 fromPosition;

		public Int3 toPosition;

		public int extend1;       // for extension

		public int extend2;       // for extension


		public PathFindingRequest(Int3 from, Int3 to, AStarMap map, AStarPathPlanner planner, int ex1 = 0, int ex2 = 0)
		{
			fromPosition = from;
			toPosition = to;
			INavGraph graph = map as INavGraph;
			AStarNode fromNode = graph.GetNodeAt(to);
			AStarNode toNode = graph.GetNodeAt(from);
			if (fromNode == null)
				UnityEngine.Debug.LogError("PathFindingRequest: fromNode is null.");
			if (toNode == null)
				UnityEngine.Debug.LogError("PathFindingRequest: toNode is null.");
			SetData(fromNode, toNode, map, planner);
			extend1 = ex1;
			extend2 = ex2;
		}
	}

}
