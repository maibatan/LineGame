using LineGame.Runtime.Core;
using LineGame.Runtime.Systems;
using LineGame.Runtime.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LineGame.Runtime.Managers
{
    public class BoardManager : StaticInstance<BoardManager>
    {
        [SerializeField]
        protected int _size;
        public int Size => _size;

        [SerializeField]
        protected int _startBallSize;
        [SerializeField]
        protected int _queuedBallSize;
        [SerializeField]
        protected Tile _tilePrefab;

        protected Dictionary<Vector2, Tile> _tiles;
        public IReadOnlyDictionary<Vector2, Tile> Tiles => _tiles;

        protected List<Tile> _tilesNoBall;

        protected Tile _selectedTile;
        public Tile SelectedTile
        {
            get { return _selectedTile; }
            set { _selectedTile = value; }
        }

        protected List<Tile> _tileHasQueueBall ;
        public List<Tile> TileHasQueueBall => _tileHasQueueBall;
        public void SpawnQueuedBall()
        {
            _tileHasQueueBall = new List<Tile>();
            for (int i = 0; i < _queuedBallSize; i++)
            {
                if (_tilesNoBall.Count <= 0) continue;
                int randomNumber = Random.Range(0, _tilesNoBall.Count);
                var tile = _tilesNoBall[randomNumber];
                tile.QueuedBall = Random.Range(0,5) != 0 ?  BallResourceSystem.Instance.GetNormalRandom() : BallResourceSystem.Instance.GetGhostlRandom();
                _tilesNoBall.RemoveAt(randomNumber);
                _tileHasQueueBall.Add(tile);
            }
            if (_tilesNoBall.Count <= 0)
            {
                GameManager.Instance.ChangeState(GameState.GameOver);
                return;
            }
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
        }
        public void GenerateBoard()
        {
            _tiles = new Dictionary<Vector2, Tile>();
            _tilesNoBall = new List<Tile>();
            for(int x =0; x < _size; x++)
            {
                for(int y = 0; y < _size; y++)
                {
                    var tile = Instantiate(_tilePrefab,new Vector3(x,y),Quaternion.identity);
                    tile.name = $"Tile {x} {y}";
                    _tiles.Add(new Vector2(x, y), tile);
                    _tilesNoBall.Add(tile);
                }
            }
            for(int i = 0; i < _startBallSize; i++)
            {
                if (_tilesNoBall.Count <= 0) return;
                int randomNumber = Random.Range(0, _tilesNoBall.Count);
                var tile = _tilesNoBall[randomNumber];
                tile.Ball = BallResourceSystem.Instance.GetNormalRandom();
                _tilesNoBall.RemoveAt(randomNumber);
            }
            Camera.main.transform.position = new Vector3((float) _size/2 -0.5f,(float) _size/2 -0.5f, Camera.main.transform.position.z);
            Camera.main.orthographicSize = (float) _size/2;
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
        }

        public void Clear(Tile tile)
        {
            if(!_tilesNoBall.Contains(tile)) _tilesNoBall.Add(tile);
        }
        public void Mark(Tile tile)
        {
            _tilesNoBall.Remove(tile);
        }
        public List<Tile> CheckHorizontalLine(Tile tile)
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
                Tile tempTile = _tiles[new Vector2(x,tile.transform.position.y)];
                if (tempTile.Ball == null) break;
                if (tile.Ball.Category != tempTile.Ball.Category && !tile.Ball.Category.TryInteract(tempTile.Ball.Category))
                {
                    break;
                }
                value += tempTile.Ball.Value;
                tileHasBallSameCategory.Add(tempTile);

            }
            if(value < 4)
            {
                tileHasBallSameCategory.Clear();
            }
            return tileHasBallSameCategory;
        }
        public List<Tile> CheckVerticalLine(Tile tile)
        {
            List<Tile> tileHasBallSameCategory = new List<Tile>();
            int value = 0;
            for (int y = (int)tile.transform.position.y - 1; y >= 0; y--)
            {
                Tile tempTile = _tiles[new Vector2(tile.transform.position.x,y)];
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
        public List<Tile> CheckMainCrossLine(Tile tile)
        {
            List<Tile> tileHasBallSameCategory = new List<Tile>();
            int value = 0;
            Vector2 tilePosition = new Vector2(tile.transform.position.x,tile.transform.position.y);
           
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
        public List<Tile> CheckSubCrossLine(Tile tile)
        {
            List<Tile> tileHasBallSameCategory = new List<Tile>();
            int value = 0;
            int x = (int)tile.transform.position.x;
            int y = (int)tile.transform.position.y;
            while (x - 1 >= 0 && y + 1 < _size)
            {
                x -= 1;
                y += 1;
                Tile tempTile = _tiles[new Vector2(x,y)];
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
                Tile tempTile = _tiles[new Vector2(x,y)];
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
    }
}
