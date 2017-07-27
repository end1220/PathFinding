
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace PathFinding
{

	public class NavMeshEditor : EditorWindow
	{
		public Vector3 offset = Vector3.zero;

		public Vector3 rotation = Vector3.zero;

		public float scale = 1;

		public int initialPenalty = 0;



		private Mesh sourceMesh;

		private int[] triangles;

		Vector3[] originalVertices;

		Int3[] _vertices;

		public Matrix4x4 matrix = Matrix4x4.identity;

		public Matrix4x4 inverseMatrix = Matrix4x4.identity;

		public NavMeshNode[] nodes;

		string saveFilePath = "";
		readonly string drawObjectName = "_NavGraphData_Gizmo";



		void OnEnable()
		{
			string scenePath = EditorUtils.GetCurrentScenePath();
			saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_navmesh.asset";
		}


		void OnGUI()
		{
			if (GUILayout.Button("Generate", GUILayout.Width(100), GUILayout.Height(60)))
				GenerateNav();

			GUILayout.Space(20);

			if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(60)))
				SaveToFile();
		}


		void OnDestroy()
		{
			ClearDrawGrid();

			UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
		}


		void GenerateNav()
		{
			try
			{
				ImportMesh();
				ScanInternal();

				navData = ScriptableObject.CreateInstance<NavMeshData>();
				navData.nodes = nodes;

				DrawGrid();

				Selection.SetActiveObjectWithContext(GameObject.Find(drawObjectName), null);
			}
			catch (System.Exception e)
			{
				UnityEngine.Debug.LogError(e.ToString());
				EditorUtility.ClearProgressBar();
			}
		}


		private void ImportMesh()
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
			{
				return;
			}

			GenerateMatrix();

			Vector3[] vectorVertices = sourceMesh.vertices;

			triangles = sourceMesh.triangles;

			GenerateNodes(vectorVertices, triangles, out originalVertices, out _vertices);
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


		void GenerateNodes(Vector3[] vectorVertices, int[] triangles, out Vector3[] originalVertices, out Int3[] vertices)
		{
			UnityEngine.Profiling.Profiler.BeginSample("Init");

			if (vectorVertices.Length == 0 || triangles.Length == 0)
			{
				originalVertices = vectorVertices;
				vertices = new Int3[0];
				nodes = new NavMeshNode[0];
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
			originalVertices = new Vector3[c];
			for (int i = 0; i < c; i++)
			{
				vertices[i] = totalIntVertices[newVertices[i]];
				originalVertices[i] = vectorVertices[newVertices[i]];
			}

			UnityEngine.Profiling.Profiler.EndSample();
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

			var connections = new List<NavMeshNode>();
			var connectionCosts = new List<uint>();

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
						connections.Add(other);
						connectionCosts.Add((uint)(node.position - other.position).costMagnitude);
					}
				}

				node.connections = connections.ToArray();
				node.connectionCosts = connectionCosts.ToArray();
			}

			UnityEngine.Profiling.Profiler.EndSample();
			UnityEngine.Profiling.Profiler.BeginSample("Rebuilding BBTree");

			//RebuildBBTree(this);

			UnityEngine.Profiling.Profiler.EndSample();

		}


		void DrawGrid()
		{
			var go = GameObject.Find(drawObjectName);
			if (go == null)
			{
				go = new GameObject(drawObjectName);
				go.AddComponent<NavMeshGizmo>();
			}
			try
			{
				NavMeshGizmo gizmo = go.GetComponent<NavMeshGizmo>();
				gizmo.navData = navData;
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
			}

		}


		void ClearDrawGrid()
		{
			var go = GameObject.Find(drawObjectName);
			if (go != null)
			{
				GameObject.DestroyImmediate(go);
			}
		}


		NavMeshData navData;
		void SaveToFile()
		{
			var existingAsset = AssetDatabase.LoadAssetAtPath<NavMeshData>(saveFilePath);
			if (existingAsset == null)
			{
				AssetDatabase.CreateAsset(navData, saveFilePath);
				AssetDatabase.Refresh();
				existingAsset = navData;
			}
			else
			{
				EditorUtility.CopySerialized(navData, existingAsset);
			}

			EditorUtility.SetDirty(navData);

			UnityEngine.Debug.Log("Saved  : " + saveFilePath);
		}


	}

}