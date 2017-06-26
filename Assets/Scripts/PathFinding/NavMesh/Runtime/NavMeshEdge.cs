
using System;
using Lite.Graph;


namespace Lite.NavMesh
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