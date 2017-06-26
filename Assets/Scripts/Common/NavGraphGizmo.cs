using UnityEngine;
using System;
using System.Collections.Generic;

using Lite.Graph;
using Lite.AStar;
using Lite.AStar.NavGraph;


namespace Lite
{
	public class NavGraphGizmo : MonoBehaviour
	{

		public BuildConfig cfg;

		public Cell[, ,] cells;

		public NavGraph3DData navData;

		public Graph3DAStarMap graphMap;

		private Color green = new Color(0.2f, 0.8f, 0.2f);
		private Color red = new Color(0.8f, 0.2f, 0.2f);
		private Color blue = new Color(0, 0, 0.5f);


		void OnDrawGizmosSelected()
		{
			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Color defaultColor = Gizmos.color;

			// begin draw
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(cfg.worldCenterPos.ToVector3(), cfg.worldSize.ToVector3());

			var gridSize = new Vector3(cfg.cellSize / 1000f, cfg.cellSize / 1000f, cfg.cellSize / 1000f);
			var minPos = navData.buildConfig.worldMinPos;
			int cellSize = navData.buildConfig.cellSize;
			int cellRadius = cellSize / 2;

			/*for (int x = 0; x < cfg.cellCountX; ++x)
			{
				for (int y = 0; y < cfg.cellCountY; ++y)
				{
					for (int z = 0; z < cfg.cellCountZ; ++z)
					{
						if (cells != null)
						{
							var cell = cells[x, y, z];
							if (cell == null)
								continue;
							Gizmos.color = cell.walkable ? green : red;
							Gizmos.DrawWireCube(cell.centerPosition, gridSize);
						}
						else if (graphMap != null)
						{
							var node = graphMap.GetNodeAt(x, y, z);
							if (node == null)
								continue;
							Gizmos.color = node.walkable ? green : red;
							int posx = minPos.x + x * cellSize + cellRadius;
							int posy = minPos.y + y * cellSize + cellRadius;
							int posz = minPos.z + z * cellSize + cellRadius;
							Vector3 pos = new Vector3(posx / 1000f, posy / 1000f, posz / 1000f);
							Gizmos.DrawWireCube(pos, gridSize);
						}
					}
				}
			}*/

			var smallSize = new Vector3(0.1f, 0.1f, 0.1f);
			for (int i = 0; i < navData.nodeList.Count; ++i)
			{
				var node = navData.nodeList[i];
				Gizmos.color = node.walkable ? green : red;
				Gizmos.DrawWireCube(node.worldPosition.ToVector3(), smallSize);
			}

			for (int i = 0; i < navData.edgeList.Count; ++i)
			{
				var edge = navData.edgeList[i];
				var from = navData.nodeDic[edge.from];
				var to = navData.nodeDic[edge.to];
				Gizmos.color = (from.walkable && to.walkable) ? green : red;
				Gizmos.DrawLine(from.worldPosition.ToVector3(), to.worldPosition.ToVector3());
			}


			// end draw

			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
		}

	}

}