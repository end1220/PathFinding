
using System;
using System.Collections.Generic;
using UnityEngine;


namespace FixedPoint
{
	/*
	 * 只使用整数运算的Transform组件
	 * 单位mm
	 * 
	 */
	public class FixTransform : MonoBehaviour
	{
		private Transform transform_u3d = null;

		private FixTransform parent;
		private List<FixTransform> children = new List<FixTransform>();

		[SerializeField]
		private FixVector3 position = FixVector3.zero;// world position
		[SerializeField]
		public FixVector3 scale = new FixVector3(FixMath.m2mm(1), FixMath.m2mm(1), FixMath.m2mm(1));// local scale
		[SerializeField]
		private ushort angle = 0;// local angle?

		private FixVector3 old_position = FixVector3.zero;
		private FixVector3 old_scale = new FixVector3(FixMath.m2mm(1), FixMath.m2mm(1), FixMath.m2mm(1));
		private ushort old_angle = 0;


		public FixVector3 Position
		{
			set
			{
				FixVector3 old = position;
				position = value;
				if (old != value)
					UpdateChildrenPosition(this, value - old);
			}
			get
			{
				return position;
			}
		}

		public ushort Angle
		{
			set
			{
				angle = value;
			}
			get
			{
				return angle;
			}
		}

		// lerp rotatation
		private bool lerpRotation = true;
		private const int MaxLerpRotationTimes = 10;
		private int lerpRotationTimes = MaxLerpRotationTimes;
		private Quaternion startRotation;
		private Quaternion targetRotation;

		[SerializeField]
		private bool aglin = false;


		void Awake()
		{
			transform_u3d = GetComponent<Transform>();
			old_position = position;
			if (aglin)
			{
				this.Position = new FixVector3(transform_u3d.position);
				this.Angle = FixMath.VectorToAngle(new FixVector3(transform_u3d.forward));
				this.scale = new FixVector3(transform_u3d.localScale);
			}
		}


		void Start()
		{
			FixTransform prt = transform_u3d.parent.GetComponent<FixTransform>();
			SetParent(prt);
		}


		void LateUpdate()
		{
			if (position != old_position)
			{
				old_position = position;
				ApplyPosition();
			}

			if (angle != old_angle)
			{
				if (lerpRotation)
				{
					lerpRotationTimes = 0;
					startRotation = transform_u3d.rotation;
					targetRotation = Quaternion.Euler(0, FixMath.AngleToDegree(angle), 0);
				}
				else
				{
					transform_u3d.rotation = Quaternion.Euler(0, FixMath.AngleToDegree(angle), 0);
				}
				old_angle = angle;
			}

			if (scale != old_scale)
			{
				old_scale = scale;
				ApplyScale();
			}

			// lerp
			LerpRotation();

		}


		public void ApplyPosition()
		{
			Vector3 pos = GetU3DPosition();
			transform_u3d.position = pos;
		}


		public void ApplyRotation()
		{
			Vector3 fwd = GetU3DForwards();
			transform_u3d.forward = fwd;
		}


		private void ApplyScale()
		{
			Vector3 s = GetU3DScale();
			transform_u3d.localScale = s;
		}


		private Vector3 GetU3DPosition()
		{
			return new Vector3(FixMath.mm2m(position.x),
				FixMath.mm2m(position.y),
				FixMath.mm2m(position.z)
				);
		}


		private Vector3 GetU3DForwards()
		{
			FixVector3 perform = FixMath.DecomposeAngle(1000, angle);
			return perform.ToVector3();
		}

		private Vector3 GetU3DScale()
		{
			return scale.ToVector3();
		}


		private void LerpRotation()
		{
			if (lerpRotationTimes < MaxLerpRotationTimes)
			{
				lerpRotationTimes++;
				transform_u3d.rotation = Quaternion.Lerp(startRotation, targetRotation, lerpRotationTimes / (float)MaxLerpRotationTimes);
			}
		}


		public void _aglin()
		{
			aglin = true;
		}


		public void SetParent(FixTransform tran)
		{
			if (parent != null)
				parent.DettachChild(this);
			if (tran != null)
				tran.AttachChild(this);
		}


		private void AttachChild(FixTransform child)
		{
			if (children.Contains(child))
			{
				Log.Error("FixTransfrom.AttachChild: repeated child.");
				return;
			}
			children.Add(child);
			child.parent = this;
		}


		private void DettachChild(FixTransform child)
		{
			if (!children.Contains(child))
			{
				Log.Error("FixTransfrom.DettachChild: not a child.");
				return;
			}
			children.Remove(child);
			child.parent = null;
		}


		private void _TryRefreshChildren(FixTransform trans)
		{
			int childrenCount = trans.children.Count;
			if (childrenCount != trans.transform.childCount)
			{
				for (int i = 0; i < trans.transform.childCount; ++i)
				{
					FixTransform fixChild = trans.transform.GetChild(i).GetComponent<FixTransform>();
					if (fixChild != null && !children.Contains(fixChild))
					{
						children.Add(fixChild);
						fixChild.parent = this;
					}
				}
			}
		}


		private void UpdateChildrenPosition(FixTransform trans, FixVector3 delta)
		{
			_TryRefreshChildren(trans);
			int count = trans.children.Count;
			for (int i = 0; i < count; ++i)
			{
				FixTransform child = trans.children[i];
				child.position = child.position + delta;
			}
		}


	}



}
