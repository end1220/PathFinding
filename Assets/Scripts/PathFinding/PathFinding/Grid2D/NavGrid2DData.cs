

using UnityEngine;


namespace PathFinding
{
	/// <summary>
	/// 存储着2D网格寻路数据
	/// </summary>
	//[CreateAssetMenu(menuName = "TwGame/Navigation Data", order = 2)]
	public class NavGrid2DData : INavData
	{
		[SerializeField]
		private int width = 0;
		public int Width { get { return width; } }

		[SerializeField]
		private int height = 0;
		public int Height { get { return height; } }

		[SerializeField]
		private int gridSize;
		public int GridSize { get { return gridSize; } }

		[SerializeField]
		private int agentRadius;
		public int AgentRadius { get { return agentRadius; } }

		[SerializeField]
		private int maxSlope;
		public int MaxSlope { get { return maxSlope; } }

		[SerializeField]
		private int minX;
		public int MinX { get { return minX; } }

		[SerializeField]
		private int minZ;
		public int MinZ { get { return minZ; } }

		[HideInInspector]
		public int[] mask = null;

		[HideInInspector]
		public int[] terrain = null;


		public ushort GetMask(int x, int y)
		{
			int bitSize = y * width + x;
			int left = bitSize % 32;
			int index = bitSize / 32;
			return (ushort)(mask[index] >> left & 1);
		}

		public byte GetTerrain(int x, int y)
		{
			int byteCount = y * width + x;
			int leftBits = byteCount % 4 * 8;
			int index = byteCount / 4;
			return (byte)(terrain[index] >> leftBits & 255);
		}

		private void _setMask(int x, int y, ushort value)
		{
			int bitSize = y * width + x;
			int left = bitSize % 32;
			int index = bitSize / 32;
			mask[index] = mask[index] | value << left;
		}

		private void _setTerrain(int x, int y, byte value)
		{
			int byteCount = y * width + x;
			int leftBits = byteCount % 4 * 8;
			int index = byteCount / 4;
			terrain[index] = terrain[index] | value << leftBits;
		}

		public void _setData(ushort[,] msk, byte[,] trrn, int w, int h, int grid, int minx, int minz, int agentRadius, int maxSlope)
		{
			width = w;
			height = h;
			gridSize = grid;
			minX = minx;
			minZ = minz;
			this.agentRadius = agentRadius;
			this.maxSlope = maxSlope;
			mask = new int[(width * height) / 32 + 1];
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					_setMask(x, y, msk[x, y]);
			terrain = new int[(width * height) / 4 + 1];
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					_setTerrain(x, y, trrn[x, y]);
		}


		#region Gizmo

		private Vector3[,] gridPosList;
		public void SetGridPosList()
		{
			if (gridPosList == null)
			{
				Vector3[,] pos = new Vector3[Width, Height];
				for (int x = 0; x < Width; ++x)
				{
					for (int z = 0; z < Height; ++z)
					{
						float fposx = FixMath.mm2m(MinX) + FixMath.mm2m(GridSize) * (x + 0.5f);
						float fposz = FixMath.mm2m(MinZ) + FixMath.mm2m(GridSize) * (z + 0.5f);
						pos[x, z] = new Vector3(fposx, 1, fposz);
					}
				}
				gridPosList = pos;
			}
		}


		public override void OnDrawGizmosSelected(Transform trans)
		{
			SetGridPosList();

			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = trans.localToWorldMatrix;

			Color defaultColor = Gizmos.color;

			float a = FixMath.mm2m(GridSize) / 2;

			// passables
			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; ++j)
				{
					Vector3 c = gridPosList[i, j];
					bool passable = GetMask(i, j) == 0;
					if (passable)
					{
						Gizmos.color = Color.green;
						DrawRect(c, a);
					}
					byte terrain = GetTerrain(i, j);
					if (terrain == (byte)TerrainType.ShortWall)
					{
						Gizmos.color = Color.red;
						DrawRect(c, a / 2);
					}
					else if (terrain == (byte)TerrainType.TallWall)
					{
						Gizmos.color = Color.cyan;
						DrawRect(c, a / 2);
					}
				}
			}

			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
		}


		private void DrawRect(Vector3 center, float a)
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

		#endregion


	}
}
