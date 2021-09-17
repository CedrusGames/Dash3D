using System;
using System.Collections;
using System.Collections.Generic;
using Radix.Core;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Radix.UI.Leaderboard
{
    //4. user
    //40 tane ekle
    //UserH =85
    public class LeaderboardController : MonoBehaviour
    {
        public GameObject LeaderBoardUserPrefab;
        public Transform ScrollPanel;
        private int _numberofUser => GameManager.instance.NumberOfUser;

        private int _UserTotalScore => SaveLoadSystem.instance.TotalScore;
        private int _UserIngameScore => SaveLoadSystem.instance.IngameScore;
        private int _MaxScore => GameManager.instance.MaxScore;
        public AnimationCurve ScoreCurve;
        private List<LeaderBoardUser> _userspool = new List<LeaderBoardUser>();
        public GameObject UpdatingPanel;
        public Text ScoreText;


        private LeaderBoardUser oldUser;
        private LeaderBoardUser newUser;
        int OldUserRank = -1;
        int NewUserRank = -1;

        public void Build()
        {
            int[] scores = new int[_numberofUser];
            for (int i = 0; i < _numberofUser; i++)
            {
                scores[i] = (int) (ScoreCurve.Evaluate((float) i / _numberofUser) * _MaxScore);
            }

            Data.scores = scores;
            UpdatingPanel.SetActive(true);
            ScoreText.text = "";
        }

        public void Reveal()
        {
            if (oldUser)
                Destroy(oldUser.Placer.gameObject);
            if (newUser)
                Destroy(newUser.Placer.gameObject);


            gameObject.SetActive(true);
            int UsersNewScore = _UserIngameScore + _UserTotalScore;

            UpdatingPanel.SetActive(true);
            ScoreText.text = UsersNewScore.ToString();
            OldUserRank = -1;
            NewUserRank = -1;
            int dif;
            for (int i = 0; i < Data.scores.Length; i++)
            {
                if (_UserTotalScore >= Data.scores[i])
                {
                    OldUserRank = i;
                    break;
                }
            }

            for (int i = 0; i < Data.scores.Length; i++)
            {
                if (UsersNewScore >= Data.scores[i])
                {
                    NewUserRank = i;
                    break;
                }
            }

            dif = OldUserRank - NewUserRank;
            foreach (var user in _userspool)
            {
                if (user)
                    Destroy(user.gameObject);
            }

            _userspool.Clear();
            if (NewUserRank > 4)
                for (int i = 4; i > 0; i--)
                {
                    LeaderBoardUser user = Instantiate(LeaderBoardUserPrefab, ScrollPanel)
                        .GetComponent<LeaderBoardUser>();
                    user.Build(Data.scores[NewUserRank - i], NewUserRank - i, false);
                    _userspool.Add(user);
                }

            newUser = Instantiate(LeaderBoardUserPrefab, ScrollPanel).GetComponent<LeaderBoardUser>();
            newUser.Build(_UserTotalScore, NewUserRank, true);
            _userspool.Add(newUser);


            if (dif != 0)
            {
                int counter = NewUserRank;
                bool trimusers = dif > 100;
                int trimcounter = 0;
                while (true)
                {
                    counter++;
                    if (counter == OldUserRank)
                    {
                        oldUser = Instantiate(LeaderBoardUserPrefab, ScrollPanel)
                            .GetComponent<LeaderBoardUser>();
                        oldUser.Build(Data.scores[OldUserRank], OldUserRank, true);
                        _userspool.Add(oldUser);
                        for (int i = 0; i < 5; i++)
                        {
                            LeaderBoardUser _u = Instantiate(LeaderBoardUserPrefab, ScrollPanel)
                                .GetComponent<LeaderBoardUser>();
                            _u.Build(Data.scores[OldUserRank + i], OldUserRank + i, false);
                            _userspool.Add(_u);
                        }

                        StartCoroutine(SlideAnim());
                        break;
                    }

                    if (trimusers)
                    {
                        trimcounter++;
                        if (trimcounter > 49)
                        {
                            counter = OldUserRank - 49;
                            trimusers = false;
                        }
                    }

                    LeaderBoardUser user = Instantiate(LeaderBoardUserPrefab, ScrollPanel)
                        .GetComponent<LeaderBoardUser>();
                    user.Build(Data.scores[counter], counter, false);
                    _userspool.Add(user);
                }
            }
            else
            {
                oldUser = Instantiate(LeaderBoardUserPrefab, ScrollPanel)
                    .GetComponent<LeaderBoardUser>();
                oldUser.Build(Data.scores[OldUserRank], OldUserRank, true);
                _userspool.Add(oldUser);
                for (int i = 0; i < 10; i++)
                {
                    LeaderBoardUser _u = Instantiate(LeaderBoardUserPrefab, ScrollPanel)
                        .GetComponent<LeaderBoardUser>();
                    _u.Build(Data.scores[OldUserRank + i + 1], OldUserRank + i + 1, false);
                    _userspool.Add(_u);
                    UpdatingPanel.SetActive(false);
                }

                _userspool.Remove(oldUser);
                Destroy(oldUser.gameObject);
            }
        }


        /*
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                SaveLoadSystem.instance.IngameScore = 125;
                Reveal();
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                SaveLoadSystem.instance.IngameScore = 250;
                Reveal();
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                SaveLoadSystem.instance.IngameScore = 1850;
                Reveal();
            }

            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                SaveLoadSystem.instance.IngameScore = 5000;
                Reveal();
            }

            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                SaveLoadSystem.instance.IngameScore = GameManager.instance.MaxScore + 100;
                Reveal();
            }

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                Reveal();
            }
        }*/

        IEnumerator SlideAnim()
        {
            var wait = new WaitForEndOfFrame();
            UpdatingPanel.SetActive(true);
            yield return new WaitForSeconds(0.25f);

            //float prefabH = newUser.GetComponent<RectTransform>().rect.height;

            float localH = oldUser.transform.localPosition.y;
            ScrollPanel.transform.localPosition = new Vector3(0, -localH, 0);

            yield return new WaitForSeconds(0.5f);
            UpdatingPanel.SetActive(false);
            float timer = 0;

            oldUser.Placer.SetParent(transform);
            while (true)
            {
                timer += Time.deltaTime * 4;
                oldUser.Placer.transform.localScale = Vector3.one + (Vector3.one * (timer * 0.25f));
                if (timer > 1)
                {
                    break;
                }

                yield return wait;
            }


            Destroy(newUser.Placer.gameObject);
            newUser.Placer = oldUser.Placer;
            timer = 0;
            float TargetScore = _UserIngameScore + _UserTotalScore;
            float currentScore = _UserTotalScore;
            float ShowScore = 0;
            while (true)
            {
                timer += Time.deltaTime;
                ShowScore = Mathf.Lerp(currentScore, TargetScore, timer);
                oldUser.UpdateValues(ShowScore.ToString("0"), oldUser.ID.text);
                if (timer > 1)
                {
                    oldUser.UpdateValues(TargetScore.ToString("00"), oldUser.ID.text);
                    break;
                }

                yield return wait;
            }


            yield return new WaitForSeconds(0.5f);


            float LeaderRankH = ScrollPanel.transform.localPosition.y;
            float ShowRank = OldUserRank;
            while (true)
            {
                ShowRank = Mathf.Lerp(OldUserRank, NewUserRank,
                    1 - ScrollPanel.transform.localPosition.y / LeaderRankH);
                oldUser.UpdateValues(ShowScore.ToString("0"), "#" + ShowRank.ToString("0"));
                ScrollPanel.transform.localPosition = Vector3.Lerp(
                    ScrollPanel.transform.localPosition,
                    Vector3.zero,
                    Time.deltaTime
                );


                if (ScrollPanel.transform.localPosition.y <= 3f)
                {
                    ScrollPanel.transform.localPosition = Vector3.zero;
                    oldUser.UpdateValues(ShowScore.ToString("0"), "#" + NewUserRank.ToString("0"));
                    break;
                }

                yield return wait;
            }

            timer = 1;
            while (true)
            {
                timer -= Time.deltaTime * 4;
                oldUser.Placer.transform.localScale = Vector3.one + (Vector3.one * (timer * 0.25f));
                if (timer <= 0)
                {
                    oldUser.Placer.transform.localScale = Vector3.one;
                    break;
                }

                yield return wait;
            }

            newUser.Placer.SetParent(newUser.transform);
            RectTransform rt = newUser.Placer.GetComponent<RectTransform>();
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            Destroy(oldUser.gameObject);
        }
    }
}