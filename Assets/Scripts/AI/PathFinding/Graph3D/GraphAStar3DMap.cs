
using System;
using System.Collections;
using System.Collections.Generic;

using TwGame.Graph;


namespace Lite.AStar
{
	public class GraphAStar3DMap : GraphAStarMap
	{
		private NavGraphData navGraphData;

		private GraphAStar3DNode[, ,] nodeMatrix;

		public void Init(NavGraphData navData)
		{
			navGraphData = navData;
			nodeMatrix = new GraphAStar3DNode[navData.buildConfig.cellCountX, navData.buildConfig.cellCountY, navData.buildConfig.cellCountZ];
			for (int i = 0; i < navData.nodeList.Count; ++i)
			{
				var node = navData.nodeList[i];
				nodeMatrix[node.x, node.y, node.z] = node;
				navData.nodeDic.Add(node.id, node);
				this.AddNode(node);
			}
			for (int i = 0; i < navData.edgeList.Count; ++i)
			{
				var edge = navData.edgeList[i];
				this.AddEdge(edge);
			}
		}


		public GraphAStar3DNode GetNodeAt(int x, int y, int z)
		{
			if (IsIndexValid(x, y, z))
				return nodeMatrix[x, y, z];
			return null;
		}


		public bool IsPointPassable(Point3D point)
		{
			if (IsIndexValid(point.x, point.y, point.z))
				return nodeMatrix[point.x, point.y, point.z].walkable;
			else
				return false;
		}

		public bool IsIndexValid(int x, int y, int z)
		{
			return (x >= 0 && x < navGraphData.buildConfig.cellCountX
				&& y >= 0 && y < navGraphData.buildConfig.cellCountY
				&& z >= 0 && z < navGraphData.buildConfig.cellCountZ
				);
		}

		public bool IsPassableBetween(GraphAStar3DNode from, GraphAStar3DNode to)
		{
			if (to.walkable == false)
				return false;

			var edgeList = GetEdgeList(from.id);
			for (int i = 0; i < edgeList.Count; ++i)
			{
				var edge = edgeList[i];
				if (edge.cost < int.MaxValue && edge.to == to.id)
					return true;
			}
			return false;
		}


		public GraphAStar3DNode GetPassableNeighborNodeAt(GraphAStar3DNode node, int x, int z)
		{
			if (node == null)
				return null;

			var edgeList = GetEdgeList(node.id);
			for (int i = 0; i < edgeList.Count; ++i)
			{
				var edge = edgeList[i];
				var neighbor = GetNodeByID(edge.to) as GraphAStar3DNode;
				if (neighbor.walkable && neighbor.x == x && neighbor.z == z)
					return neighbor;
			}
			return null;
		}

	}
}

