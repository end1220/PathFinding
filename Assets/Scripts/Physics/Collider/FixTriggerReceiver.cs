
using UnityEngine;


namespace FixedPoint
{

	[RequireComponent(typeof(FixTransform), typeof(FixCollider))]
	public abstract class FixTriggerReceiver : MonoBehaviour
	{
		//private FixCollider fixCollider;


		void Start()
		{
			//fixCollider = GetComponent<FixCollider>();
			OnStart();
		}


		public virtual void OnStart()
		{

		}


		public virtual void OnFixTriggerEnter(FixCollider trigger)
		{
			
		}


		public virtual void OnFixTriggerExit(FixCollider trigger)
		{
			
		}

	}


}