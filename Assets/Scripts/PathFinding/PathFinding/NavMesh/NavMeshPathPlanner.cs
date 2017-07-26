
using System;
using System.Collections.Generic;
using AStar;


namespace PathFinding
{
	
	public class NavMeshMapPathPlanner : GraphPathPlanner
	{
		NavMeshNode startNode;
		NavMeshNode targetNode;


		protected override bool CheckArrived(AStarNode node)
		{
			return node.id == targetNode.id;
		}

		protected override int CalCostG(AStarNode prevNode, AStarNode currentNode)
		{
			return prevNode.g + map.GetEdge(prevNode.id, currentNode.id).cost;
		}

		protected override int CalCostH(AStarNode node)
		{
			NavMeshNode calNode = node as NavMeshNode;
			int dist = (calNode.position - targetNode.position).costMagnitude;
			return dist;
		}

	}


}