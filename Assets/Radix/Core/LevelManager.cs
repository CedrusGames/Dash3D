using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radix.Core
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;
        public SubLevel[] Sublevels;
        [HideInInspector]
        private int CurrentSubLevelIndex ;
        public SubLevel CurrentSublevel => SubLevel.ActiveSubLevel;
        
        private void Awake()
        {
            instance = this;
            Sublevels[Sublevels.Length - 1].SubSystems[Sublevels[Sublevels.Length - 1].SubSystems.Length - 1]
                    .AmILastSubSystem =
                true;
        }

        private void Start()
        {
            CurrentSubLevelIndex = 0;
            Sublevels[0].StartSubLevel();
             
        }

        public void GoNextSystem()
        {
            CurrentSubLevelIndex++;
            if (CurrentSubLevelIndex == Sublevels.Length)
            {
                UIManager.instance.ShowEndLevel(false);
            }
            else
            {
                UIManager.instance.ShowGoNextSystemPanel();
            }
        }
    }
}