using UnityEngine;
using System;
using System.Collections.Generic;

using TwGame.Graph;
using TwGame.AStar;
using TwGame.AStar.NavGraph;


namespace TwGame
{
	public class NavGraph3DGizmo : MonoBehaviour
	{

		public BuildConfig cfg;

		public List<Cell> cells;
		public List<SubSpace> spaces;

		public NavGraph3DData navData;

		public Graph3DAStarMap graphMap;

		private Color green = new Color(0.2f, 0.5f, 0.2f);
		private Color red = new Color(0.5f, 0.2f, 0.2f);
		//private Color blue = new Color(0, 0, 0.5f);

		private bool drawSpaces = false;
		private bool drawNodes = false;

#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{
			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Color defaultColor = Gizmos.color;

			// begin draw
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(cfg.worldCenterPos.ToVector3(), cfg.worldSize.ToVector3());

			//var gridSize = new Vector3(cfg.cellSize / 1000f, cfg.cellSize / 1000f, cfg.cellSize / 1000f);
			//var minPos = navData.buildConfig.worldMinPos;
			float cellSize = navData.buildConfig.cellSize / 1000f;
			//float cellRadius = cellSize / 2;
			Vector3 nodesz = new Vector3(0.05f, 0.05f, 0.05f);

			if (spaces != null && drawSpaces)
			{
				for (int i = 0; i < spaces.Count; ++i)
				{
					var space = spaces[i];
					Gizmos.DrawWireCube(space.minPos.ToVector3() + (space.cellCount * cfg.cellSize).ToVector3() / 2f, (space.cellCount * cfg.cellSize).ToVector3());
				}
			}

			if (cells != null && drawNodes)
			{
				for (int i = 0; i < cells.Count; ++i)
				{
					var cell = cells[i];
					Gizmos.color = red;
					Gizmos.DrawWireCube(cell.worldPosition, nodesz);
				}
			}

			if (graphMap == null && navData.edgeList != null && navData.edgeList.Count > 0)
			{
				if (drawNodes)
				{
					for (int i = 0; i < navData.nodeList.Count; ++i)
					{
						var node = navData.nodeList[i];
						Gizmos.color = green;
						Gizmos.DrawWireCube(node.worldPosition.ToVector3(), nodesz);
					}
				}
				for (int i = 0; i < navData.edgeList.Count; ++i)
				{
					var edge = navData.edgeList[i];
					var from = navData.nodeDic[edge.from];
					var to = navData.nodeDic[edge.to];
					Gizmos.color = green;
					Gizmos.DrawLine(from.worldPosition.ToVector3(), to.worldPosition.ToVector3());
				}
			}
			else if (graphMap != null)
			{
				for (int i = 0; i < graphMap.edgeList.Count; ++i)
				{
					var edge = graphMap.edgeList[i];
					var from = graphMap.nodeDic[edge.from];
					var to = graphMap.nodeDic[edge.to];
					Gizmos.color = green;
					Gizmos.DrawLine(from.worldPosition.ToVector3(), to.worldPosition.ToVector3());
				}
			}

			// end draw

			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
		}
#endif

	}

}