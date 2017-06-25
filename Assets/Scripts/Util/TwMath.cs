
using System;
using Lite;
using UnityEngine;
using Lite;


namespace Lite
{

	// 长度单位：mm毫米、dm分米、m米

	public class TwMath
	{

		#region---------------长度--------------

		public static long ratio_mm = 1000;


		public static float mm2m(long mm)
		{
			return (float)mm / ratio_mm;
		}

		public static long m2mm(float m)
		{
			return (long)(m * ratio_mm);
		}

		#endregion




		#region----------------随机数---------------

		private static System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);

		public static void SetRandom(int value)
		{
			rnd = new System.Random(value);
		}

		/// <summary>
		/// [min, max]
		/// </summary>
		/// <returns></returns>
		public static int RandInt(int min, int max)
		{
			return rnd.Next(min, max + 1);
		}

		/// <summary>
		/// [0f, 1f)
		/// </summary>
		/// <returns></returns>
		public static float RandFloat()
		{
			return (rnd.Next(0, int.MaxValue)) / (int.MaxValue + 1.0f);
		}

		/// <summary>
		/// (-1f, 1f)
		/// </summary>
		/// <returns></returns>
		public static float RandClamp()
		{
			return RandFloat() - RandFloat();
		}

		#endregion





		#region--------------计算距离--------------

		public static long DistanceSqr(TwVector3 p1, TwVector3 p2)
		{
			return p1.DistanceSqr(p2);
		}

		public static long DistanceSqr2D(TwVector3 p1, TwVector3 p2)
		{
			p1.Set(p1.x, 0, p1.z);
			p2.Set(p2.x, 0, p2.z);
			return p1.DistanceSqr(p2);
		}

		public static TwVector3 ScaleTwVector3(TwVector3 vec, Fix64 scale)
		{
			long x = (long)((Fix64)(vec.x) * scale);
			long y = (long)((Fix64)(vec.y) * scale);
			long z = (long)((Fix64)(vec.z) * scale);
			return new TwVector3(x, y, z);
		}

		#endregion



		#region--------------计算角度--------------

		public static ushort AngleOfLine(TwVector3 origin, TwVector3 target)
		{
			TwVector3 delta = target - origin;
			Fix64 other = Fix64.Atan2((Fix64)delta.z, (Fix64)delta.x)
				/ (Fix64.Pi * (Fix64)2) * (Fix64)65535;
			//Debug.Log((ushort)(32768.0 + System.Math.Atan((float)delta.z / (float)delta.x) / pi / 2 * 65535));
			//Debug.Log((ushort)other);

			return (ushort)other;
		}


		public static ushort AngleOfLine(TwVector3 vec)
		{
			Fix64 other = Fix64.Atan2((Fix64)vec.z, (Fix64)vec.x)
				/ (Fix64.Pi * (Fix64)2) * (Fix64)65535;

			return (ushort)other;
		}


		public static ushort ToAngle(TwVector3 direction)
		{
			Fix64 other = Fix64.Atan2((Fix64)direction.z, (Fix64)direction.x)
				/ (Fix64.Pi * (Fix64)2) * (Fix64)65535;
			return (ushort)other;
		}

		public static ushort ToYAngle(TwVector3 direction)
		{
			if(Math.Abs(direction.x) < 200)
			{
				Fix64 other = Fix64.Atan2((Fix64)direction.y, (Fix64)direction.z)
					/ (Fix64.Pi * (Fix64)2) * (Fix64)65535;
				return (ushort)other;

			}else
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

		public static TwVector3 DecomposeAngle(int length, ushort angle)
		{
			TwVector3 result = new TwVector3();

			Fix64 radian = (Fix64)angle / (Fix64)65535 * (Fix64)2 * Fix64.Pi;

			result.x = (long)(Fix64.FastCos(radian) * (Fix64)length);
			result.z = (long)(Fix64.FastSin(radian) * (Fix64)length);

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


		public static TwVector3 changeAbsolute2Relative(TwVector3 point, TwVector3 originPoint, ushort angle)
		{
			//originPoint为图中A点，directionPoint为图中B点，changePoint为图中C点  
			TwVector3 rePoint = new TwVector3();

			TwVector3 directionPoint = TwMath.DecomposeAngle(1000, angle);
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

				rePoint.x = (long)xpos;
				rePoint.y = (long)ypos;
			}
			return rePoint;
		}


        #endregion

    }

}