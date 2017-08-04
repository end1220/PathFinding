

using UnityEngine;
using System.Collections.Generic;
using PathFinding.Graph3d;


namespace PathFinding
{
	public class NavGraph3DGizmo : MonoBehaviour
	{
		struct EdgeData
		{
			public Vector3 from;
			public Vector3 to;

			public EdgeData(Vector3 from, Vector3 to)
			{
				this.from = from;
				this.to = to;
			}
		}

		public BuildConfig cfg;
		public List<Cell> cells;
		public List<Cell> finalCells;
		public List<SubSpace> spaces;
		public Graph3DMap graphMap;

		private Color green = new Color(0f, 1f, 0f);
		private Color red = new Color(1f, 0f, 0f);

		public bool drawSpaces = false;
		public bool drawNodes = false;
		public bool drawGraph = false;


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
			//float cellSize = cfg.cellSize / 1000f;
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

			if (finalCells != null)
			{
				for (int i = 0; i < finalCells.Count; ++i)
				{
					var cell = finalCells[i];
					if (cell.worldPos1 == Vector3.zero || cell.worldPos2 == Vector3.zero || cell.worldPos3 == Vector3.zero || cell.worldPos4 == Vector3.zero)
						continue;
					Gizmos.color = green;
					Gizmos.DrawLine(cell.worldPos1, cell.worldPos2);
					Gizmos.DrawLine(cell.worldPos2, cell.worldPos3);
					Gizmos.DrawLine(cell.worldPos3, cell.worldPos4);
					Gizmos.DrawLine(cell.worldPos4, cell.worldPos1);
				}
			}

			if (graphMap != null && drawGraph)
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

		private void DrawWireBorder()
		{
			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Color defaultColor = Gizmos.color;
			// begin draw

			// end draw
			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
		}

	}

}