using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Assets.draco18s.translation;
using System.Text.RegularExpressions;

namespace Assets.draco18s.translation.Editor {
	/// <summary>The <see cref="TreeView"/> derived object to display all of the materials in the project.</summary>
	class EnumSelectorView : TreeView {
		/// <summary>The built-in editor icon for a folder.</summary>
		public const string folderTextureName = "Folder Icon";
		/// <summary>The built-in editor icon for a material.</summary>
		public const string materialTextureName = "cs Script Icon";

		static Texture2D[] textures = new Texture2D[2]
		{
			EditorGUIUtility.FindTexture(folderTextureName),
			EditorGUIUtility.FindTexture(materialTextureName),
		};

		/// <summary>C'tor that sets the specifics of the view and calls the Reload() method.</summary>
		/// <param name="state">TreeViews aren't serialized but the state may be maintained between project refreshes.</param>
		public EnumSelectorView(TreeViewState state) : base(state)
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            extraSpaceBeforeIconAndLabel = 4.0F;

			Reload();
        }

		protected override void RowGUI(RowGUIArgs args)
        {
			bool allowDelete = EditorWindow.GetWindow<EnumSelectorEditor>().allowDelete;

			const float toggleWidth = 18.0F;
            const float typeFieldWidth = 18.0F;

			EnumSelectorViewItem item = args.item as EnumSelectorViewItem;

            float indent = GetContentIndent(args.item);
			float indent2 = this.depthIndentWidth;

            Rect rect = args.rowRect;
            CenterRectUsingSingleLineHeight(ref rect);

            rect.x += indent;
            rect.width -= indent;
			float w = 0;
            //EditorGUI.BeginChangeCheck();
			if(!item.isFolder) {
				Rect toggleRect = rect;
				bool temp = EditorGUI.Toggle(toggleRect, item.enabled);
				if(allowDelete) {
					item.enabled = temp;
				}
				else {
					item.enabled |= temp;
				}
				w = toggleWidth;
			}

            Rect iconRect = rect;
            iconRect.x += w;
            iconRect.width = typeFieldWidth;
            GUI.DrawTexture(iconRect, textures[item.isFolder ? 0 : 1], ScaleMode.ScaleToFit);

            Rect nameRect = rect;
            nameRect.x -= indent;
			nameRect.x += indent2 + w;
            //nameRect.width -= typeFieldWidth;
            args.rowRect = nameRect;
            base.RowGUI(args);
        }
		
        protected override TreeViewItem BuildRoot()
        {
			EnumSelectorViewItem root = EnumSelectorViewItem.CreateRoot();

			foreach(var a in AppDomain.CurrentDomain.GetAssemblies()) {
				string assetPath = a.GetName().Name;
				EnumSelectorViewItem parent = GetParentItem(root, assetPath);
				foreach(Type t in a.GetTypes()) {
					if(t.IsEnum) {
						string path = t.AssemblyQualifiedName;
						EnumSelectorViewItem item = new EnumSelectorViewItem(path.GetHashCode(), t) {  isFolder = false, path = path, displayName = t.Name, enabled = Localization.IsEnumTranslated(t) };
						parent.AddChild(item);
						parent.activeChildren |= item.enabled;
					}
				}

				if(null != parent.children && parent.children.Count > 0)
					parent.children.Sort();
				if(parent.activeChildren) {
					SetExpanded(parent.id, true);
					if(!root.children.Contains(parent)) {
						TreeViewItem i = root.children.FirstOrDefault(x => {
							if(x.hasChildren) {
								return x.children.Contains(parent);
							}
							return false;
						});
						if(i != null)
							SetExpanded(i.id, true);
					}
				}
			}
			root.children.Sort();
			root.children.ForEach(x => {
				if(x.hasChildren) {
					x.children.Sort();
					x.children.RemoveAll(y => !y.hasChildren);
				}
			});
			root.children.RemoveAll(x => !x.hasChildren);
			SetupDepthsFromParentsAndChildren(root);

			return root;
        }

		protected EnumSelectorViewItem GetParentItem(EnumSelectorViewItem root, string path) {
			int ind = path.IndexOf('.');
			if(ind < 0) {
				if(root.hasChildren) {
					TreeViewItem p = root.children.FirstOrDefault(x => x.id == path.GetHashCode());
					if(p != null) return (EnumSelectorViewItem)p;
				}
				EnumSelectorViewItem item = new EnumSelectorViewItem(path.GetHashCode(), null) { isFolder = true, path = path, displayName = path };
				root.AddChild(item);
				return item;
			}
			else {
				string basePath = path.Substring(0, ind);
				string remainderPath = path.Substring(ind+1, path.Length - (ind+1));
				remainderPath = Regex.Replace(remainderPath, @"\.(Runtime|Editor)$", "");
				TreeViewItem p = null;
				TreeViewItem item = null;
				if(root.hasChildren) {
					p = root.children.FirstOrDefault(x => x.id == basePath.GetHashCode());
				}
				if(p == null) {
					p = new EnumSelectorViewItem(basePath.GetHashCode(), null) { isFolder = true, path = basePath, displayName = basePath };
					root.AddChild(p);
				}
				if(p.hasChildren) {
					item = p.children.FirstOrDefault(x => x.id == remainderPath.GetHashCode());
				}
				if(item == null) {
					item = new EnumSelectorViewItem(remainderPath.GetHashCode(), null) { isFolder = true, path = remainderPath, displayName = remainderPath };
					p.AddChild(item);
				}
				return (EnumSelectorViewItem)item;
			}
		}
	}
}
