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
		public long x;
		public long y;
		public long z;


		public TwVector3(long x, long y, long z)
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

		public void Set(long x, long y, long z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static TwVector3 zero { get { return new TwVector3(0, 0, 0); } }

		public static TwVector3 one { get { return new TwVector3(1, 1, 1); } }

		public long sqrMagnitude { get { return x * x + y * y + z * z; } }

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

		public static TwVector3 operator *(TwVector3 vec, long n)
		{
			return new TwVector3(vec.x * n, vec.y * n, vec.z * n);
		}

		public static TwVector3 operator /(TwVector3 vec, int n)
		{
			return new TwVector3(vec.x / n, vec.y / n, vec.z / n);
		}

		public static TwVector3 operator /(TwVector3 vec, long n)
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

		public long Length()
		{
			return (long)Fix64.Sqrt((Fix64)(x * x + y * y + z * z));
		}

		// 1000, not 1
		public void Normalize()
		{
			long len = Length();

			this.x *= 1000;
			this.y *= 1000;
			this.z *= 1000;

			this.x /= len;
			this.y /= len;
			this.z /= len;

		}

		public long DistanceSqr(TwVector3 vec)
		{
			long dx = this.x - vec.x;
			long dy = this.y - vec.y;
			long dz = this.z - vec.z;
			return dx * dx + dy * dy + dz * dz;
		}

		public long DistancePlaneSqr(TwVector3 vec)
		{
			long dx = this.x - vec.x;
			long dz = this.z - vec.z;
			return dx * dx + dz * dz;
		}

		public long Distance(TwVector3 vec)
		{
			return (long)System.Math.Sqrt(DistanceSqr(vec));
		}

		public long Cross(TwVector3 vec)
		{
			return this.x * vec.x + this.y * vec.y + this.z * vec.z;
		}

		public UnityEngine.Vector3 ToVector3()
		{
			return new UnityEngine.Vector3(TwMath.mm2m(x), TwMath.mm2m(y), TwMath.mm2m(z));
		}

		public long Dot(TwVector3 lhs, TwVector3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		public TwVector3 Normal()
		{
			long len = Length();
			this.x = TwMath.m2mm(this.x);
			this.y = TwMath.m2mm(this.y);
			this.z = TwMath.m2mm(this.z);
			this.x /= len;
			this.y /= len;
			this.z /= len;
			return this;
		}

		public override bool Equals(object o)
		{
			return base.Equals(o);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public string ToString()
		{
			return string.Format("{0},{1},{2}", x, y, z);
		}


	}

}