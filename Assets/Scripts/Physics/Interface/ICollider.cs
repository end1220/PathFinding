

namespace FixedPoint
{

	public interface ICollider
	{
		/// <summary>
		/// return true if the point is in collider.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		bool CheckPoint(Int3 point);

		/// <summary>
		/// return true if overlapped with other colliders
		/// </summary>
		/// <param name="collider"></param>
		/// <returns></returns>
		bool CheckOverlap(ICollider collider);

		int GetVertexCount();

		Int3 GetVertex(int index);

	}

}