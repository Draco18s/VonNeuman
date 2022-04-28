using UnityEngine;
using UnityEditor;
using Assets.draco18s.gameAssets;

[CustomPropertyDrawer(typeof(ElementData))]
public class ElementDataDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Draw label
		EditorGUIUtility.labelWidth *= 0.5f;
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		position.height = EditorGUIUtility.singleLineHeight;
        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

		float wid = position.width-8;
		//Debug.Log(wid);

        // Calculate rects
        var idRect = new Rect(position.x, position.y, 30*(wid/290), position.height);
        var nameRect = new Rect(idRect.xMax + 2, position.y, 105*(wid/290), position.height);
        var amountRect = new Rect(nameRect.xMax+2, position.y, 30*(wid/290), position.height);
        var gwpRect = new Rect(amountRect.xMax+2, position.y, 30*(wid/290), position.height);
		var typeRect = new Rect(gwpRect.xMax+2, position.y, 95*(wid/290), position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(idRect, property.FindPropertyRelative(nameof(ElementData.id)), GUIContent.none);
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative(nameof(ElementData.name)), GUIContent.none);
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative(nameof(ElementData.relativeQuant)), GUIContent.none);
        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative(nameof(ElementData.elemTyp)), GUIContent.none);

		position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(ElementData.material)), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
		EditorGUIUtility.labelWidth /= 0.5f;
    }

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return EditorGUIUtility.singleLineHeight*2 + EditorGUIUtility.standardVerticalSpacing*2;
	}
}