
using UnityEngine;
using UnityEditor;



namespace PathFinding
{

	public class Grid3DInspector : EditorInspector
	{
		Grid3DBuilder builder = new Grid3DBuilder();

		GameObject worldBoxObj;
		float tan_slope;

		public float GridSize = 1;
		public float RoleHeight = 2;
		public float RoleRadius = 0.3f;
		public int MaxSlope = 45;

		string saveFilePath = "Assets/{0}_nav.asset";


		public Grid3DInspector(PathFindingMachine machine):
			base(machine)
		{
			string scenePath = GetCurrentScenePath();
			string sceneName = GetCurrentSceneName();
			saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "/" + sceneName + "_nav.asset";

			// load previous settings
			var existingAsset = AssetDatabase.LoadAssetAtPath<Grid3DNavData>(saveFilePath);
			if (existingAsset != null)
			{
				GridSize = existingAsset.buildConfig.cellSize * 0.001f;
				RoleRadius = existingAsset.buildConfig.agentRadius * 0.001f;
				MaxSlope = existingAsset.buildConfig.maxSlope;
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

				machine.navgationData = builder.navData;
				(machine.navgationData as Grid3DNavData).finalCells = builder.finalCells;
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
			var existingAsset = AssetDatabase.LoadAssetAtPath<Grid3DNavData>(saveFilePath);
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

			machine.navgationData = builder.navData;

			EditorUtility.SetDirty(builder.navData);

			Debug.Log("File saved  : " + saveFilePath);
		}

	}

}