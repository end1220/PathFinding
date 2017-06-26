

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TwGame.NavMesh
{

	public class NavMeshBuilder
	{
		public class Settings
		{
			public float sceneWidth;
			public float sceneHeight;
			public float sceneMinx;
			public float sceneMinz;
			public float agentRadius;
			public float agentHeight;
			public float stepHeight;
			public float maxSlope;
			public float minArea;
		}

		private Settings setting;

		private NavMeshData navMeshData;

		public List<Triangle> triangles = new List<Triangle>();

		class GridInfo
		{
			public byte mask;
			public Vector3 point;
		}


		public void Build(Settings setting, ref NavMeshData data)
		{
			this.setting = setting;
			this.navMeshData = data;

			DefineWalkableAreas();
			MergePassableAreas();
			DefineBlockedAreas();
			ExpandBlockedAreas();
			SubstractBlockedAreas();
			OptimizeHoles();
			TessellatePolygons();
			Triangulate();
			BuildGraphStructure();
		}

		private void DefineWalkableAreas()
		{
			const float gridSize = 0.1f;
			int width = (int)(setting.sceneWidth / gridSize);
			int height = (int)(setting.sceneHeight / gridSize);
			GridInfo[,] gridList = new GridInfo[width,height];

			int terrainLayer = LayerMask.NameToLayer(AppConst.LayerTerrain);
			int obstacleLayer = LayerMask.NameToLayer(AppConst.LayerObstacle);
			int linkLayer = LayerMask.NameToLayer(AppConst.LayerLink);

			Ray ray = new Ray();
			ray.direction = new Vector3(0, -1, 0);

			RaycastHit hit;
			int layerMask = 1 << terrainLayer
			| 1 << obstacleLayer
			| 1 << linkLayer;

			
			float badY = 10000;
			
			for (int x = 0; x < width; ++x)
			{
				for (int z = 0; z < height; ++z)
				{
					float posx = setting.sceneMinx + x * gridSize;
					float posz = setting.sceneMinz + z * gridSize;

					gridList[x, z] = new GridInfo();
					gridList[x, z].point = new Vector3(posx, badY, posz);

					float[] sidex = new float[]
					{
						posx - setting.agentRadius,
						posx + setting.agentRadius,
						posx + setting.agentRadius,
						posx - setting.agentRadius
					};
					float[] sidez = new float[]
					{
						posz - setting.agentRadius,
						posz - setting.agentRadius,
						posz + setting.agentRadius,
						posz + setting.agentRadius
					};
					bool hitObstacle = false;
					for (int s = 0; s < 4; s++)
					{
						ray.origin = new Vector3(sidex[s], 1000, sidez[s]);
						if (Physics.Raycast(ray, out hit, 1001, layerMask))
						{
							if (hit.collider.gameObject.layer == obstacleLayer)
							{
								hitObstacle = true;
								break;
							}
						}
						else
						{
							hitObstacle = true;
							break;
						}
					}

					ray.origin = new Vector3(posx, 1000, posz);
					if (Physics.Raycast(ray, out hit, 1001, layerMask))
					{
						var hitLayer = hit.collider.gameObject.layer;

						gridList[x, z].point = new Vector3(posx, hit.point.y, posz);
						gridList[x, z].mask = (byte)(hitLayer == obstacleLayer || hitObstacle ? 1 : 0);
					}
					else
					{
						gridList[x, z].mask = 1;
					}
				}
				UpdateProgress(x, width, "");
			}

			for (int x = 0; x < width-1; ++x)
			{
				for (int z = 0; z < height-1; ++z)
				{
					if (x == 0 || z == 0 || gridList[x, z].mask == 1)
					{
						continue;
					}
					else
					{
						int block = 0
							+ gridList[x - 1, z - 1].mask
							+ gridList[x + 1, z - 1].mask
							+ gridList[x - 1, z + 1].mask 
							+ gridList[x + 1, z + 1].mask;

						if (block > 0 && block < 4)
						{
							this.navMeshData.points.Add(gridList[x, z].point);
						}
					}
				}
			}

			gridList = null;

			EditorUtility.ClearProgressBar();

		}

		private void MergePassableAreas()
		{

		}

		private void DefineBlockedAreas()
		{

		}

		private void ExpandBlockedAreas()
		{

		}

		private void SubstractBlockedAreas()
		{

		}

		private void OptimizeHoles()
		{

		}

		private void TessellatePolygons()
		{

		}

		private void Triangulate()
		{

		}

		private void BuildGraphStructure()
		{

		}

		static void UpdateProgress(int progress, int progressMax, string desc)
		{
			string title = "Baking...[" + progress + " / " + progressMax + "]";
			float value = (float)progress / (float)progressMax;
			EditorUtility.DisplayProgressBar(title, desc, value);
		}

	}

}
