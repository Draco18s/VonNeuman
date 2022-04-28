using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Assets.draco18s.translation.Editor {
    /// <summary>The custom window for editing the materials in the project.</summary>
    public class EnumSelectorEditor : EditorWindow
    {
        const string windowTitle = "Enum Selector";
		public bool allowDelete = false;

        [SerializeField] TreeViewState treeViewState = null;

        EnumSelectorView enumSelectorView { get; set; } = null;

        void OnGUI() {
            if (!Application.isPlaying) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.ExpandWidth(true));
                if (EditorGUILayout.DropdownButton(SettingsButtonContent, FocusType.Passive, SettingsButtonStyle, GUILayout.Width(20.0F))) {
                    GenericMenu settings = new GenericMenu();
                    settings.AddSeparator(string.Empty);
                    settings.AddItem(ResetItemContent, false, Reset);
                    settings.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();
				allowDelete = EditorGUILayout.ToggleLeft("Allow deletions", allowDelete);
				EditorGUILayout.Separator();
                if (null != enumSelectorView) {
                    float minimumHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 5;
                    Rect rect = GUILayoutUtility.GetRect(position.width, position.width, minimumHeight, position.height);
					enumSelectorView.OnGUI(new Rect(rect.x + 1, rect.y, rect.width - 1, rect.height - 1));
                }
            }
            else {
                EditorGUILayout.HelpBox("Batch Editor not available in Play Mode.", MessageType.Warning);
            }
        }
        
        void OnEnable() { Initialize(); }
        void OnProjectChange() { Initialize(); }

        void Reset() { }

        void Initialize() {
            if (null == treeViewState)
                treeViewState = new TreeViewState();

            enumSelectorView = new EnumSelectorView(treeViewState);
        }

        void ApplyChanges() { }

        public static void OpenEnumSelectorEditorWindow() {
            EnumSelectorEditor window = EditorWindow.GetWindow<EnumSelectorEditor>();
            window.titleContent = new GUIContent(windowTitle, EditorGUIUtility.FindTexture(EnumSelectorView.materialTextureName));
            window.Show();
        }

        static GUIStyle SettingsButtonStyle
        {
            get
            {
                if (null == settingsButtonStyle)
                    settingsButtonStyle = GUI.skin.FindStyle("IconButton") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("IconButton");

                return settingsButtonStyle;
            }
        }
        static GUIStyle settingsButtonStyle = null;

        static GUIContent SettingsButtonContent
        {
            get
            {
                if (null == settingsButtonContent)
                    settingsButtonContent = new GUIContent(EditorGUIUtility.Load("icons/d__Popup.png") as Texture2D);

                return settingsButtonContent;
            }
        }
        static GUIContent settingsButtonContent = null;

        static GUIContent ResetItemContent
        {
            get
            {
                if (null == resetItemContent)
                    resetItemContent = new GUIContent("Reset");

                return resetItemContent;
            }
        }
        static GUIContent resetItemContent = null;
    }
}
