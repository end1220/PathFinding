


namespace PathFinding
{
	[System.Serializable]
	public class Graph3DAStarNode : GraphAStarNode
	{
		public ushort z;

		public Int3 worldPosition;


		public Graph3DAStarNode(int id) :
			base(id)
		{
		}

		public Graph3DAStarNode()
		{
		}

	}

}