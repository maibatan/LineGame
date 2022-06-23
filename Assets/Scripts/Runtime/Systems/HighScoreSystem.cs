using LineGame.Runtime.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LineGame.Runtime.Systems
{
    public class HighScoreSystem : StaticInstance<HighScoreSystem>
    {
        [SerializeField]
        protected Text[] _scoreTexts;
        protected List<int> _highScores = new List<int>();
      

        #region Unity methods
        protected void Start()
        {
            for(int i = 0; i < 10; i++)
            {
              _highScores.Add(PlayerPrefs.GetInt(i.ToString(), 0));
            }
            UpdateScore();
        }


        #endregion
        protected void UpdateScore()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i >= _scoreTexts.Length) return;
                _scoreTexts[i].text = _highScores[i].ToString();
            }
        }
        public void SaveScore(int score)
        {

            for (int i = 0; i < 10; i++)
            {
                if (_highScores[i] < score)
                {
                    _highScores.Insert(i, score);
                    break;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                PlayerPrefs.SetInt(i.ToString(), _highScores[i]);
            }
            UpdateScore();
        }
    }
}
