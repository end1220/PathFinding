
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
			int dx = Math.Abs(targetNode.x - ((NavMeshNode)node).x);
			int dy = Math.Abs(targetNode.y - ((NavMeshNode)node).y);
			int dz = Math.Abs(targetNode.z - ((NavMeshNode)node).z);
			dx *= 10;
			dy *= 10;
			dz *= 10;
			int distxz = (int)(dx > dz ? 1.4f * dz + (dx - dz) : 1.4f * dx + (dz - dx));
			int dist = distxz + dy;
			return dist;
		}

	}


}