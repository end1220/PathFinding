

using UnityEngine;



[System.Serializable]
public struct Int3
{
	public int x;
	public int y;
	public int z;


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
		return new Vector3(x / 1000.0f, y / 1000.0f, z / 1000.0f);
	}

	private static int m2mm(float m)
	{
		return (int)(m * 1000);
	}

	public static Int3 zero { get { return new Int3(0, 0, 0); } }

	public static Int3 one { get { return new Int3(1, 1, 1); } }

	public int sqrMagnitude { get { return x * x + y * y + z * z; } }

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

	public override bool Equals(object o)
	{
		return base.Equals(o);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

}


