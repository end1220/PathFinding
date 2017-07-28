
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PathFinding
{

	public static class PathOptimizerNavMesh
	{

		public static void Optimize(ref List<NavMeshNode> path, ref List<Int3> vectorPath)
		{
			Apply(ref path, ref vectorPath);
		}


		static void Apply(ref List<NavMeshNode> path, ref List<Int3> vectorPath)
		{
			if (path == null || path.Count == 0 || vectorPath == null || vectorPath.Count != path.Count)
			{
				return;
			}

			List<Vector3> funnelPath = ListPool<Vector3>.Claim();

			// Claim temporary lists and try to find lists with a high capacity
			List<Vector3> left = ListPool<Vector3>.Claim(path.Count + 1);
			List<Vector3> right = ListPool<Vector3>.Claim(path.Count + 1);

			//AstarProfiler.StartProfile("Construct Funnel");

			// Add start point
			left.Add(vectorPath[0].ToVector3());
			right.Add(vectorPath[0].ToVector3());

			for (int i = 0; i < path.Count - 1; i++)
			{
				// Get the portal between path[i] and path[i+1] and add it to the left and right lists
				bool portalWasAdded = path[i].GetPortal(path[i + 1], left, right);

				if (!portalWasAdded)
				{
					// Fallback, just use the positions of the nodes
					left.Add((Vector3)path[i].position);
					right.Add((Vector3)path[i].position);

					left.Add((Vector3)path[i + 1].position);
					right.Add((Vector3)path[i + 1].position);
				}
			}

			// Add end point
			left.Add(vectorPath[vectorPath.Count - 1].ToVector3());
			right.Add(vectorPath[vectorPath.Count - 1].ToVector3());

			if (!RunFunnel(left, right, funnelPath))
			{
				// If funnel algorithm failed, degrade to simple line
				funnelPath.Add(vectorPath[0].ToVector3());
				funnelPath.Add(vectorPath[vectorPath.Count - 1].ToVector3());
			}

			// Release lists back to the pool
			//ListPool<Vector3>.Release(vectorPath);
			vectorPath.Clear();
			foreach (var v in funnelPath)
				vectorPath.Add((Int3)v);

			ListPool<Vector3>.Release(left);
			ListPool<Vector3>.Release(right);
		}

		/** Calculate a funnel path from the \a left and \a right portal lists.
		 * The result will be appended to \a funnelPath
		 */
		public static bool RunFunnel(List<Vector3> left, List<Vector3> right, List<Vector3> funnelPath)
		{
			if (left == null) throw new System.ArgumentNullException("left");
			if (right == null) throw new System.ArgumentNullException("right");
			if (funnelPath == null) throw new System.ArgumentNullException("funnelPath");

			if (left.Count != right.Count) throw new System.ArgumentException("left and right lists must have equal length");

			if (left.Count < 3)
			{
				return false;
			}

			//Remove identical vertices
			while (left[1] == left[2] && right[1] == right[2])
			{
				//System.Console.WriteLine ("Removing identical left and right");
				left.RemoveAt(1);
				right.RemoveAt(1);

				if (left.Count <= 3)
				{
					return false;
				}
			}

			Vector3 swPoint = left[2];
			if (swPoint == left[1])
			{
				swPoint = right[2];
			}

			//Test
			while (VectorMath.IsColinearXZ(left[0], left[1], right[1]) || VectorMath.RightOrColinearXZ(left[1], right[1], swPoint) == VectorMath.RightOrColinearXZ(left[1], right[1], left[0]))
			{
#if ASTARDEBUG
				Debug.DrawLine(left[1], right[1], new Color(0, 0, 0, 0.5F));
				Debug.DrawLine(left[0], swPoint, new Color(0, 0, 0, 0.5F));
#endif
				left.RemoveAt(1);
				right.RemoveAt(1);

				if (left.Count <= 3)
				{
					return false;
				}

				swPoint = left[2];
				if (swPoint == left[1])
				{
					swPoint = right[2];
				}
			}

			//Switch left and right to really be on the "left" and "right" sides
			/** \todo The colinear check should not be needed */
			if (!VectorMath.IsClockwiseXZ(left[0], left[1], right[1]) && !VectorMath.IsColinearXZ(left[0], left[1], right[1]))
			{
				//System.Console.WriteLine ("Wrong Side 2");
				List<Vector3> tmp = left;
				left = right;
				right = tmp;
			}

#if ASTARDEBUG
			for (int i = 0; i < left.Count-1; i++) {
				Debug.DrawLine(left[i], left[i+1], Color.red);
				Debug.DrawLine(right[i], right[i+1], Color.magenta);
				Debug.DrawRay(right[i], Vector3.up, Color.magenta);
			}
			for (int i = 0; i < left.Count; i++) {
				//Debug.DrawLine (right[i],left[i], Color.cyan);
			}
#endif

			funnelPath.Add(left[0]);

			Vector3 portalApex = left[0];
			Vector3 portalLeft = left[1];
			Vector3 portalRight = right[1];

			int apexIndex = 0;
			int rightIndex = 1;
			int leftIndex = 1;

			for (int i = 2; i < left.Count; i++)
			{
				if (funnelPath.Count > 2000)
				{
					Debug.LogWarning("Avoiding infinite loop. Remove this check if you have this long paths.");
					break;
				}

				Vector3 pLeft = left[i];
				Vector3 pRight = right[i];

				/*Debug.DrawLine (portalApex,portalLeft,Color.red);
				 * Debug.DrawLine (portalApex,portalRight,Color.yellow);
				 * Debug.DrawLine (portalApex,left,Color.cyan);
				 * Debug.DrawLine (portalApex,right,Color.cyan);*/

				if (VectorMath.SignedTriangleAreaTimes2XZ(portalApex, portalRight, pRight) >= 0)
				{
					if (portalApex == portalRight || VectorMath.SignedTriangleAreaTimes2XZ(portalApex, portalLeft, pRight) <= 0)
					{
						portalRight = pRight;
						rightIndex = i;
					}
					else
					{
						funnelPath.Add(portalLeft);
						portalApex = portalLeft;
						apexIndex = leftIndex;

						portalLeft = portalApex;
						portalRight = portalApex;

						leftIndex = apexIndex;
						rightIndex = apexIndex;

						i = apexIndex;

						continue;
					}
				}

				if (VectorMath.SignedTriangleAreaTimes2XZ(portalApex, portalLeft, pLeft) <= 0)
				{
					if (portalApex == portalLeft || VectorMath.SignedTriangleAreaTimes2XZ(portalApex, portalRight, pLeft) >= 0)
					{
						portalLeft = pLeft;
						leftIndex = i;
					}
					else
					{
						funnelPath.Add(portalRight);
						portalApex = portalRight;
						apexIndex = rightIndex;

						portalLeft = portalApex;
						portalRight = portalApex;

						leftIndex = apexIndex;
						rightIndex = apexIndex;

						i = apexIndex;

						continue;
					}
				}
			}

			funnelPath.Add(left[left.Count - 1]);
			return true;
		}

	}

}
