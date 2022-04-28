using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.draco18s.util.Editor;
using Assets.draco18s.crafting.properties;

using Object = UnityEngine.Object;
using Material = Assets.draco18s.crafting.Material;
using MaterialProperty = Assets.draco18s.crafting.properties.MaterialProperty;

namespace Assets.draco18s.crafting
{
	[CustomPropertyDrawer(typeof(AtomicProperties))]
	public class AtomicPropertiesDrawer : MaterialPropertyDrawer {
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			BaseOnGui(position, property, label);
			if(!property.isExpanded) return;
			//EditorGUI.LabelField(position, new GUIContent("..."));
			position.y += SPACING;
			Rect rect = EditorGUI.IndentedRect(position);
			rect.height = EditorGUIUtility.singleLineHeight;

			Rect leftRect = new Rect(rect);
			leftRect.width = (rect.width - 5)/2;

			Rect rightRect = new Rect(leftRect);
			rightRect.x += leftRect.width + 5;
			
			EditorGUI.PropertyField(leftRect,property.FindPropertyRelative(nameof(AtomicProperties.molarMass)), new GUIContent("Mass (molar)"));
			leftRect.y += SPACING;
			rightRect.y += SPACING;
			EditorGUI.PropertyField(leftRect,property.FindPropertyRelative(nameof(AtomicProperties.greenhousePotential)), new GUIContent("Greenhouse Ratio"));
			EditorGUI.PropertyField(rightRect,property.FindPropertyRelative(nameof(AtomicProperties.albedo)), new GUIContent("Albedo"));
			
			leftRect.y += SPACING;
			rightRect.y += SPACING;
			EditorGUI.PropertyField(leftRect,property.FindPropertyRelative(nameof(AtomicProperties.meltingPoint)), new GUIContent("Melting Temp (°C)"));
			EditorGUI.PropertyField(rightRect,property.FindPropertyRelative(nameof(AtomicProperties.boilingPoint)), new GUIContent("Boiling Temp (°C)"));
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return SPACING * (property.isExpanded ? 4 : 1);
		}
	}
}