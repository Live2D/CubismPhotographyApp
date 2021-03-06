/**
* Copyright(c) Live2D Inc. All rights reserved.
*
* Use of this source code is governed by the Live2D Open Software license
* that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
*/

using UnityEngine;

public class OptionalLabelAttribute : PropertyAttribute
{
    // 読み取り専用に設定
    public readonly string Value;

    // ラベルの変更
    public OptionalLabelAttribute(string value)
    {
        Value = value;
    }
}