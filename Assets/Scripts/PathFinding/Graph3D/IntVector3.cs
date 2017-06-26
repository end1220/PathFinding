

using System;
using UnityEngine;


namespace Lite.AStar
{
	[System.Serializable]
	public struct IntVector3
	{
		public int x;
		public int y;
		public int z;


		public IntVector3(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}


		public IntVector3(Vector3 vec)
		{
			x = m2mm(vec.x);
			y = m2mm(vec.y);
			z = m2mm(vec.z);
		}

		public Vector3 ToVector3()
		{
			return new Vector3(x / 1000.0f, y / 1000.0f, z / 1000.0f);
		}

		private static int m2mm(float m)
		{
			return (int)(m*1000);
		}

	}

}


