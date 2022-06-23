using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LineGame.Settings
{
    [CreateAssetMenu(fileName = "New Ball", menuName = "Settings/Ball")]
    public class Ball : ScriptableObject
    {

        [SerializeField]
        protected Category _category;
        public Category Category => _category;

        [SerializeField]
        protected int _score;
        public int Score => _score;

        [SerializeField]
        protected int _value;
        public int Value => _value;

        [SerializeField]
        protected bool _isThrought;
        public bool IsThrought => _isThrought;

        [SerializeField]
        protected Sprite _specialIcon;
        public Sprite SpecialIcon => _specialIcon;
    }
}

