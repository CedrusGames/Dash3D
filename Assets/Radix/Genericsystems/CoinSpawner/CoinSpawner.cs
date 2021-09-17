using System;
using System.Collections;
using System.Collections.Generic;
using Radix.Core;
using UnityEngine;

namespace Radix.GenericSystems
{
    public class CoinSpawner : MonoBehaviour
    {
        public static CoinSpawner instance;
        public GameObject CoinFXPrefab;
        public Transform CoinUITargetPos;
        private Camera cam;
        public GameObject CoinWorldFX;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            cam = Camera.main;
        }

        public void SpawnCoins(Vector3 WorldPos, int _count, int _overwriteCount = -1, bool UseWorldPos = true,
            bool useGravity = true)
        {
            Vector3 ScreenPos = Vector3.zero;
            if (UseWorldPos)
            {
                ScreenPos = GameManager.Cam.WorldToScreenPoint(WorldPos);
                ScreenPos.x = ScreenPos.x - Screen.width * 0.5f;
                ScreenPos.y = ScreenPos.y - Screen.height * 0.5f;
                Instantiate(CoinWorldFX, WorldPos, CoinWorldFX.transform.rotation);
            }

            int _amount = _count;

            int _lcount = _overwriteCount != -1 ? _overwriteCount : _count;
            for (int i = 0; i < _lcount; i++)
            {
                int AddAmount = 0;
                if (_amount != 0)
                {
                    AddAmount = 1;
                    _amount--;
                }


                GameObject Go = Instantiate(CoinFXPrefab, transform);
                Go.GetComponent<CoinFX>().Build(ScreenPos, CoinUITargetPos.localPosition, AddAmount, false, useGravity);
            }

            if (_amount != 0)
            {
                GameObject Go = Instantiate(CoinFXPrefab, transform);
                Go.GetComponent<CoinFX>().Build(ScreenPos, CoinUITargetPos.localPosition, _amount, false, useGravity);
            }
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnCoins(Vector3.zero, 10, -1, false);
            }
        }
#endif
    }
}