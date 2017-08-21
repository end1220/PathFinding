

namespace PathFinding.RVO
{

	public interface IRVOActor
	{
		Int3 location { get; set; }

		Int groundY { get; set; }

		bool hasReachedNavEdge { get; set; }

		int ActorCamp { get; set; }

		bool isMovable { get; set; }

	}

}