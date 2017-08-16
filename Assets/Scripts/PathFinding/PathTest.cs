

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using PathFinding;
using TwFramework;
using TwGame;


public class PathTest : MonoBehaviour
{
	public static PathTest Instance;

	public PathFindingMachine machine;

#if UNITY_EDITOR
    public DebugLine line;
#endif

	List<FixVector3> fixResult = new List<FixVector3>();

	[System.NonSerialized]
	public NavMeshNode startNode;
	[System.NonSerialized]
	public NavMeshNode endNode;

	void Awake()
	{
		Instance = this;
	}

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
		/*if (Time.timeSinceLevelLoad - lastTime > 0.1f)
		{
			lastTime = Time.timeSinceLevelLoad;
			from = new FixVector3(random.Next(-21000, 18000), 1000, random.Next(-2000, 2000));
			to = new FixVector3(random.Next(18000, 21000), 1000, random.Next(-2000, 2000));
			DoIt();
		}*/

		if (startNode != null && endNode != null)
			UnityEngine.Debug.DrawLine(startNode.position.ToVector3(), endNode.position.ToVector3(), Color.blue);

		TestClick();
	}

	System.Random random = new System.Random();
	FixVector3 from = FixVector3.zero;
	FixVector3 to = FixVector3.zero;
	void DoIt()
	{
		startNode = null;
		endNode = null;
#if UNITY_EDITOR
        line.ClearLines();
#endif

        Stopwatch watch = new Stopwatch();
		watch.Start();

		fixResult.Clear();
		machine.FindPath(from, to, ref fixResult);

		if (fixResult.Count > 0)
		{
#if UNITY_EDITOR
            line.ClearLines();
			line.AddLine(fixResult.ToArray(), Color.red);
#endif
        }

		watch.Stop();
		if (fixResult.Count > 0)
			UnityEngine.Debug.Log("ms " + watch.ElapsedMilliseconds);
	}


	void TestClick()
	{
		if (Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;
			int layerMask =
					1 << LayerMask.NameToLayer(AppConst.LayerTerrain)
					| 1 << LayerMask.NameToLayer(AppConst.LayerActor);

			if (Physics.Raycast(ray, out hit, 50, layerMask))
			{
				from = to;
				to = new FixVector3(hit.point + Vector3.up);
				DoIt();
			}
		}
	}

}

