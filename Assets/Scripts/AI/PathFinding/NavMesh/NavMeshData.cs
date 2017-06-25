
using System;
using UnityEngine;
using System.Collections.Generic;
using TwGame.Graph;


namespace TwGame.NavMesh
{
	/// <summary>
	/// 存储着3D NavMesh寻路数据
	/// </summary>
	[CreateAssetMenu(menuName = "TwGame/NavMeshData", order = 3)]
	public class NavMeshData : ScriptableObject
	{
		public List<NavMeshNode> nodeList = new List<NavMeshNode>();
		public List<NavMeshEdge> edgeList = new List<NavMeshEdge>();

		public List<Vector3> points = new List<Vector3>();

		public void Clear()
		{
			nodeList.Clear();
			edgeList.Clear();
		}

	}
}
