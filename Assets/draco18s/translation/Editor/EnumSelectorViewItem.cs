using System;

using UnityEngine;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Assets.draco18s.translation;

namespace Assets.draco18s.translation.Editor {
	/// <summary>Line-item for use in the <see cref="EnumSelectorView"/> tree.</summary>
	class EnumSelectorViewItem : TreeViewItem
    {
        /// <summary>Should this property be modified?</summary>
        public bool enabled {
			get {
				if(isFolder && null != enumType) return false;
				return Localization.IsEnumTranslated(enumType);
			}
			set {
				if(!isFolder && null != enumType) {
					Localization.SetTranslated(enumType, value);
				}
			}
		}
		public bool activeChildren = false;
        /// <summary>Is this tree item a folder or a material (<see cref="isMaterial"/>)?</summary>
        public bool isFolder;
        /// <summary>The path to the material or folder within the project directory.</summary>
        public string path;
		/// <summary>Reference to the enum's type</summary>
		private readonly Type enumType;

        /// <summary>Is this tree item a material or a folder (<see cref="isFolder"/>)?</summary>
        public bool isMaterial { get { return !isFolder; } set { isFolder = !value; } }

		/// <summary>Static method to create a root for a <see cref="EnumSelectorView"/>.</summary>
		/// <returns>The newly created root item.</returns>
		public static EnumSelectorViewItem CreateRoot() { return new EnumSelectorViewItem(0, null) { depth = -1, displayName = "ROOT" }; }

        /// <summary>C'tor to create an item with a valid reference id.</summary>
        /// <param name="id">The unique id for this item.</param>
        /// <param name="enabled">Is this item <see cref="enabled"/>?</param>
        public EnumSelectorViewItem(int id, Type type, bool enabled = false) : base(id)
        {
            this.enabled = enabled;
			enumType = type;
		}
    }
}
