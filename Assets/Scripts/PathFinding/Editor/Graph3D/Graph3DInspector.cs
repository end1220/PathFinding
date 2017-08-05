
using UnityEngine;
using UnityEditor;



namespace PathFinding
{

	public class Graph3DInspector : EditorInspector
	{
		Graph3DBuilder builder = new Graph3DBuilder();

		GameObject worldBoxObj;
		float tan_slope;

		public float GridSize = 1;
		public float RoleHeight = 2;
		public float RoleRadius = 0.3f;
		public int MaxSlope = 45;

		string saveFilePath = "Assets/{0}_navgraph.asset";


		public Graph3DInspector(PathFindingMachine machine):
			base(machine)
		{
			string scenePath = EditorUtils.GetCurrentScenePath();
			saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_navgraph.asset";

			// load previous settings
			var existingAsset = AssetDatabase.LoadAssetAtPath<NavGrid2DData>(saveFilePath);
			if (existingAsset != null)
			{
				GridSize = existingAsset.GridSize * 0.001f;
				RoleRadius = existingAsset.AgentRadius * 0.001f;
				MaxSlope = existingAsset.MaxSlope;
			}

			if (worldBoxObj == null)
				worldBoxObj = GameObject.Find("worldbox");
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
				if (worldBoxObj == null)
				{
					Debug.LogError("There is no world box object in the scene ? ? ?");
					return;
				}
				var render = worldBoxObj.GetComponent<MeshRenderer>();
				if (render == null)
				{
					Debug.LogError("World box has no MeshRenderer");
					return;
				}

				if (MaxSlope < 20 || MaxSlope > 80)
					Debug.LogError("Bad slope! XD");
				tan_slope = Mathf.Tan(MaxSlope / 180.0f * Mathf.PI);

				// build
				builder.Stetup(worldBoxObj, GridSize, RoleHeight, RoleRadius, tan_slope);
				builder.Build();

				machine.navGraph = builder.navData;
				machine.navGraph.finalCells = builder.finalCells;
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
				EditorUtility.ClearProgressBar();
			}
		}


		public override void Save()
		{
			if (builder.navData == null)
			{
				Debug.LogError("You should click generate first...");
				return;
			}

			builder.navData.SaveBytes();
			var existingAsset = AssetDatabase.LoadAssetAtPath<NavGraph3DData>(saveFilePath);
			if (existingAsset == null)
			{
				AssetDatabase.CreateAsset(builder.navData, saveFilePath);
				AssetDatabase.Refresh();
				existingAsset = builder.navData;
			}
			else
			{
				EditorUtility.CopySerialized(builder.navData, existingAsset);
			}

			machine.navGraph = builder.navData;

			EditorUtility.SetDirty(builder.navData);

			Debug.Log("File saved  : " + saveFilePath);
		}

	}

}