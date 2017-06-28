using System;
using System.Collections;
using UnityEngine;

namespace GridNavPath
{
    /**
     * A nav-mesh is a set of shapes that describe the navigation of a map. These
     * shapes are linked together allow path finding but without the high
     * resolution that tile maps require. This leads to fast path finding and 
     * potentially much more accurate map definition.
     *  
     * @author zhoukun
     *
     */
    public class NavMesh
    {
        /** The list of spaces that build up this navigation mesh */
        private ArrayList spaces = new ArrayList();

        /**
         * Create a new empty mesh
         */
        public NavMesh()
        {

        }

        /**
         * Create a new mesh with a set of spaces
         * 
         * @param spaces The spaces included in the mesh
         */
        public NavMesh(ArrayList spac)
        {
            this.spaces = spac;
        }

        /**
         * Get the number of spaces that are in the mesh
         * 
         * @return The spaces in the mesh
         */
        public int getSpaceCount()
        {
            return spaces.Count;
        }

        /**
         * Get the space at a given index
         * 
         * @param index The index of the space to retrieve
         * @return The space at the given index
         */
        public Space getSpace(int index)
        {
            return (Space)spaces[index];
        }

        /**
         * Add a single space to the mesh
         * 
         * @param space The space to be added
         */
        public void addSpace(Space space)
        {
            spaces.Add(space);
        }

        /**
         * Find the space at a given location
         * 
         * @param x The x coordinate at which to find the space 
         * @param y The y coordinate at which to find the space 
         * @return The space at the given location
         */
        public Space findSpace(float x, float y)
        {
            for (int i = 0; i < spaces.Count; i++)
            {
                Space space = getSpace(i);
                if (space.contains((int)x, (int)y))
                {
                    return space;
                }
            }
            return null;
        }

        /**
         * Find a path from the source to the target coordinates 
         * 
         * @param sx The x coordinate of the source location
         * @param sy The y coordinate of the source location 
         * @param tx The x coordinate of the target location
         * @param ty The y coordinate of the target location
         * @param optimize True if paths should be optimized
         * @return The path between the two spaces
         */
        public NavPath findPath(float sx, float sy, float tx, float ty, bool optimize)
        {
            Space source = findSpace(sx, sy);
            Space target = findSpace(tx, ty);

            if ((source == null) || (target == null))
            {
                return null;
            }
      //      LogManager.Log("===========1=============", LogType.Normal);
            for (int i = 0; i < spaces.Count; i++)
            {
                ((Space)spaces[i]).clearCost();
            }
      //      LogManager.Log("===========2=============", LogType.Normal);
            target.fill(source, tx, ty, 0);
      //      LogManager.Log("===========3=============", LogType.Normal);
            if (target.getCost() == float.MaxValue)
            {
                return null;
            }
            if (source.getCost() == float.MaxValue)
            {
                return null;
            }

            NavPath path = new NavPath();
            path.push(new Link(sx, sy, null));
      //      LogManager.Log("===========4=============", LogType.Normal);
            if (source.pickLowestCost(target, path))
            {
      //          LogManager.Log("===========5=============", LogType.Normal);
                path.push(new Link(tx, ty, null));
                if (optimize)
                {
      //              LogManager.Log("===========6=============", LogType.Normal);
                    optimizePath(path);
                }
      //          LogManager.Log("===========7=============", LogType.Normal);
                return path;
            }

            return null;
        }

        /**
         * Check if a particular path is clear
         * 
         * @param x1 The x coordinate of the starting point
         * @param y1 The y coordinate of the starting point
         * @param x2 The x coordinate of the ending point
         * @param y2 The y coordinate of the ending point
         * @param step The size of the step between points
         * @return True if there are no blockages along the path
         */
        private bool isClear(float x1, float y1, float x2, float y2, float step)
        {
            float dx = (x2 - x1);
            float dy = (y2 - y1);
            Vector3 v1 = new Vector3(x1, 0, y1);
            Vector3 v2 = new Vector3(x2, 0, y2);
            float len = MathUtility.CalcDistance2D(v1, v2);
            dx *= step;
            dx /= len;
            dy *= step;
            dy /= len;
            float steps = len / step;

            for (int i = 0; i < steps; i++)
            {
                float x = x1 + (dx * i);
                float y = y1 + (dy * i);

                if (findSpace(x, y) == null)
                {
                    return false;
                }
            }

            return true;
        }

        /**
         * Optimize a path by removing segments that arn't required
         * to reach the end point
         * 
         * @param path The path to optimize. Redundant segments will be removed
         */
        private void optimizePath(NavPath path)
        {
            int pt = 0;

            while (pt < path.length() - 2)
            {
                float sx = path.getX(pt);
                float sy = path.getY(pt);
                float nx = path.getX(pt + 2);
                float ny = path.getY(pt + 2);

                if (isClear(sx, sy, nx, ny, 0.1f))
                {
                    path.remove(pt + 1);
                }
                else
                {
                    pt++;
                }
            }
        }
    }

}