
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Lite;
using TwGame.Graph;


namespace Lite.AStar.NavGraph
{

	public class NavGraphBuilder
	{
		public BuildConfig cfg;

		public Cell[,,] cells;

		public NavGraphData navData;

		
		public void Stetup(GameObject box, float cellSize, float agentHeight, float tanSlope)
		{
			cfg = new BuildConfig();

			if (box == null)
			{
				Debug.LogError("world box is null.");
				return;
			}
			var render = box.GetComponent<MeshRenderer>();
			if (render == null)
			{
				Debug.LogError("world box has no MeshRenderer");
			}

			int terrainLayer = LayerMask.NameToLayer(AppConst.LayerTerrain);
			int obstacleLayer = LayerMask.NameToLayer(AppConst.LayerObstacle);
			int linkLayer = LayerMask.NameToLayer(AppConst.LayerLink);

			cfg.allTestMask = 1 << terrainLayer | 1 << linkLayer | 1 << obstacleLayer;
			cfg.walkableMask = 1 << terrainLayer | 1 << linkLayer;
			cfg.obstacleMask = 1 << obstacleLayer;
			var worldSize = render.bounds.size;
			cfg.box = box;
			cfg.worldSize = new Int3(worldSize);
			cfg.worldCenterPos = new Int3(render.bounds.center);
			cfg.worldMinPos = new Int3(render.bounds.min);
			cfg.cellSize = (int)TwMath.m2mm(cellSize);
			cfg.agentHeight = agentHeight;
			cfg.tanSlope = tanSlope;
			cfg.cellCountX = Mathf.RoundToInt(worldSize.x / cellSize);
			cfg.cellCountY = Mathf.RoundToInt(worldSize.y / cellSize);
			cfg.cellCountZ = Mathf.RoundToInt(worldSize.z / cellSize);
		}


		public void Build()
		{
			if (cfg.box == null)
				return;

			try
			{
				BuildCells();
				RoleHeightTesting();
				CellsToGraph();
				SlopTesting();
				EditorUtility.ClearProgressBar();
			}
			catch (System.Exception e)
			{
				EditorUtility.ClearProgressBar();
				Debug.LogError("Build Failed ! " + e.ToString());
			}
		}


		#region ------栅格化------

		private void BuildCells()
		{
			if (cfg.box == null)
				return;

			int totalSize = cfg.cellCountX * cfg.cellCountY * cfg.cellCountZ;
			cells = new Cell[cfg.cellCountX, cfg.cellCountY, cfg.cellCountZ];
			float cellSize = cfg.cellSize / 1000f;
			float cellRadius = cfg.cellSize/1000f * 0.5f;
			int count = 0;

			Vector3 starPos = cfg.worldMinPos.ToVector3() + new Vector3(cellRadius, cellRadius, cellRadius);

			for (int x = 0; x < cfg.cellCountX; x++)
			{
				for (int y = 0; y < cfg.cellCountY; y++)
				{
					for (int z = 0; z < cfg.cellCountZ; z++)
					{
						Vector3 worldPoint = starPos
							+ Vector3.up * (y * cellSize)
							+ Vector3.right * (x * cellSize)
							+ Vector3.forward * (z * cellSize);

						count++;
						EditorUtility.DisplayProgressBar(string.Format("Voxelizing {0}/{1}", count, totalSize), "", (float)count / totalSize);

						PerformCellTest(worldPoint, x, y, z);
					}
				}
			}

		}


		private void PerformCellTest(Vector3 worldPoint, int x, int y, int z)
		{
			float heightStep = cfg.cellSize / 1000f;
			float nodeDiameter = cfg.cellSize / 1000f;
			float nodeRadius = cfg.cellSize / 1000f / 2;

			Vector3 raycastPoint = worldPoint + Vector3.up * nodeRadius;
			Ray cray = new Ray(raycastPoint, Vector3.down);
			RaycastHit cRayHit;

			//bool overlapWalkable = Physics.CheckSphere(worldPoint, nodeRadius, cfg.walkableMask);

			if (Physics.Raycast(cray, out cRayHit, heightStep, cfg.allTestMask))
			{
				Vector3 actualWorldPoint = cRayHit.point;
				int hitLayer = cRayHit.transform.gameObject.layer;

				bool walkable = ((1 << hitLayer) & cfg.walkableMask) > 0;

				bool canDoObstacleUpdate = false;
				Collider[] obses = Physics.OverlapSphere(actualWorldPoint, nodeRadius, cfg.obstacleMask);
				if (obses.Length > 0)
				{
					walkable = false;
					canDoObstacleUpdate = true;
				}

				// check upper cells for role height
				if (walkable)
				{
					float testHeight = worldPoint.y;
					while (testHeight < worldPoint.y + cfg.agentHeight)
					{
						testHeight += heightStep;

					}
				}

				//Raycasting for walkable regions
				/*int movePenalty = 0;
				if (walkable)
				{
					RaycastHit hit;
					Ray ray = new Ray(actualWorldPoint + Vector3.up * nodeRadius, Vector3.down);
					if (Physics.Raycast(ray, out hit, nodeDiameter, walkableMask))
					{
						
					}
				}*/

				//Make new node in grid
				cells[x, y, z] = new Cell(worldPoint, actualWorldPoint, walkable);

			}
		}

		#endregion


		#region ------Role Height Testing------

		private void RoleHeightTesting()
		{
			int count = 0;
			int totalSize = cfg.cellCountX * cfg.cellCountY * cfg.cellCountZ;
			float cellSize = cfg.cellSize / 1000f;

			for (int x = 0; x < cfg.cellCountX; x++)
			{
				for (int y = 0; y < cfg.cellCountY; y++)
				{
					for (int z = 0; z < cfg.cellCountZ; z++)
					{
						count++;
						EditorUtility.DisplayProgressBar(string.Format("Role Height Testing {0}/{1}", count, totalSize), "", (float)count / totalSize);

						var cell = cells[x, y, z];
						if (cell == null || !cell.walkable)
							continue;

						int stepY = (int)(cfg.agentHeight / cellSize) + 1;
						float testHeight = cell.worldPosition.y;
						while (stepY > 0 && y + stepY < cfg.cellCountY)
						{
							var cellY = cells[x, y + stepY, z];
							if (cellY != null)
							{
								cell.walkable = false;
								break;
							}
							stepY--;
						}
					}
				}
			}
		}

		#endregion



		#region -------graph化--------

		private void CellsToGraph()
		{
			navData = ScriptableObject.CreateInstance<NavGraphData>();
			navData.Init(cfg);
			
			int totalSize = cfg.cellCountX * cfg.cellCountY * cfg.cellCountZ;
			int count = 0;
			float cellSize = cfg.cellSize / 1000f;

			for (int x = 0; x < cfg.cellCountX; ++x)
			{
				for (int y = 0; y < cfg.cellCountY; ++y)
				{
					for (int z = 0; z < cfg.cellCountZ; ++z)
					{
						count++;
						var cell = cells[x, y, z];
						EditorUtility.DisplayProgressBar(string.Format("building nodes {0}/{1}", count, totalSize), "", (float)count / totalSize);
						if (cell == null)
							continue;

						int id = CalcNodeId(x, y ,z);

						var node = new GraphAStar3DNode();
						node.id = id;
						node.x = x;
						node.y = y;
						node.z = z;
						node.worldPosition = new Int3(cell.worldPosition);
						//node.centerPosition = cell.centerPosition;
						node.walkable = cell.walkable;
						navData.AddNode(node);
					}
				}
			}

			count = 0;
			for (int x = 0; x < cfg.cellCountX; ++x)
			{
				for (int y = 0; y < cfg.cellCountY; ++y)
				{
					for (int z = 0; z < cfg.cellCountZ; ++z)
					{
						count++;
						var cell = cells[x, y, z];
						EditorUtility.DisplayProgressBar(string.Format("Building edges {0}/{1}", count, totalSize), "", (float)count / totalSize);
						if (cell == null)
							continue;

						int id = CalcNodeId(x, y ,z);

						for (int ix = -1; ix <= 1; ix++)
						{
							for (int iy = -1; iy <= 1; iy++)
							{
								for (int iz = -1; iz <= 1; iz++)
								{
									int realx = x + ix;
									int realy = y + iy;
									int realz = z + iz;
									if (realx >= 0 && realx < cfg.cellCountX
										&& realy >= 0 && realy < cfg.cellCountY
										&& realz >= 0 && realz < cfg.cellCountZ)
									{
										if (ix == 0 && iz == 0)
											continue;
										var neighbor = cells[realx, realy, realz];
										if (neighbor != null)
										{
											float tan = 0;
											int cost = int.MaxValue;
											int absDelta = Math.Abs(ix) + Math.Abs(iy) + Math.Abs(iz);
											if (neighbor.walkable)
											{
												float dx = cellSize;
												float dy = Math.Abs(neighbor.worldPosition.y - cell.worldPosition.y);
												float dz = cellSize;
												if (absDelta == 1)
												{
													cost = 10;
													if (ix == 0)
														tan = dy / dz;
													else if (iz == 0)
														tan = dy / dx;
													else
														tan = 0;
												}
												else if (absDelta == 2)
												{
													cost = 14;
													if (iy == 0)
														tan = dy / cellSize * 1.414f;
													else
														tan = dy / cellSize;
												}
												else if (absDelta == 3)
												{
													cost = 17;
													tan = dy / cellSize * 1.414f;
												}
											}

											if (tan > cfg.tanSlope)
												cost = int.MaxValue;
											
											int nbId = CalcNodeId(realx, realy, realz);
											var edge = new GraphAStar3DEdge(id, nbId, cost);
											navData.AddEdge(edge);
										}
									}
								}
							}
						}

					}
				}
			}

		}


		private int CalcNodeId(int x, int y, int z)
		{
			int id = x * cfg.cellCountY * cfg.cellCountZ + y * cfg.cellCountZ + z;
			return id;
		}

		#endregion


		#region -------坡度检测--------

		private void SlopTesting()
		{
			/*int count = 0;
			int totalSize = cfg.cellCountX * cfg.cellCountY * cfg.cellCountZ;

			for (int i = 0; i < navData.nodeList.Count; ++i)
			{
				count++;
				var node = navData.nodeList[i];
				if (!node.walkable)
					continue;
				var edgeList = navData.GetEdgeList(node.id);
				for (int j = 0; j < edgeList.Count; ++j)
				{
					var edge = edgeList[j];
					var to = navData.nodeDic[edge.to];
				}
			}*/
		}

		#endregion



	}


}