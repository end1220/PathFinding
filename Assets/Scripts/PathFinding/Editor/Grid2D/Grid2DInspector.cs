
using UnityEngine;
using UnityEditor;



namespace PathFinding
{

	public class Grid2DInspector : EditorInspector
	{
		Grid2DBuilder builder = new Grid2DBuilder();

		public float GridSize = 0.5f;
		public float RoleRadius = 0.5f;
		public int MaxSlope = 45;

		private string saveFilePath = "Assets/{0}_navgrid.asset";


		public Grid2DInspector(PathFindingMachine machine):
			base(machine)
		{
		string scenePath = EditorUtils.GetCurrentScenePath();
			saveFilePath = scenePath.Substring(0, scenePath.IndexOf(".unity")) + "_navgrid.asset";

			// load previous settings
			var existingAsset = AssetDatabase.LoadAssetAtPath<NavGrid2DData>(saveFilePath);
			if (existingAsset != null)
			{
				GridSize = existingAsset.GridSize * 0.001f;
				RoleRadius = existingAsset.AgentRadius * 0.001f;
				MaxSlope = existingAsset.MaxSlope;
			}

			if (builder.worldBoxObj == null)
				builder.worldBoxObj = GameObject.Find("worldbox");
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
				if (builder.worldBoxObj == null)
				{
					Debug.LogError("Build failed : There is no world box object in the scene ???");
					return;
				}
				var render = builder.worldBoxObj.GetComponent<MeshRenderer>();
				if (render == null)
				{
					Debug.LogError("Build failed : world box has no MeshRenderer");
					return;
				}

				builder.gridSize = GridSize;
				builder.agentRadius = RoleRadius;
				var worldSize = render.bounds.size;
				builder.width = (int)(worldSize.x / builder.gridSize);
				builder.height = (int)(worldSize.z / builder.gridSize);
				var worldMin = render.bounds.min;
				builder.minX = worldMin.x;
				builder.minZ = worldMin.z;
				builder.slope = MaxSlope;
				builder.tan_slope = Mathf.Tan(MaxSlope / 180.0f * Mathf.PI);

				if (!CheckParamsValid())
					return;

				builder.Build();

				machine.navgationData = builder.navData;
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
				EditorUtility.ClearProgressBar();
			}
		}


		private bool CheckParamsValid()
		{
			if (builder.gridSize < 0.5f || builder.gridSize > 1.0f)
			{
				Debug.LogError("Build failed : gird size should be [0.5, 1.0]");
				return false;
			}
			if (builder.agentRadius < 0.1f || builder.agentRadius > 1.0f)
			{
				Debug.LogError("Build failed : agent radius should be [0.1, 1.0]");
				return false;
			}
			if (MaxSlope < 0 || MaxSlope > 60)
			{
				Debug.LogError("Build failed : slope should be [0, 60].");
				return false;
			}
			return true;
		}


		public override void Save()
		{
			if (builder.navData == null)
			{
				Debug.LogError("You should click generate first...");
				return;
			}

			var existingAsset = AssetDatabase.LoadAssetAtPath<NavGrid2DData>(saveFilePath);
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