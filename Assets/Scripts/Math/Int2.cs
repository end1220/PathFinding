using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct Int2
{
    public int x;
    public int y;
    public static Int2 zero;
    private static readonly int[] Rotations;
    public Int2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    static Int2()
    {
        zero = new Int2();
        Rotations = new int[] { 1, 0, 0, 1, 0, 1, -1, 0, -1, 0, 0, -1, 0, -1, 1, 0 };
    }

    public int sqrMagnitude
    {
        get
        {
            return ((this.x * this.x) + (this.y * this.y));
        }
    }
    public long sqrMagnitudeLong
    {
        get
        {
            long x = this.x;
            long y = this.y;
            return ((x * x) + (y * y));
        }
    }
    public int magnitude
    {
        get
        {
            long x = this.x;
            long y = this.y;
            return IntMath.Sqrt((x * x) + (y * y));
        }
    }
    public static int Dot(Int2 a, Int2 b)
    {
        return ((a.x * b.x) + (a.y * b.y));
    }

    public static long DotLong(ref Int2 a, ref Int2 b)
    {
        return ((a.x * b.x) + (a.y * b.y));
    }

    public static long DotLong(Int2 a, Int2 b)
    {
        return ((a.x * b.x) + (a.y * b.y));
    }

    public static long DetLong(ref Int2 a, ref Int2 b)
    {
        return ((a.x * b.y) - (a.y * b.x));
    }

    public static long DetLong(Int2 a, Int2 b)
    {
        return ((a.x * b.y) - (a.y * b.x));
    }

    public override bool Equals(object o)
    {
        if (o == null)
        {
            return false;
        }
        Int2 num = (Int2) o;
        return ((this.x == num.x) && (this.y == num.y));
    }

    public override int GetHashCode()
    {
        return ((this.x * 0xc005) + (this.y * 0x1800d));
    }

    public static Int2 Rotate(Int2 v, int r)
    {
        r = r % 4;
        return new Int2((v.x * Rotations[r * 4]) + (v.y * Rotations[(r * 4) + 1]), (v.x * Rotations[(r * 4) + 2]) + (v.y * Rotations[(r * 4) + 3]));
    }

    public static Int2 Min(Int2 a, Int2 b)
    {
        return new Int2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
    }

    public static Int2 Max(Int2 a, Int2 b)
    {
        return new Int2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
    }

    public static Int2 FromInt3XZ(Int3 o)
    {
        return new Int2(o.x, o.z);
    }

    public static Int3 ToInt3XZ(Int2 o)
    {
        return new Int3(o.x, 0, o.y);
    }

    public override string ToString()
    {
        object[] objArray1 = new object[] { "(", this.x, ", ", this.y, ")" };
        return string.Concat(objArray1);
    }

    public void Min(ref Int2 r)
    {
        this.x = Mathf.Min(this.x, r.x);
        this.y = Mathf.Min(this.y, r.y);
    }

    public void Max(ref Int2 r)
    {
        this.x = Mathf.Max(this.x, r.x);
        this.y = Mathf.Max(this.y, r.y);
    }

    public void Normalize()
    {
        long num = this.x * 100;
        long num2 = this.y * 100;
        long a = (num * num) + (num2 * num2);
        if (a != 0)
        {
            long b = IntMath.Sqrt(a);
            this.x = (int) IntMath.Divide((long) (num * 0x3e8L), b);
            this.y = (int) IntMath.Divide((long) (num2 * 0x3e8L), b);
        }
    }

    public Int2 normalized
    {
        get
        {
            Int2 num = new Int2(this.x, this.y);
            num.Normalize();
            return num;
        }
    }
    public static Int2 ClampMagnitude(Int2 v, int maxLength)
    {
        long sqrMagnitudeLong = v.sqrMagnitudeLong;
        long num2 = maxLength;
        if (sqrMagnitudeLong > (num2 * num2))
        {
            long b = IntMath.Sqrt(sqrMagnitudeLong);
            int x = (int) IntMath.Divide((long) (v.x * maxLength), b);
            return new Int2(x, (int) IntMath.Divide((long) (v.x * maxLength), b));
        }
        return v;
    }

    public static explicit operator Vector2(Int2 ob)
    {
        return new Vector2(ob.x * 0.001f, ob.y * 0.001f);
    }

    public static explicit operator Int2(Vector2 ob)
    {
        return new Int2((int) Math.Round((double) (ob.x * 1000f)), (int) Math.Round((double) (ob.y * 1000f)));
    }

    public static Int2 operator +(Int2 a, Int2 b)
    {
        return new Int2(a.x + b.x, a.y + b.y);
    }

    public static Int2 operator -(Int2 a, Int2 b)
    {
        return new Int2(a.x - b.x, a.y - b.y);
    }

    public static bool operator ==(Int2 a, Int2 b)
    {
        return ((a.x == b.x) && (a.y == b.y));
    }

    public static bool operator !=(Int2 a, Int2 b)
    {
        return ((a.x != b.x) || (a.y != b.y));
    }

    public static Int2 operator -(Int2 lhs)
    {
        lhs.x = -lhs.x;
        lhs.y = -lhs.y;
        return lhs;
    }

    public static Int2 operator *(Int2 lhs, int rhs)
    {
        lhs.x *= rhs;
        lhs.y *= rhs;
        return lhs;
    }
}

