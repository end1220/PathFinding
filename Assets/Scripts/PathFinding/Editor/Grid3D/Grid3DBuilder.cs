
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PathFinding.Grid3D;


namespace PathFinding
{

	public class Grid3DBuilder
	{
		public BuildConfig cfg;

		public List<SubSpace> subSpaces = new List<SubSpace>();

		public List<Cell> rawCells;
		public List<Cell> finalCells;
		public Cell[, ,] cellArray;
		private int idCounter = 0;

		public Grid3DNavData navData;
		public Grid3DGraph graphMap;

		private bool Enable8Directions = true;


		public void Stetup(GameObject box, float cellSize, float agentHeight, float agentRadius, float tanSlope)
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
			cfg.cellSize = (int)FixMath.m2mm(cellSize);
			cfg.agentHeight = agentHeight;
			cfg.agentRadius = agentRadius;
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
				BuildFinalCells();
				
				EditorUtility.ClearProgressBar();
			}
			catch (Exception e)
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

			int xSpaceCount = cfg.cellCount.x / subGridCount + ((cfg.cellCount.x % subGridCount) == 0 ? 0 : 1);
			int ySpaceCount = cfg.cellCount.y / subGridCount + ((cfg.cellCount.y % subGridCount) == 0 ? 0 : 1);
			int zSpaceCount = cfg.cellCount.z / subGridCount + ((cfg.cellCount.z % subGridCount) == 0 ? 0 : 1);
			int totalCount = xSpaceCount * ySpaceCount * zSpaceCount;

			subSpaces.Clear();

			int count = 0;
			for (int x = 0; x < xSpaceCount; ++x)
			{
				for (int y = 0; y < ySpaceCount; ++y)
				{
					for (int z = 0; z < zSpaceCount; ++z)
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
							int xGrid = subGridCount;
							int yGrid = subGridCount;
							int zGrid = subGridCount;
							if (x == xSpaceCount - 1)
							{
								xGrid = cfg.cellCount.x - x * subGridCount;
							}
							if (y == ySpaceCount - 1)
							{
								yGrid = cfg.cellCount.y - y * subGridCount;
							}
							if (z == zSpaceCount - 1)
							{
								zGrid = cfg.cellCount.z - z * subGridCount;
							}
							space.cellCount = new Int3(xGrid, yGrid, zGrid);
							space.startIndex = new Int3(x * subGridCount, y * subGridCount, z * subGridCount);
							space.minPos = new Int3(cfg.worldMinPos.x + x * intSubSize, cfg.worldMinPos.y + y * intSubSize, cfg.worldMinPos.z + z * intSubSize);
						}
						count++;
						EditorUtility.DisplayProgressBar(string.Format("Subdividing scene{0}/{1}", count, totalCount), "", (float)count / totalCount);
					}
				}
			}

		}

		#endregion


		#region ------栅格化------

		private void BuildCells()
		{
			idCounter = 0;
			rawCells = new List<Cell>();
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
			float roleRadius = cfg.agentRadius;
			float checkRadius = nodeRadius + roleRadius;

			Vector3 raycastPoint = worldPoint + Vector3.up * nodeRadius;
			Ray cray = new Ray(raycastPoint, Vector3.down);
			RaycastHit cRayHit;

			if (Physics.Raycast(cray, out cRayHit, heightStep, cfg.allTestMask))
			{
				Vector3 actualWorldPoint = cRayHit.point;
				int hitLayer = cRayHit.transform.gameObject.layer;

				bool walkable = ((1 << hitLayer) & cfg.walkableMask) > 0;

				// test if obstacles in range
				Collider[] obses = Physics.OverlapSphere(actualWorldPoint, checkRadius, cfg.obstacleMask);
				if (obses.Length > 0)
				{
					walkable = false;
				}

				// test if walkables in nodeRadius + roleRadius. 
				// Affect the min distance to walls.
				float upDistance = Math.Max(checkRadius * 1.45f, nodeDiameter);
				bool overlapWalkable = Physics.CheckSphere(actualWorldPoint + Vector3.up * upDistance, checkRadius, cfg.walkableMask);
				if (overlapWalkable)
				{
					walkable = false;
				}

				// center and corner point
				Vector3 centerPos = Vector3.zero;
				Vector3 pos1 = Vector3.zero;
				Vector3 pos2 = Vector3.zero;
				Vector3 pos3 = Vector3.zero;
				Vector3 pos4 = Vector3.zero;

				//Raycasting for walkable regions, checking if the cell's conrers are in walkable areas.
				//Checking if there's enough space for agent radius.
				if (walkable)
				{
					RaycastHit hit;
					Vector3 upperPos = actualWorldPoint + Vector3.up * nodeRadius;
					Ray ray = new Ray(upperPos, Vector3.down);
					if (Physics.Raycast(ray, out hit, nodeDiameter, cfg.walkableMask))
					{
						// center pos
						centerPos = hit.point;
						// conrers pos
						ray.origin = upperPos + Vector3.up * nodeRadius + (Vector3.left + Vector3.back) * nodeRadius;
						if (Physics.Raycast(ray, out hit, nodeDiameter * 2, cfg.walkableMask))
							pos1 = hit.point;
						ray.origin = upperPos + Vector3.up * nodeRadius + (Vector3.left + Vector3.forward) * nodeRadius;
						if (Physics.Raycast(ray, out hit, nodeDiameter * 2, cfg.walkableMask))
							pos2 = hit.point;
						ray.origin = upperPos + Vector3.up * nodeRadius + (Vector3.right + Vector3.forward) * nodeRadius;
						if (Physics.Raycast(ray, out hit, nodeDiameter * 2, cfg.walkableMask))
							pos3 = hit.point;
						ray.origin = upperPos + Vector3.up * nodeRadius + (Vector3.right + Vector3.back) * nodeRadius;
						if (Physics.Raycast(ray, out hit, nodeDiameter * 2, cfg.walkableMask))
							pos4 = hit.point;

						if (pos1 == Vector3.zero || pos2 == Vector3.zero || pos3 == Vector3.zero || pos4 == Vector3.zero)
						{
							walkable = false;
						}
						else
						{
							// cliff test
							ray.origin = pos1 + Vector3.up * nodeDiameter + (Vector3.left + Vector3.back) * roleRadius;
							if (walkable && !Physics.Raycast(ray, out hit, nodeDiameter * 2, cfg.walkableMask))
								walkable = false;
							ray.origin = pos2 + Vector3.up * nodeDiameter + (Vector3.left + Vector3.forward) * roleRadius;
							if (walkable && !Physics.Raycast(ray, out hit, nodeDiameter * 2, cfg.walkableMask))
								walkable = false;
							ray.origin = pos3 + Vector3.up * nodeDiameter + (Vector3.right + Vector3.forward) * roleRadius;
							if (walkable && !Physics.Raycast(ray, out hit, nodeDiameter * 2, cfg.walkableMask))
								walkable = false;
							ray.origin = pos4 + Vector3.up * nodeDiameter + (Vector3.right + Vector3.back) * roleRadius;
							if (walkable && !Physics.Raycast(ray, out hit, nodeDiameter * 2, cfg.walkableMask))
								walkable = false;
						}
					}
					else
					{
						walkable = false;
					}
				}

				if (walkable)
				{
					int id = CalcNodeId(x, y, z);
					var cell = new Cell(id, new Int3(x, y, z), centerPos, walkable, pos1, pos2, pos3, pos4);
					cellArray[x, y, z] = cell;
					rawCells.Add(cell);
				}
			}
		}

		#endregion



		private void RoleHeightTesting()
		{
			int count = 0;
			float cellSize = cfg.cellSize / 1000f;
			float stepOneSize = cellSize / 5;

			for (int i = 0; i < rawCells.Count; ++i)
			{
				count++;
				if (count % 10 == 0)
					EditorUtility.DisplayProgressBar(string.Format("Testing role height {0}/{1}", count, rawCells.Count), "", (float)count / rawCells.Count);

				var cell = rawCells[i];
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
				// inside one up-cell
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


		private void CellsToGraph()
		{
			navData = ScriptableObject.CreateInstance<Grid3DNavData>();
			navData.Init(cfg);
			
			int count = 0;
			float cellSize = cfg.cellSize / 1000f;

			// make nodes
			for (int i = 0; i < rawCells.Count; ++i)
			{
				count++;
				if (count % 10 == 0)
					EditorUtility.DisplayProgressBar(string.Format("Building graph nodes {0}/{1}", count, rawCells.Count), "", (float)count / rawCells.Count);

				var cell = rawCells[i];
				if (cell == null || !cell.walkable)
					continue;

				int x = cell.pos.x;
				int y = cell.pos.y;
				int z = cell.pos.z;
				int id = cell.id;

				var node = new Grid3DNode();
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

			for (int i = 0; i < rawCells.Count; ++i)
			{
				if (i % 10 == 0)
					EditorUtility.DisplayProgressBar(string.Format("Building graph edges {0}/{1}", i, rawCells.Count), "", (float)i / rawCells.Count);

				var cell = rawCells[i];
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
											var edge = new Grid3DEdge(id, nbId, cost);
											navData.AddEdge(edge);
										}
									}
								}
							}
						}
					}
				}
			}

			graphMap = new Grid3DGraph();
			graphMap.Init(navData);
		}


		private void BuildFinalCells()
		{
			finalCells = new List<Cell>();
			for (int i = 0; i < rawCells.Count; ++i)
			{
				var cell = rawCells[i];
				if (cell == null || !cell.walkable)
					continue;
				finalCells.Add(cell);
			}
		}


		private int CalcNodeId(int x, int y, int z)
		{
			return idCounter++;
			//int id = x * cfg.cellCount.y * cfg.cellCount.z + y * cfg.cellCount.z + z;
			//return id;
		}

	}


}