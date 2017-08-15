
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PathFinding
{

	public static class Grid2DPathOptimizer
	{
		static List<Int2> optimalPoints = new List<Int2>();

		static Grid2DGraph gridMap;

		public static void Optimize(Grid2DGraph map, ref List<Int2> path)
		{
			gridMap = map;
			CombinePoints(ref path);
			SmoothPath(ref path);
		}


		// 合并共线点
		public static void CombinePoints(ref List<Int2> olds)
		{
			if (olds == null || olds.Count < 3)
				return;

			optimalPoints.Clear();
			optimalPoints.Add(olds[0]);

			Int2 ptA = olds[0];
			Int2 ptB = olds[1];
			Int2 ptC;

			for (int i = 2; i < olds.Count; ++i)
			{
				ptC = olds[i];
				if (ptB.x - ptA.x != ptC.x - ptB.x || ptB.y - ptA.y != ptC.y - ptB.y)
				{
					optimalPoints.Add(ptB);
				}

				ptA = ptB;
				ptB = ptC;

				if (i == olds.Count - 1)
					optimalPoints.Add(olds[i]);
			}

			olds.Clear();
			olds.AddRange(optimalPoints);

		}


		// 如果两点之间毫无阻碍，则省略中间点
		// smooth path
		public static void SmoothPath(ref List<Int2> olds)
		{
			if (olds == null || olds.Count < 3)
				return;

			optimalPoints.Clear();
			optimalPoints.Add(olds[0]);

			int checkIndex = 0;
			for (int i = 1; i < olds.Count; ++i)
			{
				bool ret = LineCast(olds[checkIndex], olds[i]);
				if (ret)
				{
					checkIndex = i - 1;
					optimalPoints.Add(olds[i - 1]);
				}

				if (i == olds.Count - 1)
					optimalPoints.Add(olds[i]);
			}

			olds.Clear();
			olds.AddRange(optimalPoints);

		}

		/// <summary>
		/// return true if hit obstacles between.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="plane"></param>
		/// <returns></returns>
		public static bool LineCast(Int2 from, Int2 to)
		{
			// y = a*x + b
			Fix64 a_xy = (Fix64)0;
			int dx = to.x - from.x;
			int dy = to.y - from.y;

			if (Math.Abs(dx) > Math.Abs(dy))
			{
				a_xy = (Fix64)dy / (Fix64)dx;
				if (from.x > to.x)
				{
					Int2 tmp = from;
					from = to;
					to = tmp;
				}
				for (int x = from.x + 1; x < to.x + 1; x++)
				{
					Fix64 cx = (Fix64)x - (Fix64)0.5f;
					Fix64 y = (Fix64)from.y + a_xy * (cx - (Fix64)from.x);
					int neary = y % (Fix64)1 > (Fix64)0.5f ? (int)(y + (Fix64)0.5f) : (int)y;

					if (Mathf.Approximately(neary, (float)y))
					{
						if (!IsNodePassable(x, neary))
							return true;
					}
					else if (neary > (float)y)
					{
						if (!IsNodePassable(x - 1, neary))
							return true;
						if (!IsNodePassable(x - 1, neary - 1))
							return true;
						if (!IsNodePassable(x, neary))
							return true;
						if (!IsNodePassable(x, neary - 1))
							return true;
					}
					else
					{
						if (!IsNodePassable(x - 1, neary + 1))
							return true;
						if (!IsNodePassable(x - 1, neary))
							return true;
						if (!IsNodePassable(x, neary + 1))
							return true;
						if (!IsNodePassable(x, neary))
							return true;
					}
				}
			}
			else
			{
				a_xy = (Fix64)dx / (Fix64)dy;
				if (from.y > to.y)
				{
					Int2 tmp = from;
					from = to;
					to = tmp;
				}
				for (int y = from.y + 1; y < to.y + 1; y++)
				{
					Fix64 cy = (Fix64)y - (Fix64)0.5f;
					Fix64 x = (Fix64)from.x + a_xy * (cy - (Fix64)from.y);
					int nearx = (Fix64)x % (Fix64)1 > (Fix64)0.5f ? (int)((Fix64)x + (Fix64)0.5f) : (int)x;

					if (Mathf.Approximately(nearx, (float)x))
					{
						if (!IsNodePassable(nearx, y))
							return true;
					}
					else if (nearx > (float)x)
					{
						if (!IsNodePassable(nearx, y - 1))
							return true;
						if (!IsNodePassable(nearx - 1, y - 1))
							return true;
						if (!IsNodePassable(nearx, y))
							return true;
						if (!IsNodePassable(nearx - 1, y))
							return true;
					}
					else
					{
						if (!IsNodePassable(nearx + 1, y - 1))
							return true;
						if (!IsNodePassable(nearx, y - 1))
							return true;
						if (!IsNodePassable(nearx + 1, y))
							return true;
						if (!IsNodePassable(nearx, y))
							return true;
					}
				}
			}
			
			return false;
		}


		private static bool IsNodePassable(int x, int y)
		{
			return gridMap.IsNodePassable(x, y);
		}

	}

}
