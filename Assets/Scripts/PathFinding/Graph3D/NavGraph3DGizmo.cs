using UnityEngine;
using System;
using System.Collections.Generic;

using Lite.Graph;
using Lite.AStar;
using Lite.AStar.NavGraph;


namespace Lite
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


		void OnDrawGizmosSelected()
		{
			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Color defaultColor = Gizmos.color;

			// begin draw
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(cfg.worldCenterPos.ToVector3(), cfg.worldSize.ToVector3());

			/*var gridSize = new Vector3(cfg.cellSize / 1000f, cfg.cellSize / 1000f, cfg.cellSize / 1000f);
			var minPos = navData.buildConfig.worldMinPos;
			int cellSize = navData.buildConfig.cellSize;
			int cellRadius = cellSize / 2;*/


			/*if (spaces != null)
			{
				for (int i = 0; i < spaces.Count; ++i)
				{
					var space = spaces[i];
					Gizmos.DrawWireCube(space.minPos.ToVector3() + (space.cellCount * cfg.cellSize).ToVector3() / 2f, (space.cellCount * cfg.cellSize).ToVector3());
				}
			}*/

			/*if (graphMap != null)
			{
				for (int x = 0; x < cfg.cellCount.x; x++)
				{
					for (int y = 0; y < cfg.cellCount.y; y++)
					{
						for (int z = 0; z < cfg.cellCount.z; z++)
						{
							var node = graphMap.NodeMatrix[x, y, z];
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

			/*var smallSize = new Vector3(0.1f, 0.1f, 0.1f);
			for (int i = 0; i < navData.nodeList.Count; ++i)
			{
				var node = navData.nodeList[i];
				Gizmos.color = node.walkable ? green : red;
				Gizmos.DrawWireCube(node.worldPosition.ToVector3(), smallSize);
			}*/

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