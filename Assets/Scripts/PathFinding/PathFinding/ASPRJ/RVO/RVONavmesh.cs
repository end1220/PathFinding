namespace PathFinding.RVO
{
	using PathFinding;
	using System;
	using System.Collections.Generic;
	using UnityEngine;


	[AddComponentMenu("PathFinding/Local Avoidance/RVO Navmesh")]
	public class RVONavmesh : GraphModifier
	{
		private Simulator lastSim;
		private List<ObstacleVertex> obstacles = new List<ObstacleVertex>();
		public Int wallHeight = 0x1388;

		/*public void AddGraphObstacles(Simulator sim, NavGraph graph)
		{
			CAddGraphObstacles storeyb = new CAddGraphObstacles();
			storeyb.sim = sim;
			storeyb.f__this = this;
			if (((this.obstacles.Count > 0) && (this.lastSim != null)) && (this.lastSim != storeyb.sim))
			{
				this.RemoveObstacles();
			}
			this.lastSim = storeyb.sim;
			INavmesh navmesh = graph as INavmesh;
			if (navmesh != null)
			{
				storeyb.uses = new int[20];
				navmesh.GetNodes(new GraphNodeDelegateCancelable(storeyb.m__2));
			}
		}

		public override void OnLatePostScan()
		{
			if (Application.isPlaying)
			{
				this.RemoveObstacles();
				NavGraph[] graphs = AstarPath.active.graphs;
				RVOSimulator simulator = UnityEngine.Object.FindObjectOfType(typeof(RVOSimulator)) as RVOSimulator;
				if (simulator == null)
				{
					throw new NullReferenceException("No RVOSimulator could be found in the scene. Please add one to any GameObject");
				}
				Simulator sim = simulator.GetSimulator();
				for (int i = 0; i < graphs.Length; i++)
				{
					this.AddGraphObstacles(sim, graphs[i]);
				}
				sim.UpdateObstacles();
			}
		}*/

		public override void OnPostCacheLoad()
		{
			this.OnLatePostScan();
		}

		public void RemoveObstacles()
		{
			if (this.lastSim != null)
			{
				Simulator lastSim = this.lastSim;
				this.lastSim = null;
				for (int i = 0; i < this.obstacles.Count; i++)
				{
					lastSim.RemoveObstacle(this.obstacles[i]);
				}
				this.obstacles.Clear();
			}
		}

		/*private sealed class CAddGraphObstacles
		{
			internal RVONavmesh f__this;
			internal Simulator sim;
			internal int[] uses;

			internal bool m__2(GraphNode _node)
			{
				int num6;
				NavMeshNode node = _node as NavMeshNode;
				this.uses[2] = num6 = 0;
				this.uses[0] = this.uses[1] = num6;
				if (node != null)
				{
					for (int i = 0; i < node.connections.Length; i++)
					{
						NavMeshNode other = node.connections[i] as NavMeshNode;
						if (other != null)
						{
							int index = node.SharedEdge(other);
							if (index != -1)
							{
								this.uses[index] = 1;
							}
						}
					}
					for (int j = 0; j < 3; j++)
					{
						if (this.uses[j] == 0)
						{
							Int3 vertex = node.GetVertex(j);
							Int3 b = node.GetVertex((j + 1) % node.GetVertexCount());
							this.f__this.obstacles.Add(this.sim.AddObstacle(vertex, b, this.f__this.wallHeight));
						}
					}
				}
				return true;
			}
		}*/

	}
}

