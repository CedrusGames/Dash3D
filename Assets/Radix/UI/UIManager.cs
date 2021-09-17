using System;
using System.Collections;
using System.Collections.Generic;
using Cedrus.Scripts._Core;
using Radix.UI.Leaderboard;
using UnityEngine;
using UnityEngine.UI;

namespace Radix.Core
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        public GameObject NextSubLevelButton;
        public GameObject UIEffectManagerPrefab;
        public Text InGameCoinText;
        public Text LevelNumber;
        public GameObject InGameMoneyTextRoot;
        public LeaderboardController leaderboardController;
        [Header("Panels")] public GameObject MainMenuPanel;

        
        public GameObject InGamePanel;
        public GameObject EndGamePanel;
        private List<GameObject> panels = new List<GameObject>();
        public Animator transitionAnimator;
        public UIEndGame EndGameUI;
        [Header("ProgressBar")]
        public Image ProgressBarImage;
        public RectTransform ProgressBarIcon;
        [Header("TapToStart")] 
        public GameObject tapToStart;

        [Header("Setting Menu")] 
        public GameObject settingsMenu;
        public Button settingsButton;
        [Header("Debug Cam")] 
        public Button camDebugButton;
        public Scrollbar camPosX;
        public Scrollbar camPosY;
        public Scrollbar camPosZ;
        public Scrollbar camRotX;
        public Scrollbar camRotY;
        public Scrollbar camRotZ;
        public GameObject cameraButton;
        public GameObject cameraList;
        [Header("Debug Player")] 
        public Button playerDebugButton;
        public Scrollbar playerSpeed;
        public Text playerSpeedValueText;// (value -.5f)*10
        public GameObject playerButton;
        public GameObject playerList;
        [Header("Close Button")] 
        public GameObject closeButton;

        public void SettingsButton()
        {
            settingsMenu.SetActive(true);
            Dash3DSubSystem.manager.CurrentGameState = Dash3DSubSystem.GameState.DebugMenu;
            settingsButton.interactable = false;
            Time.timeScale = 0;
        }

        public void DebugButton(string name)
        {
            closeButton.SetActive(true);
            if (name=="camera")
            {
                playerButton.SetActive(false);
                cameraList.SetActive(true);
                camDebugButton.interactable = false;
            }
            else
            {
                cameraButton.SetActive(false);
                playerList.SetActive(true);
                playerDebugButton.interactable = false;
            }
        }
        public void CloseButton()
        {
            Time.timeScale = 1;
            settingsMenu.SetActive(false);
            closeButton.SetActive(false);
            if (cameraButton.activeSelf)
            {
                cameraList.SetActive(false);
                playerButton.SetActive(true);
            }
            else
            {
                playerList.SetActive(false);
                cameraButton.SetActive(true);
            }
            playerDebugButton.interactable = true;
            camDebugButton.interactable = true;
            settingsButton.interactable = true;
            Dash3DSubSystem.manager.CurrentGameState = Dash3DSubSystem.GameState.MainGame;
        }

        public void setProgressBarStatus(float val)
        {
            ProgressBarImage.fillAmount = val;
            ProgressBarIcon.anchoredPosition = new Vector2(val * 256, 0);
        }
        
        private void Awake()
        {
            instance = this;
            panels.Add(MainMenuPanel);
            panels.Add(InGamePanel);
            panels.Add(EndGameUI.gameObject);
            NextSubLevelButton.SetActive(false);
            transitionAnimator.gameObject.SetActive(true);
            Instantiate(UIEffectManagerPrefab, new Vector3(0, -5000, 0), Quaternion.identity);
        }

        private void Start()
        {
            settingsButton.interactable = false;
            cameraButton.SetActive(false);
            playerButton.SetActive(false);
            closeButton.SetActive(false);
            cameraList.SetActive(false);
            playerList.SetActive(false);
            settingsMenu.SetActive(false);
            UpdateMoneyText();
            InGameMoneyTextRoot.SetActive(GameManager.instance.ShowMoneyOnUI);
            CloseAllPanels();
            MainMenuPanel.SetActive(true);
            leaderboardController.Build();
            leaderboardController.gameObject.SetActive(false);
        }

        void CloseAllPanels()
        {
            foreach (var panel in panels)
            {
                panel.SetActive(false);
            }
        }

        public void StartGame()
        {
            CloseAllPanels();
            GameManager.instance.LoadCurrentLevel();
            InGamePanel.SetActive(true);
        }


        public void GoNextSysten()
        {
            NextSubLevelButton.SetActive(false);
        }

        public void UpdateMoneyText()
        {
            InGameCoinText.text = SaveLoadSystem.instance.CurrentPlayerMoney.ToString();
        }

        public void ShowGoNextSystemPanel()
        {
            NextSubLevelButton.SetActive(true);
        }

        public void ShowEndLevel(bool fail)
        {
            CloseAllPanels();
            EndGameUI.Reveal(fail);
        }

        public void PlayTransitionAnim(string anim)
        {
            transitionAnimator.SetTrigger(anim);
        }
    }
}