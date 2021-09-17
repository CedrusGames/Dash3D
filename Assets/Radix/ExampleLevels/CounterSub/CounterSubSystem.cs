using System.Collections;
using System.Collections.Generic;
using Radix.Core;
using UnityEngine;

namespace Radix.Example
{
    public class CounterSubSystem : SubSystemRadix
    {
        public int counter = 0; 
        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.S))
                GameManager.Cam.transform.position -= GameManager.Cam.transform.forward;
                
            if (Input.GetMouseButtonDown(1))
            {
               EndSubSystem(true);
            }
            if (Input.GetMouseButtonDown(0))
            {
                counter++;
              //  Progress = counter / 5.0f;
                if (counter==5)
                {
                    EndSubSystem(false);
                }
            }
        }
    }
}