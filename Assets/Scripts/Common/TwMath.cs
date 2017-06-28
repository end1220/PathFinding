
using System;
using Lite;
using UnityEngine;
using Lite;



// 长度单位：mm毫米、dm分米、m米

public class TwMath
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


}

