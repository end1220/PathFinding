
using UnityEngine;
using UnityEditor;



namespace PathFinding
{

	public class NavMeshInspector : EditorInspector
	{
		NavMeshBuilder builder = new NavMeshBuilder();

		string saveFilePath = "Assets/{0}_navmesh.asset";

		NavMeshData navData;


		public NavMeshInspector(PathFindingMachine machine):
			base(machine)
		{
			string scenePath = EditorUtils.GetCurrentScenePath();
			saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_navmesh.asset";

			// load previous settings
			var existingAsset = AssetDatabase.LoadAssetAtPath<NavMeshData>(saveFilePath);
			if (existingAsset != null)
			{
				
			}

		}


		public override void DrawInspector()
		{
			base.DrawInspector();
		}

		public override void Clear()
		{
			AssetDatabase.DeleteAsset(saveFilePath);
		}

		public override void Bake()
		{
			try
			{
				builder.ImportMesh();
				builder.ScanInternal();

				navData = ScriptableObject.CreateInstance<NavMeshData>();
				navData.nodes = builder.nodes;

				machine.navMesh = navData;
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
				EditorUtility.ClearProgressBar();
			}
		}

		public override void Save()
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

			machine.navMesh = navData;

			EditorUtility.SetDirty(navData);

			Debug.Log("Saved  : " + saveFilePath);
		}


	}

}