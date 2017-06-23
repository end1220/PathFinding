
using System;
using TwGame.Graph;


namespace TwGame.NavMesh
{
	[Serializable]
	public class NavMeshEdge : GraphEdge
	{
		public NavMeshEdge(int from, int to, int cost):
			base(from, to, cost)
		{
			
		}
	}

}