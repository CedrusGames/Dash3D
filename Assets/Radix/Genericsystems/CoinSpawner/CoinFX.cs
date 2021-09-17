using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Radix.GenericSystems
{
    public class CoinFX : MonoBehaviour
    {
        public int AddAmount = 0;
        Vector3 targetPos;
        Vector3 startpos;
        private Vector2 Vel;
        float animTimer = 0;

        public void Build(Vector3 pos, Vector3 target, int addAmount, bool spread, bool useGravity = true)
        {
            AddAmount = addAmount;
            targetPos = target;
            startpos = pos;
            Vel = Vector3.zero;
            if (spread)
            {
                Vel.x += Random.Range(-2.0f, 2.0f);
                Vel.y += Random.Range(-2.0f, 2.0f);
            }
            else
            {
                Vel.x += Random.Range(-1.0f, 1.0f);
                Vel.y += Random.Range(-1.0f, 1.0f);
            }


            Vel *= 2;
            transform.localPosition = pos;
            delay = Random.Range(0.5f, 1.5f);
            if (!useGravity)
            {
                delay = 0;
                Vel = Vector3.zero;
            }
        }

        float delay = 0;
        private bool startPosSet = false;

        private void Update()
        {
            delay -= Time.deltaTime;
            if (delay > 0)
            {
                Vel += new Vector2(0, -Time.deltaTime * 5.5f);
                transform.localPosition += new Vector3(Vel.x, Vel.y, 0);
                return;
            }

            if (!startPosSet)
            {
                startpos = transform.localPosition;
                startPosSet = true;
            }

            animTimer += Time.deltaTime * 2;

            transform.localPosition = Vector3.Lerp(startpos, targetPos, animTimer);
            // transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, animTimer);
            if (animTimer > 0.85f)
            {
                transform.localScale = Mathf.Lerp(1, 0, (animTimer - 0.85f) / 0.15f) * Vector3.one;
            }


            if (animTimer > 1)
            {
                SaveLoadSystem.instance.CurrentPlayerMoney += AddAmount;
                Destroy(gameObject);
            }
        }
    }
}