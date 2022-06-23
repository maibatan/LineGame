using LineGame.Runtime.Core;
using LineGame.Runtime.Systems;
using LineGame.Runtime.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LineGame.Runtime.Managers
{
    public class BoardManager : StaticInstance<BoardManager>
    {

        [SerializeField]
        protected int _size;
        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }
        [SerializeField]
        protected float _sizeOfTile;
        [SerializeField]
        protected int _queuedBallSize;
        [SerializeField]
        protected Tile _tilePrefab;
        [SerializeField]
        protected Tile _selectedFirstTile;
        public Tile SelectedFirstTile
        {
            get { return _selectedFirstTile; }
            set { _selectedFirstTile = value; }
        }
        [SerializeField]
        protected Tile _selectedSecondTile;
        public Tile SelectedSecondTile
        {
            get { return _selectedSecondTile; }
            set { _selectedSecondTile = value; }
        }

        protected Dictionary<Vector2, Tile> _tiles = new Dictionary<Vector2, Tile>();
        protected List<Tile> _tilesNoBall = new List<Tile>();
        protected Queue<Tile> _queueBall = new Queue<Tile>();
       
        
        public void GenerateBoard()
        {
            for(int x =0; x < _size; x++)
            {
                for(int y = 0; y < _size; y++)
                {
                    var tile = Instantiate(_tilePrefab,new Vector3(x,y),Quaternion.identity,transform);
                    tile.name = $"Tile {x} {y}";
                    _tiles.Add(new Vector2(x, y), tile);
                    _tilesNoBall.Add(tile);
                }
            }
            for(int i = 0; i < _size; i++)
            {
                if (_tilesNoBall.Count <= 0) return;
                int randomNumber = Random.Range(0, _tilesNoBall.Count);
                var tile = _tilesNoBall[randomNumber];
                tile.Ball = BallResourceSystem.Instance.GetNormalRandom();
                _tilesNoBall.RemoveAt(randomNumber);
            }
            Camera.main.transform.position = new Vector3((float) _size/2 -0.5f, _size * _sizeOfTile - 0.5f, Camera.main.transform.position.z);
            Camera.main.orthographicSize = _size*_sizeOfTile;
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
        }
        public void SpawnBall()
        {
            while(_queueBall.Any())
            {
                _queueBall.Dequeue().AppearBall();
            }
            for (int i = 0; i < _queuedBallSize; i++)
            {
                if (_tilesNoBall.Count <= 0) break;
                int randomNumber = Random.Range(0, _tilesNoBall.Count);
                var tile = _tilesNoBall[randomNumber];
                tile.QueuedBall = Random.Range(0, 5) != 0 ? BallResourceSystem.Instance.GetNormalRandom() : BallResourceSystem.Instance.GetGhostlRandom();
                _tilesNoBall.RemoveAt(randomNumber);
                _queueBall.Enqueue(tile);
                GameManager.Instance.DisplayQueuedBall(tile.QueuedBall,i);
            }
            if (_tilesNoBall.Count <= 0)
            {
                GameManager.Instance.ChangeState(GameState.GameOver);
                return;
            }
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
        }
        public void MoveBall()
        {
         
            StartCoroutine(MoveBallRoutine());
        }
        protected IEnumerator MoveBallRoutine()
        {
            List<Vector2> path = Pathfinding.FindPathTwoWay(_selectedFirstTile, _selectedSecondTile);
            if (path != null)
            {
                if(path.Count <= 5)
                {
                    yield return _selectedFirstTile.MoveBall(path);
                    _selectedSecondTile.Ball = _selectedFirstTile.Ball;
                    _selectedFirstTile.Ball = null;
                    _tilesNoBall.Remove(_selectedSecondTile);
                    _tilesNoBall.Add(_selectedFirstTile);
                    CheckLine(_selectedSecondTile);
                    _selectedFirstTile = null;
                    _selectedSecondTile = null;
                    yield return new WaitForSeconds(0.5f);
                    GameManager.Instance.ChangeState(GameState.SpawnTurn);
                    yield break;
                }
                           
            }
            _selectedFirstTile = null;
            _selectedSecondTile = null;
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
        }
        #region Check line methods
        protected void CheckLine(Tile tile)
        {

            bool hasLine = false;
            List<Tile> horizontalLine = CheckHorizontalLine(tile);
            List<Tile> verticalLine = CheckVerticalLine(tile);
            List<Tile> subCrossLine = CheckSubCrossLine(tile);
            List<Tile> mainCrossLine = CheckMainCrossLine(tile);
            for (int i = 0; i < horizontalLine.Count; i++)
            {
                hasLine = true;
                GameManager.Instance.AddScore(horizontalLine[i].Ball.Score);
                horizontalLine[i].ExplodeBall();
                _tilesNoBall.Add(horizontalLine[i]);
            }
            for (int i = 0; i < verticalLine.Count; i++)
            {
                hasLine = true;
                GameManager.Instance.AddScore(verticalLine[i].Ball.Score);
                verticalLine[i].ExplodeBall();
                _tilesNoBall.Add(verticalLine[i]);
            }
            for (int i = 0; i < subCrossLine.Count; i++)
            {
                hasLine = true;
                GameManager.Instance.AddScore(subCrossLine[i].Ball.Score);
                subCrossLine[i].ExplodeBall();
                _tilesNoBall.Add(subCrossLine[i]);
            }
            for (int i = 0; i < mainCrossLine.Count; i++)
            {
                hasLine = true;
                GameManager.Instance.AddScore(mainCrossLine[i].Ball.Score);
                mainCrossLine[i].ExplodeBall();
                _tilesNoBall.Add(mainCrossLine[i]);
            }
            if (hasLine)
            {
                GameManager.Instance.AddScore(tile.Ball.Score);
                tile.ExplodeBall();
                _tilesNoBall.Add(tile);
            }

        }
        protected List<Tile> CheckHorizontalLine(Tile tile)
        {
            List<Tile> tileHasBallSameCategory = new List<Tile>();
            int value = 0;
            for (int x = (int)tile.transform.position.x - 1; x >= 0; x--)
            {

                Tile tempTile = _tiles[new Vector2(x, tile.transform.position.y)];
                if (tempTile.Ball == null) break;
                if (tile.Ball.Category != tempTile.Ball.Category && !tile.Ball.Category.TryInteract(tempTile.Ball.Category))
                {
                    break;
                }
                value += tempTile.Ball.Value;
                tileHasBallSameCategory.Add(tempTile);

            }
            for (int x = (int)tile.transform.position.x + 1; x < _size; x++)
            {
                Tile tempTile = _tiles[new Vector2(x, tile.transform.position.y)];
                if (tempTile.Ball == null) break;
                if (tile.Ball.Category != tempTile.Ball.Category && !tile.Ball.Category.TryInteract(tempTile.Ball.Category))
                {
                    break;
                }
                value += tempTile.Ball.Value;
                tileHasBallSameCategory.Add(tempTile);

            }
            if (value < 4)
            {
                tileHasBallSameCategory.Clear();
            }
            return tileHasBallSameCategory;
        }
        protected List<Tile> CheckVerticalLine(Tile tile)
        {
            List<Tile> tileHasBallSameCategory = new List<Tile>();
            int value = 0;
            for (int y = (int)tile.transform.position.y - 1; y >= 0; y--)
            {
                Tile tempTile = _tiles[new Vector2(tile.transform.position.x, y)];
                if (tempTile.Ball == null) break;
                if (tile.Ball.Category != tempTile.Ball.Category && !tile.Ball.Category.TryInteract(tempTile.Ball.Category))
                {
                    break;
                }
                value += tempTile.Ball.Value;
                tileHasBallSameCategory.Add(tempTile);

            }
            for (int y = (int)tile.transform.position.y + 1; y < _size; y++)
            {
                Tile tempTile = _tiles[new Vector2(tile.transform.position.x, y)];
                if (tempTile.Ball == null) break;
                if (tile.Ball.Category != tempTile.Ball.Category && !tile.Ball.Category.TryInteract(tempTile.Ball.Category))
                {
                    break;
                }
                value += tempTile.Ball.Value;
                tileHasBallSameCategory.Add(tempTile);
            }
            if (value < 4)
            {
                tileHasBallSameCategory.Clear();
            }
            return tileHasBallSameCategory;
        }
        protected List<Tile> CheckMainCrossLine(Tile tile)
        {
            List<Tile> tileHasBallSameCategory = new List<Tile>();
            int value = 0;
            Vector2 tilePosition = new Vector2(tile.transform.position.x, tile.transform.position.y);

            while (tilePosition.x - 1 >= 0 && tilePosition.y - 1 >= 0)
            {
                tilePosition.x -= 1;
                tilePosition.y -= 1;
                Tile tempTile = _tiles[tilePosition];
                if (tempTile.Ball == null) break;
                if (tile.Ball.Category != tempTile.Ball.Category && !tile.Ball.Category.TryInteract(tempTile.Ball.Category))
                {
                    break;
                }
                value += tempTile.Ball.Value;
                tileHasBallSameCategory.Add(tempTile);

            }
            tilePosition = new Vector2(tile.transform.position.x, tile.transform.position.y);
            while (tilePosition.x + 1 < _size && tilePosition.y + 1 < _size)
            {
                tilePosition.x += 1;
                tilePosition.y += 1;
                Tile tempTile = _tiles[tilePosition];
                if (tempTile.Ball == null) break;
                if (tile.Ball.Category != tempTile.Ball.Category && !tile.Ball.Category.TryInteract(tempTile.Ball.Category))
                {
                    break;
                }
                value += tempTile.Ball.Value;
                tileHasBallSameCategory.Add(tempTile);

            }
            if (value < 4)
            {
                tileHasBallSameCategory.Clear();
            }
            return tileHasBallSameCategory;
        }
        protected List<Tile> CheckSubCrossLine(Tile tile)
        {
            List<Tile> tileHasBallSameCategory = new List<Tile>();
            int value = 0;
            int x = (int)tile.transform.position.x;
            int y = (int)tile.transform.position.y;
            while (x - 1 >= 0 && y + 1 < _size)
            {
                x -= 1;
                y += 1;
                Tile tempTile = _tiles[new Vector2(x, y)];
                if (tempTile.Ball == null) break;
                if (tile.Ball.Category != tempTile.Ball.Category && !tile.Ball.Category.TryInteract(tempTile.Ball.Category))
                {
                    break;
                }
                value += tempTile.Ball.Value;
                tileHasBallSameCategory.Add(tempTile);

            }
            x = (int)tile.transform.position.x;
            y = (int)tile.transform.position.y;
            while (x + 1 < _size && y - 1 >= 0)
            {
                x += 1;
                y -= 1;
                Tile tempTile = _tiles[new Vector2(x, y)];
                if (tempTile.Ball == null) break;
                if (tile.Ball.Category != tempTile.Ball.Category && !tile.Ball.Category.TryInteract(tempTile.Ball.Category))
                {
                    break;
                }
                value += tempTile.Ball.Value;
                tileHasBallSameCategory.Add(tempTile);

            }
            if (value < 4)
            {
                tileHasBallSameCategory.Clear();
            }
            return tileHasBallSameCategory;
        }
        #endregion
        public List<Tile> GetNeighbor(Tile tile)
        {
            Vector2 tilePosition = tile.transform.position;
            List<Tile> neighbourTiles = new List<Tile>();
            if (tilePosition.x - 1 >= 0)
            {
                Vector2 position = new Vector2((int)tilePosition.x - 1, (int)tilePosition.y);
                Tile neighbourTile = _tiles[position];
                neighbourTiles.Add(neighbourTile);
            }
            if (tilePosition.x + 1 < _size)
            {
                Vector2 position = new Vector2((int)tilePosition.x + 1, (int)tilePosition.y);
                Tile neighbourTile = _tiles[position];
                neighbourTiles.Add(neighbourTile);
            }
            if (tilePosition.y - 1 >= 0)
            {
                Vector2 position = new Vector2((int)tilePosition.x, (int)tilePosition.y - 1);
                Tile neighbourTile = _tiles[position];
                neighbourTiles.Add(neighbourTile);
            }
            if (tilePosition.y + 1 < _size)
            {
                Vector2 position = new Vector2((int)tilePosition.x, (int)tilePosition.y + 1);
                Tile neighbourTile = _tiles[position];
                neighbourTiles.Add(neighbourTile);
            }
            return neighbourTiles;
        }
        public void Clear()
        {
            _selectedFirstTile = null;
            _selectedSecondTile = null;
            _tiles.Clear();
            _tilesNoBall.Clear();
            _queueBall.Clear();
            transform.DestroyChildren();
        }
    }
}
