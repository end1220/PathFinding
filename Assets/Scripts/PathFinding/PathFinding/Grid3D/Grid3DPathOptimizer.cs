
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PathFinding
{

	public static class Grid3DPathOptimizer
	{
		static List<Int3> optimalPoints = new List<Int3>();
		static Grid3DGraph graphMap;


		public static void Optimize(Grid3DGraph map, ref List<Int3> path)
		{
			graphMap = map;
			CombinePoints(ref path);
			SmoothPath(ref path);
		}


		// 合并共线点
		public static void CombinePoints(ref List<Int3> olds)
		{
			if (olds == null || olds.Count < 3)
				return;

			optimalPoints.Clear();
			optimalPoints.Add(olds[0]);

			Int3 ptA = olds[0];
			Int3 ptB = olds[1];
			Int3 ptC;

			for (int i = 2; i < olds.Count; ++i)
			{
				ptC = olds[i];
				if (ptB.x - ptA.x != ptC.x - ptB.x || ptB.y - ptA.y != ptC.y - ptB.y || ptB.z - ptA.z != ptC.z - ptB.z)
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
		public static void SmoothPath(ref List<Int3> olds)
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
		public static bool LineCast(Int3 from, Int3 to)
		{
			// y = a*x + b
			Fix64 a_xz = (Fix64)0;
			Fix64 a_y = (Fix64)0;
			int dx = to.x - from.x;
			int dy = to.y - from.y;
			int dz = to.z - from.z;

			int fromx = from.x;
			int fromy = from.y;
			int fromz = from.z;
			int tox = to.x;
			int toz = to.z;

			if (Math.Abs(dx) > Math.Abs(dz))
			{
				a_xz = (Fix64)dz / (Fix64)dx;
				a_y = (Fix64)dy / (Fix64)dx;
				if (fromx > tox)
				{
					Int3 tmp = from;
					from = to;
					to = tmp;
				}
				for (int x = fromx + 1; x < tox + 1; x++)
				{
					Fix64 cx = (Fix64)x - (Fix64)0.5f;
					Fix64 z = (Fix64)fromz + a_xz * (cx - (Fix64)fromx);
					int nearz = z % (Fix64)1 > (Fix64)0.5f ? (int)(z + (Fix64)0.5f) : (int)z;
					int y = (int)((Fix64)fromy + a_y * (cx - (Fix64)fromx));

					if (Mathf.Approximately(nearz, (float)z))
					{
						if (!IsNodePassable(x, y, nearz))
							return true;
					}
					else if (nearz > (float)z)
					{
						if (!IsNodePassable(x - 1, y, nearz))
							return true;
						if (!IsNodePassable(x - 1, y, nearz - 1))
							return true;
						if (!IsNodePassable(x, y, nearz))
							return true;
						if (!IsNodePassable(x, y, nearz - 1))
							return true;
					}
					else
					{
						if (!IsNodePassable(x - 1, y, nearz + 1))
							return true;
						if (!IsNodePassable(x - 1, y, nearz))
							return true;
						if (!IsNodePassable(x, y, nearz + 1))
							return true;
						if (!IsNodePassable(x, y, nearz))
							return true;
					}
				}
			}
			else
			{
				a_xz = (Fix64)dx / (Fix64)dz;
				a_y = (Fix64)dy / (Fix64)dz;
				if (fromz > toz)
				{
					Int3 tmp = from;
					from = to;
					to = tmp;
				}
				for (int z = fromz + 1; z < toz + 1; z++)
				{
					Fix64 cz = (Fix64)z - (Fix64)0.5f;
					Fix64 x = (Fix64)fromx + a_xz * (cz - (Fix64)fromz);
					int nearx = (Fix64)x % (Fix64)1 > (Fix64)0.5f ? (int)((Fix64)x + (Fix64)0.5f) : (int)x;
					int y = (int)((Fix64)fromy + a_y * (cz - (Fix64)fromz));

					if (Mathf.Approximately(nearx, (float)x))
					{
						if (!IsNodePassable(nearx, y, z))
							return true;
					}
					else if (nearx > (float)x)
					{
						if (!IsNodePassable(nearx, y, z - 1))
							return true;
						if (!IsNodePassable(nearx - 1, y, z - 1))
							return true;
						if (!IsNodePassable(nearx, y, z))
							return true;
						if (!IsNodePassable(nearx - 1, y, z))
							return true;
					}
					else
					{
						if (!IsNodePassable(nearx + 1, y, z - 1))
							return true;
						if (!IsNodePassable(nearx, y, z - 1))
							return true;
						if (!IsNodePassable(nearx + 1, y, z))
							return true;
						if (!IsNodePassable(nearx, y, z))
							return true;
					}
				}
			}
			
			return false;
		}


		/*private static bool IsNodeEmpty(int x, int y, int z)
		{
			return graphMap.IsNodeEmpty(x, y, z);
		}*/


		private static bool IsNodePassable(int x, int y, int z)
		{
			return graphMap.IsNodePassable(x, y, z);
		}

	}

}
