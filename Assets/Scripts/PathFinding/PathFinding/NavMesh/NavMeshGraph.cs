

using AStar;


namespace PathFinding
{
	public class NavMeshGraph : AStarMap, INavGraph
	{
		NavMeshData navData;

		public BBTree bbTree = new BBTree();


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


		public bool IsPassable(FixVector3 position)
		{
			var node = bbTree.QueryInside(position.ToVector3(), null);
			return node != null;
		}


		public FixVector3 RayCastForMoving(FixVector3 from, FixVector3 to, MoveType mov)
		{
			return to;
		}


		public FixVector3 SlideByObstacles(FixVector3 fromPos, FixVector3 oldTargetPos)
		{
			return oldTargetPos;
		}

	}
}

