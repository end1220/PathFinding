
using AStar;


namespace PathFinding
{

	public struct HitInfo
	{
		public Int3 from;
		public Int3 to;
		public Int3 hitPosition;
		public AStarNode node;

		public HitInfo(Int3 _from, Int3 _to)
		{
			from = _from;
			to = _to;
			hitPosition = from;
			node = null;
		}
	}


}
