

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using PathFinding;


namespace FixedPoint
{
	public enum PathMode
	{
		Grid,
		Graph,
		NavMesh
	}


	public class PathTest : MonoBehaviour
	{
		public PathFindingMachine machine;

		public DebugLine line;

		public int start = 0;
		public int end = 10;


		List<FixVector3> fixResult = new List<FixVector3>();
		List<Int3> i3Result = new List<Int3>();
		List<Vector3> result = new List<Vector3>();
		void OnGUI()
		{
			if (GUI.Button(new Rect(10, 10, 40, 20), "ast"))
			{
				DoIt();
			}
		}


		float lastTime = 0;
		void Update()
		{
			if (Time.timeSinceLevelLoad - lastTime > 0.1f)
			{
				lastTime = Time.timeSinceLevelLoad;
				//DoIt();
			}
		}

		System.Random random = new System.Random();
		void DoIt()
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();

			FixVector3 from = new FixVector3(random.Next(-4000, 4000), 10, random.Next(-4000, 4000));
			FixVector3 to = new FixVector3(random.Next(-4000, 4000), 10, random.Next(-4000, 4000));

			fixResult.Clear();
			i3Result.Clear();
			result.Clear();
			machine.FindPath(from, to, ref fixResult);
			//var cache = machine.FindPath(random.Next(0, 60), random.Next(0, 60));
			//i3Result.AddRange(cache);

			//for (int i = 0; i < i3Result.Count; ++i)
			//	result.Add(i3Result[i].ToVector3());

			for (int i = 0; i < fixResult.Count; ++i)
				result.Add(fixResult[i].ToVector3());

			if (result.Count > 0)
			{
				line.ClearLines();
				line.AddLine(result.ToArray(), Color.red);
			}

			watch.Stop();
			if (result.Count > 0)
				UnityEngine.Debug.Log("time " + watch.ElapsedMilliseconds);
		}


		
	}

}