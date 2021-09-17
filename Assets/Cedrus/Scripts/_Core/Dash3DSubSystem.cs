using System;
using System.Collections;
using Cedrus.Scripts.Control;
using Cedrus.Scripts.Player;
using Cedrus.Scripts.System;
using Radix.Core;
using UnityEngine;

namespace Cedrus.Scripts._Core
{
    public class Dash3DSubSystem : SubSystemRadix
    {
        public static Dash3DSubSystem manager;
        public PlayerScript player;
        
        [HideInInspector] public bool finishGame;
        public enum GameState
        {
            Prepare,
            MainGame,
            FinishGame,
            DebugMenu,
        }

        [Header("Camera")] 
        public Transform camPos;
        public Transform followCam;
        
        private bool _tapToStart;
        private GameState _currentGameState;
        [HideInInspector]public bool loseGame;
        public GameState CurrentGameState
        {
            get { return _currentGameState;}
            set
            {
                switch (value)
                {
                    case GameState.Prepare:
                        break;
                    case GameState.MainGame:
                        UIManager.instance.settingsButton.interactable = true;
                        UIManager.instance.camPosX.value=GameManager.instance.camPosX;
                        UIManager.instance.camPosY.value=GameManager.instance.camPosY;
                        UIManager.instance.camPosZ.value=GameManager.instance.camPosZ;
                        UIManager.instance.camRotX.value=GameManager.instance.camRotX;
                        UIManager.instance.camRotY.value=GameManager.instance.camRotY;
                        UIManager.instance.camRotZ.value=GameManager.instance.camRotZ;
                        UIManager.instance.playerSpeed.value=GameManager.instance.playerSpeed;
                        UIManager.instance.cameraButton.SetActive(true);
                        UIManager.instance.playerButton.SetActive(true);
                        ArrowSystem.instance.lineRenderer.gameObject.SetActive(true);
                        player.animator.SetTrigger("Start");
                        break;
                    case GameState.FinishGame:
                        UIManager.instance.settingsButton.interactable = false;
                        UIManager.instance.settingsMenu.SetActive(false);
                        UIManager.instance.cameraButton.SetActive(false);
                        UIManager.instance.playerButton.SetActive(false);
                        ArrowSystem.instance.lineRenderer.gameObject.SetActive(false);
                        if (loseGame)
                        {
                            player.IsGameOver(loseGame);
                        }
                        EndGame(loseGame);
                        break;
                    case GameState.DebugMenu:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                _currentGameState = value;
            }
        }

        private void DebugControl()
        {
            GameManager.instance.camPosX = UIManager.instance.camPosX.value;
            GameManager.instance.camPosY = UIManager.instance.camPosY.value;
            GameManager.instance.camPosZ = UIManager.instance.camPosZ.value;
            GameManager.instance.camRotX = UIManager.instance.camRotX.value;
            GameManager.instance.camRotY = UIManager.instance.camRotY.value;
            GameManager.instance.camRotZ = UIManager.instance.camRotZ.value;
            GameManager.instance.playerSpeed = UIManager.instance.playerSpeed.value;
        }

        protected override void Awake()
        {
            base.Awake();
            manager = this;
            UIManager.instance.setProgressBarStatus(0);
        }

        protected override void Start()
        {
            base.Start();
            ArrowSystem.instance.lineRenderer.gameObject.SetActive(false);
            StartCoroutine(TapToStartAnimation());
        }
        
        IEnumerator TapToStartAnimation()
        {
            UIManager.instance.tapToStart.SetActive(true);
            while (!_tapToStart)
            {
                float timer = 0f;
                while (!_tapToStart)
                {
                    timer += Time.deltaTime;
                    UIManager.instance.tapToStart.transform.localScale = Vector3.Lerp(new Vector3(2, 2, 1),new Vector3(1, 1, 1) , timer);
                    if (timer>=1f)
                    {
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
                timer = 0;
                while (!_tapToStart)
                {
                    timer += Time.deltaTime;
                    UIManager.instance.tapToStart.transform.localScale = Vector3.Lerp(new Vector3(1, 1, 1),new Vector3(2, 2, 1), timer);
                    if (timer>=1f)
                    {
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }

        }

        private Vector3 _followerCamStartPos;
        private Vector3 _followerCamStartEuler;
        
        private void StartCam()
        {
            GameManager.Cam.transform.SetParent(followCam);
            GameManager.Cam.transform.localPosition = Vector3.zero;
            GameManager.Cam.transform.localRotation=Quaternion.identity;
            Vector3 pos = camPos.transform.position;
            pos.z = player.transform.position.z;
            camPos.transform.position = pos;
            _followerCamStartPos = followCam.localPosition;
            _followerCamStartEuler = followCam.localEulerAngles;
        }

        private void TapToStart()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _tapToStart = false;
                UIManager.instance.tapToStart.SetActive(false);
                CurrentGameState = GameState.MainGame;
            }
        }

        protected override void Update()
        {
            base.Update();
            switch (CurrentGameState)
            {
                case GameState.Prepare:
                    StartCam();
                    TapToStart();
                    break;
                case GameState.MainGame:
                    CamFollow();
                    UiControl();
                    FollowerCamDebugControl(true);
                    break;
                case GameState.FinishGame:
                    FinishCam();
                    break;
                case GameState.DebugMenu:
                    FollowerCamDebugControl(false);
                    DebugControl();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
          
        }
        private void UiControl()
        {
            UIManager.instance.setProgressBarStatus((player.transform.position.z / Finish.instance.transform.position.z));
        }

        public void EndGame(bool loseGame)
        {
            StartCoroutine(FinishGame(loseGame));

        }
        IEnumerator FinishGame(bool loseGame)
        {
            GameManager.Cam.transform.SetParent(null);
            yield return new WaitForSeconds(2.5f);
            EndSubSystem(loseGame);
        }

        private void CamFollow()
        {
            Vector3 pos = camPos.transform.position;
            pos.z = Mathf.Lerp(pos.z, player.transform.position.z, Time.deltaTime * 8f);
            //pos= new Vector3(pos.x+(UIManager.instance.camPosX.value-.5f)*10,pos.y+(UIManager.instance.camPosY.value-.5f)*10,pos.z + (UIManager.instance.camPosZ.value - .5f) * 10);
            camPos.transform.position = pos;
        }

        private void FollowerCamDebugControl(bool target)
        {
            Vector3 pos = followCam.localPosition;
            Vector3 euler = followCam.localEulerAngles;
            euler.x = _followerCamStartEuler.x + (UIManager.instance.camRotX.value - .5f) * 10;
            pos.x = _followerCamStartPos.x + (UIManager.instance.camPosX.value - .5f) * 10;
            euler.y = _followerCamStartEuler.y + (UIManager.instance.camRotY.value - .5f) * 10;
            pos.y = _followerCamStartPos.y + (UIManager.instance.camPosY.value - .5f) * 10;
            euler.z = _followerCamStartEuler.z + (UIManager.instance.camRotZ.value - .5f) * 10;
            pos.z = _followerCamStartPos.z + (UIManager.instance.camPosZ.value - .5f) * 20;
            if (target)
            {
                followCam.localPosition =Vector3.Lerp(followCam.localPosition,pos,Time.deltaTime*3f);
                followCam.localEulerAngles =Vector3.Lerp(followCam.localEulerAngles,euler,Time.deltaTime*3f);
            }
            else
            {
                followCam.localPosition = pos;
                followCam.localEulerAngles = euler;
            }

        }

        private void FinishCam()
        {
            
            GameManager.Cam.fieldOfView = Mathf.Lerp(60, 40, Time.deltaTime/2f);
            followCam.transform.LookAt(Vector3.Lerp(
                followCam.transform.position, player.transform.position,
                Time.deltaTime));
        }

       


    }
}
