
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lite.NavMesh
{
	public class Polygon
	{
		public List<Vector3> verties = new List<Vector3>();

		public void AppendVertex(Vector3 vertex)
		{
			verties.Add(vertex);
		}

		public void InsertVertex(int index, Vector3 vertex)
		{
			verties.Insert(index, vertex);
		}

	}

}
