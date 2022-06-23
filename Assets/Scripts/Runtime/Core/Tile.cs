using LineGame.Runtime.Managers;
using LineGame.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LineGame.Runtime.Core
{
    public class Tile : MonoBehaviour
    {

        [SerializeField]
        protected Ball _ball;
        public Ball Ball
        {
            get { return _ball; }
            set 
            { 
                _ball = value;
                _ballVisual.SetBall(value);
            }
        }

        [SerializeField]
        protected Ball _queuedBall;
        public Ball QueuedBall
        {
            get { return _queuedBall; }
            set 
            { 
                _queuedBall = value;
                _ballVisual.SetQueuedBall(value);
            }
        }

        protected TileVisual _tileVisual;
        protected BallVisual _ballVisual;

        #region Unity Methods
        protected void Awake()
        {
            _tileVisual = GetComponentInChildren<TileVisual>();
            _ballVisual = GetComponentInChildren<BallVisual>();
        }
        protected void Start()
        {
            _tileVisual.Init((int)transform.position.x, (int)transform.position.y);
        }
       
        protected void OnMouseDown()
        {
            if (GameManager.Instance.GameState != GameState.PlayerTurn) return;
            if (BoardManager.Instance.SelectedFirstTile == null)
            {
                if (_ball == null) return;
                _tileVisual.MakeHighlight(true);
                BoardManager.Instance.SelectedFirstTile = this;
                return;
            }
            BoardManager.Instance.SelectedFirstTile._tileVisual.MakeHighlight(false);
            if(BoardManager.Instance.SelectedFirstTile == this)
            {
                BoardManager.Instance.SelectedFirstTile = null;
                return;
            }
            if (_ball != null || _queuedBall != null)
            {
                BoardManager.Instance.SelectedFirstTile = null;
                return;
            }
            BoardManager.Instance.SelectedSecondTile = this;
            GameManager.Instance.ChangeState(GameState.CheckTurn);
        }
        #endregion
        #region Methods
        
        public void AppearBall()
        {
            _ball = _queuedBall;
            _queuedBall = null;
            _ballVisual.ZoomIn();
        }
        public void ExplodeBall()
        {
            _ball = null;
            _ballVisual.Explode();
        }
        public IEnumerator MoveBall(List<Vector2> path)
        {
            yield return _ballVisual.MoveBall(path);
        }
        #endregion
    }
}
