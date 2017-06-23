
using System;
using UnityEngine;
using Lite.AStar;

namespace TwGame.NavMesh
{
	[Serializable]
	public class NavMeshNode : AStarNode
	{
		public Vector3 a = Vector3.zero;
		public Vector3 b = Vector3.zero;
		public Vector3 c = Vector3.zero;

		public Vector3 center = Vector3.zero;


		public NavMeshNode(int id) :
			base(id)
		{
			
		}


		public void SetVertex(Vector3 v1, Vector3 v2, Vector3 v3)
		{
			a = v1;
			b = v2;
			c = v3;
			center = new Vector3((a.x + b.x + c.x)/3.0f, (a.y + b.y + c.y)/3.0f, (a.z + b.z + c.z)/3.0f);
		}

	}

}

