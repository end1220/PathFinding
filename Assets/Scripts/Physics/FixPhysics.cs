

namespace FixedPoint
{

	public class FixPhysics
	{
		/// <summary>
		/// 判断点是否在多边形内（仅限xz平面上）
		/// </summary>
		/// <param name="point"></param>
		/// <param name="poly"></param>
		/// <returns></returns>
		public static bool IsPointInPoly(Int3 p, Int3[] poly)
		{
			int count = poly.Length;
			if (count < 3)
				return false;

			bool result = false;
			for (int i = 0, j = count - 1; i < count; i++)
			{
				var p1 = poly[i];
				var p2 = poly[j];

				if (p1.x < p.x && p2.x >= p.x || p2.x < p.x && p1.x >= p.x)
				{
					if ((Fix64)p1.z + (Fix64)(p.x - p1.x) / (Fix64)(p2.x - p1.x) * (Fix64)(p2.z - p1.z) < (Fix64)p.z)
					{
						result = !result;
					}
				}
				j = i;
			}
			return result;
		}

	}

}