/**
* Copyright(c) Live2D Inc. All rights reserved.
*
* Use of this source code is governed by the Live2D Open Software license
* that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
*/

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OptionalLabelAttribute))]
public class OptionalLabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        // 新規にラベルを設定
        var optionalLabel = attribute as OptionalLabelAttribute;

        // インスペクタへ反映
        EditorGUI.PropertyField(rect, property, new GUIContent(optionalLabel.Value), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Propertyの高さを取得
        return EditorGUI.GetPropertyHeight(property, true);
    }
}