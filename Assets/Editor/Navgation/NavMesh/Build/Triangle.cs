
using UnityEngine;


namespace Lite.NavMesh
{
	public class Triangle
	{
		public Vector3 a = Vector3.zero;
		public Vector3 b = Vector3.zero;
		public Vector3 c = Vector3.zero;

		public void SetVertex(Vector3 v1, Vector3 v2, Vector3 v3)
		{
			a = v1;
			b = v2;
			c = v3;
		}
	}

}
