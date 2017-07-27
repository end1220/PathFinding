


namespace PathFinding
{
	[System.Serializable]
	public class Graph3DNode : AStar.AStarNode
	{
		public ushort x;
		public ushort y;
		public ushort z;

		public Int3 worldPosition;


		public Graph3DNode(int id) :
			base(id)
		{
		}

		public Graph3DNode()
		{
		}

	}

}