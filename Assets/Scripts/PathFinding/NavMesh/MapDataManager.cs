
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.jdxk;
using com.jdxk.Configs;

namespace GridNavPath
{
    public class MapDataManager
    {
        private static Hashtable m_MapData = new Hashtable();
        
        public static MapData getMapData(int nID) {
            return (MapData)m_MapData[nID];
        }
        public static void InitMapData(int nSceneId) {
            if (!m_MapData.ContainsKey(nSceneId))
            {
                SceneBase_Tbl scene = ConfigPool.Instance.GetDataByKey(typeof(SceneBase_Tbl), nSceneId) as SceneBase_Tbl;
                if (null != scene)
                {
                    string fileName = "Mapdata/" + scene.sceneMapData;
                    MapData data = new MapData();
                    if (data.loadMap(fileName))
                    {
                        m_MapData.Add(scene.id, data);
                    }
                }
            }
        }
    }
}