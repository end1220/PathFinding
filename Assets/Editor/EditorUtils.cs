
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;



public static class EditorUtils
{

	public static string GetCurrentScenePath()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		string scenePath = "";
		var paths = AssetDatabase.GetAllAssetPaths();
		foreach (var v in paths)
		{
			if (Path.GetFileName(v) == sceneName + ".unity")
			{
				scenePath = v;
				break;
			}
		}
		return scenePath;
	}

}