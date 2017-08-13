
using System;
using UnityEngine;


namespace PathFinding.Graph3d
{

	public class Cell
	{
		public Int3 pos;
		public Vector3 worldPosition;
		public Vector3 worldPos1;
		public Vector3 worldPos2;
		public Vector3 worldPos3;
		public Vector3 worldPos4;
		public bool walkable;
		public int id;

		public Cell(int id, Int3 pos, Vector3 worldPos, bool walkable, Vector3 worldPos1, Vector3 worldPos2, Vector3 worldPos3, Vector3 worldPos4)
		{
			this.id = id;
			this.pos = pos;
			this.worldPosition = worldPos;
			this.walkable = walkable;
			this.worldPos1 = worldPos1;
			this.worldPos2 = worldPos2;
			this.worldPos3 = worldPos3;
			this.worldPos4 = worldPos4;
		}
	}

	[Serializable]
	public class BuildConfig
	{
		[NonSerialized]
		public int allTestMask;
		[NonSerialized]
		public int walkableMask;
		[NonSerialized]
		public int obstacleMask;
		public int cellSize;
		public float agentHeight;
		public float agentRadius;
		public int agentHeightStep;
		public int maxSlope;
		[NonSerialized]
		public float tanSlope;
		[NonSerialized]
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