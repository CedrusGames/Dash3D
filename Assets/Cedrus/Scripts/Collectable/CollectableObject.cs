using Cedrus.Scripts.Player;
using Radix.GenericSystems;
using UnityEngine;

namespace Cedrus.Scripts.Collectable
{
    public class CollectableObject : MonoBehaviour
    {
        private void Update()
        {
            transform.eulerAngles += new Vector3(0, 1, 0)*Time.deltaTime;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerScript>())
            {
                CoinSpawner.instance.SpawnCoins(transform.position, 1, 1, true, false);
                Destroy(this.gameObject);
            }
        }
    }
}
