
using AStar;


namespace PathFinding
{
	public abstract class INavGraph : AStarMap
	{
		public abstract void Init(INavData data);

		public abstract bool IsWalkable(Int3 position);

		public abstract Int3 GetNearestPosition(Int3 position);

		// return true if hit obstacle
		public abstract bool LineCastForMoving(ref HitInfo hit, MoveType mov);

		public abstract Int3 SlideByObstacles(Int3 from, Int3 to, Int3 hit);

		public abstract AStarNode GetNodeAt(Int3 position);

	}
}

