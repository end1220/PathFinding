using UnityEngine;
using System;
using System.Collections.Generic;

using Lite.AStar;


public class NavGrid2DGizmo : MonoBehaviour
{
	private int width;
	private int height;
	private Vector3[,] gridPosList;
	
	NavGrid2DData navigation;

	private int targetX = -1;
	private int targetY = -1;

	private int currentX = -1;
	private int currentY = -1;

	public Vector3 playerPosition = Vector3.zero;

	public Vector3 from = Vector3.zero;

	public Vector3 to = Vector3.zero;

	public Vector3 block = Vector3.zero;


	public void SetGridPosList(NavGrid2DData mask, Vector3[,] grids, int w, int h)
	{
		navigation = mask;
		gridPosList = grids;
		width = w;
		height = h;
	}


	void OnDrawGizmosSelected()
	{
		Transform trans = gameObject.transform;

		Matrix4x4 defaultMatrix = Gizmos.matrix;
		Gizmos.matrix = trans.localToWorldMatrix;

		Color defaultColor = Gizmos.color;

		float a = navigation.gridSize/2;

		// passables
		for (int i = 0; i < width; ++i)
		{
			for (int j = 0; j < height; ++j)
			{
				Vector3 c = gridPosList[i, j];
				bool passable = navigation.At(i, j) == 0;
				if (passable)
				{
					Gizmos.color = Color.green;
					DrawRect(c, a);
				}
			}
		}

		// unpassables
		for (int i = 0; i < width; ++i)
		{
			for (int j = 0; j < height; ++j)
			{
				Vector3 c = gridPosList[i, j];
				bool passable = navigation.At(i, j) == 0;
				if (!passable)
				{
					Gizmos.color = Color.red;
					DrawRect(c, a);
				}
			}
		}

		// border lines
		Gizmos.DrawLine(gridPosList[0, 0], gridPosList[width-1, 0]);
		Gizmos.DrawLine(gridPosList[0, 0], gridPosList[0, height-1]);
		Gizmos.DrawLine(gridPosList[width - 1, height - 1], gridPosList[width - 1, 0]);
		Gizmos.DrawLine(gridPosList[width - 1, height - 1], gridPosList[0, height - 1]);

		// special marks
		if (-1 != targetX && -1 != targetY)
		{
			Vector3 c = gridPosList[targetX, targetY];
			Gizmos.color = Color.cyan;
			DrawRect(c, a * 0.8f);
		}
		if (-1 != currentX && -1 != currentY)
		{
			Vector3 c = gridPosList[currentX, currentY];
			Gizmos.color = Color.red;
			DrawRect(c, a * 0.7f);
		}

		Gizmos.color = Color.red;
		DrawRect(playerPosition, a * 0.5f);

		Gizmos.DrawLine(from, to);
		DrawRect(from, 0.1f);
		DrawRect(to, 0.1f);
		DrawRect(block, 0.1f);

		Gizmos.color = defaultColor;
		Gizmos.matrix = defaultMatrix;

	}


	private void DrawRect(Vector3 center, float a)
	{
		var v1 = new Vector3(center.x - a, center.y, center.z - a);
		var v2 = new Vector3(center.x + a, center.y, center.z - a);
		var v3 = new Vector3(center.x + a, center.y, center.z + a);
		var v4 = new Vector3(center.x - a, center.y, center.z + a);
		Gizmos.DrawLine(v1, v2);
		Gizmos.DrawLine(v2, v3);
		Gizmos.DrawLine(v3, v4);
		Gizmos.DrawLine(v4, v1);
	}


	public void MarkTargetGrid(int x, int y)
	{
		targetX = x;
		targetY = y;
	}

	public void MarkCurrentGrid(int x, int y)
	{
		currentX = x;
		currentY = y;
	}

}