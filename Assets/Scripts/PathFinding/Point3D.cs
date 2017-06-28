

using System;
using System.Collections;
using System.Collections.Generic;


namespace Lite.AStar
{

	public struct Point3D
	{
		public int x;
		public int y;
		public int z;


		public Point3D(int x, int y)
		{
			this.x = x;
			this.y = y;
			this.z = 0;
		}


		public Point3D(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}


		public Point3D(Point3D src)
		{
			this.x = src.x;
			this.y = src.y;
			this.z = src.z;
		}

	}

}


