
using System.Collections.Generic;
using UnityEngine;
using TwGame.NavMesh;


public class TestNavMesh : MonoBehaviour
{

	private NavMeshPathPlanner planer = new NavMeshPathPlanner();
	private NavMeshMap map = new NavMeshMap();

	public NavMeshData data;


	void Awake()
	{
		if (data != null)
		{
			map.Init(data);
			planer.Setup(map);
		}
	}

	void Update()
	{

	}

	void OnGUI()
	{

	}

}