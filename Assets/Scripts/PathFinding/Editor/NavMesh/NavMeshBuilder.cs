
using System.Collections.Generic;
using UnityEngine;


namespace PathFinding
{

	public class NavMeshBuilder
	{
		private Vector3 offset = Vector3.zero;
		private Vector3 rotation = Vector3.zero;
		private float scale = 1;

		private int initialPenalty = 0;

		private Mesh sourceMesh;
		private Matrix4x4 matrix = Matrix4x4.identity;
		private Matrix4x4 inverseMatrix = Matrix4x4.identity;

		public Int3[] _vertices;
		public int[] _triangles;

		public NavMeshNode[] nodes;


		public void ImportMesh()
		{
			UnityEngine.AI.NavMeshTriangulation triangulatedNavMesh = UnityEngine.AI.NavMesh.CalculateTriangulation();

			Mesh mesh = new Mesh();
			mesh.name = "ExportedNavMesh";
			mesh.vertices = triangulatedNavMesh.vertices;
			mesh.triangles = triangulatedNavMesh.indices;
			sourceMesh = mesh;
		}


		public void ScanInternal()
		{
			if (sourceMesh == null)
				return;

			GenerateMatrix();

			_triangles = sourceMesh.triangles;
			Vector3[] originalVertices = sourceMesh.vertices;

			CombineRepeatedVertices(originalVertices, _triangles, out _vertices);
			InsertTriangles(ref _triangles, _vertices);
			GenerateNodes(_triangles, _vertices);
		}

		public void SetMatrix(Matrix4x4 m)
		{
			matrix = m;
			inverseMatrix = m.inverse;
		}

		public void GenerateMatrix()
		{
			SetMatrix(Matrix4x4.TRS(offset, Quaternion.Euler(rotation), new Vector3(scale, scale, scale)));
		}


		void CombineRepeatedVertices(Vector3[] vectorVertices, int[] triangles, out Int3[] vertices)
		{
			if (vectorVertices.Length == 0 || triangles.Length == 0)
			{
				vertices = new Int3[0];
				nodes = new NavMeshNode[0];
				Debug.LogError("NavMeshBuilder: vertices count is 0??");
				return;
			}

			vertices = new Int3[vectorVertices.Length];

			int c = 0;

			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = (Int3)matrix.MultiplyPoint3x4(vectorVertices[i]);
			}

			var hashedVerts = new Dictionary<Int3, int>();

			var newVertices = new int[vertices.Length];

			UnityEngine.Profiling.Profiler.EndSample();
			UnityEngine.Profiling.Profiler.BeginSample("Hashing");

			for (int i = 0; i < vertices.Length; i++)
			{
				if (!hashedVerts.ContainsKey(vertices[i]))
				{
					newVertices[c] = i;
					hashedVerts.Add(vertices[i], c);
					c++;
				}
			}

			for (int x = 0; x < triangles.Length; x++)
			{
				Int3 vertex = vertices[triangles[x]];

				triangles[x] = hashedVerts[vertex];
			}

			Int3[] totalIntVertices = vertices;
			vertices = new Int3[c];
			for (int i = 0; i < c; i++)
			{
				vertices[i] = totalIntVertices[newVertices[i]];
			}
		}


		void GenerateNodes(int[] triangles, Int3[] vertices)
		{
			UnityEngine.Profiling.Profiler.BeginSample("Constructing Nodes");

			nodes = new NavMeshNode[triangles.Length / 3];

			for (int i = 0; i < nodes.Length; i++)
			{
				nodes[i] = new NavMeshNode(i);
				NavMeshNode node = nodes[i];

				node.Penalty = initialPenalty;
				node.Walkable = true;

				node.v0 = vertices[triangles[i * 3]];
				node.v1 = vertices[triangles[i * 3 + 1]];
				node.v2 = vertices[triangles[i * 3 + 2]];

				if (!VectorMath.IsClockwiseXZ(node.v0, node.v1, node.v2))
				{
					Int3 tmp = node.v0;
					node.v0 = node.v2;
					node.v2 = tmp;
				}

				if (VectorMath.IsColinearXZ(node.v0, node.v1, node.v2))
				{
					Debug.DrawLine((Vector3)node.v0, (Vector3)node.v1, Color.red);
					Debug.DrawLine((Vector3)node.v1, (Vector3)node.v2, Color.red);
					Debug.DrawLine((Vector3)node.v2, (Vector3)node.v0, Color.red);
				}

				// Make sure position is correctly set
				node.UpdatePositionFromVertices();
			}

			UnityEngine.Profiling.Profiler.EndSample();

			var sides = new Dictionary<Int2, NavMeshNode>();

			for (int i = 0, j = 0; i < triangles.Length; j += 1, i += 3)
			{
				sides[new Int2(triangles[i + 0], triangles[i + 1])] = nodes[j];
				sides[new Int2(triangles[i + 1], triangles[i + 2])] = nodes[j];
				sides[new Int2(triangles[i + 2], triangles[i + 0])] = nodes[j];
			}

			UnityEngine.Profiling.Profiler.BeginSample("Connecting Nodes");

			var connections = new List<int>();
			var connectionCosts = new List<int>();

			for (int i = 0, j = 0; i < triangles.Length; j += 1, i += 3)
			{
				connections.Clear();
				connectionCosts.Clear();

				NavMeshNode node = nodes[j];

				for (int q = 0; q < 3; q++)
				{
					NavMeshNode other;
					if (sides.TryGetValue(new Int2(triangles[i + ((q + 1) % 3)], triangles[i + q]), out other))
					{
						connections.Add(other.id);
						connectionCosts.Add((node.position - other.position).costMagnitude);
					}
				}

				node.connections = connections.ToArray();
				node.connectionCosts = connectionCosts.ToArray();
			}

			UnityEngine.Profiling.Profiler.EndSample();

			UnityEngine.Profiling.Profiler.BeginSample("Rebuilding BBTree");

			//RebuildBBTree(this);

			UnityEngine.Profiling.Profiler.EndSample();

			//Debug.Log("Node Count " + nodes.Length);
		}

		struct ExchangeTriangleIndex
		{
			public int index1;
			public int index2;
			public int nextIndex;
		}

		void InsertTriangles(ref int[] triangles, Int3[] vertices)
		{
			List<int> insertTriangles = new List<int>();
			insertTriangles.Clear();

			Dictionary<Int3, ExchangeTriangleIndex> exchangeList = new Dictionary<Int3, ExchangeTriangleIndex>();

			Int3[] v0 = new Int3[3];
			Int3[] v1 = new Int3[3];

			int triangleCount = triangles.Length / 3;
			for (int i = 0; i < triangleCount; i++)
			{
				v0[0] = vertices[triangles[i * 3]];
				v0[1] = vertices[triangles[i * 3 + 1]];
				v0[2] = vertices[triangles[i * 3 + 2]];

				for (int j = i + 1; j < triangleCount; j++)
				{
					v1[0] = vertices[triangles[j * 3]];
					v1[1] = vertices[triangles[j * 3 + 1]];
					v1[2] = vertices[triangles[j * 3 + 2]];

					// must have only one same vertex
					int sameVertexCount = 0;
					for (int m = 0; m < 3; m++)
						for (int n = 0; n < 3; n++)
							if (v0[m] == v1[n])
								sameVertexCount++;
					if (sameVertexCount != 1)
						continue;

					// find middle point if exist
					for (int m = 0; m < 3; m++)
					{
						for (int n = 0; n < 3; n++)
						{
							// not the same vertex
							if (v0[m] == v1[n] || v0[(m + 1) % 3] == v1[n])
								continue;
							// must colinear
							if (!VectorMath.IsColinearXZ(v0[m], v0[(m + 1) % 3], v1[n]))
								continue;
							// v1 must be middle vertex
							Int3 middle = v1[n];
							var d1 = middle - v0[m];
							var d2 = middle - v0[(m + 1) % 3];
							if (d1.x * d2.x > 0 || d1.y * d2.y > 0 || d1.z * d2.z > 0)
								continue;
							// already found it.
							if (exchangeList.ContainsKey(middle))
								continue;

							ExchangeTriangleIndex exchange = new ExchangeTriangleIndex();
							exchange.index1 = i * 3 + ((m + 1) % 3);
							exchange.index2 = j * 3 + n;
							exchange.nextIndex = i * 3 + ((m + 2) % 3);
							exchangeList.Add(middle, exchange);

							break;
						}
					}
				}
			}

			if (exchangeList.Count > 0)
			{
				//Debug.Log("exchange count " + exchangeList.Count);
				foreach (var item in exchangeList)
				{
					ExchangeTriangleIndex data = item.Value;
					int oldIdx = triangles[data.index1];
					triangles[data.index1] = triangles[data.index2];
					int nextIdx = triangles[data.nextIndex];

					insertTriangles.Add(triangles[data.index2]);
					insertTriangles.Add(oldIdx);
					insertTriangles.Add(nextIdx);

					/*newPoints.Add(vertices[triangles[data.index2]]);
					newPoints.Add(vertices[oldIdx]);
					newPoints.Add(vertices[nextIdx]);*/

				}

				if (insertTriangles.Count > 0)
				{
					//Debug.Log("new triangle count " + insertTriangles.Count);
					int[] newTriangles = new int[triangles.Length + insertTriangles.Count];
					for (int i = 0; i < triangles.Length; ++i)
						newTriangles[i] = triangles[i];
					for (int i = 0; i < insertTriangles.Count; ++i)
						newTriangles[triangles.Length + i] = insertTriangles[i];
					triangles = newTriangles;
				}
			}

		}
		
		//public List<Int3> newPoints = new List<Int3>();
		
	}

}

