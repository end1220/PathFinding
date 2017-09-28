

using System.Collections.Generic;
using AStar;


namespace PathFinding
{

	public class NavMeshPathPlanner : IPathPlanner
	{
		static bool enableTactical = true;
		
		public override bool Process(AStarContext context)
		{
			PathFindingRequest request = context.Request as PathFindingRequest;

			ProcessingNode startNode = context.GetStartNode();
			ProcessingNode endNode = context.GetTargetNode();
			if (startNode == null || endNode == null)
				return false;

			FindPath(startNode, endNode, context);
			List<Int3> path = context.rawPathPoints;
			if (path.Count > 0)
			{
				// set the first and last point to 'from' and 'to'.
				path[0] = request.fromPosition;
				path[path.Count - 1] = request.toPosition;
				// then optimize
				NavMeshPathOptimizer.Optimize(ref context.rawPathNodeCache, ref path);
			}

			return context.rawPathPoints.Count >= 2;
		}


		private void FindPath(ProcessingNode start, ProcessingNode end, AStarContext context)
		{
			ProcessingNode endNode = DoAStar(context);

			context.rawPathNodeCache.Clear();
			context.rawPathPoints.Clear();
			ProcessingNode pathNode = endNode;
			while (pathNode != null)
			{
				NavMeshNode navNode = pathNode.astarNode as NavMeshNode;
				context.rawPathNodeCache.Add(navNode);
				context.rawPathPoints.Add(navNode.position);
				pathNode = pathNode.prev;
			}
		}


		protected override bool CheckArrived(ProcessingNode node, AStarContext context)
		{
			ProcessingNode targetNode = context.GetTargetNode();
			return node.astarNode.id == targetNode.astarNode.id;
		}


		/// <summary>
		/// apply InfluenceMap to G vlaue，to avoide eg. go throgh enemies
		/// </summary>
		protected override int CalCostG(ProcessingNode prevNode, ProcessingNode currentNode, AStarContext context)
		{
			if (enableTactical)
			{
				int tac = TacticalCost(prevNode.astarNode as NavMeshNode, currentNode.astarNode as NavMeshNode, context);
				return prevNode.g + tac;
			}
			else
				return prevNode.g + (prevNode.astarNode as NavMeshNode).GetConnectionCost(currentNode.astarNode.id);
		}


		protected override int CalCostH(ProcessingNode node, AStarContext context)
		{
			float heuristicScale = 1.0f;
			NavMeshNode calNode = node.astarNode as NavMeshNode;
			NavMeshNode targetNode = context.GetTargetNode().astarNode as NavMeshNode;
			int dist = (calNode.position - targetNode.position).costMagnitude;
			return UnityEngine.Mathf.RoundToInt(dist * heuristicScale);
		}


		/// <summary>
		/// calculate tactical G
		/// </summary>
		private int TacticalCost(NavMeshNode prevNode, NavMeshNode currentNode, AStarContext context)
		{
			int distCost = prevNode.GetConnectionCost(currentNode.id);
			int tacCost = 0;

			PathFindingRequest request = context.Request as PathFindingRequest;

			// doer's team
			TwGame.Team team = (TwGame.Team)request.extend1;
			
			switch (team)
			{
				case TwGame.Team.Neutral:
					tacCost = 0;
					break;
				case TwGame.Team.Team_1:
				case TwGame.Team.Team_2:
					{
						int MaxInfluence = TwGame.ComInfluenceMap.MaxTeamStrengthValue;
						int cur = TwGame.AIUtil.GetTeamStrength(currentNode.position, team);
						int pre = TwGame.AIUtil.GetTeamStrength(prevNode.position, team);
						// avarage influence between current node's position and previous node's position.
						int infl = (cur + pre) >> 1;
						if (infl > 0)
						{
							tacCost = System.Math.Max(-distCost + 1, -distCost * infl / MaxInfluence);
						}
						else if (infl < 0)
						{
							tacCost = -distCost * infl / MaxInfluence * 2;
						}
					}
					break;
			}
			
			return distCost + tacCost;
		}

	}


}