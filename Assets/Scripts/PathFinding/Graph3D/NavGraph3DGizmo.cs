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


			if (spaces != null)
			{
				for (int i = 0; i < spaces.Count; ++i)
				{
					var space = spaces[i];
					Gizmos.DrawWireCube(space.minPos.ToVector3() + (space.cellCount * cfg.cellSize).ToVector3() / 2f, (space.cellCount * cfg.cellSize).ToVector3());
				}
			}

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

			/*if (BattleMap.Instance != null && BattleMap.Instance.currentPoint3D.x != 0)
			{
				int x = BattleMap.Instance.currentPoint3D.x;
				int y = BattleMap.Instance.currentPoint3D.y;
				int z = BattleMap.Instance.currentPoint3D.z;
				Gizmos.color = blue;
				int posx = minPos.x + x * cellSize + cellRadius;
				int posy = minPos.y + y * cellSize + cellRadius;
				int posz = minPos.z + z * cellSize + cellRadius;
				Vector3 pos = new Vector3(posx / 1000f, posy / 1000f, posz / 1000f);
				Gizmos.DrawWireCube(pos, gridSize);
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

			/*if (BattleMap.Instance != null)
			{
				if (BattleMap.Instance.currentNode != null)
				{
					var node = BattleMap.Instance.currentNode;
					Gizmos.color = blue;
					Gizmos.DrawWireCube(node.worldPosition.ToVector3(), smallSize);
				}
				if (BattleMap.Instance.targetNode != null)
				{
					var node = BattleMap.Instance.targetNode;
					Gizmos.color = red;
					Gizmos.DrawWireCube(node.worldPosition.ToVector3(), smallSize);
				}

				if (ActorManager.Instance.SelfController != null)
				{
					var actor = ActorManager.Instance.SelfController;
					var actorSize = new Vector3(0.05f, 0.05f, 0.05f);
					Gizmos.DrawWireCube(actor.GetPosition().ToVector3(), actorSize);
				}
			}*/

			// end draw

			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
		}


		/*private void DrawRect(Vector3 center, float a)
		{
			var v1 = new Vector3(center.x - a, center.y, center.z - a);
			var v2 = new Vector3(center.x + a, center.y, center.z - a);
			var v3 = new Vector3(center.x + a, center.y, center.z + a);
			var v4 = new Vector3(center.x - a, center.y, center.z + a);
			Gizmos.DrawLine(v1, v2);
			Gizmos.DrawLine(v2, v3);
			Gizmos.DrawLine(v3, v4);
			Gizmos.DrawLine(v4, v1);
		}


		private void DrawAABBBox()
		{
			float minx = TwMath.mm2m(minX);
			float miny = TwMath.mm2m(minY);
			float minz = TwMath.mm2m(minZ);

			float maxx = minx + width * TwMath.mm2m(gridSize);
			float maxy = miny + height * TwMath.mm2m(heightSize);
			float maxz = minz + length * TwMath.mm2m(gridSize);

			var v1 = new Vector3(minx, miny, minz);
			var v2 = new Vector3(minx, maxy, minz);
			var v3 = new Vector3(maxx, maxy, minz);
			var v4 = new Vector3(maxx, miny, minz);

			var v5 = new Vector3(minx, miny, maxz);
			var v6 = new Vector3(minx, maxy, maxz);
			var v7 = new Vector3(maxx, maxy, maxz);
			var v8 = new Vector3(maxx, miny, maxz);

			Gizmos.DrawLine(v1, v2);
			Gizmos.DrawLine(v2, v3);
			Gizmos.DrawLine(v3, v4);
			Gizmos.DrawLine(v4, v1);

			Gizmos.DrawLine(v5, v6);
			Gizmos.DrawLine(v6, v7);
			Gizmos.DrawLine(v7, v8);
			Gizmos.DrawLine(v8, v5);

			Gizmos.DrawLine(v1, v5);
			Gizmos.DrawLine(v2, v6);
			Gizmos.DrawLine(v3, v7);
			Gizmos.DrawLine(v4, v8);
		}*/

	}

}