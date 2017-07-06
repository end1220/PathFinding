using UnityEngine;
using System.Collections;

namespace Lite
{

	/*
	 * FixVector3目的是用整数替代浮点数，从而避免浮点数计算的精度问题。
	 * 单位:mm
	 * 
	 */
	[System.Serializable]
	public struct FixVector3
	{
		public int x;
		public int y;
		public int z;


		public FixVector3(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public FixVector3(UnityEngine.Vector3 vec)
		{
			this.x = FixMath.m2mm(vec.x);
			this.y = FixMath.m2mm(vec.y);
			this.z = FixMath.m2mm(vec.z);
		}

		public void Set(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static FixVector3 zero { get { return new FixVector3(0, 0, 0); } }

		public static FixVector3 one { get { return new FixVector3(1, 1, 1); } }

		public long sqrMagnitude { get { return (long)x * (long)x + (long)y * (long)y + (long)z * (long)z; } }

		public static FixVector3 operator +(FixVector3 vec1, FixVector3 vec2)
		{
			return new FixVector3(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
		}

		public static FixVector3 operator -(FixVector3 vec1, FixVector3 vec2)
		{
			return new FixVector3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
		}

		public static FixVector3 operator *(FixVector3 vec, int n)
		{
			return new FixVector3(vec.x * n, vec.y * n, vec.z * n);
		}

		public static FixVector3 operator /(FixVector3 vec, int n)
		{
			return new FixVector3(vec.x / n, vec.y / n, vec.z / n);
		}

		public static bool operator ==(FixVector3 vec1, FixVector3 vec2)
		{
			return vec1.x == vec2.x && vec1.y == vec2.y && vec1.z == vec2.z;
		}

		public static bool operator !=(FixVector3 vec1, FixVector3 vec2)
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

		public long DistanceSqr(FixVector3 vec)
		{
			return (this - vec).sqrMagnitude;
		}

		public long DistancePlaneSqr(FixVector3 vec)
		{
			var tmp = this - vec;
			tmp.y = 0;
			return tmp.sqrMagnitude;
		}

		public UnityEngine.Vector3 ToVector3()
		{
			return new UnityEngine.Vector3(FixMath.mm2m(x), FixMath.mm2m(y), FixMath.mm2m(z));
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