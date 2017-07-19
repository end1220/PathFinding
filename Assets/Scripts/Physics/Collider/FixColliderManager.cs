
using System.Collections.Generic;
using UnityEngine;


namespace FixedPoint
{

	public class FixColliderManager/* : MonoBehaviour, TwFramework.IManager*/
	{
		public static FixColliderManager Instance { get; private set; }

		private List<FixCollider> staticColliderList = new List<FixCollider>();

		private List<FixCollider> movableColliderList = new List<FixCollider>();


		public void Init()
		{
			Instance = this;
		}


		public void Destroy()
		{
			staticColliderList.Clear();
			movableColliderList.Clear();
		}


		public void RenderTick()
		{
			// do nothing
		}


		public void FrameTick()
		{
			// moveables and statics
			foreach (var movable in movableColliderList)
			{
				foreach (var statc in staticColliderList)
				{
					if (movable == statc)
						continue;
					bool overlapped = statc.CheckOverlap(movable);
					if (overlapped)
					{
						if (movable.AlreadyOverlapped(statc))
							continue;
						movable.AddNewOverlapped(statc);
						if (statc.IsTrigger)
							movable.OnFixTriggerEnter(statc);
						if (movable.IsTrigger)
							statc.OnFixTriggerEnter(movable);
					}
					else
					{
						if (movable.AlreadyOverlapped(statc))
						{
							movable.RemoveOverlapped(statc);
							if (statc.IsTrigger)
								movable.OnFixTriggerExit(statc);
							if (movable.IsTrigger)
								statc.OnFixTriggerExit(movable);
						}
					}
				}
			}

			// moveables and moveables
			foreach (var movable in movableColliderList)
			{
				foreach (var movable2 in movableColliderList)
				{
					if (movable == movable2)
						continue;
					bool overlapped = movable.CheckOverlap(movable2);
					if (overlapped)
					{
						if (movable.AlreadyOverlapped(movable2))
							continue;
						movable.AddNewOverlapped(movable2);
						if (movable2.IsTrigger)
							movable.OnFixTriggerEnter(movable2);
					}
					else
					{
						if (movable.AlreadyOverlapped(movable2))
						{
							movable.RemoveOverlapped(movable2);
							if (movable2.IsTrigger)
								movable.OnFixTriggerExit(movable2);
						}
					}
				}
			}

		}


		public void RegisterCollider(FixCollider collider)
		{
			if (collider.IsStatic)
				staticColliderList.Add(collider);
			else
				movableColliderList.Add(collider);
		}

		public void UnregisterCollider(FixCollider collider)
		{
			if (collider.IsStatic)
				staticColliderList.Remove(collider);
			else
				movableColliderList.Remove(collider);
		}


		public void ReregisterCollider(FixCollider collider)
		{
			if (staticColliderList.Contains(collider))
				staticColliderList.Remove(collider);
			else if (movableColliderList.Contains(collider))
				movableColliderList.Remove(collider);

			RegisterCollider(collider);
		}

	}


}