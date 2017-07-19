
using System;
using System.IO;
using UnityEngine;


public class Log
{

	public static void Info(string text)
	{
		//var mgr = GameFramework.GetMgr<LogManager>();
		//mgr.Info(text);
		Debug.Log(text);
	}

	public static void Warning(string text)
	{
		//var mgr = GameFramework.GetMgr<LogManager>();
		//mgr.Warning(text);
		Debug.LogWarning(text);
	}

	public static void Error(string text)
	{
		//var mgr = GameFramework.GetMgr<LogManager>();
		//mgr.Error(text);
		Debug.LogError(text);
	}

}
