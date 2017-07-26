
using UnityEngine;
using PathFinding.Graph3d;


namespace PathFinding
{
	public class NavMeshGizmo : MonoBehaviour
	{

		public NavMeshNode[] nodes;

		private Color green = new Color(0.2f, 0.5f, 0.2f);
		private Color red = new Color(0.5f, 0.2f, 0.2f);


		void OnDrawGizmosSelected()
		{
			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Color defaultColor = Gizmos.color;

			// begin draw

			for (int i = 0; i < nodes.Length; i++)
			{
				NavMeshNode node = nodes[i] as NavMeshNode;

				float a1 = VectorMath.SignedTriangleAreaTimes2XZ((Vector3)node.v0, (Vector3)node.v1, (Vector3)node.v2);

				long a2 = VectorMath.SignedTriangleAreaTimes2XZ(node.v0, node.v1, node.v2);
				if (a1 * a2 < 0) Debug.LogError(a1 + " " + a2);


				if (VectorMath.IsClockwiseXZ(node.v0, node.v1, node.v2))
				{
					Debug.DrawLine((Vector3)node.v0, (Vector3)node.v1, Color.green);
					Debug.DrawLine((Vector3)node.v1, (Vector3)node.v2, Color.green);
					Debug.DrawLine((Vector3)node.v2, (Vector3)node.v0, Color.green);
				}
				else
				{
					Debug.DrawLine((Vector3)node.v0, (Vector3)node.v1, Color.red);
					Debug.DrawLine((Vector3)node.v1, (Vector3)node.v2, Color.red);
					Debug.DrawLine((Vector3)node.v2, (Vector3)node.v0, Color.red);
				}
			}


			// end draw

			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
		}

	}

}