
using UnityEngine;
using Lite;


namespace Lite.AStar.NavGraph
{

	public class Cell
	{
		public Vector3 centerPosition;
		public Vector3 worldPosition;
		public bool walkable;

		public Cell(Vector3 pos, Vector3 worldPos, bool walkable)
		{
			this.centerPosition = pos;
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
		public float tanSlope;
		public GameObject box;
		public Int3 worldSize;
		public Int3 worldCenterPos;
		public Int3 worldMinPos;
		public int cellCountX;
		public int cellCountY;
		public int cellCountZ;
	}


}