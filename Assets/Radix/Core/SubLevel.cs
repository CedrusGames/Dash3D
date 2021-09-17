using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radix.Core
{
    public class SubLevel : MonoBehaviour
    {
        public static SubLevel ActiveSubLevel;
        public SubSystemRadix[] SubSystems;
        
        private int SubSystemIndex = 0;
        public string title = "";
        public virtual void Awake()
        {
            ActiveSubLevel = this;
        }

        public virtual void StartSubLevel()
        {
            SubSystems[0].StartSubSystem();
        }

        public void GoNextSubSystem()
        {
            SubSystems[SubSystemIndex].OnStopSubSystem(); 
            SubSystemIndex++;
            if (SubSystemIndex >= SubSystems.Length)
            {
               // HideProgressBars();
                LevelManager.instance.GoNextSystem();
            }
            else
            {
                SubSystems[SubSystemIndex].StartSubSystem();
            }
        }

        public virtual void StopSubLevel()
        {
            
        } 
        protected virtual  void Start()
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
    }
}