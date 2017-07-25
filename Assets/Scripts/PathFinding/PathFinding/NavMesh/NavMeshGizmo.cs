
using UnityEngine;
using PathFinding.Graph3d;


namespace PathFinding
{
	public class NavMeshGizmo : MonoBehaviour
	{
		public BuildConfig cfg;

		public NavMeshData navData;

		private Color green = new Color(0.2f, 0.5f, 0.2f);
		private Color red = new Color(0.5f, 0.2f, 0.2f);


		void OnDrawGizmosSelected()
		{
			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Color defaultColor = Gizmos.color;

			// begin draw
			

			// end draw

			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
		}

	}

}