using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct VLine
{
    public Int2 point;
    public Int2 direction;
}

