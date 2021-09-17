using System;
using Cedrus.Scripts._Core;
using Cedrus.Scripts.Player;
using Radix.GenericSystems;
using UnityEngine;

namespace Cedrus.Scripts.Collectable
{
    public class Coin : MonoBehaviour
    {
   
        void Update()
        {
            switch (Dash3DSubSystem.manager.CurrentGameState)
            {
                case Dash3DSubSystem.GameState.Prepare:
                    break;
                case Dash3DSubSystem.GameState.MainGame:
                    transform.Rotate(0,5,0);
                    break;
                case Dash3DSubSystem.GameState.FinishGame:
                    break;
                case Dash3DSubSystem.GameState.DebugMenu:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.gameObject.GetComponent<PlayerScript>();
            if (player)
            {
                CoinSpawner.instance.SpawnCoins(transform.position,1,-1,true,false);
            
                Destroy(gameObject);
            }
        }
    }
}
