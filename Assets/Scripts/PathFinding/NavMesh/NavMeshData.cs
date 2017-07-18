
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graph;
using AStar.NavGraph;


namespace AStar
{
	/// <summary>
	/// 存储着navmesh数据
	/// </summary>
	public class NavMeshData : ScriptableObject
	{
		[HideInInspector]
		public BuildConfig buildConfig;


#if UNITY_EDITOR

		public void Init(BuildConfig cfg)
		{
			this.buildConfig = cfg;
		}


		public void AddNode(Graph3DAStarNode node)
		{
			
		}


		public void AddEdge(Graph3DAStarEdge edge)
		{
			
		}


		public void SaveBytes()
		{
			
		}

#endif

	}
}
