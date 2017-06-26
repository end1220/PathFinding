
using System;
using System.Collections.Generic;
using UnityEngine;

using Lite.AStar;


namespace Lite
{

	public static class PathOptimizer
	{
		static List<Point2D> optimalPoints = new List<Point2D>();


		public static void Optimize(ref List<Point2D> path)
		{
			CombinePoints(ref path);
			SmoothPath(ref path);
		}


		// 合并共线点
		public static void CombinePoints(ref List<Point2D> olds)
		{
			if (olds == null || olds.Count < 3)
				return;

			//List<Point2D> optimalPoints = new List<Point2D>();
			optimalPoints.Clear();
			optimalPoints.Add(olds[0]);

			Point2D ptA = olds[0];
			Point2D ptB = olds[1];
			Point2D ptC;

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


		// smooth path
		public static void SmoothPath(ref List<Point2D> olds)
		{
			if (olds == null || olds.Count < 3)
				return;

			//List<Point2D> optimalPoints = new List<Point2D>();
			optimalPoints.Clear();
			optimalPoints.Add(olds[0]);

			int checkIndex = 0;
			for (int i = 1; i < olds.Count; ++i)
			{
				bool ret = IsPassableBetween(olds[checkIndex], olds[i]);
				if (!ret)
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


		public static bool IsPassableBetween(Point2D from, Point2D to)
		{
			// y = a*x + b
			float a = (float)0;
			int dx = to.x - from.x;
			int dy = to.y - from.y;
			if (Math.Abs(dx) > Math.Abs(dy))
			{
				a = (float)dy / (float)dx;
				if (from.x > to.x)
				{
					Point2D tmp = from;
					from = to;
					to = tmp;
				}
				for (int x = from.x + 1; x < to.x + 1; x++)
				{
					float cx = (float)x - (float)0.5f;
					float y = (float)from.y + a * (cx - (float)from.x);
					int neary = y % (float)1 > (float)0.5f ? (int)(y + (float)0.5f) : (int)y;

					if (Mathf.Approximately(neary, (float)y))
					{
						if (!IsPassable(x, neary))
							return false;
					}
					else if (neary > (float)y)
					{
						if (!IsPassable(x - 1, neary))
							return false;
						if (!IsPassable(x - 1, neary - 1))
							return false;
						if (!IsPassable(x, neary))
							return false;
						if (!IsPassable(x, neary - 1))
							return false;
					}
					else
					{
						if (!IsPassable(x - 1, neary + 1))
							return false;
						if (!IsPassable(x - 1, neary))
							return false;
						if (!IsPassable(x, neary + 1))
							return false;
						if (!IsPassable(x, neary))
							return false;
					}
				}
			}
			else
			{
				a = (float)dx / (float)dy;
				if (from.y > to.y)
				{
					Point2D tmp = from;
					from = to;
					to = tmp;
				}
				for (int y = from.y + 1; y < to.y + 1; y++)
				{
					float cy = (float)y - (float)0.5f;
					float x = (float)from.x + a * (cy - (float)from.y);
					int nearx = (float)x % (float)1 > (float)0.5f ? (int)((float)x + (float)0.5f) : (int)x;

					if (Mathf.Approximately(nearx, (float)x))
					{
						if (!IsPassable(nearx, y))
							return false;
					}
					else if (nearx > (float)x)
					{
						if (!IsPassable(nearx, y - 1))
							return false;
						if (!IsPassable(nearx - 1, y - 1))
							return false;
						if (!IsPassable(nearx, y))
							return false;
						if (!IsPassable(nearx - 1, y))
							return false;
					}
					else
					{
						if (!IsPassable(nearx + 1, y - 1))
							return false;
						if (!IsPassable(nearx, y - 1))
							return false;
						if (!IsPassable(nearx + 1, y))
							return false;
						if (!IsPassable(nearx, y))
							return false;
					}
				}
			}

			return true;
		}


		private static bool IsPassable(int x, int y)
		{
			return false;// BattleMap.Instance.IsPassable(x, y);
		}

	}

}
