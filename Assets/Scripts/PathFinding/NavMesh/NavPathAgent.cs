using UnityEngine;
using com.jdxk;
using System.Collections;

namespace GridNavPath
{
    public class NavPathAgent
    {
        //�ƶ��ٶ�
        public float CurrentSpeed;
        //Ŀ��·��ڵ�
        public ArrayList targetPosition = null;
        //��ǰ�н����ڼ���·����
        public int curPosIndex = 0;
        //�Ƿ���Կ�ʼѰ·
        public bool enabled = false;
        //Ѱ·ֹͣ����
        public float radius = 0f;
        //Untiy�Լ���Ѱ·����������Ѱ·��
        protected UnityEngine.AI.NavMeshAgent uAgent = null;
        //�Ƿ�ʹ��Ѱ·��ʽѰ���м�ڵ�
        private bool m_bIsFindPath = false;
        //����Ѱ·ʱ��Ѱ·״̬
        private UnityEngine.AI.NavMeshPathStatus pathState = UnityEngine.AI.NavMeshPathStatus.PathInvalid;
        //��λ��
        private Vector3 m_NullPosition = new Vector3(0,0,0);
        private Vector3 m_startPosition = new Vector3();
        //��ǰ·���ϵ���һ���ڵ�
        protected Vector3 nextPosition = new Vector3();

        public bool IsFindPath{get{ return m_bIsFindPath;}}

        public Vector3 destination
        {
            get
            {
                if (m_bIsFindPath && uAgent != null)
                {
                    return uAgent.destination;
                }
                if (targetPosition != null && curPosIndex < targetPosition.Count)
                {
                    return (Vector3)targetPosition[curPosIndex];
                }
                else
                {
                    return m_NullPosition;
                }
            }

        }
        public UnityEngine.AI.NavMeshPathStatus pathStatus
        {
            get
            {
                if (m_bIsFindPath && uAgent != null)
                {
                    return uAgent.pathStatus;
                }
                if (targetPosition != null && curPosIndex < targetPosition.Count)
                {
                    return pathState;
                }
                else
                {
                    return UnityEngine.AI.NavMeshPathStatus.PathInvalid;
                }
            }

        }

        public NavPathAgent()
        {
            CurrentSpeed = ConstValue.Instance.PlayerDefaultSpeed;
        }

        public bool InitAgent(GameObject obj)
        {
            try
            {
                uAgent = obj.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (uAgent == null)
                    uAgent = obj.AddComponent<UnityEngine.AI.NavMeshAgent>();
                uAgent.speed = CurrentSpeed;
                uAgent.radius = float.Epsilon;
                uAgent.acceleration = 100;
                uAgent.angularSpeed = 0;

                return true;
            }
            catch (UnityException uex)
            {
                LogManager.Log(uex.ToString(), LogType.Fatal);
            }
            return false;
        }
        //ʣ��Ѱ·����
        public float GetRemainingDistance(Transform playerTrans)
        {
            //
            if (m_bIsFindPath && uAgent != null)
            {
                return uAgent.remainingDistance;
            }
            //
            if (targetPosition != null && curPosIndex < targetPosition.Count)
            {
                return MathUtility.CalcDistance2D((Vector3)targetPosition[curPosIndex], playerTrans.position);
            }
            return 0.0f;
        }

        public void Stop()
        {
            curPosIndex = 0;
            enabled = false;
            if(null != targetPosition)
            {
                pathState = UnityEngine.AI.NavMeshPathStatus.PathInvalid;
                targetPosition.Clear();
            }
            if (uAgent != null)
            {
                uAgent.Stop();
            }
        }

        public void startMove(Transform playerTrans, Vector3 point, bool bFindPath)
        {
            bFindPath = true;
            curPosIndex = 0;
            m_bIsFindPath = bFindPath;
            if (null != targetPosition)
            {
                targetPosition.Clear();
            }
            do
            {
                if (bFindPath)//ҪѰ·
                {
                    if (uAgent == null)
                    {
                        return;
                    }
                    uAgent.enabled = true;
                    uAgent.Resume();
                    uAgent.SetDestination(point);
                }
                else//����ҪѰ·��ֱ��Ŀ���
                {
                    if (null == targetPosition)
                    {
                        targetPosition = new ArrayList();
                    }
                    //�����ǰλ��
                    m_startPosition = playerTrans.position;
                    targetPosition.Add(point);
                    if ( targetPosition.Count > 0)
                    {
                        pathState = UnityEngine.AI.NavMeshPathStatus.PathPartial;
                    }
                }
                
            } while (false);
            //׼����ʼ��Ѱ·ϵͳ
            enabled = true;
        }

        public void MoveToTargetPos(Transform player)
        {
            if (m_bIsFindPath)
            {
                return;
            }
            Vector3 tarPos = new Vector3();
            if (targetPosition != null && curPosIndex < targetPosition.Count)
            {
                tarPos = (Vector3)targetPosition[curPosIndex];
            }
            if (tarPos.magnitude == 0)
            {
                return;
            }
            Vector3 curPos = player.position;
            float nMoveDist = CurrentSpeed * Time.deltaTime;
            float fTargetDist = MathUtility.CalcDistance2D(curPos, tarPos);
			
			Vector3 nextPos = new Vector3();
			if ( fTargetDist > 0.1f){
                //nextPos = curPos *( nMoveDist / fTargetDist);
                nextPos.x = curPos.x + nMoveDist * (tarPos.x - curPos.x) / fTargetDist;
                nextPos.z = curPos.z + nMoveDist * (tarPos.z - curPos.z) / fTargetDist;
                //nextPos.y = curPos.y;
			}
            nextPos.y = Utility.GetTerrainY(nextPos);
            player.transform.position = nextPos;
            //�ж��Ƿ����յ�
            float fStopRange = 0.1f;
            if (curPosIndex >= (targetPosition.Count-1))//���һ���ڵ�
            {
                fStopRange = this.radius;
            }
            if (MathUtility.CalcDistance2D(nextPos, tarPos) <= fStopRange)
            {
                curPosIndex += 1;
                m_startPosition = tarPos;
                if (curPosIndex >= targetPosition.Count)
                {
                    pathState = UnityEngine.AI.NavMeshPathStatus.PathComplete;
                    //���һ���ڵ��ˣ�ֹͣ�ƶ�
                    Stop();
                }
            }
        }

        public void Resume()
        {
            if (m_bIsFindPath && uAgent != null)
            {
                uAgent.Resume();
            }
        }

        //ˢ��һ��
        public void Refresh()
        {
            if (m_bIsFindPath && null != uAgent)
            {
                uAgent.enabled = false;
                uAgent.enabled = true;
            }
        }

        // �ر�NavMesh������NavMesh���ý�ɫ��λ�ã�������Ե��ͣ�£���
        // �ӵ�����������������������
        public void CloseAgent()
        {
            if (null != uAgent)
                uAgent.enabled = false;
        }

        //ʼ�տ���Ŀ���
        public void LookAtTargetImmediately(BaseObj player)
        {
            if (null == player)
            {
                return;
            }
            if (uAgent != null && m_bIsFindPath)
            {
                if (!Utility.IsSamePosition2D(nextPosition, uAgent.steeringTarget))
                {
                    Utility.LookAtTargetImmediately(player, uAgent.steeringTarget - player.GetPosition());
                    nextPosition = uAgent.steeringTarget;
                }
            }
            else
            {
                if (targetPosition != null && curPosIndex < targetPosition.Count)
                {
                    Utility.LookAtTargetImmediately(player, (Vector3)targetPosition[curPosIndex] - player.GetPosition());
                }
            }
        }

        //ʼ�տ���Ŀ���
        public void LookAtTargetSlerp(BaseObj player)
        {
            if (null == player)
            {
                return;
            }
            if (uAgent != null && m_bIsFindPath)
            {
                if (!Utility.IsSamePosition2D(nextPosition, uAgent.steeringTarget))
                {
                    Utility.LookAtTargetSlerp(player, uAgent.steeringTarget - player.GetPosition(), 10f);
                    nextPosition = uAgent.steeringTarget;
                }
            }
            else
            {
                if (targetPosition != null && curPosIndex < targetPosition.Count)
                {
                    Utility.LookAtTargetSlerp(player, (Vector3)targetPosition[curPosIndex] - player.GetPosition(), 10f);
                }
            }
        }

        public bool UpdateMove(Vector3 point)
        {
            if (m_bIsFindPath && null != uAgent)
            {
                uAgent.SetDestination(point);
                //uAgent.
                return true;
            }
            return false;
        }
        public bool ResetNextPosition()
        {
            if (null != uAgent)
            {
                nextPosition = new Vector3();
                return true;
            }
            return false;
        }
    }

}