using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Radix.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public static Camera Cam;
        public LevelManager[] Levels;
        public LevelManager[] RandomLevels;
        //DebugMenuFloatValue
        [HideInInspector] public float camPosX;
        [HideInInspector] public float camPosY;
        [HideInInspector] public float camPosZ;
        [HideInInspector] public float camRotX;
        [HideInInspector] public float camRotY;
        [HideInInspector] public float camRotZ;
        [HideInInspector] public float playerSpeed;

        private void Start()
        {
            camPosX = .5f;
            camPosY = .5f;
            camPosZ = .5f;
            camRotX = .5f;
            camRotY = .5f;
            camRotZ = .5f;
            playerSpeed = .5f;
        }

        #region Settings

        [Space(10)]
        // [field: SerializeField] private bool CanMove { get; set; }
        [Header("Settings")]
        [Tooltip("Oyun bittiğinde yeni bölüme geçerken olacak anim")]
        public ScreenTransitionAnim GoToNextLevelAnim = ScreenTransitionAnim.none;
        [Tooltip("Bölüm sonunda bir sonraki bölüme geçmek için çıkan tuşun kaç saniye sonra çıkacağı")]
        public float ShowNextButtonDelay=1;
        public bool ShowGoToLevelUI = false;
        public bool ShowMoneyOnUI = false;


        [Space(10)]
        [Header("LeaderBoard")]
        public bool ShowLeaderboard = false;
        public int MaxScore = 35000; 
        public int NumberOfUser = 15000; 

        #endregion

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (ShowGoToLevelUI)
                if (GUI.Button(new Rect(10, 10, 150, 100), "Test tuşu"))
                {
                    print("Burada BölümSeç Olacak!");
                }
        }
#endif

        private void Awake()
        {
            instance = this;
            Cam = Camera.main;
            SaveLoadSystem.Load();
            if (RandomLevels.Length == 0)
            {
                RandomLevels = Levels;
            }
        }

        public void LoadNextLevel()
        {
            SaveLoadSystem.instance.LevelIndex++;
            SaveLoadSystem.instance.TotalScore += SaveLoadSystem.instance.IngameScore;
            SaveLoadSystem.instance.IngameScore = 0;
            if (SaveLoadSystem.instance.LevelIndex > Levels.Length - 1)
            {
                SaveLoadSystem.instance.RandomLevelIndex = Random.Range(0, RandomLevels.Length);
                LoadLevel();
            }
            else
            {
                LoadLevel();
            }
            SaveLoadSystem.Save();
        }

        public void LoadCurrentLevel()
        {
            LoadLevel();
        }

        public void LoadLevel()
        {
            if (LevelManager.instance)
            {
                DestroyImmediate(LevelManager.instance.gameObject);
            }

            if (SaveLoadSystem.instance.LevelIndex > Levels.Length - 1)
            {
                Instantiate(RandomLevels[SaveLoadSystem.instance.RandomLevelIndex]);
            }
            else
            {
                Instantiate(Levels[SaveLoadSystem.instance.LevelIndex]);    
            }

            UIManager.instance.LevelNumber.text = (SaveLoadSystem.instance.LevelIndex+1).ToString();
            UIManager.instance.InGamePanel.SetActive(true);

        }


        #region CameraMotion

        Vector3 camStartPos;
        Quaternion camStartRot;
        Vector3 CamTargetPos;
        Quaternion CamTargetRot;

        public delegate void EndOfCamMoveAnimDelegate();
        public delegate void EndOfAnimDelegate();

        public event EndOfCamMoveAnimDelegate EndOfCamMoveAnimEvent;
        public event EndOfCamMoveAnimDelegate EndOfAnimEvent;

        public void BlinkCam(Vector3 pos, Quaternion rot, ScreenTransitionAnim anim, float waitDelay = 1.0f)
        {
            StartCoroutine(BlinkCamAnim(pos, rot, anim, waitDelay));
        }

        IEnumerator BlinkCamAnim(Vector3 pos, Quaternion rot, ScreenTransitionAnim anim, float waitDelay)
        {
            playTransitionStartAnim(anim);

            yield return new WaitForSeconds(waitDelay);

            playTransitionEndAnim(anim);
            Cam.transform.position = pos;
            Cam.transform.rotation = rot;
            EndOfCamMoveAnimEvent?.Invoke();
        }

        public void PlayTransitionAnim(ScreenTransitionAnim anim,float delay)
        {
            StartCoroutine(PlayTransitionAnim_Routine(anim, delay));
        }

        IEnumerator PlayTransitionAnim_Routine(ScreenTransitionAnim anim,float delay)
        {
            playTransitionStartAnim(anim);
            yield return new WaitForSeconds(delay);
            playTransitionEndAnim(anim);
            if (EndOfAnimEvent != null)
                EndOfAnimEvent.Invoke();
        }
         

        void playTransitionStartAnim(ScreenTransitionAnim anim)
        {
            switch (anim)
            {
                case ScreenTransitionAnim.Fade:
                    UIManager.instance.PlayTransitionAnim("FadeIN");
                    break;
                case ScreenTransitionAnim.Slide:
                    UIManager.instance.PlayTransitionAnim("SlideIN");
                    break;
                default:
                    break;
            }
        }

        void playTransitionEndAnim(ScreenTransitionAnim anim)
        {
            switch (anim)
            {
                case ScreenTransitionAnim.Fade:
                    UIManager.instance.PlayTransitionAnim("FadeOut");
                    break;
                case ScreenTransitionAnim.Slide:
                    UIManager.instance.PlayTransitionAnim("SlideOUT");
                    break;
                default:
                    break;
            }
        }

        public void MoveCam(Vector3 Pos, Quaternion Rot, ScreenTransitionAnim anim, float speed = 1,
            float animdelay = 0)
        {
         
            camStartPos = Cam.transform.position;
            camStartRot = Cam.transform.rotation;
            CamTargetPos = Pos;
            CamTargetRot = Rot;
            playTransitionStartAnim(anim);
            StartCoroutine(MoveCamAnim(animdelay, anim, speed));
        }

        IEnumerator MoveCamAnim(float delay, ScreenTransitionAnim anim, float animSpeed)
        {
         
            yield return new WaitForSeconds(delay);
           
            float moveCamTimer = 0;
            bool endAnimPlayed = false;
            while (true)
            {
                moveCamTimer += Time.deltaTime * animSpeed;
                yield return new WaitForEndOfFrame();
                Cam.transform.position = Vector3.Lerp(camStartPos, CamTargetPos, moveCamTimer);
                Cam.transform.rotation = Quaternion.Lerp(camStartRot, CamTargetRot, moveCamTimer);
                if (moveCamTimer >= 0.5f)
                {
                    if (!endAnimPlayed && anim != ScreenTransitionAnim.none)
                    {
                        playTransitionEndAnim(anim);
                        endAnimPlayed = true;
                    }
                }

                if (moveCamTimer >= 1)
                {
                    Cam.transform.position = CamTargetPos;
                    Cam.transform.rotation = CamTargetRot;
                    if (EndOfCamMoveAnimEvent != null)
                        EndOfCamMoveAnimEvent.Invoke();
                    break;
                }
            }
        }

        #endregion
    }
}