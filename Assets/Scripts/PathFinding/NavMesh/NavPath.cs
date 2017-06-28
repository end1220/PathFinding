
using System.Collections;

namespace GridNavPath
{
    /**
     * A path across a navigation mesh
     * 
     * @author zhoukun
     */
    public class NavPath
    {
        /** The list of links that form this path */
        private ArrayList links = new ArrayList();

        /**
         * Create a new path
         */
        public NavPath()
        {
        }

        /**
         * Push a link to the end of the path
         * 
         * @param link The link to the end of the path
         */
        public void push(Link link)
        {
            links.Add(link);
        }

        /**
         * Get the length of the path
         * 
         * @return The number of steps in the path
         */
        public int length()
        {
            return links.Count;
        }

        /**
         * Get the x coordinate of the given step
         * 
         * @param step The index of the step to retrieve
         * @return The x coordinate at the given step index
         */
        public float getX(int step)
        {
            return ((Link)links[step]).getX();
        }

        /**
         * Get the y coordinate of the given step
         * 
         * @param step The index of the step to retrieve
         * @return The y coordinate at the given step index
         */
        public float getY(int step)
        {
            return ((Link)links[step]).getY();
        }

        /**
         * Get a string representation of this instance
         * 
         * @return The string representation of this instance
         */
        public string toString()
        {
            return "[Path length=" + length() + "]";
        }

        /**
         * Remove a step in the path
         * 
         * @param i
         */
        public void remove(int i)
        {
            links.Remove(i);
        }
    }

}