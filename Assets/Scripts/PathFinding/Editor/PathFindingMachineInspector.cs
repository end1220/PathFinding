

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
			machine.pathMode = (PathMode)EditorGUILayout.EnumPopup("Navgation Mode", machine.pathMode);
			machine.EnableMultiThread = EditorGUILayout.Toggle("Enable Multi Thread", machine.EnableMultiThread);
			machine.navgationData = EditorGUILayout.ObjectField("Nav Asset", machine.navgationData, typeof(INavData), false) as INavData;

			SelectInspector();
			inspector.DrawInspector();

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

		void SelectInspector()
		{
			switch (machine.pathMode)
			{
				case PathMode.Grid2D:
					inspector = grid2d;
					break;
				case PathMode.Graph3D:
					inspector = graph3d;
					break;
				case PathMode.NavMesh:
					inspector = navMesh;
					break;
			}
		}

		private void MarkDirty()
		{
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			EditorUtility.SetDirty(machine);
		}


	}
}