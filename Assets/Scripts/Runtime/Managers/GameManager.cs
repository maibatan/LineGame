using LineGame.Runtime.Systems;
using LineGame.Runtime.Utilities;
using LineGame.Settings;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LineGame.Runtime.Managers
{
    public class GameManager : StaticInstance<GameManager>
    {
        public event Action OnChangeState;
        [SerializeField]
        protected Sprite[] _numbers;
        [SerializeField]
        protected Image[] _displayScore;
        [SerializeField]
        protected int _playerScore;
        [SerializeField]
        protected Image[] _ballQueue;
        [SerializeField]
        protected GameObject _gameOver;
        [SerializeField]
        protected GameState _gameState;
        public GameState GameState => _gameState;

        #region Methods
        protected void UpdateScore()
        {
            int score = Mathf.Clamp(_playerScore,0,(int)Mathf.Pow(10,_displayScore.Length)-1);
            for(int i = _displayScore.Length -1 ; i >= 0 ; i--)
            {
                if (score == 0) break;
                int number = score % 10;
                _displayScore[i].sprite = _numbers[number];
                score /= 10;
            }
        }
       
        public void StartGame()
        {
            ChangeState(GameState.GenerateBoard);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
        public void ChangeState(GameState newState)
        {
            _gameState = newState;
            OnChangeState?.Invoke();
            switch(newState)
            {
                case GameState.GenerateBoard:
                    BoardManager.Instance.GenerateBoard();
                    break;
                case GameState.PlayerTurn:
                    break;
                case GameState.CheckTurn:
                    BoardManager.Instance.MoveBall();
                    break;
                case GameState.SpawnTurn:
                    BoardManager.Instance.SpawnBall();
                    break;
                case GameState.GameOver:
                    HighScoreSystem.Instance.SaveScore(_playerScore);
                    _playerScore = 0;
                    SoundSystem.Instance.PlayGameOverSound();
                    _gameOver.SetActive(true);
                    break;
            }
        }
        public void AddScore(int score)
        {
            _playerScore += score;
            UpdateScore();
        }
        public void DisplayQueuedBall(Ball queueBall, int position)
        {
            if (position >= _ballQueue.Length) return;
            _ballQueue[position].color = queueBall.Category.Color;
            Image imageSpecial = _ballQueue[position].transform.GetChild(0).GetComponent<Image>();
            if (queueBall.SpecialIcon != null)
            {
                imageSpecial.sprite = queueBall.SpecialIcon;
                imageSpecial.color = new Color(1, 1, 1, 1);
            }
            else imageSpecial.color = new Color(1, 1, 1, 0);
        }
        #endregion
    }
    public enum GameState
    {
        GenerateBoard,
        PlayerTurn,
        CheckTurn,
        SpawnTurn,
        GameOver
    }
}
