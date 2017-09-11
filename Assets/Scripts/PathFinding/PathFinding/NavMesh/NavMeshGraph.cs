
using System.Collections.Generic;
using UnityEngine;
using AStar;


namespace PathFinding
{
	public class NavMeshGraph : AStarMap, INavGraph
	{
		NavMeshData navData;

		public BBTree bbTree = new BBTree();

		const long maxNearestNodeDistance = 100 * 1000;
		const long maxNearestNodeDistanceSqr = maxNearestNodeDistance * maxNearestNodeDistance;

		public List<NavMeshNode> trace = new List<NavMeshNode>();



		public void Init(INavData data)
		{
			navData = data as NavMeshData;

			for (int i = 0; i < navData.nodes.Length; i++)
			{
				NavMeshNode node = navData.nodes[i] as NavMeshNode;
				AddNode(node);
			}

			bbTree.RebuildFrom(navData.nodes);
		}


		public override int GetNeighbourNodeCount(AStarNode node)
		{
			return (node as NavMeshNode).connections.Length;
		}


		public override AStarNode GetNeighbourNode(AStarNode node, int index)
		{
			int id = (node as NavMeshNode).connections[index];
			return GetNode(id) as AStarNode;
		}


		public bool IsWalkable(Int3 position)
		{
			var node = bbTree.QueryInside(position, null);
			return node != null;
		}


		public Int3 GetNearestPosition(Int3 position)
		{
			if (IsWalkable(position))
				return position;
			NNInfo info = GetNearest(position, NNConstraint.None);
			Int3 clamp = info.clampedPosition;
			Int3 dir = info.node.position - clamp;
			Int3 mod = clamp;
			int d = 10;
			while (mod == clamp && d > 0)
			{
				mod = clamp + dir / d;
				d--;
			}
			return mod;
		}


		static Int3 ClosestPointOnNode(NavMeshNode node, Int3 pos)
		{
			return Polygon.ClosestPointOnTriangle(node.v0, node.v1, node.v2, pos);
		}


		public NNInfo GetNearest(Int3 position, NNConstraint constraint, bool accurateNearestNode = false)
		{
			if (constraint == null)
				constraint = NNConstraint.None;

			//Searches in radiuses of 0.05 - 0.2 - 0.45 ... 1.28 times the average of the width and depth of the bbTree
			int w = (bbTree.Size.Width + bbTree.Size.Height) / 100;

			NNInfo query = bbTree.QueryCircle(position, w, constraint);

			if (query.node == null)
			{
				for (int i = 1; i <= 8; i++)
				{
					query = bbTree.QueryCircle(position, i * i * w, constraint);
					if (query.node != null || (i - 1) * (i - 1) * w > maxNearestNodeDistance * 2)
					{ // *2 for a margin
						break;
					}
				}
			}

			if (query.node != null)
			{
				query.clampedPosition = ClosestPointOnNode(query.node, position);
			}

			if (query.constrainedNode != null)
			{
				if (constraint.constrainDistance && ((Vector3)query.constrainedNode.position - position.vec3).sqrMagnitude > maxNearestNodeDistanceSqr)
				{
					query.constrainedNode = null;
				}
				else
				{
					query.constClampedPosition = ClosestPointOnNode(query.constrainedNode, position);
				}
			}

			return query;
		}


		public bool LineCastForMoving(ref HitInfo hit, MoveType mov)
		{
			Int3 from = hit.from;
			Int3 to = hit.to;
			hit.hitPosition = from;

			if (trace != null)
				trace.Clear();

			Int3 end = to;
			Int3 origin = from;

			if (origin == end)
			{
				hit.hitPosition = from;
				return false;
			}

			NavMeshNode node = GetNearest(from, NNConstraint.None).node;
			if (node == null)
			{
				Debug.LogError("Could not find a valid node to start from");
				hit.hitPosition = from;
				return true;
			}

			origin = node.ClosestPointOnNode(origin);

			List<Int3> left = Util.ListPool<Int3>.Claim();
			List<Int3> right = Util.ListPool<Int3>.Claim();

			int counter = 0;
			while (true)
			{
				counter++;
				if (counter > 2000)
				{
					Debug.LogError("Linecast was stuck in infinite loop. Breaking.");
					Util.ListPool<Int3>.Release(left);
					Util.ListPool<Int3>.Release(right);
					return true;
				}

				NavMeshNode newNode = null;

				if (trace != null) trace.Add(node);

				if (node.ContainsPoint(end))
				{
					hit.hitPosition = to;
					Util.ListPool<Int3>.Release(left);
					Util.ListPool<Int3>.Release(right);
					return false;
				}

				for (int i = 0; i < node.connections.Length; i++)
				{
					left.Clear();
					right.Clear();

					NavMeshNode other = GetNode(node.connections[i]) as NavMeshNode;
					if (!node.GetPortal(other, left, right)) continue;

					Int3 a = left[0];
					Int3 b = right[0];

					//i.e Left or colinear
					if (!VectorMath.RightXZ(a, b, origin))
					{
						if (VectorMath.RightXZ(a, b, end))
						{
							//Since polygons are laid out in clockwise order, the ray would intersect (if intersecting) this edge going in to the node, not going out from it
							continue;
						}
					}

					float factor1, factor2;

					if (VectorMath.LineIntersectionFactorXZ(a, b, origin, end, out factor1, out factor2))
					{
						//Intersection behind the start
						if (factor2 < 0) continue;

						if (factor1 >= 0 && factor1 <= 1)
						{
							newNode = other;
							break;
						}
					}
				}

				if (newNode == null)
				{
					//Possible edge hit
					int vs = node.GetVertexCount();

					for (int i = 0; i < vs; i++)
					{
						var a = node.GetVertex(i);
						var b = node.GetVertex((i + 1) % vs);


						//i.e left or colinear
						if (!VectorMath.RightXZ(a, b, origin))
						{
							//Since polygons are laid out in clockwise order, the ray would intersect (if intersecting) this edge going in to the node, not going out from it
							if (VectorMath.RightXZ(a, b, end))
							{
								//Since polygons are laid out in clockwise order, the ray would intersect (if intersecting) this edge going in to the node, not going out from it
								continue;
							}
						}

						float factor1, factor2;
						if (VectorMath.LineIntersectionFactorXZ(a, b, origin, end, out factor1, out factor2))
						{
							if (factor2 < 0) continue;

							if (factor1 >= 0 && factor1 <= 1)
							{
								Vector3 intersectionPoint = (Vector3)a + (Vector3)(b - a) * factor1;
								hit.hitPosition = new Int3(intersectionPoint);

								Util.ListPool<Int3>.Release(left);
								Util.ListPool<Int3>.Release(right);

								return true;
							}
						}
					}

					//Ok, this is wrong...
					Debug.LogWarning("Linecast failing because point not inside node, and line does not hit any edges of it");

					Util.ListPool<Int3>.Release(left);
					Util.ListPool<Int3>.Release(right);

					return false;
				}

				node = newNode;
			}

		}


		public Int3 SlideByObstacles(Int3 from, Int3 to, Int3 hit)
		{
			Int groundY;
			bool collidered;
			Int3 delta = NavMeshUtility.Move(from, to - from, out groundY, out collidered);

			return from + delta;
		}


	}
}

