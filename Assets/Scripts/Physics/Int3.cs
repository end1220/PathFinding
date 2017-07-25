

using UnityEngine;



[System.Serializable]
public struct Int3
{
	public int x;
	public int y;
	public int z;


	public const int Precision = 1000;

	public const float FloatPrecision = 1000F;

	public const float PrecisionFactor = 0.001F;


	public Int3(int x, int y)
	{
		this.x = x;
		this.y = y;
		this.z = 0;
	}

	public Int3(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Int3(Vector3 vec)
	{
		x = m2mm(vec.x);
		y = m2mm(vec.y);
		z = m2mm(vec.z);
	}

	public Int3(FixVector3 vec)
	{
		x = (int)(vec.x);
		y = (int)(vec.y);
		z = (int)(vec.z);
	}

	public Vector3 ToVector3()
	{
		return new Vector3(x * PrecisionFactor, y * PrecisionFactor, z * PrecisionFactor);
	}

	public static explicit operator Int3(Vector3 ob)
	{
		return new Int3(
			m2mm(ob.x),
			m2mm(ob.y),
			m2mm(ob.z)
			);
	}

	public static explicit operator Vector3(Int3 ob)
	{
		return new Vector3(ob.x * PrecisionFactor, ob.y * PrecisionFactor, ob.z * PrecisionFactor);
	}

	private static int m2mm(float m)
	{
		return (int)(m * Precision);
	}

	public static Int3 zero { get { return new Int3(0, 0, 0); } }

	public static Int3 one { get { return new Int3(1, 1, 1); } }

	public int sqrMagnitude { get { return x * x + y * y + z * z; } }

	public float magnitude
	{
		get
		{
			double _x = x;
			double _y = y;
			double _z = z;

			return (float)System.Math.Sqrt(_x * _x + _y * _y + _z * _z);
		}
	}

	public int costMagnitude
	{
		get
		{
			return (int)System.Math.Round(magnitude);
		}
	}

	public long sqrMagnitudeLong
	{
		get
		{
			long _x = x;
			long _y = y;
			long _z = z;
			return (_x * _x + _y * _y + _z * _z);
		}
	}

	public int this[int i]
	{
		get
		{
			return i == 0 ? x : (i == 1 ? y : z);
		}
		set
		{
			if (i == 0) x = value;
			else if (i == 1) y = value;
			else z = value;
		}
	}

	public static Int3 operator +(Int3 vec1, Int3 vec2)
	{
		return new Int3(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
	}

	public static Int3 operator -(Int3 vec1, Int3 vec2)
	{
		return new Int3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
	}

	public static Int3 operator *(Int3 vec, int n)
	{
		return new Int3(vec.x * n, vec.y * n, vec.z * n);
	}

	public static Int3 operator /(Int3 vec, int n)
	{
		return new Int3(vec.x / n, vec.y / n, vec.z / n);
	}

	public static bool operator ==(Int3 vec1, Int3 vec2)
	{
		return vec1.x == vec2.x && vec1.y == vec2.y && vec1.z == vec2.z;
	}

	public static bool operator !=(Int3 vec1, Int3 vec2)
	{
		return vec1.x != vec2.x || vec1.y != vec2.y || vec1.z != vec2.z;
	}

	public static Int3 operator *(Int3 lhs, Vector3 rhs)
	{
		lhs.x = (int)System.Math.Round(lhs.x * rhs.x);
		lhs.y = (int)System.Math.Round(lhs.y * rhs.y);
		lhs.z = (int)System.Math.Round(lhs.z * rhs.z);

		return lhs;
	}

	public static Int3 operator *(Int3 lhs, float rhs)
	{
		lhs.x = (int)System.Math.Round(lhs.x * rhs);
		lhs.y = (int)System.Math.Round(lhs.y * rhs);
		lhs.z = (int)System.Math.Round(lhs.z * rhs);

		return lhs;
	}

	public static int Dot(Int3 lhs, Int3 rhs)
	{
		return
			lhs.x * rhs.x +
			lhs.y * rhs.y +
			lhs.z * rhs.z;
	}

	public static long DotLong(Int3 lhs, Int3 rhs)
	{
		return
			(long)lhs.x * (long)rhs.x +
			(long)lhs.y * (long)rhs.y +
			(long)lhs.z * (long)rhs.z;
	}

	/** Normal in 2D space (XZ).
	 * Equivalent to Cross(this, Int3(0,1,0) )
	 * except that the Y coordinate is left unchanged with this operation.
	 */
	public Int3 Normal2D()
	{
		return new Int3(z, y, -x);
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


