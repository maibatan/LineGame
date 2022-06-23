using LineGame.Runtime.Utilities;
using LineGame.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LineGame.Runtime.Systems
{
    public class BallResourceSystem : StaticInstance<BallResourceSystem>
    {
        protected List<Ball> _normalBall;
        protected List<Ball> _ghostBall;

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
            AssembleResources();
        }
        #endregion
        #region Methods
        private void AssembleResources()
        {
            _normalBall = Resources.LoadAll<Ball>("Balls/Normal").ToList();
            _ghostBall = Resources.LoadAll<Ball>("Balls/Ghost").ToList();
        }
        public Ball GetNormalRandom()
        {
            if (_normalBall.Count <= 0) return null;
            return _normalBall[Random.Range(0, _normalBall.Count)];
        }
        public Ball GetGhostlRandom()
        {
            if (_ghostBall.Count <= 0) return null;
            return _ghostBall[Random.Range(0, _normalBall.Count)];
        }
        #endregion
    }
}
