using UnityEngine;

namespace Cedrus.Scripts.Ostacles
{
    public class Rotator : MonoBehaviour
    {
        public Vector3 LocalRotateSpeed=Vector3.forward;


        private void Update()
        {
            transform.Rotate(LocalRotateSpeed,Space.Self);
        }
    }
}
