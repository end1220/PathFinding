
using UnityEngine;
using Lite;
using Lite;


namespace Lite.AStar.NavGraph
{

	public class Cell
	{
		public Int3 pos;
		public Vector3 worldPosition;
		public bool walkable;
		public int id;

		public Cell(int id, Int3 pos, Vector3 worldPos, bool walkable)
		{
			this.id = id;
			this.pos = pos;
			this.worldPosition = worldPos;
			this.walkable = walkable;
		}
	}

	[System.Serializable]
	public class BuildConfig
	{
		public int allTestMask;
		public int walkableMask;
		public int obstacleMask;
		public int cellSize;
		public float agentHeight;
		public int agentHeightStep;
		public float tanSlope;
		public GameObject box;
		public Int3 worldSize;
		public Int3 worldCenterPos;
		public Int3 worldMinPos;
		public Int3 cellCount;
	}

	/// <summary>
	/// 子空间
	/// 从3D场景空间生成路径Graph信息时，
	/// 进行划分子空间，可以快速剔除完全空的子空间，从而减少生成时间。
	/// </summary>
	public class SubSpace
	{
		public Int3 minPos;
		public Int3 startIndex;
		public Int3 cellCount;
	}


}