using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

namespace Radix.GenericSystems
{
    public class RunnerController : MonoBehaviour
    {
        [Tooltip("Sağa sola kayacak model")]
        public Transform ModelRoot;

        [Header("Settings")]
        public bool ControlCamera = false;

        public float XClampRange = 5;
        public float forwardSpeed_ms = 3;
        public float SwipePower = 1;
        public float Gravity = -9.81f;
        public float SnapDistance = 1;
        public float RayDistance = 3;
        private float currentSpeed_ms = 0;

        private Transform camRoot;
        private Camera cam;
        private bool snapping = false;

        private void Start()
        {
            if (ControlCamera)
            {
                cam = Camera.main;
                camRoot = new GameObject("CamRoot").transform;
                camRoot.parent = transform.parent;
                camRoot.position = transform.position;
                camRoot.rotation = transform.rotation;
                cam.transform.parent = camRoot;
            }
        }

        private void OnDestroy()
        {
            if (camRoot)
            {
                if (cam.transform.parent == camRoot)
                {
                    cam.transform.parent = null;
                }

                Destroy(camRoot.gameObject);
            }
        }

        private void Update()
        {
            UpdateSideMovement();
            UpdateForwardMovement();
            GravityUpdate();
            if (ControlCamera)
            {
                Vector3 CamRootPos = transform.position;
                CamRootPos.y = ModelRoot.position.y;
                camRoot.transform.position = CamRootPos;
                Quaternion tarrot = ModelRoot.rotation;
                Vector3 _buffer = tarrot.eulerAngles;
                camRoot.transform.rotation = Quaternion.Euler(_buffer.x, 0, 0);
            }
        }

        private Vector3 OldPos = Vector3.zero;
        private float RotSampleTimer;


        void GravityUpdate()
        {
            if (Gravity == 0)
                return;


            RaycastHit hit;
            RotSampleTimer += Time.deltaTime;
            if (Physics.Raycast(ModelRoot.position + Vector3.up, Vector3.down, out hit, RayDistance))
            {
                Debug.Log(hit.distance);
                if (hit.distance < SnapDistance||snapping)
                {
                    Vector3 dir = ModelRoot.position - OldPos;
                    dir.x = 0;
                    Quaternion TargetRot = Quaternion.LookRotation(dir);
                    ModelRoot.rotation = Quaternion.Lerp(ModelRoot.rotation, TargetRot, Time.deltaTime * 2);
                    ModelRoot.position = hit.point;
                    snapping = true;
                }
            }
            else
            {
                ModelRoot.position += new Vector3(0, Time.deltaTime * Gravity, 0);
                snapping = false;
            }

            if (RotSampleTimer >= 0.1f)
            {
                OldPos = ModelRoot.position;
                RotSampleTimer = 0;
            }
        }

        private float StartInputX = 0;
        private float StartLocalPosX = 0;

        void UpdateSideMovement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartInputX = Input.mousePosition.x;
                StartLocalPosX = ModelRoot.localPosition.x;
            }

            if (Input.GetMouseButton(0))
            {
                float currentX = Input.mousePosition.x;
                float dif = currentX - StartInputX;
                dif = dif / Screen.width;
                dif = Mathf.Clamp(dif, -1, 1);
                Vector3 localPos = ModelRoot.localPosition;
                localPos.x = StartLocalPosX + dif * XClampRange * 2;
                if (localPos.x > XClampRange || localPos.x < -XClampRange)
                {
                    localPos.x = Mathf.Clamp(localPos.x, -XClampRange, XClampRange);
                    StartInputX = Input.mousePosition.x;
                    StartLocalPosX = ModelRoot.localPosition.x;
                }

                ModelRoot.localPosition = localPos;
            }
        }

        void UpdateForwardMovement()
        {
            currentSpeed_ms = forwardSpeed_ms;

            transform.position += new Vector3(0, 0f, currentSpeed_ms * Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.19f, 0.85f, 0.06f, 1);
            Vector3 startPos = transform.position + transform.right * -XClampRange;
            Vector3 endPos = transform.position + transform.right * XClampRange;
            Gizmos.DrawLine(startPos, endPos);
            Gizmos.DrawCube(startPos, Vector3.one * 0.1f);
            Gizmos.DrawCube(endPos, Vector3.one * 0.1f);
            Gizmos.color = new Color(1f, 0.06f, 0.1f, 0.75f);
            Gizmos.DrawCube(ModelRoot.position - new Vector3(0, SnapDistance - 1, 0), Vector3.one * 0.35f);
            Gizmos.DrawLine(
                ModelRoot.position+Vector3.up,
                ModelRoot.position+Vector3.down*RayDistance
                );
        }
    }
}