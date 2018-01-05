using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SymmetrySprite), true)]
public class SymmetrySpriteInspector : UISpriteInspector {
	protected override void DrawCustomProperties (){
		NGUIEditorTools.DrawProperty("SymmetryType", serializedObject, "symmetryType", GUILayout.MinWidth(20f));
		NGUIEditorTools.DrawProperty("Type", serializedObject, "mType", GUILayout.MinWidth(20f));

		if (NGUISettings.unifiedTransform)
		{
			DrawColor(serializedObject, mWidget);
		}
		else DrawInspectorProperties(serializedObject, mWidget, true);
	}
}
