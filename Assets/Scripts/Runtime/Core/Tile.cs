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
                _ballVisual.InitBall(value);
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
                _ballVisual.InitQueuedBall(value);
                GameManager.Instance.OnChangeState += DequeueBall;
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
            if (BoardManager.Instance.SelectedTile == null)
            {
                if (_ball == null) return;
                _tileVisual.MakeHighlight(true);
                BoardManager.Instance.SelectedTile = this;
                return;
            }
            Tile selectedTile = BoardManager.Instance.SelectedTile;
            BoardManager.Instance.SelectedTile._tileVisual.MakeHighlight(false);
            BoardManager.Instance.SelectedTile = null;
            if (_ball != null || _queuedBall != null)
            {
                return;
            }
            selectedTile.MoveBall(this);

        }
        #endregion
        #region Methods
        
        protected void DequeueBall()
        {
            if (GameManager.Instance.GameState != GameState.SpawnTurn) return;
            _ball = _queuedBall;
            _queuedBall = null;
            _ballVisual.ZoomIn();
            GameManager.Instance.OnChangeState -= DequeueBall;
        }
        protected void ExplodeBall()
        {
            GameManager.Instance.AddScore(_ball.Score);
            _ball = null;
            _ballVisual.Explode();
            BoardManager.Instance.Clear(this);
        }
        protected void CheckLine()
        {
            
            bool hasLine = false;
            List<Tile> horizontalLine = BoardManager.Instance.CheckHorizontalLine(this);
            List<Tile> verticalLine = BoardManager.Instance.CheckVerticalLine(this);
            List<Tile> subCrossLine = BoardManager.Instance.CheckSubCrossLine(this);
            List<Tile> mainCrossLine = BoardManager.Instance.CheckMainCrossLine(this);
            for (int i = 0; i < horizontalLine.Count; i++)
            {
                hasLine = true;
                horizontalLine[i].ExplodeBall();
            }
            for (int i = 0; i < verticalLine.Count; i++)
            {
                hasLine = true;
                verticalLine[i].ExplodeBall();
            }
            for (int i = 0; i < subCrossLine.Count; i++)
            {
                hasLine = true;
                subCrossLine[i].ExplodeBall();
            }
            for (int i = 0; i < mainCrossLine.Count; i++)
            {
                hasLine = true;
                mainCrossLine[i].ExplodeBall();
            }
            if (hasLine)
            {
                ExplodeBall();
            }
           
        }
        protected IEnumerator MoveBallRoutine(Tile endTile)
        {
            List<Vector2> path = Pathfinding.FindPath(this, endTile);
            if (path != null)
            {
                if (path.Count <= 3)
                {
                    
                    yield return _ballVisual.MoveBall(path);
                    endTile.Ball = _ball;
                    _ball = null;
                    endTile.CheckLine();
                    BoardManager.Instance.Clear(this);
                    BoardManager.Instance.Mark(endTile);
                    yield return new WaitForSeconds(0.75f);
                    GameManager.Instance.ChangeState(GameState.SpawnTurn);
                }

            }
        }
        public void MoveBall(Tile endTile)
        {
            StartCoroutine(MoveBallRoutine(endTile));
        }
        #endregion
    }
}
