
using UnityEngine;
using UnityEditor;



namespace PathFinding
{

	public class Grid2DBuilder
	{
		class GridInfo
		{
			public ushort mask;
			public Vector3 point;
			public int layer;
			public byte terrain;
		}

		public GameObject worldBoxObj;

		public float minX;
		public float minZ;

		public float gridSize;
		public float agentRadius;
		public int width;
		public int height;
		GridInfo[,] gridList;
		public int slope;
		public float tan_slope;

		public NavGrid2DData navData;

		const float badY = 10000;


		public void Build()
		{
			InitFill();
			TerrainTest();
			SlopeTest();
			DynamicObstacleTest();
			CreateAsset();
		}

		/// <summary>
		/// 初始化填充数据
		/// </summary>
		private void InitFill()
		{
			gridList = new GridInfo[width, height];
			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					gridList[x, y] = new GridInfo();
					gridList[x, y].mask = 0;
				}
			}
		}

		/// <summary>
		/// 地形类型检查，简单设置objstacle地表为不可走.
		/// 包含了角色半径的处理
		/// </summary>
		private void TerrainTest()
		{
			int terrainLayer = LayerMask.NameToLayer(AppConst.LayerTerrain);
			int obstacleLayer = LayerMask.NameToLayer(AppConst.LayerObstacle);
			int linkLayer = LayerMask.NameToLayer(AppConst.LayerLink);

			// get hit points.
			Ray ray = new Ray();
			ray.direction = new Vector3(0, -1, 0);

			RaycastHit hit;
			int layerMask = 1 << terrainLayer
				| 1 << obstacleLayer
				| 1 << linkLayer;

			float sideLen = (gridSize / 2.0f) * 0.5f;
			sideLen += agentRadius;
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
					string[] sideTag = new string[4] { null, null, null, null };
					int hitIndex = 0;
					bool hitObstacle = false;
					for (int s = 0; s < 4; s++)
					{
						ray.origin = new Vector3(sidex[s], 500, sidez[s]);
						if (Physics.Raycast(ray, out hit, 1000, layerMask))
						{
							if (hit.collider.gameObject.layer == obstacleLayer)
							{
								hitObstacle = true;
								hitIndex = s;
								sideTag[s] = hit.collider.gameObject.tag;
								break;
							}
						}
					}

					ray.origin = new Vector3(centerx, 500, centerz);
					float fposy = badY;
					int hitLayer = -1;
					string tag = null;
					if (Physics.Raycast(ray, out hit, 1000, layerMask))
					{
						fposy = hit.point.y;
						hitLayer = hit.collider.gameObject.layer;
						tag = hit.collider.gameObject.tag;
					}
					gridList[x, y].point = new Vector3(centerx, fposy, centerz);
					gridList[x, y].layer = hitObstacle ? obstacleLayer : hitLayer;
					gridList[x, y].terrain = (byte)GetTerrainType(hitObstacle ? sideTag[hitIndex] : tag);
				}
				UpdateProgress(x, width, "");
			}
		}

		/// <summary>
		/// 坡度检查，超过设定坡度则不可走。
		/// 另外对墙类型做特殊处理
		/// </summary>
		private void SlopeTest()
		{
			//int terrainLayer = LayerMask.NameToLayer(AppConst.LayerTerrain);
			int obstacleLayer = LayerMask.NameToLayer(AppConst.LayerObstacle);
			int linkLayer = LayerMask.NameToLayer(AppConst.LayerLink);
			float badY = 10000;

			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					var cp = gridList[x, y];

					if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
					{
						cp.mask = 1;
					}
					else if (cp.layer == obstacleLayer)
					{
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

						float tanxl = Mathf.Abs((cp.point.y - xl.point.y) / (cp.point.x - xl.point.x));
						float tanxr = Mathf.Abs((cp.point.y - xr.point.y) / (cp.point.x - xr.point.x));
						float tanzl = Mathf.Abs((cp.point.y - zl.point.y) / (cp.point.z - zl.point.z));
						float tanzr = Mathf.Abs((cp.point.y - zr.point.y) / (cp.point.z - zr.point.z));

						float tanx = Mathf.Min(tanxl, tanxr);
						float tanz = Mathf.Min(tanzl, tanzr);

						if (xrLayer == linkLayer || xlLayer == linkLayer)
							tanx = 0;
						if (zrLayer == linkLayer || zlLayer == linkLayer)
							tanz = 0;

						float maxTan = Mathf.Max(tanx, tanz);

						int msk = maxTan > tan_slope ? 1 : 0;
						if (msk > 0)
						{
							if (Mathf.Approximately(maxTan, tanxl))
							{
								if (xl.terrain == (byte)TerrainType.ShortWall || xl.terrain == (byte)TerrainType.TallWall)
									cp.terrain = xl.terrain;
							}
							else if (Mathf.Approximately(maxTan, tanxr))
							{
								if (xr.terrain == (byte)TerrainType.ShortWall || xr.terrain == (byte)TerrainType.TallWall)
									cp.terrain = xr.terrain;
							}
							else if (Mathf.Approximately(maxTan, tanzl))
							{
								if (zl.terrain == (byte)TerrainType.ShortWall || zl.terrain == (byte)TerrainType.TallWall)
									cp.terrain = zl.terrain;
							}
							else if (Mathf.Approximately(maxTan, tanzr))
							{
								if (zr.terrain == (byte)TerrainType.ShortWall || zr.terrain == (byte)TerrainType.TallWall)
									cp.terrain = zr.terrain;
							}
						}
						cp.mask = (ushort)msk;
					}

					if (cp.terrain == (byte)TerrainType.Default)
						cp.terrain = cp.mask > 0 ? (byte)TerrainType.Unwalkable : (byte)TerrainType.Walkable;
				}
				UpdateProgress(x, width, "");
			}

			EditorUtility.ClearProgressBar();
		}


		private void DynamicObstacleTest()
		{

		}


		private void CreateAsset()
		{
			NavGrid2DData nav = ScriptableObject.CreateInstance<NavGrid2DData>();
			ushort[,] maskList = new ushort[width, height];
			byte[,] terrain = new byte[width, height];
			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					maskList[x, y] = gridList[x, y].mask;
					terrain[x, y] = gridList[x, y].terrain;
				}
			}
			nav._setData(maskList, terrain, width, height, 
				FixMath.m2mm(gridSize), FixMath.m2mm(minX), FixMath.m2mm(minZ),
				FixMath.m2mm(agentRadius), slope);
			navData = nav;
		}


		private static TerrainType GetTerrainType(string tag)
		{
			if (string.IsNullOrEmpty(tag))
				return TerrainType.Default;
			else if (tag == "ShortWall")
				return TerrainType.ShortWall;
			else if (tag == "TallWall")
				return TerrainType.TallWall;
			return TerrainType.Default;
		}


		static void UpdateProgress(int progress, int progressMax, string desc)
		{
			string title = "Building[" + progress + " / " + progressMax + "]";
			float value = (float)progress / (float)progressMax;
			EditorUtility.DisplayProgressBar(title, desc, value);
		}

	}

}