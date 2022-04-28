using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.crafting
{
    [CreateAssetMenu(menuName="Crafting/Categories",fileName="New Category List")]
    public class CategoryList : ScriptableObject {
        [SerializeField] private List<string> _categories;
        public ReadOnlyCollection<string> categories {
            get {
                return _categories.AsReadOnly();
            }
        }
    }
}
