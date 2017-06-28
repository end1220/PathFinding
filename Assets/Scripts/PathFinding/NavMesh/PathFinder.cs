
using System.Collections;
using UnityEngine;

namespace GridNavPath
{

    public class PathFinder
    {
        public static float PLAYER_POS_BASE = 1.0f;
        // The mesh built for this map 
        private NavMesh navMesh = null;
        // The path if there is one current found between the two points
        private NavPath path = null;
        // The tile based map we're searching across - loaded from a raw file
        private MapData dataMap = null;
        //每个网格大小
        private int m_nNodeSize = 0;
        //是否已经初始化完毕
        private bool m_bIsInit = false;

        public bool IsInitFinish
        {
            get
            {
                return m_bIsInit;
            }
        } 

        public int NodeSize
        {
            get
            {
                return m_nNodeSize;
            }
            set
            {
                m_nNodeSize = value;
                if (0 == m_nNodeSize)
                {
                    m_nNodeSize = 1;
                }
            }
        }
        public bool init(MapData data)
        {
            if (null == data)
            {
                return false;
            }
            navMesh = NavMeshBuilder.build(data);
            if (navMesh == null)
            {
                return false;
            }
            NodeSize = data.getMapScale();

            dataMap = data;
            m_bIsInit = true;
            return true;
        }

        public float getPathMapX(float nX)
        {
            //nX += PLAYER_POS_BASE / 2;
            return nX * m_nNodeSize / PLAYER_POS_BASE;
        }

        public float getPathMapZ(float nZ)
        {
            //nZ += PLAYER_POS_BASE / 2;
            return nZ * m_nNodeSize / PLAYER_POS_BASE;
        }
        public float getSceneMapX(float nX)
        {
            return nX * PLAYER_POS_BASE / m_nNodeSize;
        }

        public float getSceneMapZ(float nZ)
        {
            return nZ * PLAYER_POS_BASE / m_nNodeSize;
        }

        public ArrayList searchPath(float startX, float startZ, float endX, float endZ)
        {
            float nStartPtX = getPathMapX(startX);
            float nStartPtY = getPathMapZ(startZ);
            float nEndPtX = getPathMapX(endX);
            float nEndPtY = getPathMapZ(endZ);
            //开始寻路
            //path = new NavPath();
            //path.push(new Link(nEndPtX, nEndPtY, null));
            path = navMesh.findPath(nStartPtX, nStartPtY, nEndPtX, nEndPtY, true);
            if (null == path || path.length() <= 0)
            {
                return null;
            }
            //转换寻路结果为Pos点
            ArrayList ret = new ArrayList();
            for (int i = 0; i < path.length(); ++i)
            {
                Vector3 newpt = new Vector3(0, 0, 0);
                newpt.x = (getSceneMapX(path.getX(i)));
                newpt.z = (getSceneMapZ(path.getY(i)));
                Utility.GetTerrainY(newpt, out newpt);
                ret.Add(newpt);
            }
            return ret;
        }

        public ArrayList searchPath(Vector3 startPos, float endX, float endZ)
        {
            return searchPath(startPos.x, startPos.z, endX, endZ);
        }
        public bool isCanGo(float nX, float nZ)
        {
            float nMapX = getPathMapX(nX);
            float nMapZ = getPathMapZ(nZ);

            if (null == this.navMesh.findSpace(nMapX, nMapZ))
            {
                return false;
            }
            return true;
        }

    }
}
