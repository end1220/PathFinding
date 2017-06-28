using System.IO;
using System.Text;
using UnityEngine;

namespace GridNavPath
{

    public class MapData
    {

        private int[] _LimitFlag = new int[] { 0 };
        private int[,] _MapData = null;
        private int m_nMapScale = 0;
        private int m_nCX = 0;
        private int m_nCZ = 0;
        public bool loadMap(string strFileName)
        {

            try
            {
                var ta = Resources.Load(strFileName) as TextAsset;
                if (ta == null || ta.bytes.Length <= 0)
                {
                    return false;
                }

                int nOffSet = 0;
                int nFileType = System.BitConverter.ToInt32(ta.bytes,nOffSet);
                nOffSet += sizeof(int);
                int nVersion = System.BitConverter.ToInt32(ta.bytes,nOffSet);
                nOffSet += sizeof(int);
                m_nCX = System.BitConverter.ToInt32(ta.bytes,nOffSet)+1;
                nOffSet += sizeof(int);
                m_nCZ = System.BitConverter.ToInt32(ta.bytes,nOffSet)+1;
                nOffSet += sizeof(int);
                m_nMapScale = System.BitConverter.ToInt32(ta.bytes,nOffSet);
                nOffSet += sizeof(int);
                this._MapData = new int[m_nCX,m_nCZ];
                for (int i=0;i<m_nCX;++i) {
                    for (int j=0;j<m_nCZ;++j) {
                        this._MapData[i,j] = ta.bytes[nOffSet];
                        nOffSet += sizeof(byte);
                    }
                }

            }
            catch
            {
                return false;
            }
            return true;
        }
        public int[] getLimitFlag()
        {
            return this._LimitFlag;
        }
        public int[,] getMapData()
        {
            return this._MapData;
        }
        public int getMapScale()
        {
            return this.m_nMapScale;
        }
        public int getCX()
        {
            return m_nCX;
        }
        public void setCX(int nCX)
        {
            this.m_nCX = nCX;
        }
        public int getCZ()
        {
            return m_nCZ;
        }
        public void setCZ(int nCZ)
        {
            this.m_nCZ = nCZ;
        }

        public int getWidthInTiles()
        {
            return m_nCX;
        }

        public int getHeightInTiles()
        {
            return m_nCZ;
        }

        /**
         * 是否不可行走区域
         * @param tx
         * @param ty
         * @return
         */
        public bool blocked(int x, int y)
        {
            if (x < 0 || x >= m_nCX)
            {
                return false;
            }
            if (y < 0 || y >= m_nCZ)
            {
                return false;
            }
            return _MapData[x,y] == 0;
        }

        public float getCost(int tx, int ty)
        {
            return 1;
        }
    }
}