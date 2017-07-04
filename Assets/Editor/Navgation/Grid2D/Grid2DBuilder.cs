using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using Lite;
using Lite.AStar;
using Lite.Graph;


public class Grid2DBuilder
{
	class GridInfo
	{
		public ushort mask;
		public Vector3 point;
		public int layer;
	}

	public GameObject worldBoxObj;

	public float minX;
	public float minZ;

	public float gridSize;
	public float agentRadius;
	public int width;
	public int height;
	GridInfo[,] gridList;
	public float tan_slope;

	public NavGrid2DData navigation;


	public void Build()
	{
		Fill_Grid();
	}


	private void Fill_Grid()
	{
		int terrainLayer = LayerMask.NameToLayer(AppConst.LayerTerrain);
		int obstacleLayer = LayerMask.NameToLayer(AppConst.LayerObstacle);
		int linkLayer = LayerMask.NameToLayer(AppConst.LayerLink);

		gridList = new GridInfo[width, height];
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				gridList[x, y] = new GridInfo();
				gridList[x, y].mask = 0;
			}
		}

		// get hit points.
		Ray ray = new Ray();
		ray.direction = new Vector3(0, -1, 0);

		RaycastHit hit;
		int layerMask = 1 << terrainLayer
			| 1 << obstacleLayer
			| 1 << linkLayer;

		float sideLen = (gridSize/2.0f) * 0.5f;
		sideLen += agentRadius;
		float badY = 10000;
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				// center
				float centerx = minX + gridSize * (x + 0.5f);
				float centerz = minZ + gridSize * (y + 0.5f);

				// sides
				float[] sidex = new float[]
				{
					centerx - sideLen,
					centerx - sideLen,
					centerx + sideLen,
					centerx + sideLen,
				};
				float[] sidez = new float[]
				{
					centerz - sideLen,
					centerz + sideLen,
					centerz + sideLen,
					centerz - sideLen,
				};
				bool hitObstacle = false;
				for (int s = 0; s < 4; s++)
				{
					ray.origin = new Vector3(sidex[s], 500, sidez[s]);
					if (Physics.Raycast(ray, out hit, 1000, layerMask))
					{
						if (hit.collider.gameObject.layer == obstacleLayer)
						{
							hitObstacle = true;
							break;
						}
					}
				}

				ray.origin = new Vector3(centerx, 500, centerz);
				float fposy = badY;
				int hitLayer = -1;
				if (Physics.Raycast(ray, out hit, 1000, layerMask))
				{
					fposy = hit.point.y;
					hitLayer = hit.collider.gameObject.layer;
				}
				gridList[x, y].point = new Vector3(centerx, fposy, centerz);
				gridList[x, y].layer = hitObstacle ? obstacleLayer : hitLayer;
			}
			UpdateProgress(x, width, "");
		}

		

		// calculate slope values.
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				var cp = gridList[x, y];

				if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
				{
					// not passable.
					cp.mask = 1;
				}
				else if (cp.layer == obstacleLayer)
				{
					// not passable.
					cp.mask = 1;
				}
				else
				{
					
					var xr = gridList[x + 1, y];
					var xl = gridList[x - 1, y];
					var zr = gridList[x, y + 1];
					var zl = gridList[x, y - 1];

					int xrLayer = xr.layer;
					int xlLayer = xl.layer;
					int zrLayer = zr.layer;
					int zlLayer = zl.layer;

					if (cp.point.y == badY || (xr.point.y == badY && xl.point.y == badY) || (zr.point.y == badY && zl.point.y == badY))
					{
						cp.mask = 1;
						continue;
					}
					//float tanx = Mathf.Abs((xr.point.y - xl.point.y) / (xr.point.x - xl.point.x));
					//float tanz = Mathf.Abs((zr.point.y - zl.point.y) / (zr.point.z - zl.point.z));
					float tanx = Mathf.Min(Mathf.Abs((cp.point.y - xl.point.y) / (cp.point.x - xl.point.x)), Mathf.Abs((cp.point.y - xr.point.y) / (cp.point.x - xr.point.x)));
					float tanz = Mathf.Min(Mathf.Abs((cp.point.y - zl.point.y) / (cp.point.z - zl.point.z)), Mathf.Abs((cp.point.y - zr.point.y) / (cp.point.z - zr.point.z)));
					if (xrLayer == linkLayer || xlLayer == linkLayer)
						tanx = 0;
					if (zrLayer == linkLayer || zlLayer == linkLayer)
						tanz = 0;
					float maxTan = Mathf.Max(tanx, tanz);
					cp.mask = (ushort)(maxTan > tan_slope ? 1 : 0);
				}
			}
			UpdateProgress(x, width, "");
		}

		NavGrid2DData nav = ScriptableObject.CreateInstance<NavGrid2DData>();
		ushort[,] maskList = new ushort[width, height];
		for (int x = 0; x < width; ++x)
			for (int y = 0; y < height; ++y)
				maskList[x, y] = gridList[x, y].mask;
		nav._setData(maskList, width, height, (int)TwMath.m2mm(gridSize), (int)TwMath.m2mm(minX), (int)TwMath.m2mm(minZ));
		navigation = nav;

		EditorUtility.ClearProgressBar();
	}


	static void UpdateProgress(int progress, int progressMax, string desc)
	{
		string title = "Building[" + progress + " / " + progressMax + "]";
		float value = (float)progress / (float)progressMax;
		EditorUtility.DisplayProgressBar(title, desc, value);
	}


}