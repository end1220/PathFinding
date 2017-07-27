
using System;
using AStar;


namespace PathFinding
{
	public class NavMeshMap : GraphAStarMap
	{
		NavMeshData navData;

		public void InitMap(NavMeshData data)
		{
			navData = data;

			for (int i = 0; i < navData.nodes.Length; i++)
			{
				NavMeshNode node = navData.nodes[i] as NavMeshNode;
				this.AddNode(node);
			}

			for (int i = 0; i < navData.nodes.Length; i++)
			{
				NavMeshNode node = navData.nodes[i] as NavMeshNode;

				for (int c = 0; c < node.connections.Length; ++c)
				{
					Graph.GraphEdge edge = new Graph.GraphEdge(node.id, node.connections[c].id, (int)node.connectionCosts[c]);
					this.AddEdge(edge);
				}
			}

		}

	}
}

