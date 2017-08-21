namespace PathFinding.RVO
{
    using System;

    public class ObstacleVertex
    {
        public bool convex;
        public Int2 dir;
        public Int height;
        public bool ignore;
        public RVOLayer layer;
        public ObstacleVertex next;
        public Int3 position;
        public ObstacleVertex prev;
        public bool split;
    }
}

