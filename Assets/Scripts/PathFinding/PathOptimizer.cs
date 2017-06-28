
using System;
using System.Collections.Generic;
using UnityEngine;
using Lite.AStar;


namespace Lite
{

	public static class PathOptimizer
	{
		static List<Point3D> optimalPoints = new List<Point3D>();
		private static bool isPlane = false;


		public static void Optimize(ref List<Point3D> path)
		{
			CombinePoints(ref path);
			SmoothPath(ref path);
		}


		// 合并共线点
		public static void CombinePoints(ref List<Point3D> olds)
		{
			if (olds == null || olds.Count < 3)
				return;

			//List<Point3D> optimalPoints = new List<Point3D>();
			optimalPoints.Clear();
			optimalPoints.Add(olds[0]);

			Point3D ptA = olds[0];
			Point3D ptB = olds[1];
			Point3D ptC;

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


		// 如果两点之间视线毫无阻碍，则省略中间点
		// smooth path
		public static void SmoothPath(ref List<Point3D> olds)
		{
			if (olds == null || olds.Count < 3)
				return;

			//List<Point3D> optimalPoints = new List<Point3D>();
			optimalPoints.Clear();
			optimalPoints.Add(olds[0]);

			isPlane = true;
			for (int i = 1; i < olds.Count; ++i)
			{
				if (olds[i].z > 0)
				{
					isPlane = false;
					break;
				}
			}

			int checkIndex = 0;
			for (int i = 1; i < olds.Count; ++i)
			{
				bool ret = LineCast(olds[checkIndex], olds[i], isPlane);
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
		public static bool LineCast(Point3D from, Point3D to, bool plane)
		{
			if (!plane && Math.Abs(from.y - to.y) > 5)
			{
				return true;
			}
			// y = a*x + b
			Fix64 a_xy = (Fix64)0;
			//Fix64 a_z = (Fix64)0;
			int dx = to.x - from.x;
			int dy = to.y - from.y;
			//int dz = to.z - from.z;

			int z = from.z;
			int fromx = from.x;
			int fromy = from.y;
			//int fromz = from.z;
			int tox = to.x;
			int toy = to.y;

			if (!plane)
			{
				z = from.y;
				dy = to.z - from.z;
				//dz = to.y - from.y;
				fromy = from.z;
				toy = to.z;
				//fromz = from.y;
			}

			if (Math.Abs(dx) > Math.Abs(dy))
			{
				a_xy = (Fix64)dy / (Fix64)dx;
				//a_z = (Fix64)dz / (Fix64)dx;
				if (fromx > tox)
				{
					Point3D tmp = from;
					from = to;
					to = tmp;
				}
				for (int x = fromx + 1; x < tox + 1; x++)
				{
					Fix64 cx = (Fix64)x - (Fix64)0.5f;
					Fix64 y = (Fix64)fromy + a_xy * (cx - (Fix64)fromx);
					int neary = y % (Fix64)1 > (Fix64)0.5f ? (int)(y + (Fix64)0.5f) : (int)y;
					//int z = (int)((Fix64)fromz + a_z * (cx - (Fix64)fromx));

					if (Mathf.Approximately(neary, (float)y))
					{
						if (!IsPassable(x, neary, z))
							return true;
					}
					else if (neary > (float)y)
					{
						if (!IsPassable(x - 1, neary, z))
							return true;
						if (!IsPassable(x - 1, neary - 1, z))
							return true;
						if (!IsPassable(x, neary, z))
							return true;
						if (!IsPassable(x, neary - 1, z))
							return true;
					}
					else
					{
						if (!IsPassable(x - 1, neary + 1, z))
							return true;
						if (!IsPassable(x - 1, neary, z))
							return true;
						if (!IsPassable(x, neary + 1, z))
							return true;
						if (!IsPassable(x, neary, z))
							return true;
					}
				}
			}
			else
			{
				a_xy = (Fix64)dx / (Fix64)dy;
				//a_z = (Fix64)dz / (Fix64)dy;
				if (fromy > toy)
				{
					Point3D tmp = from;
					from = to;
					to = tmp;
				}
				for (int y = fromy + 1; y < toy + 1; y++)
				{
					Fix64 cy = (Fix64)y - (Fix64)0.5f;
					Fix64 x = (Fix64)fromx + a_xy * (cy - (Fix64)fromy);
					int nearx = (Fix64)x % (Fix64)1 > (Fix64)0.5f ? (int)((Fix64)x + (Fix64)0.5f) : (int)x;
					//int z = (int)((Fix64)fromz + a_z * (cy - (Fix64)fromy));

					if (Mathf.Approximately(nearx, (float)x))
					{
						if (!IsPassable(nearx, y, z))
							return true;
					}
					else if (nearx > (float)x)
					{
						if (!IsPassable(nearx, y - 1, z))
							return true;
						if (!IsPassable(nearx - 1, y - 1, z))
							return true;
						if (!IsPassable(nearx, y, z))
							return true;
						if (!IsPassable(nearx - 1, y, z))
							return true;
					}
					else
					{
						if (!IsPassable(nearx + 1, y - 1, z))
							return true;
						if (!IsPassable(nearx, y - 1, z))
							return true;
						if (!IsPassable(nearx + 1, y, z))
							return true;
						if (!IsPassable(nearx, y, z))
							return true;
					}
				}
			}
			
			return false;
		}


		private static bool IsPassable(int x, int y, int z)
		{
			if (!isPlane)
			{
				int tmp = z;
				z = y;
				y = tmp;
			}
			//return BattleMap.Instance.IsPassable(x, y, z);
			return false;
		}

	}

}
