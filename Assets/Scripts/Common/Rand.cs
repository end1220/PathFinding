
using System;
using Lite;
using UnityEngine;



public class Rand
{

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


}

