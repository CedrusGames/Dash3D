using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cedrus.Scripts.Ostacles
{
    public class ObstacleMover : MonoBehaviour
    {
        public Transform MovingObstacle;
        public Transform PointA;
        public Transform PointB;

        [Header("Birden fazla obje seç:"), Space(1), Header("Otomatik değiştirmek için.")]
        [SerializeField]
        public float STARTDELAY = 1;

        public float MotionTime = 1;
        public bool EffectRotation = true;
        public bool ShakeBeforeMove = true;
        public float ShakeTime = 0.25f;
        public float ShakeAmount = 0.1f;
        public float ShakeRot_Amount = 0.01f;
        public float Point_A_StayDelay = 3;
        public float Point_B_StayDelay = 3;

        private Vector3 Point_A_Pos;
        private Vector3 Point_B_Pos;
        private Quaternion Point_A_Rot;
        private Quaternion Point_B_Rot;

        private void Start()
        {
            Point_A_Pos = PointA.position;
            Point_B_Pos = PointB.position;
            Point_A_Rot = PointA.rotation;
            Point_B_Rot = PointB.rotation;
            Invoke(nameof(MoveAnim), STARTDELAY);
        }

        void MoveAnim()
        {
            StartCoroutine(MoveAnimation());
        }

        IEnumerator MoveAnimation()
        {
            var wait = new WaitForEndOfFrame();
            float timer = 0;
            float movetime = MotionTime;
            Vector3 shakeDir = (Point_B_Pos - Point_A_Pos).normalized * ShakeAmount;
            Quaternion shakeRot = Quaternion.Lerp(Point_A_Rot, Point_B_Rot, ShakeRot_Amount);

            //SHAKE
            if (ShakeBeforeMove)
            {
                while (true)
                {
                    timer += Time.deltaTime;
                    if (timer > Point_A_StayDelay - ShakeTime)
                    {
                        float shaker = (Mathf.Sin(timer * 100) + 1) * 0.5f;
                        MovingObstacle.transform.position = Vector3.Lerp(Point_A_Pos, Point_A_Pos + shakeDir, shaker);
                        if (EffectRotation)
                            MovingObstacle.transform.rotation = Quaternion.Lerp(Point_A_Rot, shakeRot, shaker);
                    }

                    if (timer > Point_A_StayDelay)
                    {
                        break;
                    }

                    yield return wait;
                }
            }
            else
            {
                yield return new WaitForSeconds(Point_A_StayDelay);
            }


            //PointA to PointB MOTION
            timer = 0;
            while (true)
            {
                timer += Time.deltaTime;
                float D_tim = timer / movetime;
                MovingObstacle.transform.position = Vector3.Lerp(Point_A_Pos, Point_B_Pos, D_tim);
                if (EffectRotation)
                    MovingObstacle.transform.rotation = Quaternion.Lerp(Point_A_Rot, Point_B_Rot, D_tim);
                if (timer > movetime)
                    break;
                yield return wait;
            }

            shakeRot = Quaternion.Lerp(Point_B_Rot, Point_A_Rot, ShakeRot_Amount);
            //SHAKE
            timer = 0;
            if (ShakeBeforeMove)
            {
                while (true)
                {
                    timer += Time.deltaTime;
                    if (timer > Point_B_StayDelay - ShakeTime)
                    {
                        float shaker = (Mathf.Sin(timer * 100) + 1) * 0.5f;
                        MovingObstacle.transform.position = Vector3.Lerp(Point_B_Pos, Point_B_Pos + shakeDir, shaker);
                        if (EffectRotation)
                            MovingObstacle.transform.rotation = Quaternion.Lerp(Point_B_Rot, shakeRot, shaker);
                    }

                    if (timer > Point_B_StayDelay)
                    {
                        break;
                    }

                    yield return wait;
                }
            }
            else
            {
                yield return new WaitForSeconds(Point_B_StayDelay);
            }

            //PointB To PointA Motion
            timer = 0;
            while (true)
            {
                timer += Time.deltaTime;
                float D_tim = timer / movetime;
                MovingObstacle.transform.position = Vector3.Lerp(Point_B_Pos, Point_A_Pos, D_tim);
                if (EffectRotation)
                    MovingObstacle.transform.rotation = Quaternion.Lerp(Point_B_Rot, Point_A_Rot, D_tim);
                if (timer > movetime)
                    break;
                yield return wait;
            }


            StartCoroutine(MoveAnimation());
        }

       private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;
            Gizmos.color = new Color(0f, 1f, 0f, 0.45f);
            Gizmos.DrawLine(PointA.position, PointB.position);
            Gizmos.DrawSphere(PointA.position, 0.1f);
            Gizmos.DrawSphere(PointB.position, 0.1f);
        }

        /*private void OnDrawGizmosSelected()
    {
        
        Handles.Label(transform.position + Vector3.up * 0.5f,"TEST");
    }

    public string SelectionNumber = "";*/
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ObstacleMover))]
    [CanEditMultipleObjects]
    public class ObstacleMoverEditor : Editor
    {
        private float _offsetAmount = 0;
        private float _startDelay = 0;

        public override void OnInspectorGUI()
        {
            ObstacleMover myTarget = (ObstacleMover) target;
            _startDelay = myTarget.STARTDELAY;
            var selections = Selection.objects;
            if (selections.Length > 1)
            {
                _offsetAmount = EditorGUILayout.FloatField("Offset Amount", _offsetAmount);
                if (GUILayout.Button("Başlangıç Gecikmesi Offset"))
                {
                    for (int i = 0; i < selections.Length; i++)
                    {
                        ObstacleMover om = ((GameObject) selections[i]).GetComponent<ObstacleMover>();

                        if (om)
                        {
                            om.STARTDELAY = _offsetAmount * i;
                            EditorUtility.SetDirty(om);
                        }
                    }
                }
            }

            // myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
            // EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
            DrawDefaultInspector();
        }

        private void OnSceneGUI()
        {
            var selections = Selection.objects;
            Handles.BeginGUI();
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            if (selections.Length > 2)
                for (int i = 0; i < selections.Length; i++)
                {
                    ObstacleMover om = ((GameObject) selections[i]).GetComponent<ObstacleMover>();

                    if (om)
                    {
                        Vector3 pos = om.transform.position + Vector3.up*1.5f  ;
                        Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);
                        GUI.Label(new Rect(pos2D.x, pos2D.y, 100, 100), i.ToString(), style);
                    }
                }

            Handles.EndGUI();
        }
    }
#endif
}
