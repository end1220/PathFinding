using UnityEngine;
using System.Collections;

namespace Lite
{

	/*
	 * TwVector3目的是用整数替代浮点数，从而避免浮点数计算的精度问题。
	 * 单位:mm
	 * 
	 */
	[System.Serializable]
	public struct TwVector3
	{
		public int x;
		public int y;
		public int z;


		public TwVector3(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public TwVector3(UnityEngine.Vector3 vec)
		{
			this.x = TwMath.m2mm(vec.x);
			this.y = TwMath.m2mm(vec.y);
			this.z = TwMath.m2mm(vec.z);
		}

		public void Set(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static TwVector3 zero { get { return new TwVector3(0, 0, 0); } }

		public static TwVector3 one { get { return new TwVector3(1, 1, 1); } }

		public long sqrMagnitude { get { return (long)x * (long)x + (long)y * (long)y + (long)z * (long)z; } }

		public static TwVector3 operator +(TwVector3 vec1, TwVector3 vec2)
		{
			return new TwVector3(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
		}

		public static TwVector3 operator -(TwVector3 vec1, TwVector3 vec2)
		{
			return new TwVector3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
		}

		public static TwVector3 operator *(TwVector3 vec, int n)
		{
			return new TwVector3(vec.x * n, vec.y * n, vec.z * n);
		}

		public static TwVector3 operator /(TwVector3 vec, int n)
		{
			return new TwVector3(vec.x / n, vec.y / n, vec.z / n);
		}

		public static bool operator ==(TwVector3 vec1, TwVector3 vec2)
		{
			return vec1.x == vec2.x && vec1.y == vec2.y && vec1.z == vec2.z;
		}

		public static bool operator !=(TwVector3 vec1, TwVector3 vec2)
		{
			return vec1.x != vec2.x || vec1.y != vec2.y || vec1.z != vec2.z;
		}

		public int Length()
		{
			return (int)Fix64.Sqrt((Fix64)sqrMagnitude);
		}

		// 1000, not 1
		public void Normalize()
		{
			int len = Length();

			this.x *= 1000;
			this.y *= 1000;
			this.z *= 1000;

			this.x /= len;
			this.y /= len;
			this.z /= len;

		}

		public long DistanceSqr(TwVector3 vec)
		{
			return (this - vec).sqrMagnitude;
		}

		public long DistancePlaneSqr(TwVector3 vec)
		{
			var tmp = this - vec;
			tmp.y = 0;
			return tmp.sqrMagnitude;
		}

		public UnityEngine.Vector3 ToVector3()
		{
			return new UnityEngine.Vector3(TwMath.mm2m(x), TwMath.mm2m(y), TwMath.mm2m(z));
		}

		public override bool Equals(object o)
		{
			return base.Equals(o);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0},{1},{2}", x, y, z);
		}


	}

}