using System.Collections.Generic;
using UnityEngine;

namespace Cedrus.Scripts.Ostacles
{
     public class Cannon : MonoBehaviour
     {
          public GameObject CanonBallPrefab;
          public Transform CannonBallSpawnPoint;
          public Transform CannonBallTargetPos;
          public float BallSpawnPeriod=1;
          public float BallSpeed = 10;

          public float TimerOffset = 0;
          private float timer=0;

          private List<Transform> Balls = new List<Transform>();
          private List<float> BallTimes = new List<float>();

          private float moveTime = 10;
          private void Start()
          {
               moveTime = (CannonBallSpawnPoint.position - CannonBallTargetPos.position).magnitude/BallSpeed;
          }

          private void Update()
          {
               timer += Time.deltaTime;
               if (timer>=BallSpawnPeriod)
               {
                    timer = 0;
                    Transform ball =
                         Instantiate(CanonBallPrefab, CannonBallSpawnPoint.position, CannonBallSpawnPoint.rotation).transform;
                    ball.parent = transform.parent;
                    Balls.Add(ball);
                    BallTimes.Add(0);
               }



               bool DestroyFirst = false;
               for (int i = 0; i < Balls.Count; i++)
               {
                    BallTimes[i] += Time.deltaTime;
                    float tim = BallTimes[i] / moveTime;
                    Balls[i].position = Vector3.Lerp(CannonBallSpawnPoint.position, CannonBallTargetPos.position, tim);
                    if (tim >= 1)
                    {
                         DestroyFirst = true;
                    }
               }

               if (DestroyFirst)
               {
                    Destroy(Balls[0].gameObject);
                    Balls.RemoveAt(0);
                    BallTimes.RemoveAt(0);
               }
          }
     

          private void OnDrawGizmos()
          {
          
               Gizmos.color=new Color(0f, 1f, 0f, 0.46f);
               Gizmos.DrawLine(CannonBallSpawnPoint.position,CannonBallTargetPos.position);
         
               Gizmos.DrawSphere(CannonBallTargetPos.position,0.5f);
          
          
          }
     }
}
