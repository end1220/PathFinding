
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FixedPoint
{

	[RequireComponent(typeof(FixTransform))]
	public abstract class FixCollider : MonoBehaviour, ICollider
	{
		[System.NonSerialized]
		public string guid;

		[SerializeField]
		private bool isTrigger = false;
		public bool IsTrigger
		{
			get
			{
				return isTrigger;
			}

			set
			{
				isTrigger = value;
			}
		}

		[SerializeField]
		private bool isStatic = false;
		public bool IsStatic
		{
			get
			{
				return isStatic;
			}

			set
			{
				isStatic = value;
				FixColliderManager.Instance.ReregisterCollider(this);
			}
		}

		private Dictionary<string, FixCollider> overlappedColliders = new Dictionary<string, FixCollider>();

		protected FixTransform fixTran;

		private FixTriggerReceiver triggerReceiver;


		public void SetColliderInfo(bool isTrigger, bool isStatic)
		{
			this.IsTrigger = isTrigger;
			this.IsStatic = isStatic;
		}


		void Awake()
		{
			guid = System.Guid.NewGuid().ToString();
		}


		void Start()
		{
			fixTran = GetComponent<FixTransform>();
			triggerReceiver = GetComponent<FixTriggerReceiver>();
			OnStart();
		}

		void OnEnable()
		{
			FixColliderManager.Instance.RegisterCollider(this);
		}

		void OnDisable()
		{
			FixColliderManager.Instance.UnregisterCollider(this);
		}

		void OnDestroy()
		{
			FixColliderManager.Instance.UnregisterCollider(this);
		}

		public bool CheckPoint(FixVector3 position)
		{
			return IsPointIn(position);
		}


		public bool CheckOverlap(ICollider collider)
		{
			return IsOverlapped(collider);
		}


		public int GetVertexCount()
		{
			return VertexCount();
		}


		public FixVector3 GetVertex(int index)
		{
			return Vertex(index);
		}


		protected virtual void OnStart()
		{
			RecalculateVectexes();
		}


		protected abstract bool IsPointIn(FixVector3 position);

		protected virtual bool IsOverlapped(ICollider collider)
		{
			int vertexCount = collider.GetVertexCount();
			for (int i = 0; i < vertexCount; ++i)
			{
				var vert = collider.GetVertex(i);
				if (IsPointIn(vert))
					return true;
			}
			return false;
		}

		protected abstract int VertexCount();

		protected abstract FixVector3 Vertex(int index);

		protected virtual void RecalculateVectexes()
		{

		}

		public bool AlreadyOverlapped(FixCollider collider)
		{
			return overlappedColliders.ContainsKey(collider.guid);
		}


		public void AddNewOverlapped(FixCollider collider)
		{
			overlappedColliders.Add(collider.guid, collider);
		}


		public void RemoveOverlapped(FixCollider collider)
		{
			overlappedColliders.Remove(collider.guid);
		}


		public void OnFixTriggerEnter(ICollider trigger)
		{
			if (triggerReceiver != null)
			{
				triggerReceiver.OnFixTriggerEnter(trigger as FixCollider);
			}
			else
			{
				Log.Error("FixCollider.OnFixTriggerEnter: You are probably missing a FixTriggerReceiver component.");
			}
		}


		public void OnFixTriggerExit(ICollider trigger)
		{
			if (triggerReceiver != null)
			{
				triggerReceiver.OnFixTriggerExit(trigger as FixCollider);
			}
			else
			{
				Log.Error("FixCollider.OnFixTriggerExit: You are probably missing a FixTriggerReceiver component.");
			}
		}

	}


}