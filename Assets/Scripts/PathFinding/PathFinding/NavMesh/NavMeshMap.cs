

using AStar;


namespace PathFinding
{
	public class NavMeshMap : AStarMap
	{
		NavMeshData navData;

		public BBTree bbTree = new BBTree();


		public void InitMap(NavMeshData data)
		{
			navData = data;

			for (int i = 0; i < navData.nodes.Length; i++)
			{
				NavMeshNode node = navData.nodes[i] as NavMeshNode;
				AddNode(node);
			}

			// 			for (int i = 0; i < navData.nodes.Length; i++)
			// 			{
			// 				NavMeshNode node = navData.nodes[i] as NavMeshNode;
			// 
			// 				for (int c = 0; c < node.connections.Length; ++c)
			// 				{
			// 					GraphEdge edge = new GraphEdge(node.id, node.connections[c].id, (int)node.connectionCosts[c]);
			// 					this.AddEdge(edge);
			// 				}
			// 			}

			bbTree.RebuildFrom(navData.nodes);

		}


		public override int GetNeighbourNodeCount(AStarNode node)
		{
			return (node as NavMeshNode).connections.Length;
		}

		public override AStarNode GetNeighbourNode(AStarNode node, int index)
		{
			int id = (node as NavMeshNode).connections[index];
			return GetNode(id) as AStarNode;
		}


	}
}

