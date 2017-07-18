
using UnityEngine;


namespace FixedPoint
{
	
	/*
	 * 只使用整数运算的BoxCollider组件
	 * 单位mm
	 * 目前只支持绕y轴旋转
	 * 
	 */
	public class FixBoxCollider : FixCollider
	{
		public FixVector3 size = new FixVector3(FixMath.m2mm(1), FixMath.m2mm(1), FixMath.m2mm(1));
		public FixVector3 center = FixVector3.zero;

		private FixVector3[] localVertexesOriginal = new FixVector3[8];
		private FixVector3[] localVertexesTranslated = new FixVector3[8];
		private FixVector3[] localVertexesTranslatedUpper = new FixVector3[4];// 上表面4顶点

		private readonly int[] xOffset = { -1, 1, 1, -1, -1, 1, 1, -1 };
		private readonly int[] yOffset = { 1, 1, 1, 1, -1, -1, -1, -1 };
		private readonly int[] zOffset = { 1, 1, -1, -1, 1, 1, -1, -1 };


		public void SetBoxInfo(FixVector3 size, FixVector3 center)
		{
			this.size = size;
			this.center = center;
		}


		protected override bool IsPointIn(FixVector3 position)
		{
			// height test
			if (position.y > fixTran.Position.y + localVertexesTranslated[0].y || position.y < fixTran.Position.y + localVertexesTranslated[4].y)
				return false;

			// poly test
			for (int i = 0; i < 4; ++i)
				localVertexesTranslatedUpper[i] = fixTran.Position + localVertexesTranslated[i];
			bool ret = FixPhysics.IsPointInPoly(position, localVertexesTranslatedUpper);
			return ret;
		}


		protected override int VertexCount()
		{
			return localVertexesTranslated.Length;
		}


		protected override FixVector3 Vertex(int index)
		{
			return fixTran.Position + localVertexesTranslated[index];
		}


		protected override void RecalculateVectexes()
		{
			if (fixTran == null)
				fixTran = GetComponent<FixTransform>();

			FixVector3 scale = fixTran.scale;
			Fix64 sx = (Fix64)scale.x / (Fix64)FixMath.ratio_mm;
			Fix64 sy = (Fix64)scale.y / (Fix64)FixMath.ratio_mm;
			Fix64 sz = (Fix64)scale.z / (Fix64)FixMath.ratio_mm;
			Fix64 hx = (Fix64)(size.x >> 1) * sx;
			Fix64 hy = (Fix64)(size.y >> 1) * sy;
			Fix64 hz = (Fix64)(size.z >> 1) * sz;
			for (int i = 0; i < 8; ++i)
			{
				Fix64 x = (Fix64)xOffset[i] * hx + (Fix64)center.x * sx;
				Fix64 y = (Fix64)yOffset[i] * hy + (Fix64)center.y * sy;
				Fix64 z = (Fix64)zOffset[i] * hz + (Fix64)center.z * sz;
				localVertexesOriginal[i] = new FixVector3((int)x, (int)y, (int)z);
			}
			
			for (int i = 0; i < 8; ++i)
			{
				int localY = localVertexesOriginal[i].y;
				FixVector3 vertex = localVertexesOriginal[i];
				vertex.y = 0;
				ushort angle0 = FixMath.VectorToAngle(vertex);
				int len = vertex.Length;
				angle0 = FixMath.DecAngle(angle0, FixMath.quarterAngle);
				var decomVec = FixMath.DecomposeAngle(len, FixMath.AddAngle(fixTran.Angle, angle0));
				localVertexesTranslated[i] = new FixVector3(0, localY, 0) + decomVec;
			}
		}


		void OnDrawGizmosSelected()
		{
			if (Application.isPlaying)
			{
				Matrix4x4 oldMatrix = Gizmos.matrix;
				Color oldColor = Gizmos.color;
				Gizmos.color = Color.cyan;
				// begin draw
				var basePos = fixTran.Position.ToVector3();
				Gizmos.DrawLine(basePos + localVertexesTranslated[0].ToVector3(), basePos + localVertexesTranslated[1].ToVector3());
				Gizmos.DrawLine(basePos + localVertexesTranslated[1].ToVector3(), basePos + localVertexesTranslated[2].ToVector3());
				Gizmos.DrawLine(basePos + localVertexesTranslated[2].ToVector3(), basePos + localVertexesTranslated[3].ToVector3());
				Gizmos.DrawLine(basePos + localVertexesTranslated[3].ToVector3(), basePos + localVertexesTranslated[0].ToVector3());

				Gizmos.DrawLine(basePos + localVertexesTranslated[4].ToVector3(), basePos + localVertexesTranslated[5].ToVector3());
				Gizmos.DrawLine(basePos + localVertexesTranslated[5].ToVector3(), basePos + localVertexesTranslated[6].ToVector3());
				Gizmos.DrawLine(basePos + localVertexesTranslated[6].ToVector3(), basePos + localVertexesTranslated[7].ToVector3());
				Gizmos.DrawLine(basePos + localVertexesTranslated[7].ToVector3(), basePos + localVertexesTranslated[4].ToVector3());

				Gizmos.DrawLine(basePos + localVertexesTranslated[0].ToVector3(), basePos + localVertexesTranslated[4].ToVector3());
				Gizmos.DrawLine(basePos + localVertexesTranslated[1].ToVector3(), basePos + localVertexesTranslated[5].ToVector3());
				Gizmos.DrawLine(basePos + localVertexesTranslated[2].ToVector3(), basePos + localVertexesTranslated[6].ToVector3());
				Gizmos.DrawLine(basePos + localVertexesTranslated[3].ToVector3(), basePos + localVertexesTranslated[7].ToVector3());

				// end draw
				Gizmos.color = oldColor;
				Gizmos.matrix = oldMatrix;
			}
			else
			{
				Matrix4x4 oldMatrix = Gizmos.matrix;
				Gizmos.matrix = transform.localToWorldMatrix;
				Color oldColor = Gizmos.color;
				Gizmos.color = Color.green;
				// begin draw

				Gizmos.DrawWireCube(Vector3.zero, size.ToVector3());

				// end draw
				Gizmos.color = oldColor;
				Gizmos.matrix = oldMatrix;
			}
			
		}


	}


}