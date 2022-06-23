using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LineGame.Settings
{
    [CreateAssetMenu(fileName = "New Category", menuName = "Settings/Category")]
    public class Category : ScriptableObject
    {
        [SerializeField]
        protected List<Category> _interactWith = new List<Category>();

        [Header("Visual Effect")]
        [SerializeField]
        protected Color _color;
        public Color Color => _color;

        public bool TryInteract(Category category)
        {
            return _interactWith.Contains(category);
        }
    }
}

