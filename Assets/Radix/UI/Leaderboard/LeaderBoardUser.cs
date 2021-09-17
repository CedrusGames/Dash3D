using UnityEngine;
using UnityEngine.UI;

namespace Radix.UI.Leaderboard
{
    public class LeaderBoardUser : MonoBehaviour
    {
        public Text ID;
        public Text Score;
        public Text Name;
        public Transform Placer;

        public void Build(int _score, int _id, bool User)
        {
            ID.text = "#" + _id.ToString();
            Score.text = _score.ToString();
            if (User)
                Name.text = "YOU";
            else
                Name.text = Data.Names[Random.Range(0, Data.Names.Length)];
        }

        public void UpdateValues(string _score, string _ID)
        {
            Score.text = _score;
            ID.text = _ID;
        }
    }
}