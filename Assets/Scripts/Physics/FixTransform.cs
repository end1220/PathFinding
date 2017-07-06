
using System;
using UnityEngine;


namespace Lite
{
	/*
	 * 只使用整数运算的Transform组件
	 * 单位mm
	 * 
	 */
	public class FixTransform : MonoBehaviour
	{
		public FixVector3 position = FixVector3.zero;
		public FixVector3 scale = new FixVector3(FixMath.m2mm(1), FixMath.m2mm(1), FixMath.m2mm(1));
		public ushort angle = 0;

		private FixVector3 old_position = FixVector3.zero;
		private FixVector3 old_scale = new FixVector3(FixMath.m2mm(1), FixMath.m2mm(1), FixMath.m2mm(1));
		private ushort old_angle = 0;

		private Transform transform_u3d = null;

		private const int MaxLerpRotationTimes = 10;
		private int lerpRotationTimes = MaxLerpRotationTimes;
		private Quaternion startRotation;
		private Quaternion targetRotation;

		[SerializeField]
		private bool aglin = false;


		void Awake()
		{
			transform_u3d = GetComponent<Transform>();

			if (!aglin)
			{
				ApplyPosition();
				ApplyRotation();
			}
			old_position = position;
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
				lerpRotationTimes = 0;
				old_angle = angle;
				startRotation = transform_u3d.rotation;
				targetRotation = Quaternion.Euler(0, FixMath.ToDegree(angle), 0);
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


		public Vector3 GetU3DPosition()
		{
			return new Vector3(FixMath.mm2m(position.x),
				FixMath.mm2m(position.y),
				FixMath.mm2m(position.z)
				);
		}


		public Vector3 GetU3DForwards()
		{
			FixVector3 perform = FixMath.DecomposeAngle(1000, angle);
			return new Vector3(FixMath.mm2m(perform.x),
				FixMath.mm2m(perform.y),
				FixMath.mm2m(perform.z)
				);

		}

		public Vector3 GetU3DScale()
		{
			return new Vector3(FixMath.mm2m(scale.x),
				FixMath.mm2m(scale.y),
				FixMath.mm2m(scale.z)
				);
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
			var tran = transform;
			angle = FixMath.ToAngle(new FixVector3(tran.forward));
		}

	}



}
