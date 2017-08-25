

using PathFinding;
using PathFinding.RVO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class PathFindingUtility
{
	private static Int3[] _staticVerts = new Int3[3];
	private static string acotrName;
	private static List<NavMeshNode> checkedNodes = new List<NavMeshNode>();
	public static int MaxDepth = 4;
	public static bool MoveAxisY = true;
	public static int ValidateTargetMaxDepth = 10;
	public static float ValidateTargetRadiusScale = 1.5f;

	private static float CalculateY(Vector3 pf, NavMeshNode node)
	{
		Vector3 vector;
		Vector3 vector2;
		Vector3 vector3;
		node.GetPoints(out vector, out vector2, out vector3);
		float num = ((vector2.z - vector3.z) * (vector.x - vector3.x)) + ((vector3.x - vector2.x) * (vector.z - vector3.z));
		float num2 = 1f / num;
		float num3 = ((vector2.z - vector3.z) * (pf.x - vector3.x)) + ((vector3.x - vector2.x) * (pf.z - vector3.z));
		num3 *= num2;
		float num4 = ((vector3.z - vector.z) * (pf.x - vector3.x)) + ((vector.x - vector3.x) * (pf.z - vector3.z));
		num4 *= num2;
		float num5 = (1f - num3) - num4;
		return (((num3 * vector.y) + (num4 * vector2.y)) + (num5 * vector3.y));
	}

	private static void CalculateY(ref Int3 point, NavMeshNode node)
	{
		float num = CalculateY((Vector3)point, node);
		point.y = Mathf.RoundToInt(num * 1000f);
	}

	private static float CalculateY_Clamped(Vector3 pf, NavMeshNode node)
	{
		Vector3 vector;
		Vector3 vector2;
		Vector3 vector3;
		node.GetPoints(out vector, out vector2, out vector3);
		float num = ((vector2.z - vector3.z) * (vector.x - vector3.x)) + ((vector3.x - vector2.x) * (vector.z - vector3.z));
		float num2 = 1f / num;
		float num3 = ((vector2.z - vector3.z) * (pf.x - vector3.x)) + ((vector3.x - vector2.x) * (pf.z - vector3.z));
		num3 *= num2;
		num3 = Mathf.Clamp01(num3);
		float num4 = ((vector3.z - vector.z) * (pf.x - vector3.x)) + ((vector.x - vector3.x) * (pf.z - vector3.z));
		num4 *= num2;
		num4 = Mathf.Clamp01(num4);
		float num5 = Mathf.Clamp01((1f - num3) - num4);
		return (((num3 * vector.y) + (num4 * vector2.y)) + (num5 * vector3.y));
	}

	/*private static bool CheckNearestNodeIntersection(NavMeshNode node, Int3 srcLoc, Int3 destLoc, ref int edge)
	{
		if (!node.ContainsPoint(destLoc))
		{
			int num = 0;
			Int3[] numArray = _staticVerts;
			node.GetPoints(out numArray[0], out numArray[1], out numArray[2]);
			float maxValue = float.MaxValue;
			int num3 = -1;
			Vector3 vector = (Vector3)srcLoc;
			Vector3 vector2 = (Vector3)destLoc;
			for (int i = 0; i < 3; i++)
			{
				if (Polygon.Intersects(numArray[i], numArray[(i + 1) % 3], srcLoc, destLoc))
				{
					bool flag;
					num++;
					Vector3 b = Polygon.IntersectionPoint((Vector3)numArray[i], (Vector3)numArray[(i + 1) % 3], vector, vector2, out flag);
					//DebugHelper.Assert(flag);
					float num5 = vector.XZSqrMagnitude(ref b);
					if (num5 < maxValue)
					{
						maxValue = num5;
						num3 = i;
					}
				}
			}
			if ((num != 2) || (num3 == -1))
			{
				return false;
			}
			edge = num3;
		}
		return true;
	}*/

	/*public static Int3 FindValidTarget(IRVOActor actor, Int3 start, Int3 end, out bool bResult)
	{
		int actorCamp = (int)actor.ActorCamp;
		NavMeshNode nearestNode = null;
		List<object> objs = null;
		int count = 0;
		bResult = false;
		if (AstarPath.active != null)
		{
			int num5;
			int num6;
			int num7;
			int num8;
			AstarData data = AstarPath.active.GetData(actorCamp);
			if (data == null)
			{
				return end;
			}
			data.rasterizer.GetCellPosClamped(out num5, out num6, start);
			data.rasterizer.GetCellPosClamped(out num7, out num8, end);
			bool flag = num5 < num7;
			bool flag2 = num6 < num8;
			int num = !flag ? (num5 - num7) : (num7 - num5);
			int num2 = !flag2 ? (num6 - num8) : (num8 - num6);
			for (int i = 0; i <= num; i++)
			{
				for (int j = 0; j <= num2; j++)
				{
					int x = num5 + (i * (!flag ? -1 : 1));
					int y = num6 + (j * (!flag2 ? -1 : 1));
					objs = data.rasterizer.GetObjs(x, y);
					if (objs != null)
					{
						count = objs.Count;
						if (count != 0)
						{
							Int3 num10;
							Int3 num11;
							if (count > 2)
							{
								if (data.rasterizer.IntersectionSegment(x, y, start, end) && data.CheckSegmentIntersects(start, end, x, y, out num10, out nearestNode))
								{
									if (nearestNode != null)
									{
										num11 = num10;
										bResult = MakePointInTriangle(ref num10, nearestNode, -4, 4, -4, 4, num11);
									}
									return num10;
								}
							}
							else if (data.CheckSegmentIntersects(start, end, x, y, out num10, out nearestNode))
							{
								if (nearestNode != null)
								{
									num11 = num10;
									bResult = MakePointInTriangle(ref num10, nearestNode, -4, 4, -4, 4, num11);
								}
								return num10;
							}
						}
					}
				}
			}
		}
		return end;
	}

	public static Int3 FindValidTarget(IRVOActor actor, Int3 start, Int3 end, int radius, out bool bResult)
	{
		long num = radius * radius;
		if (start.XZSqrMagnitude(ref end) < num)
		{
			return FindValidTarget(actor, start, end, out bResult);
		}
		Int3 num3 = end - start;
		Int3 num4 = start + num3.NormalizeTo(radius);
		return FindValidTarget(actor, start, num4, out bResult);
	}*/

	private static void GetAllNodesByVert(ref List<TMNodeInfo> nodeInfos, NavMeshNode startNode, Int3 vertex)
	{
		if (nodeInfos == null)
		{
			nodeInfos = new List<TMNodeInfo>();
		}
		for (int i = 0; i < nodeInfos.Count; i++)
		{
			TMNodeInfo info2 = nodeInfos[i];
			if (info2.node == startNode)
			{
				return;
			}
		}
		int num2 = -1;
		if (startNode.v0 == vertex)
		{
			num2 = 0;
		}
		else if (startNode.v1 == vertex)
		{
			num2 = 1;
		}
		else if (startNode.v2 == vertex)
		{
			num2 = 2;
		}
		else
		{
			return;
		}
		TMNodeInfo item = new TMNodeInfo();
		item.vi = num2;
		item.node = startNode;
		item.v0 = startNode.GetVertex(num2 % 3);
		item.v1 = startNode.GetVertex((num2 + 1) % 3);
		item.v2 = startNode.GetVertex((num2 + 2) % 3);
		nodeInfos.Add(item);
		if (startNode.connections != null)
		{
			for (int j = 0; j < startNode.connections.Length; j++)
			{
				NavMeshNode node = startNode.graph.GetNode(startNode.connections[j]) as NavMeshNode;
				if (node != null)
				{
					GetAllNodesByVert(ref nodeInfos, node, vertex);
				}
			}
		}
	}

	/*public static bool GetGroundY(IRVOActor actor, out Int groundY)
	{
		if ((AstarPath.active == null) || (actor == null))
		{
			groundY = (actor == null) ? 0 : actor.groundY;
			return false;
		}
		groundY = actor.groundY;
		acotrName = (actor == null) ? string.Empty : actor.name;
		Int3 location = actor.location;
		NavMeshNode locatedByRasterizer = AstarPath.active.astarData.GetLocatedByRasterizer(location);
		if (locatedByRasterizer == null)
		{
			return false;
		}
		float num2 = CalculateY_Clamped((Vector3)location, locatedByRasterizer);
		groundY = (Int)num2;
		return true;
	}

	public static bool GetGroundY(Int3 pos, out Int groundY)
	{
		if (AstarPath.active == null)
		{
			groundY = pos.y;
			return false;
		}
		groundY = pos.y;
		acotrName = "null";
		NavMeshNode locatedByRasterizer = AstarPath.active.astarData.GetLocatedByRasterizer(pos);
		if (locatedByRasterizer == null)
		{
			return false;
		}
		float num = CalculateY_Clamped((Vector3)pos, locatedByRasterizer);
		groundY = (Int)num;
		return true;
	}*/

	private static void getMinMax(out int min, out int max, long axis, ref VFactor factor)
	{
		long num = axis * factor.nom;
		int num2 = (int)(num / factor.den);
		if (num < 0L)
		{
			min = num2 - 1;
			max = num2;
		}
		else
		{
			min = num2;
			max = num2 + 1;
		}
	}

	/*private static NavMeshNode getNearestNode(Vector3 position)
	{
		NNConstraint constraint = new NNConstraint();
		constraint.distanceXZ = true;
		constraint.constrainWalkability = false;
		constraint.constrainArea = false;
		constraint.constrainTags = false;
		constraint.constrainDistance = false;
		constraint.graphMask = -1;
		return (AstarPath.active.GetNearest(position, constraint).node as NavMeshNode);
	}*/

	private static Int3 InternalMove(Int3 srcLoc, Int3 delta, ref Int groundY, IRVOActor actor, MoveDirectionState state = null)
	{
		Int3 num4;
		if ((delta.x == 0) && (delta.z == 0))
		{
			return delta;
		}
		Int3 end = srcLoc + delta;
		int edge = -1;
		//int actorCamp = (int)actor.ActorCamp;
		//AstarData data = AstarPath.active.GetData(actorCamp);
		NavMeshGraph graph = PathFindingMachine.Instance.navgationGraph as NavMeshGraph;
		NavMeshNode node = graph.bbTree.QueryInside((Vector3)srcLoc, null);
		if (node == null)
		{
			NavMeshNode node2 = graph.bbTree.QueryInside((Vector3)end, null);// data.IntersectByRasterizer(srcLoc, end, out edge);
			edge = node.EdgeIntersect(node.position, end);
			if (node2 == null)
			{
				return Int3.zero;
			}
			node = node2;
		}
		if (state != null)
		{
			state.BeginMove();
			MoveFromNode(node, edge, srcLoc, end, state, out num4);
			state.EndMove();
		}
		else
		{
			MoveFromNode(node, edge, srcLoc, end, null, out num4);
		}
		checkedNodes.Clear();
		groundY = num4.y;
		if (!MoveAxisY)
		{
			num4.y = srcLoc.y;
		}
		return (num4 - srcLoc);
	}

	/*public static bool IsValidTarget(IRVOActor actor, Int3 target)
	{
		if (AstarPath.active == null)
		{
			return false;
		}
		int actorCamp = (int)actor.TheActorMeta.ActorCamp;
		return (AstarPath.active.GetLocatedByRasterizer(target, actorCamp) != null);
	}*/

	private static bool MakePointInTriangle(ref Int3 result, NavMeshNode node, int minX, int maxX, int minZ, int maxZ, Int3 offset)
	{
		Int3 num;
		Int3 num2;
		Int3 num3;
		node.GetPoints(out num, out num2, out num3);
		long num4 = num2.x - num.x;
		long num5 = num3.x - num2.x;
		long num6 = num.x - num3.x;
		long num7 = num2.z - num.z;
		long num8 = num3.z - num2.z;
		long num9 = num.z - num3.z;
		for (int i = minX; i <= maxX; i++)
		{
			for (int j = minZ; j <= maxZ; j++)
			{
				int num12 = i + offset.x;
				int num13 = j + offset.z;
				if (((((num4 * (num13 - num.z)) - ((num12 - num.x) * num7)) <= 0L) && (((num5 * (num13 - num2.z)) - ((num12 - num2.x) * num8)) <= 0L)) && (((num6 * (num13 - num3.z)) - ((num12 - num3.x) * num9)) <= 0L))
				{
					result.x = num12;
					result.z = num13;
					return true;
				}
			}
		}
		return false;
	}

	public static Int3 Move(IRVOActor actor, Int3 delta, out Int groundY, MoveDirectionState state = null)
	{
		if (actor.isMovable)
		{
			groundY = (actor == null) ? 0 : actor.groundY;
			return InternalMove(actor.location, delta, ref groundY, actor, state);
		}
		groundY = actor.groundY;
		return Int3.zero;
	}

	public static Int3 Move(IRVOActor actor, Int3 delta, out Int groundY, out bool collided, MoveDirectionState state = null)
	{
		Int3 num = Move(actor, delta, out groundY, state);
		collided = (num.x != delta.x) || (num.z != delta.z);
		return num;
	}

	private static void MoveAlongEdge(NavMeshNode node, int edge, Int3 srcLoc, Int3 destLoc, MoveDirectionState state, out Int3 result, bool checkAnotherEdge = true)
	{
		bool flag;
		//DebugHelper.Assert((edge >= 0) && (edge <= 2));
		Int3 vertex = node.GetVertex(edge);
		Int3 num2 = node.GetVertex((edge + 1) % 3);
		Int3 a = destLoc - srcLoc;
		a.y = 0;
		Int3 lhs = num2 - vertex;
		lhs.y = 0;
		lhs.NormalizeTo(0x3e8);
		int num5 = 0;
		if (state != null)
		{
			num5 = a.magnitude2D * 0x3e8;
			Int3 num6 = !state.enabled ? a : state.firstAdjDir;
			if (Int3.Dot(ref lhs, ref num6) < 0)
			{
				num5 = -num5;
				num6 = -lhs;
			}
			else
			{
				num6 = lhs;
			}
			if (!state.enabled)
			{
				state.enabled = true;
				state.firstAdjDir = Int3.Lerp(a, num6, 1, 3);
				state.firstDir = state.curDir;
				state.adjDir = num6;
			}
			else if (Int3.Dot(ref state.adjDir, ref num6) >= 0)
			{
				state.adjDir = num6;
			}
			else
			{
				num5 = 0;
			}
			state.applied = true;
		}
		else
		{
			num5 = (lhs.x * a.x) + (lhs.z * a.z);
		}
		Int3 rhs = Polygon.IntersectionPoint(ref vertex, ref num2, ref srcLoc, ref destLoc, out flag);
		if (!flag)
		{
			if (!VectorMath.IsColinearXZ(vertex, num2, srcLoc) || !VectorMath.IsColinearXZ(vertex, num2, destLoc))
			{
				result = srcLoc;
				return;
			}
			if (num5 >= 0)
			{
				int num8 = (lhs.x * (num2.x - vertex.x)) + (lhs.z * (num2.z - vertex.z));
				int num9 = (lhs.x * (destLoc.x - vertex.x)) + (lhs.z * (destLoc.z - vertex.z));
				rhs = (num8 <= num9) ? num2 : destLoc;
				//DebugHelper.Assert((num8 >= 0) && (num9 >= 0));
			}
			else
			{
				int num10 = (-lhs.x * (vertex.x - num2.x)) - (lhs.z * (vertex.z - num2.z));
				int num11 = (-lhs.x * (destLoc.x - num2.x)) - (lhs.z * (destLoc.z - num2.z));
				rhs = (Mathf.Abs(num10) <= Mathf.Abs(num11)) ? vertex : destLoc;
				//DebugHelper.Assert((num10 >= 0) && (num11 >= 0));
			}
		}
		int num12 = -IntMath.Sqrt(vertex.XZSqrMagnitude(rhs) * 0xf4240L);
		int num13 = IntMath.Sqrt(num2.XZSqrMagnitude(rhs) * 0xf4240L);
		if ((num5 >= num12) && (num5 <= num13))
		{
			result = IntMath.Divide(lhs, (long)num5, 0xf4240L) + rhs;
			if (!node.ContainsPoint(result))
			{
				int num16;
				int num17;
				int num18;
				int num19;
				Vector3 vector = (Vector3)(num2 - vertex);
				vector.y = 0f;
				vector.Normalize();
				Int3 num14 = num2 - vertex;
				num14.y = 0;
				num14 *= 0x2710;
				long magnitude = num14.magnitude;
				VFactor factor = new VFactor();
				factor.nom = num5;
				factor.den = magnitude * 0x3e8L;
				getMinMax(out num16, out num18, (long)num14.x, ref factor);
				getMinMax(out num17, out num19, (long)num14.z, ref factor);
				if (!MakePointInTriangle(ref result, node, num16, num18, num17, num19, srcLoc) && !MakePointInTriangle(ref result, node, num16 - 4, num18 + 4, num17 - 4, num19 + 4, srcLoc))
				{
					result = srcLoc;
				}
			}
			if (MoveAxisY)
			{
				CalculateY(ref result, node);
			}
		}
		else
		{
			int num20;
			int num21;
			Int3 num22;
			int num24;
			if (num5 < num12)
			{
				num20 = num5 - num12;
				num21 = (edge + 2) % 3;
				num22 = vertex;
			}
			else
			{
				num20 = num5 - num13;
				num21 = (edge + 1) % 3;
				num22 = num2;
			}
			Int3 num23 = (Int3)((lhs * num20) / 1000000f);
			NavMeshNode neighborByEdge = node.GetNeighborByEdge(num21, out num24);
			if (neighborByEdge != null)
			{
				checkedNodes.Add(node);
				MoveFromNode(neighborByEdge, num24, num22, num23 + num22, state, out result);
			}
			else
			{
				if (checkAnotherEdge)
				{
					Int3 num27 = node.GetVertex((edge + 2) % 3) - num22;
					if (Int3.Dot(num27.NormalizeTo(0x3e8), num23) > 0)
					{
						checkedNodes.Add(node);
						MoveAlongEdge(node, num21, num22, num23 + num22, state, out result, false);
						return;
					}
				}
				result = num22;
			}
		}
	}

	private static void MoveFromNode(NavMeshNode node, int startEdge, Int3 srcLoc, Int3 destLoc, MoveDirectionState state, out Int3 result)
	{
		result = srcLoc;
		while (node != null)
		{
			int num;
			int num10;
			int count = 2;
			if (node.IsVertex(srcLoc, out num))
			{
				Int3 vertex = node.GetVertex(num);
				List<TMNodeInfo> nodeInfos = null;
				GetAllNodesByVert(ref nodeInfos, node, vertex);
				NavMeshNode node2 = null;
				int vi = -1;
				for (int i = 0; i < nodeInfos.Count; i++)
				{
					TMNodeInfo info = nodeInfos[i];
					if ((!checkedNodes.Contains(info.node) && !VectorMath.RightXZ(info.v0, info.v2, destLoc)) && VectorMath.RightOrColinearXZ(info.v0, info.v1, destLoc))
					{
						node2 = info.node;
						vi = info.vi;
						break;
					}
				}
				if (node2 != null)
				{
					node = node2;
					startEdge = (vi + 1) % 3;
					count = 1;
				}
				else
				{
					int num6 = -1;
					VFactor factor3 = new VFactor();
					factor3.nom = -2L;
					factor3.den = 1L;
					VFactor factor = factor3;
					for (int j = 0; j < nodeInfos.Count; j++)
					{
						TMNodeInfo info2 = nodeInfos[j];
						if (!checkedNodes.Contains(info2.node))
						{
							int num8;
							VFactor factor2 = info2.GetCosineAngle(destLoc, state, out num8);
							if (factor2 > factor)
							{
								factor = factor2;
								num6 = num8;
								node2 = info2.node;
							}
						}
					}
					if (node2 != null)
					{
						MoveAlongEdge(node2, num6, srcLoc, destLoc, state, out result, true);
						break;
					}
				}
			}
			int edge = -1;
			if (startEdge == -1)
			{
				edge = node.EdgeIntersect(srcLoc, destLoc);
			}
			else
			{
				edge = node.EdgeIntersect(srcLoc, destLoc, startEdge, count);
			}
			if (edge == -1)
			{
				if (node.ContainsPoint(destLoc))
				{
					result = destLoc;
					if (MoveAxisY)
					{
						CalculateY(ref result, node);
					}
				}
				else
				{
					edge = node.GetColinearEdge(srcLoc, destLoc);
					if (edge != -1)
					{
						MoveAlongEdge(node, edge, srcLoc, destLoc, state, out result, true);
					}
				}
				break;
			}
			NavMeshNode neighborByEdge = node.GetNeighborByEdge(edge, out num10);
			if (neighborByEdge != null)
			{
				node = neighborByEdge;
				startEdge = num10 + 1;
				count = 2;
			}
			else
			{
				MoveAlongEdge(node, edge, srcLoc, destLoc, state, out result, true);
				break;
			}
		}
	}

	public static Int3 MoveLerp(IRVOActor actor, Int3 srcPos, Int3 delta, out Int groundY)
	{
		if (actor.isMovable)
		{
			groundY = (actor == null) ? 0 : actor.groundY;
			return InternalMove(srcPos, delta, ref groundY, actor, null);
		}
		groundY = actor.groundY;
		return Int3.zero;
	}

	/*public static bool ValidateTarget(Int3 loc, Int3 target, out Int3 newTarget, out int nodeIndex)
	{
		newTarget = target;
		nodeIndex = -1;
		if (AstarPath.active == null)
		{
			return false;
		}
		AstarData astarData = AstarPath.active.astarData;
		if (astarData.GetLocatedByRasterizer(target) == null)
		{
			int edge = -1;
			NavMeshNode node = astarData.IntersectByRasterizer(target, loc, out edge);
			if (node == null)
			{
				return false;
			}
			Int3[] numArray = _staticVerts;
			node.GetPoints(out numArray[0], out numArray[1], out numArray[2]);
			bool intersects = false;
			Int3 result = Polygon.IntersectionPoint(ref target, ref loc, ref numArray[edge], ref numArray[(edge + 1) % 3], out intersects);
			if (!intersects)
			{
				return false;
			}
			if (!MakePointInTriangle(ref result, node, -4, 4, -4, 4, Int3.zero))
			{
				return false;
			}
			newTarget = result;
		}
		return true;
	}*/

	[StructLayout(LayoutKind.Sequential)]
	private struct TMNodeInfo
	{
		public NavMeshNode node;
		public int vi;
		public Int3 v0;
		public Int3 v1;
		public Int3 v2;
		public VFactor GetCosineAngle(Int3 dest, MoveDirectionState state, out int edgeIndex)
		{
			Int3 rhs = this.v1 - this.v0;
			Int3 num2 = this.v2 - this.v0;
			Int3 lhs = dest - this.v0;
			lhs.NormalizeTo(0x3e8);
			rhs.NormalizeTo(0x3e8);
			num2.NormalizeTo(0x3e8);
			long num4 = Int3.DotXZLong(ref lhs, ref rhs);
			long num5 = Int3.DotXZLong(ref lhs, ref num2);
			VFactor factor = new VFactor();
			factor.den = 0xf4240L;
			if (num4 > num5)
			{
				edgeIndex = this.vi;
				factor.nom = num4;
				return factor;
			}
			edgeIndex = (this.vi + 2) % 3;
			factor.nom = num5;
			return factor;
		}
	}
}

