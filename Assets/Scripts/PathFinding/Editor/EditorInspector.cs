
using System;
using System.Linq;
using System.Reflection;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;



namespace PathFinding
{

	public class EditorInspector
	{
		protected PathFindingMachine machine;


		public EditorInspector(PathFindingMachine machine)
		{
			this.machine = machine;
		}

		public virtual void Clear() { }

		public virtual void Bake() { }

		public virtual void Save() { }


		public virtual void DrawInspector()
		{
			DrawDefaultProperties();
		}


		protected void DrawDefaultProperties()
		{
			object target = this;
			Type nodeType = target.GetType();
			var fields = from fi in nodeType.GetFields(BindingFlags.Instance | BindingFlags.Public/* | BindingFlags.NonPublic*/)
						 select fi;
			var properties = from pi in nodeType.GetProperties(BindingFlags.Instance | BindingFlags.Public/* | BindingFlags.NonPublic*/)
							 select pi;

			foreach (var field in fields)
			{
				object value = null;
				string label = field.Name;
				if (TryToDrawField(label, field.GetValue(target), field.FieldType, out value))
				{
					field.SetValue(target, value);
				}
			}
			foreach (var property in properties)
			{
				object value = null;
				string label = property.Name;
				if (TryToDrawField(label, property.GetValue(target, null), property.PropertyType, out value))
				{
					property.SetValue(target, value, null);
				}
			}

		}


		protected static bool TryToDrawField(string label, object currentValue, Type type, out object value)
		{
			bool success = true;

			if (type == typeof(bool))
			{
				value = EditorGUILayout.Toggle(label, (bool)currentValue);
			}
			else if (type == typeof(int))
			{
				value = EditorGUILayout.IntField(label, (int)currentValue);
			}
			else if (type == typeof(float))
			{
				value = EditorGUILayout.FloatField(label, (float)currentValue);
			}
			else if (type == typeof(string))
			{
				value = EditorGUILayout.TextField(label, (string)currentValue);
			}
			else if (type == typeof(Vector2))
			{
				value = EditorGUILayout.Vector2Field(label, (Vector2)currentValue);
			}
			else if (type == typeof(Vector3))
			{
				value = EditorGUILayout.Vector3Field(label, (Vector3)currentValue);
			}
			else if (type == typeof(LayerMask))
			{
				LayerMask lm = (LayerMask)currentValue;
				lm.value = EditorGUILayout.MaskField(label, lm.value, UnityEditorInternal.InternalEditorUtility.layers);
				value = lm;
			}
			else if (type.IsEnum)
			{
				value = EditorGUILayout.EnumPopup(label, (Enum)currentValue);
			}
			else
			{
				value = null;
				success = false;
			}

			return success;
		}


		protected static string GetCurrentSceneName()
		{
			return SceneManager.GetActiveScene().name;
		}


		protected static string GetCurrentScenePath()
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

}