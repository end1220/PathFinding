

using System;


namespace FixedPoint
{
	[Serializable]
	public struct Int2
	{
		public int x;
		public int y;


		public Int2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		private static int m2mm(float m)
		{
			return (int)(m*1000);
		}

		public static Int2 zero { get { return new Int2(0, 0); } }

		public static Int2 one { get { return new Int2(1, 1); } }

		public int sqrMagnitude { get { return x * x + y * y; } }

		public static Int2 operator +(Int2 vec1, Int2 vec2)
		{
			return new Int2(vec1.x + vec2.x, vec1.y + vec2.y);
		}

		public static Int2 operator -(Int2 vec1, Int2 vec2)
		{
			return new Int2(vec1.x - vec2.x, vec1.y - vec2.y);
		}

		public static Int2 operator *(Int2 vec, int n)
		{
			return new Int2(vec.x * n, vec.y * n);
		}

		public static Int2 operator /(Int2 vec, int n)
		{
			return new Int2(vec.x / n, vec.y / n);
		}

		public static bool operator ==(Int2 vec1, Int2 vec2)
		{
			return vec1.x == vec2.x && vec1.y == vec2.y;
		}

		public static bool operator !=(Int2 vec1, Int2 vec2)
		{
			return vec1.x != vec2.x || vec1.y != vec2.y;
		}

		public override bool Equals(object o)
		{
			return base.Equals(o);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	}

}


