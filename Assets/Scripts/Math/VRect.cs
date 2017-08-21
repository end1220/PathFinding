﻿using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct VRect
{
    private int m_XMin;
    private int m_YMin;
    private int m_Width;
    private int m_Height;
    public VRect(int left, int top, int width, int height)
    {
        this.m_XMin = left;
        this.m_YMin = top;
        this.m_Width = width;
        this.m_Height = height;
    }

    public VRect(VRect source)
    {
        this.m_XMin = source.m_XMin;
        this.m_YMin = source.m_YMin;
        this.m_Width = source.m_Width;
        this.m_Height = source.m_Height;
    }

    public static VRect MinMaxRect(int left, int top, int right, int bottom)
    {
        return new VRect(left, top, right - left, bottom - top);
    }

    public void Set(int left, int top, int width, int height)
    {
        this.m_XMin = left;
        this.m_YMin = top;
        this.m_Width = width;
        this.m_Height = height;
    }

    public int x
    {
        get
        {
            return this.m_XMin;
        }
        set
        {
            this.m_XMin = value;
        }
    }
    public int y
    {
        get
        {
            return this.m_YMin;
        }
        set
        {
            this.m_YMin = value;
        }
    }
    public Int2 position
    {
        get
        {
            return new Int2(this.m_XMin, this.m_YMin);
        }
        set
        {
            this.m_XMin = value.x;
            this.m_YMin = value.y;
        }
    }
    public Int2 center
    {
        get
        {
            return new Int2(this.x + (this.m_Width >> 1), this.y + (this.m_Height >> 1));
        }
        set
        {
            this.m_XMin = value.x - (this.m_Width >> 1);
            this.m_YMin = value.y - (this.m_Height >> 1);
        }
    }
    public Int2 min
    {
        get
        {
            return new Int2(this.xMin, this.yMin);
        }
        set
        {
            this.xMin = value.x;
            this.yMin = value.y;
        }
    }
    public Int2 max
    {
        get
        {
            return new Int2(this.xMax, this.yMax);
        }
        set
        {
            this.xMax = value.x;
            this.yMax = value.y;
        }
    }
    public int width
    {
        get
        {
            return this.m_Width;
        }
        set
        {
            this.m_Width = value;
        }
    }
    public int height
    {
        get
        {
            return this.m_Height;
        }
        set
        {
            this.m_Height = value;
        }
    }
    public Int2 size
    {
        get
        {
            return new Int2(this.m_Width, this.m_Height);
        }
        set
        {
            this.m_Width = value.x;
            this.m_Height = value.y;
        }
    }
    public int xMin
    {
        get
        {
            return this.m_XMin;
        }
        set
        {
            int xMax = this.xMax;
            this.m_XMin = value;
            this.m_Width = xMax - this.m_XMin;
        }
    }
    public int yMin
    {
        get
        {
            return this.m_YMin;
        }
        set
        {
            int yMax = this.yMax;
            this.m_YMin = value;
            this.m_Height = yMax - this.m_YMin;
        }
    }
    public int xMax
    {
        get
        {
            return (this.m_Width + this.m_XMin);
        }
        set
        {
            this.m_Width = value - this.m_XMin;
        }
    }
    public int yMax
    {
        get
        {
            return (this.m_Height + this.m_YMin);
        }
        set
        {
            this.m_Height = value - this.m_YMin;
        }
    }
    public override string ToString()
    {
        object[] args = new object[] { this.x, this.y, this.width, this.height };
        return string.Format("(x:{0:F2}, y:{1:F2}, width:{2:F2}, height:{3:F2})", args);
    }

    public string ToString(string format)
    {
        object[] args = new object[] { this.x.ToString(format), this.y.ToString(format), this.width.ToString(format), this.height.ToString(format) };
        return string.Format("(x:{0}, y:{1}, width:{2}, height:{3})", args);
    }

    public bool Contains(Int2 point)
    {
        return ((((point.x >= this.xMin) && (point.x < this.xMax)) && (point.y >= this.yMin)) && (point.y < this.yMax));
    }

    public bool Contains(Int3 point)
    {
        return ((((point.x >= this.xMin) && (point.x < this.xMax)) && (point.y >= this.yMin)) && (point.y < this.yMax));
    }

    public bool Contains(Int3 point, bool allowInverse)
    {
        if (!allowInverse)
        {
            return this.Contains(point);
        }
        bool flag = false;
        if ((((this.width < 0f) && (point.x <= this.xMin)) && (point.x > this.xMax)) || (((this.width >= 0f) && (point.x >= this.xMin)) && (point.x < this.xMax)))
        {
            flag = true;
        }
        return (flag && ((((this.height < 0f) && (point.y <= this.yMin)) && (point.y > this.yMax)) || (((this.height >= 0f) && (point.y >= this.yMin)) && (point.y < this.yMax))));
    }

    private static VRect OrderMinMax(VRect rect)
    {
        if (rect.xMin > rect.xMax)
        {
            int xMin = rect.xMin;
            rect.xMin = rect.xMax;
            rect.xMax = xMin;
        }
        if (rect.yMin > rect.yMax)
        {
            int yMin = rect.yMin;
            rect.yMin = rect.yMax;
            rect.yMax = yMin;
        }
        return rect;
    }

    public bool Overlaps(VRect other)
    {
        return ((((other.xMax > this.xMin) && (other.xMin < this.xMax)) && (other.yMax > this.yMin)) && (other.yMin < this.yMax));
    }

    public bool Overlaps(VRect other, bool allowInverse)
    {
        VRect rect = this;
        if (allowInverse)
        {
            rect = OrderMinMax(rect);
            other = OrderMinMax(other);
        }
        return rect.Overlaps(other);
    }

    public override int GetHashCode()
    {
        return (((this.x.GetHashCode() ^ (this.width.GetHashCode() << 2)) ^ (this.y.GetHashCode() >> 2)) ^ (this.height.GetHashCode() >> 1));
    }

    public override bool Equals(object other)
    {
        if (!(other is VRect))
        {
            return false;
        }
        VRect rect = (VRect) other;
        return (((this.x.Equals(rect.x) && this.y.Equals(rect.y)) && this.width.Equals(rect.width)) && this.height.Equals(rect.height));
    }

    public static bool operator !=(VRect lhs, VRect rhs)
    {
        return ((((lhs.x != rhs.x) || (lhs.y != rhs.y)) || (lhs.width != rhs.width)) || (lhs.height != rhs.height));
    }

    public static bool operator ==(VRect lhs, VRect rhs)
    {
        return ((((lhs.x == rhs.x) && (lhs.y == rhs.y)) && (lhs.width == rhs.width)) && (lhs.height == rhs.height));
    }
}

