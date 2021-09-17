using System;
using System.Collections;
using Cedrus.Scripts._Core;
using Cedrus.Scripts.System;
using Dreamteck.Splines;
using Radix.Core;
using UnityEngine;

namespace Cedrus.Scripts.Player
{
    public class PlayerScript : MonoBehaviour
    {
        public Transform signTransform;
        bool stop = false;
        public bool run = true;
        public Animator animator;
       
        public GameObject trailRenderer;
    
        void Start()
        {
            GameManager.Cam.fieldOfView = 60;
            animator = GetComponent<Animator>();
            IsGameOver(false);
            GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
            trailRenderer.SetActive(false);
        }
       
        void Update()
        {
            switch (Dash3DSubSystem.manager.CurrentGameState)
            {
                case Dash3DSubSystem.GameState.Prepare:
                    break;
                case Dash3DSubSystem.GameState.MainGame:
                    PlayerDash();
                    break;
                case Dash3DSubSystem.GameState.FinishGame:
                    break;
                case Dash3DSubSystem.GameState.DebugMenu:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PlayerDash()
        {
            if (run)
            {
                transform.position += signTransform.forward * (Time.deltaTime * 20*(UIManager.instance.playerSpeed.value+1f));//* ((UIManager.instance.playerSpeed.value-.5f)*10));
                transform.eulerAngles = signTransform.eulerAngles;
            }
            if (!ArrowSystem.instance.dash)
            {
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                stop = true;
            }
            if (Input.GetMouseButton(0))
            {
                //todo maybe
            }
            if (Input.GetMouseButtonUp(0))
            {
                stop = false;
                StartCoroutine(Move());
            }
        }

        public void IsGameOver(bool state)
        {
            animator.enabled = !state;
            foreach (var item in GetComponentsInChildren<Rigidbody>())
            {
                item.isKinematic = !state;
            }
            foreach (var item in GetComponentsInChildren<Collider>())
            {
                item.enabled = state;
            }
        }
        IEnumerator Move()
        {
            trailRenderer.SetActive(true);
            run = false;
            float speed = 90;
            int pointNumber = 1;
            Vector3 startPos = transform.position;
            float timer = 0;
            if (ArrowSystem.instance.point[pointNumber]!=null)
            {
                Vector3 targetPos = new Vector3(ArrowSystem.instance.point[pointNumber].transform.position.x,
                    transform.position.y, ArrowSystem.instance.point[pointNumber].transform.position.z);
                animator.Play("Teleport");
                float Lenght = (targetPos - startPos).magnitude;
                float TotalTime = Lenght / speed;
                while (true)
                {
                    if (stop)
                    {
                        break;
                    }
                    timer += Time.deltaTime;
                    float finalTimer = timer / TotalTime;
                    transform.position = Vector3.Lerp(startPos, targetPos, finalTimer);
                    transform.LookAt(targetPos);
                    if (finalTimer >= 1)
                    {
                        timer = 0;
                        finalTimer = 0;
                        startPos = transform.position;
                        pointNumber++;
                        if (pointNumber > 2)
                        {
                            animator.SetTrigger("Blend");
                            break;
                        }
                        if (ArrowSystem.instance.point[pointNumber] == null)
                        {
                            break;
                        }
                        targetPos = new Vector3(ArrowSystem.instance.point[pointNumber].transform.position.x,
                            transform.position.y, ArrowSystem.instance.point[pointNumber].transform.position.z);
                        Lenght = (targetPos - startPos).magnitude;
                        TotalTime = Lenght / speed;
                    }
                    yield return new WaitForEndOfFrame();
                }
                foreach (var item in ArrowSystem.instance.point)
                {
                    item.SetActive(false);
                }
                ArrowSystem.instance.lineRenderer.enabled = false;
                ArrowSystem.instance.point[0].transform.position = Vector3.zero;
                ArrowSystem.instance.point[1].transform.position = Vector3.zero;
                ArrowSystem.instance.point[2].transform.position = Vector3.zero;
                ArrowSystem.instance.LineRendererSystem();
                run = true;
                trailRenderer.SetActive(false);
                ArrowSystem.instance.dash = false;
            }
        }
    }
}