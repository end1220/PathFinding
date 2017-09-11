


namespace PathFinding
{
	public interface INavGraph
	{
		void Init(INavData data);

		bool IsWalkable(Int3 position);

		Int3 GetNearestPosition(Int3 position);

		// return true if hit obstacle
		bool LineCastForMoving(ref HitInfo hit, MoveType mov);

		Int3 SlideByObstacles(Int3 from, Int3 to, Int3 hit);

	}
}

