using UnityEngine;


namespace PathFinding
{
	public class PointNode : GraphNode
	{
		public GraphNode[] connections;
		public uint[] connectionCosts;

		/** GameObject this node was created from (if any).
		 * \warning When loading a graph from a saved file or from cache, this field will be null.
		 */
		public GameObject gameObject;

		/** Used for internal linked list structure.
		 * \warning Do not modify
		 */
		public PointNode next;

		public void SetPosition(Int3 value)
		{
			position = value;
		}


		public override void GetConnections(GraphNodeDelegate del)
		{
			if (connections == null) return;
			for (int i = 0; i < connections.Length; i++) del(connections[i]);
		}

		public override void ClearConnections(bool alsoReverse)
		{
			if (alsoReverse && connections != null)
			{
				for (int i = 0; i < connections.Length; i++)
				{
					connections[i].RemoveConnection(this);
				}
			}

			connections = null;
			connectionCosts = null;
		}


		public override bool ContainsConnection(GraphNode node)
		{
			for (int i = 0; i < connections.Length; i++) if (connections[i] == node) return true;
			return false;
		}

		/** Add a connection from this node to the specified node.
		 * If the connection already exists, the cost will simply be updated and
		 * no extra connection added.
		 *
		 * \note Only adds a one-way connection. Consider calling the same function on the other node
		 * to get a two-way connection.
		 */
		public override void AddConnection(GraphNode node, uint cost)
		{
			if (node == null) throw new System.ArgumentNullException();

			if (connections != null)
			{
				for (int i = 0; i < connections.Length; i++)
				{
					if (connections[i] == node)
					{
						connectionCosts[i] = cost;
						return;
					}
				}
			}

			int connLength = connections != null ? connections.Length : 0;

			var newconns = new GraphNode[connLength + 1];
			var newconncosts = new uint[connLength + 1];
			for (int i = 0; i < connLength; i++)
			{
				newconns[i] = connections[i];
				newconncosts[i] = connectionCosts[i];
			}

			newconns[connLength] = node;
			newconncosts[connLength] = cost;

			connections = newconns;
			connectionCosts = newconncosts;
		}

		/** Removes any connection from this node to the specified node.
		 * If no such connection exists, nothing will be done.
		 *
		 * \note This only removes the connection from this node to the other node.
		 * You may want to call the same function on the other node to remove its eventual connection
		 * to this node.
		 */
		public override void RemoveConnection(GraphNode node)
		{
			if (connections == null) return;

			for (int i = 0; i < connections.Length; i++)
			{
				if (connections[i] == node)
				{
					int connLength = connections.Length;

					var newconns = new GraphNode[connLength - 1];
					var newconncosts = new uint[connLength - 1];
					for (int j = 0; j < i; j++)
					{
						newconns[j] = connections[j];
						newconncosts[j] = connectionCosts[j];
					}
					for (int j = i + 1; j < connLength; j++)
					{
						newconns[j - 1] = connections[j];
						newconncosts[j - 1] = connectionCosts[j];
					}

					connections = newconns;
					connectionCosts = newconncosts;
					return;
				}
			}
		}




	}
}
