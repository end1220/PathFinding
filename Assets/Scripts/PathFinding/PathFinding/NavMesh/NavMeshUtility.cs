

using PathFinding;
using PathFinding.RVO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class NavMeshUtility
{
	
	private static List<NavMeshNode> checkedNodes = new List<NavMeshNode>();

	public static bool MoveAxisY = true;


	[StructLayout(LayoutKind.Sequential)]
	private struct CheckNodeInfo
	{
		public NavMeshNode node;
		public int vi;
		public Int3 v0;
		public Int3 v1;
		public Int3 v2;
		public VFactor GetCosineAngle(Int3 dest, out int edgeIndex)
		{
			Int3 rhs = v1 - v0;
			Int3 num2 = v2 - v0;
			Int3 lhs = dest - v0;
			lhs.NormalizeTo(1000);
			rhs.NormalizeTo(1000);
			num2.NormalizeTo(1000);
			long num4 = Int3.DotXZLong(ref lhs, ref rhs);
			long num5 = Int3.DotXZLong(ref lhs, ref num2);
			VFactor factor = new VFactor();
			factor.den = 0xf4240L;
			if (num4 > num5)
			{
				edgeIndex = vi;
				factor.nom = num4;
				return factor;
			}
			edgeIndex = (vi + 2) % 3;
			factor.nom = num5;
			return factor;
		}
	}


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


	/// <summary>
	/// 所有共用vertex的三角形
	/// </summary>
	private static void GetAllNodesByVert(ref List<CheckNodeInfo> nodeInfos, NavMeshNode startNode, Int3 vertex)
	{
		if (nodeInfos == null)
		{
			nodeInfos = new List<CheckNodeInfo>();
		}
		for (int i = 0; i < nodeInfos.Count; i++)
		{
			CheckNodeInfo info2 = nodeInfos[i];
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
		CheckNodeInfo item = new CheckNodeInfo();
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

	

	private static Int3 InternalMove(Int3 srcLoc, Int3 delta, ref Int groundY)
	{
		Int3 num4;
		if ((delta.x == 0) && (delta.z == 0))
		{
			return delta;
		}
		Int3 end = srcLoc + delta;
		int edge = -1;
		NavMeshGraph graph = PathFindingMachine.Instance.navgationGraph as NavMeshGraph;
		NavMeshNode node = graph.bbTree.QueryInside(srcLoc, null);
		if (node == null)
		{
			NavMeshNode node2 = graph.bbTree.QueryInside(end, null);
			if (node2 == null)
			{
				return Int3.zero;
			}
			else
			{
				edge = node2.EdgeIntersect(srcLoc, end);
				node = node2;
			}
		}
		
		MoveFromNode(node, edge, srcLoc, end, out num4);
		
		checkedNodes.Clear();
		groundY = num4.y;
		if (!MoveAxisY)
		{
			num4.y = srcLoc.y;
		}
		return (num4 - srcLoc);
	}

	

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


	/*public static Int3 Move(IRVOActor actor, Int3 delta, out Int groundY)
	{
		if (actor.isMovable)
		{
			groundY = (actor == null) ? 0 : actor.groundY;
			return InternalMove(actor.location, delta, ref groundY);
		}
		groundY = actor.groundY;
		return Int3.zero;
	}


	public static Int3 Move(IRVOActor actor, Int3 delta, out Int groundY, out bool collided)
	{
		Int3 num = Move(actor, delta, out groundY);
		collided = (num.x != delta.x) || (num.z != delta.z);
		return num;
	}*/


	public static Int3 Move(Int3 from, Int3 delta, out Int groundY)
	{
		groundY = from.y;
		return InternalMove(from, delta, ref groundY);
	}


	public static Int3 Move(Int3 from, Int3 delta, out Int groundY, out bool collided)
	{
		Int3 num = Move(from, delta, out groundY);
		collided = (num.x != delta.x) || (num.z != delta.z);
		return num;
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="node">所在node</param>
	/// <param name="edge">所沿着的边</param>
	/// <param name="srcLoc">from</param>
	/// <param name="destLoc">to</param>
	/// <param name="result">实际可到达的点</param>
	private static void MoveAlongEdge(NavMeshNode node, int edge, Int3 srcLoc, Int3 destLoc, out Int3 result, bool checkAnotherEdge = true)
	{
		bool flag;
		//DebugHelper.Assert((edge >= 0) && (edge <= 2));
		Int3 vertex = node.GetVertex(edge);
		Int3 num2 = node.GetVertex((edge + 1) % 3);
		Int3 a = destLoc - srcLoc;
		a.y = 0;
		Int3 lhs = num2 - vertex;
		lhs.y = 0;
		lhs.NormalizeTo(1000);
		int num5 = 0;
		
		// 点乘移动向量和边向量
		num5 = (lhs.x * a.x) + (lhs.z * a.z);

		// 移动向量和边向量的交点
		Int3 rhs = Polygon.IntersectionPoint(ref vertex, ref num2, ref srcLoc, ref destLoc, out flag);

		// 不相交？
		if (!flag)
		{
			// 边和from to存在不共线，则不能移动
			if (!VectorMath.IsColinearXZ(vertex, num2, srcLoc) || !VectorMath.IsColinearXZ(vertex, num2, destLoc))
			{
				result = srcLoc;
				return;
			}

			// 计算本三角形内可移动到的点 rhs
			// 移动向量和边向量 同向
			if (num5 >= 0)
			{
				int num8 = (lhs.x * (num2.x - vertex.x)) + (lhs.z * (num2.z - vertex.z));
				int num9 = (lhs.x * (destLoc.x - vertex.x)) + (lhs.z * (destLoc.z - vertex.z));
				rhs = (num8 <= num9) ? num2 : destLoc;
				//DebugHelper.Assert((num8 >= 0) && (num9 >= 0));
			}
			else
			{
				// 移动向量和边向量 异向
				int num10 = (-lhs.x * (vertex.x - num2.x)) - (lhs.z * (vertex.z - num2.z));
				int num11 = (-lhs.x * (destLoc.x - num2.x)) - (lhs.z * (destLoc.z - num2.z));
				rhs = (Mathf.Abs(num10) <= Mathf.Abs(num11)) ? vertex : destLoc;
				//DebugHelper.Assert((num10 >= 0) && (num11 >= 0));
			}
		}

		// 计算交点到边的顶点的距离1000倍
		int num12 = -IntMath.Sqrt(vertex.XZSqrMagnitude(rhs) * 0xf4240L);// 一百万
		int num13 = IntMath.Sqrt(num2.XZSqrMagnitude(rhs) * 0xf4240L);

		// num5是在边上的投影

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
				num14 *= 10000;
				long magnitude = num14.magnitude;
				VFactor factor = new VFactor();
				factor.nom = num5;
				factor.den = magnitude * 1000L;
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
				MoveFromNode(neighborByEdge, num24, num22, num23 + num22, out result);
			}
			else
			{
				if (checkAnotherEdge)
				{
					Int3 num27 = node.GetVertex((edge + 2) % 3) - num22;
					if (Int3.Dot(num27.NormalizeTo(1000), num23) > 0)
					{
						checkedNodes.Add(node);
						MoveAlongEdge(node, num21, num22, num23 + num22, out result, false);
						return;
					}
				}
				result = num22;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="node"></param>
	/// <param name="startEdge"></param>
	/// <param name="srcLoc"></param>
	/// <param name="destLoc"></param>
	/// <param name="state"></param>
	/// <param name="result">实际可移动到的点</param>
	private static void MoveFromNode(NavMeshNode node, int startEdge, Int3 srcLoc, Int3 destLoc, out Int3 result)
	{
		result = srcLoc;
		while (node != null)
		{
			int num;
			int num10;
			int count = 2;
			// 当前在顶点上
			if (node.IsVertex(srcLoc, out num))
			{
				// 找到所有包含srcLoc这顶点的三角形
				Int3 vertex = node.GetVertex(num);
				List<CheckNodeInfo> nodeInfos = null;
				GetAllNodesByVert(ref nodeInfos, node, vertex);

				// 找到其中将要穿过的三角形
				NavMeshNode node2 = null;
				int vi = -1;
				for (int i = 0; i < nodeInfos.Count; i++)
				{
					CheckNodeInfo info = nodeInfos[i];
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
					// 将要穿过的边
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
						CheckNodeInfo info2 = nodeInfos[j];
						if (!checkedNodes.Contains(info2.node))
						{
							int num8;
							VFactor factor2 = info2.GetCosineAngle(destLoc, out num8);
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
						MoveAlongEdge(node2, num6, srcLoc, destLoc, out result, true);
						break;
					}
				}
			}

			// 将要穿过的边
			int edge = -1;
			if (startEdge == -1)
			{
				edge = node.EdgeIntersect(srcLoc, destLoc);
			}
			else
			{
				edge = node.EdgeIntersect(srcLoc, destLoc, startEdge, count);
			}

			// 不会穿越边
			if (edge == -1)
			{
				if (node.ContainsPoint(destLoc))
				{
					// 三角形内移动
					result = destLoc;
					if (MoveAxisY)
					{
						CalculateY(ref result, node);
					}
				}
				else
				{
					// 沿边所在直线移动
					edge = node.GetColinearEdge(srcLoc, destLoc);
					if (edge != -1)
					{
						MoveAlongEdge(node, edge, srcLoc, destLoc, out result, true);
					}
				}
				break;
			}

			// 会穿过边，则看相邻三角形
			NavMeshNode neighborByEdge = node.GetNeighborByEdge(edge, out num10);
			if (neighborByEdge != null)
			{
				node = neighborByEdge;
				startEdge = num10 + 1;
				count = 2;
			}
			else
			{
				MoveAlongEdge(node, edge, srcLoc, destLoc, out result, true);
				break;
			}
		}
	}


}

