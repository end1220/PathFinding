
using AStar;
using System.Collections.Generic;
using UnityEngine;


namespace PathFinding
{

	public static class NavMeshPathOptimizer
	{
		static List<Int3> funnelPath = new List<Int3>();
		static List<Int3> left = new List<Int3>();
		static List<Int3> right = new List<Int3>();


		public static void Optimize(ref List<AStarNode> path, ref List<Int3> vectorPath)
		{
			
			if (path == null || path.Count == 0 || vectorPath == null || vectorPath.Count != path.Count)
			{
				return;
			}

			funnelPath.Clear();
			left.Clear();
			right.Clear();

			// Add start point
			left.Add(vectorPath[0]);
			right.Add(vectorPath[0]);

			for (int i = 0; i < path.Count - 1; i++)
			{
				NavMeshNode node0 = path[i] as NavMeshNode;
				NavMeshNode node1 = path[i + 1] as NavMeshNode;
				// Get the portal between path[i] and path[i+1] and add it to the left and right lists
				bool portalWasAdded = node0.GetPortal(node1, left, right);

				if (!portalWasAdded)
				{
					// Fallback, just use the positions of the nodes
					left.Add(node0.position);
					right.Add(node0.position);

					left.Add(node1.position);
					right.Add(node1.position);
				}
			}

			// Add end point
			left.Add(vectorPath[vectorPath.Count - 1]);
			right.Add(vectorPath[vectorPath.Count - 1]);

			if (!RunFunnel(left, right, funnelPath))
			{
				// If funnel algorithm failed, degrade to simple line
				funnelPath.Add(vectorPath[0]);
				funnelPath.Add(vectorPath[vectorPath.Count - 1]);
			}

			vectorPath.Clear();
			vectorPath.AddRange(funnelPath);

			left.Clear();
			right.Clear();
		}

		
		public static bool RunFunnel(List<Int3> left, List<Int3> right, List<Int3> funnelPath)
		{
			if (left.Count != right.Count) 
				throw new System.ArgumentException("left and right lists must have equal length");

			if (left.Count < 3)
				return false;

			//Remove identical vertices
			while (left[1] == left[2] && right[1] == right[2])
			{
				left.RemoveAt(1);
				right.RemoveAt(1);

				if (left.Count <= 3)
				{
					return false;
				}
			}

			Int3 swPoint = left[2];
			if (swPoint == left[1])
			{
				swPoint = right[2];
			}

			//Test
			while (VectorMath.IsColinearXZ(left[0], left[1], right[1]) || VectorMath.RightOrColinearXZ(left[1], right[1], swPoint) == VectorMath.RightOrColinearXZ(left[1], right[1], left[0]))
			{
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
				List<Int3> tmp = left;
				left = right;
				right = tmp;
			}

			funnelPath.Add(left[0]);

			Int3 portalApex = left[0];
			Int3 portalLeft = left[1];
			Int3 portalRight = right[1];

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

				Int3 pLeft = left[i];
				Int3 pRight = right[i];

				long rightArea = VectorMath.SignedTriangleAreaTimes2XZ(portalApex, portalRight, pRight);
				if (rightArea >= 0)
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

				long leftArea = VectorMath.SignedTriangleAreaTimes2XZ(portalApex, portalLeft, pLeft);
				if (leftArea <= 0)
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
