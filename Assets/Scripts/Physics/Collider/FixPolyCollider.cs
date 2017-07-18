
using UnityEngine;
using System.Collections.Generic;


namespace FixedPoint
{
	
	/*
	 * 只使用整数运算的PolyCollider组件
	 * 单位mm
	 * 目前只支持绕y轴旋转
	 * 
	 */
	public class FixPolyCollider : FixCollider
	{
		public FixVector3 size = new FixVector3(FixMath.m2mm(1), FixMath.m2mm(1), FixMath.m2mm(1));

		private List<FixVector3> localVertexes = new List<FixVector3>();

		private readonly int[] xOffset = { -1, 1, 1, -1, -1, 1, 1, -1 };
		private readonly int[] yOffset = { 1, 1, 1, 1, -1, -1, -1, -1 };
		private readonly int[] zOffset = { 1, 1, -1, -1, 1, 1, -1, -1 };

		protected override bool IsPointIn(FixVector3 position)
		{
			FixTransform twTran = GetComponent<FixTransform>();
			long minX = twTran.Position.x - size.x / 2;
			long maxX = twTran.Position.x + size.x / 2;
			long minY = twTran.Position.y - size.y / 2;
			long maxY = twTran.Position.y + size.y / 2;
			long minZ = twTran.Position.z - size.z / 2;
			long maxZ = twTran.Position.z + size.z / 2;

			bool ret = position.x > minX && position.x < maxX
				&& position.y > minY && position.y < maxY
				&& position.z > minZ && position.z < maxZ;

			return ret;
		}


		protected override int VertexCount()
		{
			return localVertexes.Count;
		}


		protected override FixVector3 Vertex(int index)
		{
			return localVertexes[index];
		}


		void OnDrawGizmosSelected()
		{
			Matrix4x4 oldMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Color oldColor = Gizmos.color;
			Gizmos.color = Color.cyan;
			// begin draw

			Gizmos.DrawWireCube(Vector3.zero, size.ToVector3());

			// end draw
			Gizmos.color = oldColor;
			Gizmos.matrix = oldMatrix;
		}


	}


}