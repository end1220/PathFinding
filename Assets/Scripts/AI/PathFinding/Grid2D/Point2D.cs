

using System;
using System.Collections;
using System.Collections.Generic;


namespace TwGame.AStar
{

	public struct Point2D
	{
		public int x;
		public int y;


		public Point2D(int x, int y)
		{
			this.x = x;
			this.y = y;
		}


		public Point2D(Point2D src)
		{
			this.x = src.x;
			this.y = src.y;
		}

	}

}


