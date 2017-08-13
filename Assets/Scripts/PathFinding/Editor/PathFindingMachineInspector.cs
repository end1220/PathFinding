

using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;



namespace PathFinding
{
	[CustomEditor(typeof(PathFindingMachine))]
	public class PathFindingMachineInspector : Editor
	{
		private PathFindingMachine machine;

		private EditorInspector inspector;
		private Grid2DInspector grid2d;
		private Graph3DInspector graph3d;
		private NavMeshInspector navMesh;


		void OnEnable()
		{
			machine = target as PathFindingMachine;
			EnaureInspector();
		}


		public override void OnInspectorGUI()
		{
			if (machine.pathMode == PathMode.Grid2D)
			{
				inspector = grid2d;
				machine.navGrid = EditorGUILayout.ObjectField("Nav Asset", machine.navGrid, typeof(NavGrid2DData), false) as NavGrid2DData;
			}
			else if (machine.pathMode == PathMode.Graph3D)
			{
				inspector = graph3d;
				machine.navGraph = EditorGUILayout.ObjectField("Nav Asset", machine.navGraph, typeof(NavGraph3DData), false) as NavGraph3DData;
			}
			else if (machine.pathMode == PathMode.NavMesh)
			{
				inspector = navMesh;
				machine.navMesh = EditorGUILayout.ObjectField("Nav Asset", machine.navMesh, typeof(NavMeshData), false) as NavMeshData;
			}

			EditorGUILayout.Separator();
			machine.pathMode = (PathMode)EditorGUILayout.EnumPopup("Navgation Mode", machine.pathMode);
			EditorGUILayout.Separator();

			inspector.DrawInspector();

			EditorGUILayout.Separator();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Clear", GUILayout.Width(80), GUILayout.Height(40)))
			{
				inspector.Clear();
				MarkDirty();
			}
			GUILayout.Space(20);
			if (GUILayout.Button("Bake", GUILayout.Width(80), GUILayout.Height(40)))
			{
				inspector.Bake();
				inspector.Save();
				MarkDirty();
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.Separator();

		}


		void EnaureInspector()
		{
			if (grid2d == null)
				grid2d = new Grid2DInspector(machine);
			if (graph3d == null)
				graph3d = new Graph3DInspector(machine);
			if (navMesh == null)
				navMesh = new NavMeshInspector(machine);
		}


		private void MarkDirty()
		{
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			EditorUtility.SetDirty(machine);
		}


	}
}