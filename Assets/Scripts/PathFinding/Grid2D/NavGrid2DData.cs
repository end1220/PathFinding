﻿
using System;
using UnityEngine;
using System.Collections.Generic;


namespace Lite.AStar
{
	/// <summary>
	/// 存储着2D网格寻路数据
	/// </summary>
	//[CreateAssetMenu(menuName = "Lite/Navigation Data", order = 2)]
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


		public void _setData(int[,] msk, int w, int h, int grid, int minx, int minz)
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