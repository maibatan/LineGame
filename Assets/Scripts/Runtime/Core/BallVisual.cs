using LineGame.Runtime.Systems;
using LineGame.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LineGame.Runtime.Core
{

    public class BallVisual : MonoBehaviour
    {
        [SerializeField]
        protected Color _baseColor;
        [SerializeField]
        protected float _minSize;
        [SerializeField]
        protected float _maxSize;
        [SerializeField]
        protected SpriteRenderer _ballRenderer;
        [SerializeField]
        protected SpriteRenderer _specialRenderer;
        [SerializeField]
        protected GameObject _explodeEffect;
        #region Unity methods
        protected void Awake()
        {
            _ballRenderer.color = _baseColor;
        }
        #endregion
        #region Methods

        protected IEnumerator ZoomInVisual()
        {
            while(transform.localScale.x < _maxSize)
            {
                yield return new WaitForFixedUpdate();
                transform.localScale += new Vector3(0.01f, 0.01f);           
            }
           
        }
        protected IEnumerator ExplosiveVisual()
        {
            SoundSystem.Instance.PlayExplodeSound();
            _ballRenderer.color = _baseColor;
            _specialRenderer.sprite = null;
            _explodeEffect.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _explodeEffect.SetActive(false);
        }
        public void ZoomIn()
        {
            StartCoroutine(ZoomInVisual());
        }
        public void Explode()
        {
            StartCoroutine(ExplosiveVisual());
        }
        public void SetQueuedBall(Ball ball)
        {
            _ballRenderer.color = ball.Category.Color;
            transform.localScale = new Vector3(_minSize, _minSize);
            _specialRenderer.sprite = ball.SpecialIcon;
        }
        public void SetBall(Ball ball)
        {
            if (ball == null) return;
            _ballRenderer.color = ball.Category.Color;
            transform.localScale = new Vector3(_maxSize, _maxSize);
            _specialRenderer.sprite = ball.SpecialIcon;
        }

        public IEnumerator MoveBall(List<Vector2> path)
        {

            for (int i = 0; i < path.Count; i++)
            {
                SoundSystem.Instance.PlayMoveSound();
                Vector3 direction =  (Vector3)path[i] - transform.position;
                while(((Vector3)path[i] - transform.position).magnitude >=0.1f)
                {
                    yield return new WaitForFixedUpdate();
                    transform.position += direction.normalized/20;
                }

            }        
            _ballRenderer.color = _baseColor;
            _specialRenderer.sprite = null; 
            transform.localPosition = Vector3.zero;
        }
        #endregion
    }
}
