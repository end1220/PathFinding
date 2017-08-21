
using System;
using System.Collections.Generic;
using UnityEngine;
using PathFinding.Grid3D;


namespace PathFinding
{
	/// <summary>
	/// 存储着3D网格稀疏图寻路数据
	/// </summary>
	public class Grid3DNavData : INavData
	{
		[HideInInspector]
		public bool bytesMode = false;// 序列化图的节点和边数据是否使用bytes

		[HideInInspector]
		public BuildConfig buildConfig;

		[HideInInspector]
		public byte[] nodeBytes = null;

		[HideInInspector]
		public byte[] edgeBytes = null;

		public int nodeCount;

		public int edgeCount;

		[HideInInspector]
		public List<Grid3DNode> nodeList = new List<Grid3DNode>();

		[HideInInspector]
		public List<Grid3DEdge> edgeList = new List<Grid3DEdge>();
		
		[NonSerialized]
		public Dictionary<int, Grid3DNode> nodeDic = new Dictionary<int, Grid3DNode>();


		public int NodeSize
		{
			get
			{
				// int : id, int3.x.y.z  = 4 * 4
				// ushort: x y z  = 3 * 2
				return 22;
			}
		}

		public int EdgeSize
		{
			get
			{	// int: from, to, cost  = 3 * 4
				return 12;
			}
		}


#if UNITY_EDITOR

		public void Init(BuildConfig cfg)
		{
			this.buildConfig = cfg;
		}


		public void AddNode(Grid3DNode node)
		{
			nodeList.Add(node);
			nodeDic.Add(node.id, node);
			nodeCount = Math.Max(nodeCount, node.id);
		}


		public void AddEdge(Grid3DEdge edge)
		{
			edgeList.Add(edge);
		}


		public void SaveBytes()
		{
			nodeCount = nodeList.Count;
			edgeCount = edgeList.Count;

			if (bytesMode)
			{
				nodeBytes = new byte[nodeList.Count * NodeSize];
				edgeBytes = new byte[edgeList.Count * EdgeSize];

				for (int i = 0; i < nodeList.Count; ++i)
				{
					var node = nodeList[i];
					int index = i * NodeSize;
					ByteTool.IntToBytes(nodeBytes, index, node.id);
					index += 4;
					ByteTool.UshortToBytes(nodeBytes, index, node.x);
					index += 2;
					ByteTool.UshortToBytes(nodeBytes, index, node.y);
					index += 2;
					ByteTool.UshortToBytes(nodeBytes, index, node.z);
					index += 2;
					ByteTool.IntToBytes(nodeBytes, index, node.worldPosition.x);
					index += 4;
					ByteTool.IntToBytes(nodeBytes, index, node.worldPosition.y);
					index += 4;
					ByteTool.IntToBytes(nodeBytes, index, node.worldPosition.z);
				}

				for (int i = 0; i < edgeList.Count; ++i)
				{
					var edge = edgeList[i];
					int index = i * EdgeSize;
					ByteTool.IntToBytes(edgeBytes, index, edge.from);
					index += 4;
					ByteTool.IntToBytes(edgeBytes, index, edge.to);
					index += 4;
					ByteTool.IntToBytes(edgeBytes, index, edge.cost);
				}

				nodeList.Clear();
				edgeList.Clear();
			}
			else
			{

			}
			
		}

#endif


		public void ReleaseMemory()
		{
			if (bytesMode)
			{
				nodeBytes = null;
				edgeBytes = null;
			}
			else
			{
				nodeList.Clear();
				edgeList.Clear();
			}
		}


		public Grid3DNode ParseNode(int index)
		{
			Grid3DNode node = new Grid3DNode();
			int bytesIndex = index * NodeSize;
			node.id = ByteTool.IntFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 4;
			node.x = ByteTool.UshortFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 2;
			node.y = ByteTool.UshortFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 2;
			node.z = ByteTool.UshortFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 2;
			node.worldPosition.x = ByteTool.IntFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 4;
			node.worldPosition.y = ByteTool.IntFromBytes(nodeBytes, bytesIndex);
			bytesIndex += 4;
			node.worldPosition.z = ByteTool.IntFromBytes(nodeBytes, bytesIndex);

			return node;
		}


		public Grid3DEdge ParseEdge(int index)
		{
			Grid3DEdge edge = new Grid3DEdge();
			int bytesIndex = index * EdgeSize;
			edge.from = ByteTool.IntFromBytes(edgeBytes, bytesIndex);
			bytesIndex += 4;
			edge.to = ByteTool.IntFromBytes(edgeBytes, bytesIndex);
			bytesIndex += 4;
			edge.cost = ByteTool.IntFromBytes(edgeBytes, bytesIndex);
			
			return edge;
		}


		#region Gizmos

		[NonSerialized]
		public List<Cell> cells;
		[NonSerialized]
		public List<Cell> finalCells;
		[NonSerialized]
		public List<SubSpace> spaces;
		[NonSerialized]
		public Grid3DGraph graphMap;

		private Color green = new Color(0f, 1f, 0f);
		private Color red = new Color(1f, 0f, 0f);

		private bool drawSpaces = false;
		private bool drawNodes = true;
		private bool drawGraph = false;
		public override void OnDrawGizmosSelected(Transform trans)
		{
			if (graphMap == null)
			{
				graphMap = new Grid3DGraph();
				graphMap.Init(this);
			}

			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = trans.localToWorldMatrix;
			Color defaultColor = Gizmos.color;

			// begin draw
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
					Gizmos.DrawWireCube((Vector3)space.minPos + (Vector3)(space.cellCount * buildConfig.cellSize) / 2f, (Vector3)(space.cellCount * buildConfig.cellSize));
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
#if UNITY_EDITOR
                for (int i = 0; i < graphMap.edgeList.Count; ++i)
				{
					var edge = graphMap.edgeList[i];
					var from = graphMap.nodeDic[edge.from];
					var to = graphMap.nodeDic[edge.to];
					Gizmos.color = green;
					Gizmos.DrawLine((Vector3)from.worldPosition, (Vector3)to.worldPosition);
				}
#endif
            }

			// end draw

			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
		}

		#endregion

	}
}
