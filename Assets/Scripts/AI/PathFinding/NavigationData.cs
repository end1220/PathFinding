
using System;
using UnityEngine;
using System.Collections.Generic;


namespace Lite.AStar
{
	/// <summary>
	/// 存储着2D网格寻路数据
	/// </summary>
	//[CreateAssetMenu(menuName = "Lite/Navigation Data", order = 2)]
	public class NavigationData : ScriptableObject
	{
		public int width = 0;
		public int Width { get { return width; } }

		public int height = 0;
		public int Height { get { return height; } }

		public float gridSize;
		public float GridSize { get { return gridSize; } }

		public float minX;
		public float MinX { get { return minX; } }

		public float minZ;
		public float MinZ { get { return minZ; } }

		public int[] mask = null;


		public int At(int x, int y)
		{
			int bitSize = y * width + x;
			int left = bitSize % 32;
			int index = bitSize / 32;
			return mask[index] >> left & 1;
		}

		private void _setPoint(int x, int y, int value)
		{
			int bitSize = y * width + x;
			int left = bitSize % 32;
			int index = bitSize / 32;
			mask[index] = mask[index] | value << left;
		}


		public void _setData(int[,] msk, int w, int h, float grid, float minx, float minz)
		{
			width = w;
			height = h;
			gridSize = grid;
			minX = minx;
			minZ = minz;
			mask = new int[(width * height) / 32 + 1];
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					_setPoint(x, y, msk[x, y]);
		}


	}
}
