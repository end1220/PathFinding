﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Lite.Graph;


namespace Lite.AStar.NavGraph
{

	public class Graph3DBuilder
	{
		public BuildConfig cfg;

		public List<SubSpace> subSpaces = new List<SubSpace>();

		public List<Cell> cells;
		public Cell[, ,] cellArray;
		private int idCounter = 0;

		public NavGraph3DData navData;

		private bool Enable8Directions = true;

		
		public void Stetup(GameObject box, float cellSize, float agentHeight, float tanSlope)
		{
			var render = box.GetComponent<MeshRenderer>();

			cfg = new BuildConfig();

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
			cfg.agentHeightStep = (int)Math.Round(agentHeight / cellSize);
			cfg.tanSlope = tanSlope;
			cfg.cellCount.x = Mathf.RoundToInt(worldSize.x / cellSize);
			cfg.cellCount.y = Mathf.RoundToInt(worldSize.y / cellSize);
			cfg.cellCount.z = Mathf.RoundToInt(worldSize.z / cellSize);
		}


		public void Build()
		{
			if (cfg.box == null)
				return;

			try
			{
				BuildSubSpace();
				BuildCells();
				RoleHeightTesting();
				CellsToGraph();
				
				EditorUtility.ClearProgressBar();
			}
			catch (System.Exception e)
			{
				EditorUtility.ClearProgressBar();
				Debug.LogError("Build Failed ! " + e.ToString());
			}
		}


		#region ------划分子空间------

		private void BuildSubSpace()
		{
			int subGridCount = 10 * 1000 / cfg.cellSize;

			int intSubSize = subGridCount * cfg.cellSize;
			float subSize = intSubSize / 1000f;
			float halfSubSize = subSize / 2f;
			Vector3 worldMinPos = cfg.worldMinPos.ToVector3();

			int xcount = cfg.cellCount.x / subGridCount + ((cfg.cellCount.x % subGridCount) == 0 ? 0 : 1);
			int ycount = cfg.cellCount.y / subGridCount + ((cfg.cellCount.y % subGridCount) == 0 ? 0 : 1);
			int zcount = cfg.cellCount.z / subGridCount + ((cfg.cellCount.z % subGridCount) == 0 ? 0 : 1);
			int totalCount = xcount * ycount * zcount;

			subSpaces.Clear();

			int count = 0;
			for (int x = 0; x < xcount; ++x)
			{
				for (int y = 0; y < ycount; ++y)
				{
					for (int z = 0; z < zcount; ++z)
					{
						float startx = worldMinPos.x + x * subSize;
						float starty = worldMinPos.y + y * subSize;
						float startz = worldMinPos.z + z * subSize;
						Vector3 center = new Vector3(startx + halfSubSize, starty + halfSubSize, startz + halfSubSize);
						Vector3 halfExt = new Vector3(halfSubSize, halfSubSize, halfSubSize);
						var colliders = Physics.OverlapBox(center, halfExt, Quaternion.identity, cfg.allTestMask);
						if (colliders.Length > 0)
						{
							var space = new SubSpace();
							subSpaces.Add(space);
							space.cellCount = new Int3(subGridCount, subGridCount, subGridCount);
							space.startIndex = new Int3(x * subGridCount, y * subGridCount, z * subGridCount);
							space.minPos = new Int3(cfg.worldMinPos.x + x * intSubSize, cfg.worldMinPos.y + y * intSubSize, cfg.worldMinPos.z + z * intSubSize);
						}
						count++;
						EditorUtility.DisplayProgressBar(string.Format("Subdividing {0}/{1}", count, totalCount), "", (float)count / totalCount);
					}
				}
			}

		}

		#endregion


		#region ------栅格化------

		private void BuildCells()
		{
			idCounter = 0;
			cells = new List<Cell>();
			cellArray = new Cell[cfg.cellCount.x, cfg.cellCount.y, cfg.cellCount.z];

			float cellSize = cfg.cellSize / 1000f;
			float cellRadius = cellSize / 2f;

			for (int i = 0; i < subSpaces.Count; ++i)
			{
				SubSpace space = subSpaces[i];
				Vector3 startPos = space.minPos.ToVector3() + new Vector3(cellRadius, cellRadius, cellRadius);
				for (int x = 0; x < space.cellCount.x; ++x)
				{
					for (int y = 0; y < space.cellCount.y; ++y)
					{
						for (int z = 0; z < space.cellCount.z; ++z)
						{
							Vector3 worldPoint = startPos + Vector3.up * (y * cellSize) + Vector3.right * (x * cellSize) + Vector3.forward * (z * cellSize);
							PerformCellTest(worldPoint, space.startIndex.x + x, space.startIndex.y + y, space.startIndex.z + z);
						}
					}
				}

				EditorUtility.DisplayProgressBar(string.Format("Voxelizing {0}/{1}", i+1, subSpaces.Count), "", (float)i+1 / subSpaces.Count);
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

				// test if there're obstacles besides
				Collider[] obses = Physics.OverlapSphere(actualWorldPoint, nodeRadius, cfg.obstacleMask);
				if (obses.Length > 0)
				{
					walkable = false;
				}

				//Raycasting for walkable regions
				if (walkable)
				{
					RaycastHit hit;
					Ray ray = new Ray(actualWorldPoint + Vector3.up * nodeRadius, Vector3.down);
					if (Physics.Raycast(ray, out hit, nodeDiameter, cfg.walkableMask))
					{
						//Make new cell
						actualWorldPoint = hit.point;
						int id = CalcNodeId(x, y, z);
						var cell = new Cell(id, new Int3(x, y, z), actualWorldPoint, walkable);
						cellArray[x, y, z] = cell;
						cells.Add(cell);
					}
				}
			}
		}

		#endregion


		#region ------角色高度测试------

		private void RoleHeightTesting()
		{
			int count = 0;
			//int totalSize = cfg.cellCount.x * cfg.cellCount.y * cfg.cellCount.z;
			float cellSize = cfg.cellSize / 1000f;
			float stepOneSize = cellSize / 5;

			for (int i = 0; i < cells.Count; ++i)
			{
				count++;
				if (count % 10 == 0)
					EditorUtility.DisplayProgressBar(string.Format("Role Height Testing {0}/{1}", count, cells.Count), "", (float)count / cells.Count);

				var cell = cells[i];
				if (cell == null || !cell.walkable)
					continue;

				int x = cell.pos.x;
				int y = cell.pos.y;
				int z = cell.pos.z;
				int stepY = cfg.agentHeightStep + 1;
				while (stepY > 0 && y + stepY < cfg.cellCount.y)
				{
					var cellY = cellArray[x, y + stepY, z];
					if (cellY != null)
					{
						cell.walkable = false;
						break;
					}
					stepY--;
				}
				// inside one step
				float stepOne = 0;
				while (stepOne < cellSize)
				{
					stepOne += stepOneSize;
					Collider[] obses = Physics.OverlapSphere(cell.worldPosition + Vector3.up * stepOne, stepOneSize, cfg.obstacleMask);
					if (obses.Length > 0)
					{
						cell.walkable = false;
						break;
					}
				}
			}
		}

		#endregion



		#region -------graph化--------

		private void CellsToGraph()
		{
			navData = ScriptableObject.CreateInstance<NavGraph3DData>();
			navData.Init(cfg);
			
			//int totalSize = cfg.cellCount.x * cfg.cellCount.y * cfg.cellCount.z;
			int count = 0;
			float cellSize = cfg.cellSize / 1000f;

			// make nodes
			for (int i = 0; i < cells.Count; ++i)
			{
				count++;
				if (count % 10 == 0)
					EditorUtility.DisplayProgressBar(string.Format("Building nodes {0}/{1}", count, cells.Count), "", (float)count / cells.Count);

				var cell = cells[i];
				if (cell == null || !cell.walkable)
					continue;

				int x = cell.pos.x;
				int y = cell.pos.y;
				int z = cell.pos.z;
				int id = cell.id;

				var node = new Graph3DAStarNode();
				node.id = id;
				node.x = (ushort)x;
				node.y = (ushort)y;
				node.z = (ushort)z;
				node.worldPosition = new Int3(cell.worldPosition);
				//node.walkable = cell.walkable;
				navData.AddNode(node);
			}

			// make edges
			HashSet<string> edgeKeySet = new HashSet<string>();

			for (int i = 0; i < cells.Count; ++i)
			{
				if (i % 10 == 0)
					EditorUtility.DisplayProgressBar(string.Format("Building edges {0}/{1}", i, cells.Count), "", (float)i / cells.Count);

				var cell = cells[i];
				if (cell == null || !cell.walkable)
					continue;

				int x = cell.pos.x;
				int y = cell.pos.y;
				int z = cell.pos.z;
				int id = cell.id;

				for (int ix = -1; ix <= 1; ix++)
				{
					for (int iy = -1; iy <= 1; iy++)
					{
						for (int iz = -1; iz <= 1; iz++)
						{
							int realx = x + ix;
							int realy = y + iy;
							int realz = z + iz;
							if (realx >= 0 && realx < cfg.cellCount.x
								&& realy >= 0 && realy < cfg.cellCount.y
								&& realz >= 0 && realz < cfg.cellCount.z)
							{
								if (ix == 0 && iz == 0)
									continue;

								if (!Enable8Directions && ix != 0 && iz != 0)
									continue;

								var neighbor = cellArray[realx, realy, realz];
								if (neighbor != null)
								{
									int nbId = neighbor.id;
									int cost = int.MaxValue;
									int absDelta = Math.Abs(ix) + Math.Abs(iy) + Math.Abs(iz);
									if (neighbor.walkable)
									{
										// to_from edge has been already added, so ignore it
										if (edgeKeySet.Contains(string.Format("{0}_{1}", nbId, id)))
											continue;
										else
											edgeKeySet.Add(string.Format("{0}_{1}", id, nbId));

										float tan = 0;
										float dx = cellSize;
										float dy = Math.Abs(neighbor.worldPosition.y - cell.worldPosition.y);
										float dz = cellSize;
										float distance = (cell.worldPosition - neighbor.worldPosition).magnitude;
										cost = (int)Math.Round(distance / (cfg.cellSize / 1000f) * 10);
										if (absDelta == 1)
										{
											if (ix == 0)
												tan = dy / dz;
											else if (iz == 0)
												tan = dy / dx;
											else
												tan = 0;
										}
										else if (absDelta == 2)
										{
											if (iy == 0)
												tan = dy / cellSize * 1.414f;
											else
												tan = dy / cellSize;
										}
										else if (absDelta == 3)
										{
											tan = dy / cellSize * 1.414f;
										}
										if (tan > cfg.tanSlope)
											cost = int.MaxValue;

										if (cost != int.MaxValue)
										{
											var edge = new Graph3DAStarEdge(id, nbId, cost);
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
			return idCounter++;
			/*int id = x * cfg.cellCount.y * cfg.cellCount.z + y * cfg.cellCount.z + z;
			return id;*/
		}

		#endregion



	}


}