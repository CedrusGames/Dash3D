using Cedrus.Scripts._Core;
using Cedrus.Scripts.Player;
using UnityEngine;

namespace Cedrus.Scripts.Ostacles
{
    public class Obstacle : MonoBehaviour
    {
        public enum ObstacleType
        {
            UpDownWall,
            Spinner,
            Pendulum,
            H_Pole,
            Gate,
            CogWheel,
            CannonBall,
            StaticObject,
            Hummer
        }
        public ObstacleType obstacleType;
        public GameObject moveObject;
        public bool movement = true;
        public Transform[] point = new Transform[2];


        [Range(-1, 1)]
        public float speed;
        float timer = 0;
        int pointIndex = 0;
        int currentIndex = 1;
        bool once = false;
        bool once2 = false;
        Vector3 pendulumCache;
        bool open = false;
        void ObstacleMovement()
        {
            if (speed == 0)
            {
                speed = 0.01f;
            }

            if (movement)
            {
                switch (obstacleType)
                {
                    case ObstacleType.UpDownWall:

                        MoveTransform();
                        break;
                    case ObstacleType.Spinner:
                        moveObject.transform.Rotate(0, 1 * speed, 0);
                        break;
                    case ObstacleType.Pendulum:
                        if (!once)
                        {
                            pendulumCache = transform.localEulerAngles;
                            once = true;
                        }

                        if (!once2)
                        {
                            transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * 50, 120 - pendulumCache.z));
                            if (transform.localEulerAngles.z >= 120)
                            {
                                once2 = true;
                            }
                        }
                        else
                        {
                            transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * 50, +120));
                        }

                        break;
                    case ObstacleType.H_Pole:

                        break;
                    case ObstacleType.Gate:

                        if (open)
                        {
                            timer += Time.deltaTime * 2;

                            GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, -Mathf.Lerp(0, 90, timer), 0);
                            GetComponentsInChildren<Transform>()[2].eulerAngles = new Vector3(0, Mathf.Lerp(0, 90, timer), 0);
                            if (timer >= 1)
                            {
                                timer = 0;
                                open = false;
                            }
                        }
                        break;


                    case ObstacleType.CogWheel:
                        moveObject.transform.Rotate(0, 0, 1 * (speed + 0.5f * speed / Mathf.Abs(speed)));
                        break;
                    case ObstacleType.CannonBall:
                        timer += Time.deltaTime * (speed + 0.5f * speed / Mathf.Abs(speed));

                        if (timer > 1)
                        {
                            timer = 0;

                            moveObject.transform.position = point[0].transform.position;


                        }

                        moveObject.transform.position = Vector3.Lerp(point[0].transform.position, point[1].transform.position, timer);


                        break;
                    case ObstacleType.StaticObject:

                        break;
                    case ObstacleType.Hummer:
                        MoveTransform();
                        break;

                }
            }
        }
        void MoveTransform()
        {
            timer += Time.deltaTime * speed;

            if (timer > 1)
            {
                timer = 0;

                pointIndex++;
                currentIndex--;
                if (pointIndex == 2)
                {
                    pointIndex = 0;
                    currentIndex = 1;
                }
            }
            moveObject.transform.position = Vector3.Lerp(point[currentIndex].transform.position, point[pointIndex].transform.position, timer);


        }
        private void Update()
        {
            ObstacleMovement();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerScript>())
            {
                open = true;
            }
        }
        private void OnCollisionEnter(Collision other)
        {
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            if (player)
            {
            
                foreach (var item in player.GetComponentsInChildren<Rigidbody>())
                {
                    item.AddExplosionForce(5, this.gameObject.transform.position, 2, 1, ForceMode.Acceleration);
                }

                Dash3DSubSystem.manager.loseGame = true;
                Dash3DSubSystem.manager.CurrentGameState = Dash3DSubSystem.GameState.FinishGame;
            }
        }
    }
}
