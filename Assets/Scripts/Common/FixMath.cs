
using System;
using UnityEngine;
using Lite;



// 长度单位：mm毫米、dm分米、m米

public class FixMath
{

	#region---------------长度--------------

	public static int ratio_mm = 1000;


	public static float mm2m(int mm)
	{
		return (float)mm / ratio_mm;
	}

	public static int m2mm(float m)
	{
		return (int)(m * ratio_mm);
	}

	#endregion



	#region--------------计算距离--------------

	public static long DistanceSqr(FixVector3 p1, FixVector3 p2)
	{
		return p1.DistanceSqr(p2);
	}

	public static long DistanceSqr2D(FixVector3 p1, FixVector3 p2)
	{
		p1.Set(p1.x, 0, p1.z);
		p2.Set(p2.x, 0, p2.z);
		return p1.DistanceSqr(p2);
	}

	public static FixVector3 ScaleFixVector3(FixVector3 vec, Fix64 scale)
	{
		int x = (int)((Fix64)(vec.x) * scale);
		int y = (int)((Fix64)(vec.y) * scale);
		int z = (int)((Fix64)(vec.z) * scale);
		return new FixVector3(x, y, z);
	}

	#endregion


	#region--------------计算角度--------------

	public static ushort AngleOfLine(FixVector3 origin, FixVector3 target)
	{
		FixVector3 delta = target - origin;
		Fix64 other = Fix64.Atan2((Fix64)delta.z, (Fix64)delta.x)
			/ (Fix64.Pi * (Fix64)2) * (Fix64)65535;
		//Debug.Log((ushort)(32768.0 + System.Math.Atan((float)delta.z / (float)delta.x) / pi / 2 * 65535));
		//Debug.Log((ushort)other);

		return (ushort)other;
	}


	public static ushort AngleOfLine(FixVector3 vec)
	{
		Fix64 other = Fix64.Atan2((Fix64)vec.z, (Fix64)vec.x)
			/ (Fix64.Pi * (Fix64)2) * (Fix64)65535;

		return (ushort)other;
	}


	public static ushort ToAngle(FixVector3 direction)
	{
		Fix64 other = Fix64.Atan2((Fix64)direction.z, (Fix64)direction.x)
			/ (Fix64.Pi * (Fix64)2) * (Fix64)65535;
		return (ushort)other;
	}

	public static ushort ToYAngle(FixVector3 direction)
	{
		if (Math.Abs(direction.x) < 200)
		{
			Fix64 other = Fix64.Atan2((Fix64)direction.y, (Fix64)direction.z)
				/ (Fix64.Pi * (Fix64)2) * (Fix64)65535;
			return (ushort)other;

		}
		else
		{
			Fix64 other = Fix64.Atan2((Fix64)direction.y, (Fix64)direction.x)
				/ (Fix64.Pi * (Fix64)2) * (Fix64)65535;
			return (ushort)other;
		}
	}

	public static float ToDegree(ushort angle)
	{
		//float dgr = (-(float)(angle - ushort.MaxValue / 4) / (float)ushort.MaxValue) * 360;
		float dgr = -(float)(angle) / (float)ushort.MaxValue * 360 + 90;
		return dgr;
	}

	public static FixVector3 DecomposeAngle(int length, ushort angle)
	{
		FixVector3 result = new FixVector3();

		Fix64 radian = (Fix64)angle / (Fix64)65535 * (Fix64)2 * Fix64.Pi;

		result.x = (int)(Fix64.FastCos(radian) * (Fix64)length);
		result.z = (int)(Fix64.FastSin(radian) * (Fix64)length);

		return result;
	}

	public static ushort AddAngle(ushort angle, ushort delta)
	{
		if (angle + delta < 0)
			return (ushort)(65535 + angle + delta);

		if (angle + delta > 65535)
			return (ushort)(angle + delta - 65535);

		return (ushort)(angle + delta);
	}

	public static ushort DecAngle(ushort angle, ushort delta)
	{
		if (angle - delta < 0)
			return (ushort)(65535 + angle - delta);

		return (ushort)(angle - delta);
	}


	public static FixVector3 changeAbsolute2Relative(FixVector3 point, FixVector3 originPoint, ushort angle)
	{
		//originPoint为图中A点，directionPoint为图中B点，changePoint为图中C点  
		FixVector3 rePoint = new FixVector3();

		FixVector3 directionPoint = FixMath.DecomposeAngle(1000, angle);
		directionPoint.x = originPoint.x + directionPoint.x;
		directionPoint.y = originPoint.y + directionPoint.y;
		directionPoint.y = originPoint.z + directionPoint.z;

		if (originPoint == directionPoint)//方向点跟原点重合，就用平行于原坐标的x轴来算就行了  
		{//AB点重合，方向指向哪里都没所谓，肯定按原来的做方便  
			rePoint.x = point.x - originPoint.x;
			rePoint.y = point.y - originPoint.y;
		}
		else
		{
			//计算三条边  
			//计算三条边
			Fix64 a = (Fix64)directionPoint.DistanceSqr(point);
			a = Fix64.Sqrt(a);
			Fix64 b = (Fix64)point.DistanceSqr(originPoint);
			b = Fix64.Sqrt(b);
			Fix64 c = (Fix64)directionPoint.DistanceSqr(originPoint);
			c = Fix64.Sqrt(c);

			Fix64 cosA = (b * b + c * c - a * a) / ((Fix64)2 * b * c);//余弦 
			Fix64 xpos = a * cosA;//相对坐标x 
			Fix64 ypos = Fix64.Sqrt(a * a - xpos * xpos);//相对坐标y  

			rePoint.x = (int)xpos;
			rePoint.y = (int)ypos;
		}
		return rePoint;
	}


	#endregion

}

