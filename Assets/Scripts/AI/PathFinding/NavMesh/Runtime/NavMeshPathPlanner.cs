using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lite.AStar;


namespace Lite.NavMesh
{
	
	public class NavMeshPathPlanner : AStarPathPlanner
	{
		private int startID;
		private int endID;
		private NavMeshNode startNode;
		private NavMeshNode targetNode;

		public Point2D[] FindPath(int startID, int endID)
		{
			this.startID = endID;
			this.endID = startID;

			startNode = map.GetNodeByID(this.startID) as NavMeshNode;
			targetNode = map.GetNodeByID(this.endID) as NavMeshNode;

			NavMeshNode endNode = DoAStar(startNode) as NavMeshNode;

			// build path points.
			int pointCount = 0;
			NavMeshNode pathNode = endNode;
			while (pathNode != null)
			{
				pointCount++;
				pathNode = pathNode.prev as NavMeshNode;
			}
			Point2D[] pointArray = new Point2D[pointCount];
			pathNode = endNode;
			int index = 0;
			while (pathNode != null)
			{
				pointArray[index++] = new Point2D((int)pathNode.center.x, (int)pathNode.center.y);
				pathNode = pathNode.prev as NavMeshNode;
			}
			Cleanup();
			return pointArray;
		}

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
			// 两个三角形重心距离？
			NavMeshNode thisNode = node as NavMeshNode;
			var thisCenter = thisNode.center;
			var distSqr = (thisCenter - targetNode.center).sqrMagnitude;
			return (int)distSqr;
		}

	}


}