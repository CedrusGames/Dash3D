using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radix.Core
{
    public class SubSystemRadix : MonoBehaviour
    {
        public static SubSystemRadix ActiveSubSystem;

        [HideInInspector]
        public bool AmILastSubSystem = false;

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }

        protected virtual void LateUpdate()
        {
        }

        protected virtual void FixedUpdate()
        {
        }

        public virtual void StartSubSystem()
        {
            
        }
 
        public virtual void OnStopSubSystem()
        {
            enabled = false;
        }

        protected void EndSubSystem(bool fail)
        {
            if (fail)
            {
                UIManager.instance.ShowEndLevel(true);
                OnStopSubSystem();
            }
            else
            {
                SubLevel.ActiveSubLevel.GoNextSubSystem();
            }
           
        }
    }
}