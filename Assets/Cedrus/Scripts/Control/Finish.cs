using Cedrus.Scripts._Core;
using Cedrus.Scripts.Player;
using Cedrus.Scripts.System;
using UnityEngine;

namespace Cedrus.Scripts.Control
{
    public class Finish : MonoBehaviour
    {
        public static Finish instance;
        private void Awake()
        {
            instance = this;
        }
        private void OnTriggerEnter(Collider other)
        {
            PlayerScript player = other.GetComponent<PlayerScript>();
            if (player)
            {
                if (!player.run)
                {
                    player.animator.applyRootMotion = true;
                    player.animator.Play("Dance");
                }
                else
                {
                    player.run = false; 
                    player.animator.applyRootMotion = true;
                    player.animator.Play("Dance");
                }
                Dash3DSubSystem.manager.loseGame = false;
                Dash3DSubSystem.manager.CurrentGameState = Dash3DSubSystem.GameState.FinishGame;
            }
        }
  
    }
}
