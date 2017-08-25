using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct Int3
{
    public const int Precision = 0x3e8;
    public const float FloatPrecision = 1000f;
    public const float PrecisionFactor = 0.001f;
    public int x;
    public int y;
    public int z;
    public static readonly Int3 zero;
    public static readonly Int3 one;
    public static readonly Int3 half;
    public static readonly Int3 forward;
    public static readonly Int3 up;
    public static readonly Int3 right;


    public Int3(Vector3 position)
    {
        this.x = (int) Math.Round((double) (position.x * 1000f));
        this.y = (int) Math.Round((double) (position.y * 1000f));
        this.z = (int) Math.Round((double) (position.z * 1000f));
    }

    public Int3(int _x, int _y, int _z)
    {
        this.x = _x;
        this.y = _y;
        this.z = _z;
    }

    static Int3()
    {
        zero = new Int3(0, 0, 0);
        one = new Int3(0x3e8, 0x3e8, 0x3e8);
        half = new Int3(500, 500, 500);
        forward = new Int3(0, 0, 0x3e8);
        up = new Int3(0, 0x3e8, 0);
        right = new Int3(0x3e8, 0, 0);
    }

	public void Set(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3 ToVector3()
	{
		return new Vector3(x * 0.001f, y * 0.001f, z * 0.001f);
	}

    public Int3 DivBy2()
    {
        this.x = this.x >> 1;
        this.y = this.y >> 1;
        this.z = this.z >> 1;
        return this;
    }

	public int this[int i]
    {
        get
        {
            return ((i != 0) ? ((i != 1) ? this.z : this.y) : this.x);
        }
        set
        {
            if (i == 0)
            {
                this.x = value;
            }
            else if (i == 1)
            {
                this.y = value;
            }
            else
            {
                this.z = value;
            }
        }
    }
    public static float Angle(Int3 lhs, Int3 rhs)
    {
        double d = ((double) Dot(lhs, rhs)) / (lhs.magnitude * rhs.magnitude);
        d = (d >= -1.0) ? ((d <= 1.0) ? d : 1.0) : -1.0;
        return (float) Math.Acos(d);
    }

    public static VFactor AngleInt(Int3 lhs, Int3 rhs)
    {
        long den = lhs.magnitude * rhs.magnitude;
        return IntMath.acos((long) Dot(ref lhs, ref rhs), den);
    }

    public static int Dot(ref Int3 lhs, ref Int3 rhs)
    {
        return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
    }

    public static int Dot(Int3 lhs, Int3 rhs)
    {
        return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
    }

    public static long DotLong(Int3 lhs, Int3 rhs)
    {
        return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
    }

    public static long DotLong(ref Int3 lhs, ref Int3 rhs)
    {
        return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
    }

    public static long DotXZLong(ref Int3 lhs, ref Int3 rhs)
    {
        return ((lhs.x * rhs.x) + (lhs.z * rhs.z));
    }

    public static long DotXZLong(Int3 lhs, Int3 rhs)
    {
        return ((lhs.x * rhs.x) + (lhs.z * rhs.z));
    }

    public static Int3 Cross(ref Int3 lhs, ref Int3 rhs)
    {
        return new Int3(IntMath.Divide((int) ((lhs.y * rhs.z) - (lhs.z * rhs.y)), 0x3e8), IntMath.Divide((int) ((lhs.z * rhs.x) - (lhs.x * rhs.z)), 0x3e8), IntMath.Divide((int) ((lhs.x * rhs.y) - (lhs.y * rhs.x)), 0x3e8));
    }

    public static Int3 Cross(Int3 lhs, Int3 rhs)
    {
        return new Int3(IntMath.Divide((int) ((lhs.y * rhs.z) - (lhs.z * rhs.y)), 0x3e8), IntMath.Divide((int) ((lhs.z * rhs.x) - (lhs.x * rhs.z)), 0x3e8), IntMath.Divide((int) ((lhs.x * rhs.y) - (lhs.y * rhs.x)), 0x3e8));
    }

    public static Int3 MoveTowards(Int3 from, Int3 to, int dt)
    {
        Int3 num2 = to - from;
        if (num2.sqrMagnitudeLong <= (dt * dt))
        {
            return to;
        }
        Int3 num = to - from;
        return (from + num.NormalizeTo(dt));
    }

    public Int3 Normal2D()
    {
        return new Int3(this.z, this.y, -this.x);
    }

    public Int3 NormalizeTo(int newMagn)
    {
        long num = this.x * 100;
        long num2 = this.y * 100;
        long num3 = this.z * 100;
        long a = ((num * num) + (num2 * num2)) + (num3 * num3);
        if (a != 0)
        {
            long b = IntMath.Sqrt(a);
            long num6 = newMagn;
            this.x = (int) IntMath.Divide((long) (num * num6), b);
            this.y = (int) IntMath.Divide((long) (num2 * num6), b);
            this.z = (int) IntMath.Divide((long) (num3 * num6), b);
        }
        return this;
    }

    public long Normalize()
    {
        long num = this.x << 7;
        long num2 = this.y << 7;
        long num3 = this.z << 7;
        long a = ((num * num) + (num2 * num2)) + (num3 * num3);
        if (a == 0)
        {
            return 0L;
        }
        long b = IntMath.Sqrt(a);
        long num6 = 0x3e8L;
        this.x = (int) IntMath.Divide((long) (num * num6), b);
        this.y = (int) IntMath.Divide((long) (num2 * num6), b);
        this.z = (int) IntMath.Divide((long) (num3 * num6), b);
        return (b >> 7);
    }

    public Vector3 vec3
    {
        get
        {
            return new Vector3(this.x * 0.001f, this.y * 0.001f, this.z * 0.001f);
        }
    }
    public Int2 xz
    {
        get
        {
            return new Int2(this.x, this.z);
        }
    }
    public int magnitude
    {
        get
        {
            long x = this.x;
            long y = this.y;
            long z = this.z;
            return IntMath.Sqrt(((x * x) + (y * y)) + (z * z));
        }
    }
    public int magnitude2D
    {
        get
        {
            long x = this.x;
            long z = this.z;
            return IntMath.Sqrt((x * x) + (z * z));
        }
    }
    public Int3 RotateY(ref VFactor radians)
    {
        Int3 num;
        VFactor factor;
        VFactor factor2;
        IntMath.sincos(out factor, out factor2, radians.nom, radians.den);
        long num2 = factor2.nom * factor.den;
        long num3 = factor2.den * factor.nom;
        long b = factor2.den * factor.den;
        num.x = (int) IntMath.Divide((long) ((this.x * num2) + (this.z * num3)), b);
        num.z = (int) IntMath.Divide((long) ((-this.x * num3) + (this.z * num2)), b);
        num.y = 0;
        return num.NormalizeTo(0x3e8);
    }

    public Int3 RotateY(int degree)
    {
        Int3 num;
        VFactor factor;
        VFactor factor2;
        IntMath.sincos(out factor, out factor2, (long) (0x7ab8 * degree), 0x1b7740L);
        long num2 = factor2.nom * factor.den;
        long num3 = factor2.den * factor.nom;
        long b = factor2.den * factor.den;
        num.x = (int) IntMath.Divide((long) ((this.x * num2) + (this.z * num3)), b);
        num.z = (int) IntMath.Divide((long) ((-this.x * num3) + (this.z * num2)), b);
        num.y = 0;
        return num.NormalizeTo(0x3e8);
    }

    public int costMagnitude
    {
        get
        {
            return this.magnitude;
        }
    }
    public float worldMagnitude
    {
        get
        {
            double x = this.x;
            double y = this.y;
            double z = this.z;
            return (((float) Math.Sqrt(((x * x) + (y * y)) + (z * z))) * 0.001f);
        }
    }
	public long sqrLength
	{
		get { return sqrMagnitudeLong; }
	}

	public double sqrMagnitude
    {
        get
        {
            double x = this.x;
            double y = this.y;
            double z = this.z;
            return (((x * x) + (y * y)) + (z * z));
        }
    }
    public long sqrMagnitudeLong
    {
        get
        {
            long x = this.x;
            long y = this.y;
            long z = this.z;
            return (((x * x) + (y * y)) + (z * z));
        }
    }
    public long sqrMagnitudeLong2D
    {
        get
        {
            long x = this.x;
            long z = this.z;
            return ((x * x) + (z * z));
        }
    }
    public int unsafeSqrMagnitude
    {
        get
        {
            return (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
        }
    }
    public Int3 abs
    {
        get
        {
            return new Int3(Math.Abs(this.x), Math.Abs(this.y), Math.Abs(this.z));
        }
    }
    [Obsolete("Same implementation as .magnitude")]
    public float safeMagnitude
    {
        get
        {
            double x = this.x;
            double y = this.y;
            double z = this.z;
            return (float) Math.Sqrt(((x * x) + (y * y)) + (z * z));
        }
    }
    [Obsolete(".sqrMagnitude is now per default safe (.unsafeSqrMagnitude can be used for unsafe operations)")]
    public float safeSqrMagnitude
    {
        get
        {
            float num = this.x * 0.001f;
            float num2 = this.y * 0.001f;
            float num3 = this.z * 0.001f;
            return (((num * num) + (num2 * num2)) + (num3 * num3));
        }
    }
    public override string ToString()
    {
        object[] objArray1 = new object[] { "( ", this.x, ", ", this.y, ", ", this.z, ")" };
        return string.Concat(objArray1);
    }

    public override bool Equals(object o)
    {
        if (o == null)
        {
            return false;
        }
        Int3 num = (Int3) o;
        return (((this.x == num.x) && (this.y == num.y)) && (this.z == num.z));
    }

    public override int GetHashCode()
    {
        return (((this.x * 0x466f45d) ^ (this.y * 0x127409f)) ^ (this.z * 0x4f9ffb7));
    }

    public static Int3 Lerp(Int3 a, Int3 b, float f)
    {
        return new Int3(Mathf.RoundToInt(a.x * (1f - f)) + Mathf.RoundToInt(b.x * f), Mathf.RoundToInt(a.y * (1f - f)) + Mathf.RoundToInt(b.y * f), Mathf.RoundToInt(a.z * (1f - f)) + Mathf.RoundToInt(b.z * f));
    }

    public static Int3 Lerp(Int3 a, Int3 b, VFactor f)
    {
        return new Int3(((int) IntMath.Divide((long) ((b.x - a.x) * f.nom), f.den)) + a.x, ((int) IntMath.Divide((long) ((b.y - a.y) * f.nom), f.den)) + a.y, ((int) IntMath.Divide((long) ((b.z - a.z) * f.nom), f.den)) + a.z);
    }

    public static Int3 Lerp(Int3 a, Int3 b, int factorNom, int factorDen)
    {
        return new Int3(IntMath.Divide((int) ((b.x - a.x) * factorNom), factorDen) + a.x, IntMath.Divide((int) ((b.y - a.y) * factorNom), factorDen) + a.y, IntMath.Divide((int) ((b.z - a.z) * factorNom), factorDen) + a.z);
    }

    public long XZSqrMagnitude(Int3 rhs)
    {
        long num = this.x - rhs.x;
        long num2 = this.z - rhs.z;
        return ((num * num) + (num2 * num2));
    }

    public long XZSqrMagnitude(ref Int3 rhs)
    {
        long num = this.x - rhs.x;
        long num2 = this.z - rhs.z;
        return ((num * num) + (num2 * num2));
    }

    public bool IsEqualXZ(Int3 rhs)
    {
        return ((this.x == rhs.x) && (this.z == rhs.z));
    }

    public bool IsEqualXZ(ref Int3 rhs)
    {
        return ((this.x == rhs.x) && (this.z == rhs.z));
    }

    public static bool operator ==(Int3 lhs, Int3 rhs)
    {
        return (((lhs.x == rhs.x) && (lhs.y == rhs.y)) && (lhs.z == rhs.z));
    }

    public static bool operator !=(Int3 lhs, Int3 rhs)
    {
        return (((lhs.x != rhs.x) || (lhs.y != rhs.y)) || (lhs.z != rhs.z));
    }

    public static explicit operator Int3(Vector3 ob)
    {
        return new Int3((int) Math.Round((double) (ob.x * 1000f)), (int) Math.Round((double) (ob.y * 1000f)), (int) Math.Round((double) (ob.z * 1000f)));
    }

    public static explicit operator Vector3(Int3 ob)
    {
        return new Vector3(ob.x * 0.001f, ob.y * 0.001f, ob.z * 0.001f);
    }

    public static Int3 operator -(Int3 lhs, Int3 rhs)
    {
        lhs.x -= rhs.x;
        lhs.y -= rhs.y;
        lhs.z -= rhs.z;
        return lhs;
    }

    public static Int3 operator -(Int3 lhs)
    {
        lhs.x = -lhs.x;
        lhs.y = -lhs.y;
        lhs.z = -lhs.z;
        return lhs;
    }

    public static Int3 operator +(Int3 lhs, Int3 rhs)
    {
        lhs.x += rhs.x;
        lhs.y += rhs.y;
        lhs.z += rhs.z;
        return lhs;
    }

    public static Int3 operator *(Int3 lhs, int rhs)
    {
        lhs.x *= rhs;
        lhs.y *= rhs;
        lhs.z *= rhs;
        return lhs;
    }

    public static Int3 operator *(Int3 lhs, float rhs)
    {
        lhs.x = (int) Math.Round((double) (lhs.x * rhs));
        lhs.y = (int) Math.Round((double) (lhs.y * rhs));
        lhs.z = (int) Math.Round((double) (lhs.z * rhs));
        return lhs;
    }

    public static Int3 operator *(Int3 lhs, double rhs)
    {
        lhs.x = (int) Math.Round((double) (lhs.x * rhs));
        lhs.y = (int) Math.Round((double) (lhs.y * rhs));
        lhs.z = (int) Math.Round((double) (lhs.z * rhs));
        return lhs;
    }

    public static Int3 operator *(Int3 lhs, Vector3 rhs)
    {
        lhs.x = (int) Math.Round((double) (lhs.x * rhs.x));
        lhs.y = (int) Math.Round((double) (lhs.y * rhs.y));
        lhs.z = (int) Math.Round((double) (lhs.z * rhs.z));
        return lhs;
    }

    public static Int3 operator *(Int3 lhs, Int3 rhs)
    {
        lhs.x *= rhs.x;
        lhs.y *= rhs.y;
        lhs.z *= rhs.z;
        return lhs;
    }

    public static Int3 operator /(Int3 lhs, float rhs)
    {
        lhs.x = (int) Math.Round((double) (((float) lhs.x) / rhs));
        lhs.y = (int) Math.Round((double) (((float) lhs.y) / rhs));
        lhs.z = (int) Math.Round((double) (((float) lhs.z) / rhs));
        return lhs;
    }

    public static implicit operator string(Int3 ob)
    {
        return ob.ToString();
    }
}

