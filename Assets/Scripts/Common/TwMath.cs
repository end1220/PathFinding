
using System;
using Lite;
using UnityEngine;



public class TwMath
{

	public static int ratio_mm = 1000;


	public static float mm2m(int mm)
	{
		return (float)mm / ratio_mm;
	}


	public static int m2mm(float m)
	{
		return (int)(m * ratio_mm);
	}

}

