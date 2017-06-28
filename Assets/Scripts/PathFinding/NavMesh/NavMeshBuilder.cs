
using System.Collections;

namespace GridNavPath
{
    /**
     * The builder responsible for converting a tile based map into
     * a navigation mesh
     * 
     * @author zhoukun
     */
    public class NavMeshBuilder
    {

        /**
         * Build a navigation mesh based on a tile map
         * 
         * @param map The map to build the navigation mesh from
         * @return The newly created navigation mesh 
         */
        public static NavMesh build(MapData map)
        {

            ArrayList spaces = new ArrayList();
            //第二步
            int nIdGen = 1;
            for (int x = 0; x < map.getWidthInTiles(); x++)
            {
                for (int y = 0; y < map.getHeightInTiles(); y++)
                {
                    if (!map.blocked(x, y))
                    {
                        Space ss = new Space(x, y, 1, 1);
                        ss.setId(nIdGen++);
                        spaces.Add(ss);
                    }
                }
            }

     //       LogManager.Log("2,beforemerge space count: " + spaces.Count, LogType.Normal);
            //第三步
            while (mergeSpaces(spaces)) { }
     //       LogManager.Log("3,aftermegre space count: " + spaces.Count, LogType.Normal);
            //第四步
            linkSpaces(spaces);

            return new NavMesh(spaces);
        }

        /**
         * Merge the spaces that have been created to optimize out anywhere
         * we can.
         * 
         * @param spaces The list of spaces to be merged
         * @return True if a merge occured and we'll have to start the merge
         * process again
         */
        private static bool mergeSpaces(ArrayList spaces)
        {
            for (int source = 0; source < spaces.Count; source++)
            {
                Space a = (Space)spaces[source];

                for (int target = source + 1; target < spaces.Count; target++)
                {
                    Space b = (Space)spaces[target];

                    if (a.canMerge(b))
                    {
                        spaces.Remove(a);
                        spaces.Remove(b);
                        spaces.Add(a.merge(b));
                        return true;
                    }
                }
            }

            return false;
        }

        /**
         * Determine the links between spaces
         * 
         * @param spaces The spaces to link up
         */
        private static void linkSpaces(ArrayList spaces)
        {
            for (int source = 0; source < spaces.Count; source++)
            {
                Space a = (Space)spaces[source];

                for (int target = source + 1; target < spaces.Count; target++)
                {
                    Space b = (Space)spaces[target];

                    if (a.hasJoinedEdge(b))
                    {
                        a.link(b);
                        b.link(a);
                    }
                }
            }
        }
    }
}