using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace Radix.Core
{
    public class UIEffectsManager : MonoBehaviour
    {
        public static UIEffectsManager instance;
        public GameObject EndGameWinEffect;
        public GameObject EndGameFailEffect;
        public Camera MyCam;

        private void Awake()
        {
            instance = this;
            DisableAllEffects();
        }

        private void Start()
        {
            var dt = GameManager.Cam.GetUniversalAdditionalCameraData();
            dt.cameraStack.Add(MyCam);
            MyCam.gameObject.SetActive(false);
        }

        public void PlayEndGameEffect(bool fail )
        {
            MyCam.gameObject.SetActive(true);
            if (fail)
            {
                EndGameFailEffect.SetActive(true);
            }
            else
            {
                EndGameWinEffect.SetActive(true);
            }
        }

        public void DisableAllEffects()
        {
            EndGameWinEffect.SetActive(false);
            EndGameFailEffect.SetActive(false);
            MyCam.gameObject.SetActive(false);
        }
    }
}