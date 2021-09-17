using System.Collections;
using System.Collections.Generic;
using Radix.Core;
using UnityEngine;

namespace Radix.GenericSystems
{
    public class CameraMotionSubSystem : SubSystemRadix
    {
         [Header("Motion")] public Transform moveTargetTransform;
        public ScreenTransitionAnim targetAnimationType=ScreenTransitionAnim.Slide;
        public ScreenTransitionAnimMove TargetMoveType=ScreenTransitionAnimMove.Move;
        [Tooltip("Kamera hareket hızı")]
        public float CamMoveSpeed = 1;
        [Tooltip("Hareket esnasında animasyonu bekletmek için kullanılan gecikme")]
        public float animationDelay = 0;
        [Tooltip("Kamera hareket etmeden önceki gecikme (s)")]
        public float StartDelay = 0;
        public float delayForNextSub = 0;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void StartSubSystem()
        {
            base.StartSubSystem();
            StartCoroutine(Delay());
        }

        public override void OnStopSubSystem()
        {
            base.OnStopSubSystem();
        }

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(StartDelay); 
            MoveCameraToTarget();
        }

        public void MoveCameraToTarget()
        {
            if (TargetMoveType == ScreenTransitionAnimMove.none)
            {
              EndSubSystem(false);
                return;
            }

            GameManager.instance.EndOfCamMoveAnimEvent += OnCameraReachedToTarget;

            if (TargetMoveType == ScreenTransitionAnimMove.Move)
                GameManager.instance.MoveCam(moveTargetTransform.position, moveTargetTransform.rotation,
                    targetAnimationType, CamMoveSpeed,animationDelay);
            if (TargetMoveType == ScreenTransitionAnimMove.Blink)
                GameManager.instance.BlinkCam(moveTargetTransform.position, moveTargetTransform.rotation,
                    targetAnimationType, animationDelay);
        }

        private void OnCameraReachedToTarget()
        {
            StartCoroutine(nameof(CameraReachedRoutine));
        }

        IEnumerator CameraReachedRoutine()
        {
            yield return new WaitForSeconds(delayForNextSub);
            EndSubSystem(false);
            GameManager.instance.EndOfCamMoveAnimEvent -= OnCameraReachedToTarget;
        }

    }
}