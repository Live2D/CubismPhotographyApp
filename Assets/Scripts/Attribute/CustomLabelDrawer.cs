/**
* Copyright(c) Live2D Inc. All rights reserved.
*
* Use of this source code is governed by the Live2D Open Software license
* that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
*/

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CustomLabelAttribute))]
public class CustomLabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 新規にラベルを設定
        var customLabel = attribute as CustomLabelAttribute;

        // インスペクタへ反映
        EditorGUI.PropertyField(position, property, new GUIContent(customLabel.Value), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Propertyの高さを取得
        return EditorGUI.GetPropertyHeight(property, true);
    }
}