using Cedrus.Scripts._Core;
using Cedrus.Scripts.Player;
using UnityEngine;

namespace Cedrus.Scripts.Ostacles
{
    public class PlayerKiller : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            if (player)
            {
                foreach (var item in player.GetComponentsInChildren<Rigidbody>())
                {
                    item.AddExplosionForce(5, this.gameObject.transform.position, 2, 1, ForceMode.Acceleration);
                }
                Dash3DSubSystem.manager.loseGame=true;
                Dash3DSubSystem.manager.CurrentGameState = Dash3DSubSystem.GameState.FinishGame;
            }
        }
    }
}
