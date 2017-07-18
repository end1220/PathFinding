

namespace FixedPoint
{

	public interface ITriggerReceiver
	{
		void OnEnter(ICollider collider);

		void OnExit(ICollider collider);
	}

}