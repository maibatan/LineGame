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
        protected GameState _gameState;
        public GameState GameState => _gameState;

        #region Unity Methods
        protected void Start()
        {
            ChangeState(GameState.GenerateBoard);
        }
        #endregion
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
        protected void UpdateBallQueue()
        {
            for(int i =0; i < _ballQueue.Length; i++)
            {
                Ball ball = BoardManager.Instance.TileHasQueueBall[i].QueuedBall;
                _ballQueue[i].color = ball.Category.Color;
                Image imageSpecial = _ballQueue[i].transform.GetChild(0).GetComponent<Image>();
                if(ball.SpecialIcon != null)
                {
                    imageSpecial.sprite = ball.SpecialIcon;
                    imageSpecial.color = new Color(1,1,1,1);
                }
                else imageSpecial.color = new Color(1, 1, 1, 0);
            }
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
                case GameState.SpawnTurn:
                    BoardManager.Instance.SpawnQueuedBall();
                    UpdateBallQueue();
                    break;
                case GameState.GameOver:
                    SoundSystem.Instance.PlayGameOverSound();
                    break;
            }
        }
        public void AddScore(int score)
        {
            _playerScore += score;
            UpdateScore();
        }
        #endregion
    }
    public enum GameState
    {
        GenerateBoard,
        PlayerTurn,
        SpawnTurn,
        GameOver
    }
}
