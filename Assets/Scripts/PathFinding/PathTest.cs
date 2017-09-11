

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using PathFinding;


public class PathTest : MonoBehaviour
{
	public static PathTest Instance;

	public PathFindingMachine machine;

#if UNITY_EDITOR
    public DebugLine line;
#endif

	List<Int3> fixResult = new List<Int3>();

	[System.NonSerialized]
	public NavMeshNode startNode;
	[System.NonSerialized]
	public NavMeshNode endNode;

	System.Random random = new System.Random();
	Int3 from = Int3.zero;
	Int3 to = Int3.zero;




	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		machine = FindObjectOfType<PathFindingMachine>();
#if UNITY_EDITOR
		line = FindObjectOfType<DebugLine>();
#endif
	}

/*void OnGUI()
{
	if (GUI.Button(new Rect(10, 10, 40, 20), "ast"))
	{
		from = new Int3(-4900, 840, 13030);
		to = new Int3(14330, 840, 2660);
		DoIt();
	}
}*/


	void Update()
	{
		TestClick();

		//if (startNode != null && endNode != null)
		//	UnityEngine.Debug.DrawLine((Vector3)startNode.position, (Vector3)endNode.position, Color.blue);

		UnityEngine.Debug.DrawLine(from.vec3 + Vector3.up * 0.1f, to.vec3 + Vector3.up * 0.1f, Color.blue);

		UnityEngine.Debug.DrawLine(from.vec3 + Vector3.right * 0.05f + Vector3.up * 0.1f, hit.vec3 + Vector3.right * 0.05f + Vector3.up * 0.1f, Color.red);

		/*var graph = machine.navgationGraph as NavMeshGraph;
		for (int i = 0; i < graph.trace.Count-1; ++i)
		{
			UnityEngine.Debug.DrawLine((Vector3)graph.trace[i].position, (Vector3)graph.trace[i+1].position, Color.green);
		}*/


		if (startNode != null)
		{
			DrawLine((Vector3)startNode.v0, (Vector3)startNode.v1, Color.green);
			DrawLine((Vector3)startNode.v1, (Vector3)startNode.v2, Color.green);
			DrawLine((Vector3)startNode.v2, (Vector3)startNode.v0, Color.green);
			DrawRect((Vector3)hit, 0.1f, Color.red);
		}
	}

	private void DrawLine(Vector3 from, Vector3 to, Color color)
	{
		UnityEngine.Debug.DrawLine(from + Vector3.up * 0.1f, to + Vector3.up * 0.1f, color);
	}

	private void DrawRect(Vector3 p, float a, Color color)
	{
		Vector3 v0 = p + new Vector3(-a, 0, -a);
		Vector3 v1 = p + new Vector3(a, 0, -a);
		Vector3 v2 = p + new Vector3(a, 0, a);
		Vector3 v3 = p + new Vector3(-a, 0, a);

		UnityEngine.Debug.DrawLine(v0, v1, color);
		UnityEngine.Debug.DrawLine(v1, v2, color);
		UnityEngine.Debug.DrawLine(v2, v3, color);
		UnityEngine.Debug.DrawLine(v3, v0, color);
	}

	
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
		if (!machine.FindPath(from, to, ref fixResult))
		{
			UnityEngine.Debug.Log("ms " + watch.ElapsedMilliseconds);
			to = PathFindingMachine.Instance.GetNearestPosition(to);
			bool ret = machine.FindPath(from, to, ref fixResult);
			if (!ret)
			{
				UnityEngine.Debug.Log("cannot find a path to " + to.ToString());
			}
		}

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
				to = new Int3(hit.point + Vector3.up * 0.01f);
				//DoIt();
				DoLinecast();
				//DoTriangleTest();
			}
		}
	}


	Int3 hit = Int3.zero;
	void DoLinecast()
	{
		//from = new Int3(-7336, 0, -18282);
		//to = new Int3(-5336, 0, -18282);

		var graph = machine.navgationGraph as NavMeshGraph;
		HitInfo hitInfo = new HitInfo(from, to);
		if (graph.LineCastForMoving(ref hitInfo, MoveType.Normal))
		{
			hit = hitInfo.hitPosition;
		}
		else
			hit = to;
	}

	void DoTriangleTest()
	{
		var graph = machine.navgationGraph as NavMeshGraph;
		var info = graph.GetNearest(to, null);
		var node = info.node;
		hit = Polygon.ClosestPointOnTriangle(node.v0, node.v1, node.v2, to);
		startNode = node;
	}




}

