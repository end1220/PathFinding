using System;
using System.Runtime.InteropServices;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct Int
{
    public int i;
    public Int(int i)
    {
        this.i = i;
    }

    public Int(float f)
    {
        this.i = (int) Math.Round((double) (f * 1000f));
    }

    public override bool Equals(object o)
    {
        if (o == null)
        {
            return false;
        }
        Int num = (Int) o;
        return (this.i == num.i);
    }

    public override int GetHashCode()
    {
        return this.i.GetHashCode();
    }

    public static Int Min(Int a, Int b)
    {
        return new Int(Math.Min(a.i, b.i));
    }

    public static Int Max(Int a, Int b)
    {
        return new Int(Math.Max(a.i, b.i));
    }

    public override string ToString()
    {
        return this.scalar.ToString();
    }

    public float scalar
    {
        get
        {
            return (this.i * 0.001f);
        }
    }
    public static explicit operator Int(float f)
    {
        return new Int((int) Math.Round((double) (f * 1000f)));
    }

    public static implicit operator Int(int i)
    {
        return new Int(i);
    }

    public static explicit operator float(Int ob)
    {
        return (ob.i * 0.001f);
    }

    public static explicit operator long(Int ob)
    {
        return (long) ob.i;
    }

    public static Int operator +(Int a, Int b)
    {
        return new Int(a.i + b.i);
    }

    public static Int operator -(Int a, Int b)
    {
        return new Int(a.i - b.i);
    }

    public static bool operator ==(Int a, Int b)
    {
        return (a.i == b.i);
    }

    public static bool operator !=(Int a, Int b)
    {
        return (a.i != b.i);
    }
}

