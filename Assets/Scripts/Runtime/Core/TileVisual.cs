using System.Collections;
using UnityEngine;

namespace LineGame.Runtime.Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TileVisual : MonoBehaviour
    {
        [SerializeField]
        protected Color _baseColor;
        [SerializeField]
        protected Color _offsetColor;
        [SerializeField]
        protected GameObject _highlight;

        protected SpriteRenderer _renderer;

        #region Unity Methods
        protected void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }
        #endregion
        #region Methods
        public void Init(int x, int y)
        {
            bool isOffset = (x + y) % 2 == 1;
            _renderer.color = isOffset ? _offsetColor : _baseColor;
        }
        public void MakeHighlight(bool value)
        {
            _highlight.SetActive(value);
        }
        #endregion
    }
}
