


namespace PathFinding
{
	public interface INavGraph
	{
		void Init(INavData data);

		bool IsPassable(FixVector3 position);

		//FixVector3 GetNearestForce(FixVector3 position, );

		FixVector3 RayCastForMoving(FixVector3 from, FixVector3 to, MoveType mov);

		FixVector3 SlideByObstacles(FixVector3 fromPos, FixVector3 oldTargetPos);

	}
}

