
using UnityEngine;
using AStar;


namespace PathFinding
{
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


		private float Angle = 60;
		private float Radius = 5;
		private int segmentDigree = 10;
		private Mesh mesh;

		private void RegenerateMesh()
		{
			int segmentCount = (int)(Angle / segmentDigree) + 1;

			int vertexCount = segmentCount + 2;
			int triangleCount = segmentCount;

			Vector3[] vertices = new Vector3[vertexCount];
			int[] triangles = new int[triangleCount * 3];

			float maxDigree = 90 + Angle * 0.5f;
			float minDigree = 90 - Angle * 0.5f;
			float height = 0.25f;
			vertices[0] = new Vector3(0, height, 0);
			//遵循顺时针三点确定一面
			for (int i = 1; i < vertices.Length; ++i)
			{
				float deltaDigree = vertices.Length - i == 1 ? minDigree : (i - 1) * segmentDigree;
				float digree = maxDigree - deltaDigree;
				float x = Radius * (float)System.Math.Cos(digree / 180 * System.Math.PI);
				float y = height;
				float z = Radius * (float)System.Math.Sin(digree / 180 * System.Math.PI);
				vertices[i] = new Vector3(x, y, z);
			}

			int index = 0;
			for (int i = 0; i < triangles.Length;)
			{
				triangles[i++] = 0;
				triangles[i++] = index + 1;
				triangles[i++] = index + 2;
				index++;
			}

			mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
		}


		public void SetGridPosList(NavGrid2DData mask, Vector3[,] grids, int w, int h)
		{
			navigation = mask;
			gridPosList = grids;
			width = w;
			height = h;

			RegenerateMesh();
		}


		void OnDrawGizmosSelected()
		{
			Transform trans = gameObject.transform;

			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = trans.localToWorldMatrix;

			Color defaultColor = Gizmos.color;

			float a = FixMath.mm2m(navigation.GridSize) / 2;

			Gizmos.DrawMesh(mesh);

			// passables
			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; ++j)
				{
					Vector3 c = gridPosList[i, j];
					bool passable = navigation.GetMask(i, j) == 0;
					if (passable)
					{
						Gizmos.color = Color.green;
						DrawRect(c, a);
					}
					byte terrain = navigation.GetTerrain(i, j);
					if (terrain == (byte)TerrainType.ShortWall)
					{
						Gizmos.color = Color.red;
						DrawRect(c, a / 2);
					}
					else if (terrain == (byte)TerrainType.TallWall)
					{
						Gizmos.color = Color.cyan;
						DrawRect(c, a / 2);
					}
				}
			}

			// unpassables
			/*for (int i = 0; i < width; ++i)
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
			}*/

			// border lines
			Gizmos.DrawLine(gridPosList[0, 0], gridPosList[width - 1, 0]);
			Gizmos.DrawLine(gridPosList[0, 0], gridPosList[0, height - 1]);
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

}