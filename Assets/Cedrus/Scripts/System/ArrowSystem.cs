using System;
using Cedrus.Scripts._Core;
using Cedrus.Scripts.Player;
using UnityEngine;

namespace Cedrus.Scripts.System
{
    public class ArrowSystem : MonoBehaviour
    {
        public GameObject arrowBody;
        public GameObject arrowHead;
        public Transform arrowHeadPoint;
        
        Vector3 startPoint;
        Vector3 deltaPosition;
        public LineRenderer lineRenderer;
        public Ray ray;
        float rayLenght;
       

        public GameObject[] point = new GameObject[3];
        public RaycastHit hit;
        public RaycastHit hit2;

        public static ArrowSystem instance;
        private float screenWidth;
        [HideInInspector] public bool dash = false;
        private void Awake()
        {
            instance = this;
        }
    
        private void Start()
        {
            screenWidth = Screen.width;
            lineRenderer.enabled = false;
        }
        private void Update()
        {
            switch (Dash3DSubSystem.manager.CurrentGameState)
            {
                case Dash3DSubSystem.GameState.Prepare:
                    break;
                case Dash3DSubSystem.GameState.MainGame:
                    ArrowCreateSystem();
                    RaySystem();
                    LineRendererSystem();
                    RayLenghtCalculate();
                    break;
                case Dash3DSubSystem.GameState.FinishGame:
                    break;
                case Dash3DSubSystem.GameState.DebugMenu:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        void ArrowCreateSystem()
        {
            arrowBody.transform.localPosition = Vector3.zero;
            arrowHead.transform.position = arrowHeadPoint.position;
            arrowHead.transform.rotation = arrowHeadPoint.rotation;
            if (Input.GetMouseButtonDown(0))
            {
                
                arrowBody.SetActive(true);
                arrowHead.SetActive(true);
                point[0].transform.position = Dash3DSubSystem.manager.player.transform.position;
                point[1].transform.position = Dash3DSubSystem.manager.player.transform.position;
                point[2].transform.position =Dash3DSubSystem.manager.player.transform.position;

                transform.position = Dash3DSubSystem.manager.player.transform.position;
                startPoint = Input.mousePosition/screenWidth;
            }
            if (Input.GetMouseButton(0))
            {
                if (_length>1.5f)
                {
                    foreach (var item in point)
                    {
                        item.SetActive(true);
                    }
                    lineRenderer.enabled = true;
                    dash = true;
                }
                else
                {
                    foreach (var item in point)
                    {
                        item.SetActive(false);
                    }
                    dash = false;
                    lineRenderer.enabled = false;
                }

                transform.position = Dash3DSubSystem.manager.player.transform.position;
                deltaPosition = (Input.mousePosition/screenWidth - startPoint)*500f;
                
                arrowBody.transform.localScale = new Vector3(0.1f, 0.01f, Mathf.Clamp(+(deltaPosition.y > 0 ? -1 : 1 * deltaPosition.magnitude) / 50, 0, 4));
                arrowBody.transform.localRotation= Quaternion.Euler(0,Mathf.Clamp(deltaPosition.x*-0.25f,-85,85),0);
            }
            if (Input.GetMouseButtonUp(0))
            {
                arrowBody.SetActive(false);
                arrowHead.SetActive(false);
                lineRenderer.enabled = false;
                foreach (var item in point)
                {
                    item.SetActive(false);
                }
            }
        }

        private float _length;
        void RaySystem()
        {
            rayLenght = arrowBody.transform.localScale.z * 5;
            ray = new Ray(arrowBody.transform.position + new Vector3(0, 0.3f, 0), arrowBody.transform.forward);

            point[0].transform.position = ray.origin;

            if (Physics.Raycast(ray, out hit, rayLenght))
            {
                point[1].transform.position = hit.point - ray.direction / 2;

                Ray ray2 = new Ray(point[1].transform.position, Vector3.Reflect(arrowBody.transform.forward, hit.normal));

                if (Physics.Raycast(ray2, out hit2, (rayLenght - (hit.point - ray.origin).magnitude)))
                {
                    point[2].transform.position = hit2.point - ray2.direction / 2;
                }
                else
                {
                    point[2].transform.position = Vector3.Reflect(arrowBody.transform.forward, hit.normal) * (rayLenght - (hit.point - ray.origin).magnitude) + hit.point;
                }

            }
            else
            {

                point[1].transform.position = ray.direction * rayLenght + ray.origin;
                point[2].transform.position = point[1].transform.position;
            }

           

        }

        private void RayLenghtCalculate()
        {
            _length = Vector3.Distance(point[0].transform.position, point[1].transform.position) +
                      Vector3.Distance(point[1].transform.position, point[2].transform.position);
        }

        public void LineRendererSystem()
        {
            lineRenderer.SetPosition(0, point[0].transform.position);
            lineRenderer.SetPosition(1, point[1].transform.position);
            lineRenderer.SetPosition(2, point[2].transform.position);
        }
    }
}
