

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
				DoIt();
			}
		}

		System.Random random = new System.Random();
		void DoIt()
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();

			//machine.FindPath();

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