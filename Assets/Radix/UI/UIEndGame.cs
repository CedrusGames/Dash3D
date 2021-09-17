using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radix.Core
{
    public class UIEndGame : MonoBehaviour
    {
        public GameObject FailPanel;
        public GameObject WinPanel;
        public GameObject NextLevelButton;
        public GameObject ReTryButton;

        private void Awake()
        {
            gameObject.SetActive(false);
            NextLevelButton.SetActive(false);
            ReTryButton.SetActive(false);
        }

        public void Reveal(bool fail)
        {
            gameObject.SetActive(true);
            if (fail)
            {
                FailPanel.SetActive(true);
                WinPanel.SetActive(false);
                ReTryButton.SetActive(true);
            }
            else
            {
                FailPanel.SetActive(false);
                WinPanel.SetActive(true);
                StartCoroutine(ShowNextButtondelay(GameManager.instance.ShowNextButtonDelay));
                if(GameManager.instance.ShowLeaderboard)
                    UIManager.instance.leaderboardController.Reveal();
            }

            UIEffectsManager.instance.PlayEndGameEffect(fail);
        }

        public void ReTrySameLevel()
        {
            ReTryButton.SetActive(false);
            gameObject.SetActive(false);
            UIEffectsManager.instance.DisableAllEffects();
            GameManager.instance.LoadCurrentLevel();
        }

        IEnumerator ShowNextButtondelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            NextLevelButton.gameObject.SetActive(true);
        }

        public void GoNextLevel()
        {
            NextLevelButton.SetActive(false);
            if (GameManager.instance.GoToNextLevelAnim != ScreenTransitionAnim.none)
            {
                GameManager.instance.EndOfAnimEvent += EndOfTransitionAnim;
                GameManager.instance.PlayTransitionAnim(GameManager.instance.GoToNextLevelAnim, 1);
            }
            else
            {
                nextLevel();
            }
        }

        private void EndOfTransitionAnim()
        {
            GameManager.instance.EndOfAnimEvent -= EndOfTransitionAnim;
            nextLevel();
        }

        private void nextLevel()
        {
            gameObject.SetActive(false);
            UIEffectsManager.instance.DisableAllEffects();
            GameManager.instance.LoadNextLevel();
        }
    }
}