

using UnityEngine;


namespace PathFinding
{
	/// <summary>
	/// 存储着2D网格寻路数据
	/// </summary>
	//[CreateAssetMenu(menuName = "TwGame/Navigation Data", order = 2)]
	public class NavGrid2DData : ScriptableObject
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

		public void _setData(ushort[,] msk, byte[,] trrn, int w, int h, int grid, int minx, int minz)
		{
			width = w;
			height = h;
			gridSize = grid;
			minX = minx;
			minZ = minz;
			mask = new int[(width * height) / 32 + 1];
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					_setMask(x, y, msk[x, y]);
			terrain = new int[(width * height) / 4 + 1];
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					_setTerrain(x, y, trrn[x, y]);
		}


	}
}
