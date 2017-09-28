
using System.Collections.Generic;


namespace AStar
{

	public class AStarEngine
	{
		private Queue<AStarRequest> requestQuque = new Queue<AStarRequest>();

		private AStarContext context = new AStarContext();

		public AStarContext Context { get { return context; } }

		public AStarPathPlanner planner;


		public void Init()
		{

		}


		public void Process()
		{
			while (requestQuque.Count > 0)
			{
				AStarRequest request = requestQuque.Dequeue();
				ProcessRequest(request);
			}
		}


		public void AddRequest(AStarRequest request)
		{
			requestQuque.Enqueue(request);
		}


		public bool ProcessRequest(AStarRequest request)
		{
			context.SetRequest(request);
			return planner.Process(context);
		}


	}


}