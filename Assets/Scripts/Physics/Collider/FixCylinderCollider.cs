
using UnityEngine;


namespace FixedPoint
{
	/*
	 * 只使用整数运算的圆柱Collider组件
	 * 单位mm
	 * 目前只支持绕y轴旋转
	 */
	public class FixCylinderCollider : FixCollider
	{
		public int height = FixMath.m2mm(1);
		public int radius = FixMath.m2mm(1);

		private FixVector3[] localVertexes = new FixVector3[1];


		protected override bool IsPointIn(FixVector3 position)
		{
			long distSqr = FixMath.DistanceSqr2D(fixTran.Position, position);

			int minY = fixTran.Position.y - height / 2;
			int maxY = fixTran.Position.y + height / 2;

			bool ret = distSqr < radius * radius
				&& position.y > minY && position.y < maxY;

			return ret;
		}


		protected override int VertexCount()
		{
			return localVertexes.Length;
		}


		protected override FixVector3 Vertex(int index)
		{
			return fixTran.Position + localVertexes[index];
		}


		protected override void RecalculateVectexes()
		{	
			if (fixTran == null)
				fixTran = GetComponent<FixTransform>();

			float radiusFloat = FixMath.mm2m(radius);

			// re gen points
			int pointCount = 10 + (int)(radiusFloat * 3);
			if (localVertexes.Length != pointCount)
				localVertexes = new FixVector3[pointCount];
			float angle = 360f / pointCount;
			Quaternion r = transform.rotation;
			for (int i = 0; i < pointCount; i++)
			{
				Quaternion q = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y - (angle * i), r.eulerAngles.z);
				Vector3 v = (q * Vector3.forward) * radiusFloat;
				v.Set(v.x, 0.0f, v.z);
				localVertexes[i] = new FixVector3(v);
			}
		}


		void OnDrawGizmosSelected()
		{
			RecalculateVectexes();

			// begin draw
			Matrix4x4 defaultMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Color defaultColor = Gizmos.color;
			Gizmos.color = Color.cyan;

			Vector3 centerPos = Vector3.zero;
			float halfHeight = FixMath.mm2m(height)/2;
			Vector3 centerTop = centerPos + new Vector3(0, halfHeight, 0);
			Vector3 centerBtm = centerPos + new Vector3(0, -halfHeight, 0);
			Gizmos.DrawWireCube(centerTop, new Vector3(0.05f, 0.01f, 0.05f));
			Gizmos.DrawWireCube(centerBtm, new Vector3(0.05f, 0.01f, 0.05f));

			for (int i = 0; i < localVertexes.Length-1; i++)
			{
				Gizmos.DrawLine(centerTop + localVertexes[i].ToVector3(), centerTop + localVertexes[i + 1].ToVector3());
				Gizmos.DrawLine(centerBtm + localVertexes[i].ToVector3(), centerBtm + localVertexes[i + 1].ToVector3());
				Gizmos.DrawLine(centerTop + localVertexes[i].ToVector3(), centerBtm + localVertexes[i].ToVector3());
			}

			Gizmos.DrawLine(centerTop + localVertexes[localVertexes.Length - 1].ToVector3(), centerTop + localVertexes[0].ToVector3());
			Gizmos.DrawLine(centerBtm + localVertexes[localVertexes.Length - 1].ToVector3(), centerBtm + localVertexes[0].ToVector3());
			Gizmos.DrawLine(centerTop + localVertexes[localVertexes.Length - 1].ToVector3(), centerBtm + localVertexes[localVertexes.Length - 1].ToVector3());

			Gizmos.color = defaultColor;
			Gizmos.matrix = defaultMatrix;
			// end draw

		}

		

	}


}