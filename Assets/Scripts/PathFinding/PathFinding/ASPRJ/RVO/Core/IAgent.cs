﻿namespace PathFinding.RVO
{
    using System;
    using System.Collections.Generic;

    public interface IAgent
    {
        void SetYPosition(Int yCoordinate);
        void Teleport(Int3 pos);

       /* int AgentTimeHorizon { get; set; }

        RVOLayer CollidesWith { get; set; }

        bool DebugDraw { get; set; }

        VInt3 DesiredVelocity { get; set; }

        VInt Height { get; set; }*/

        Int3 InterpolatedPosition { get; }

       /* RVOLayer Layer { get; set; }

        bool Locked { get; set; }

        int MaxNeighbours { get; set; }

        VInt MaxSpeed { get; set; }

        VInt NeighbourDist { get; set; }

        List<ObstacleVertex> NeighbourObstacles { get; }

        int ObstacleTimeHorizon { get; set; }

        VInt3 Position { get; }

        VInt Radius { get; set; }

        VInt3 Velocity { get; set; }*/
    }
}

