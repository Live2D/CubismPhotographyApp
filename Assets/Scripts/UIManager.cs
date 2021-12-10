/**
* Copyright(c) Live2D Inc. All rights reserved.
*
* Use of this source code is governed by the Live2D Open Software license
* that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField, OptionalLabel("UI透明化ボタン"), Tooltip("UIの表示非表示を切り替えるボタン")]
    public Button UiTransparency;

    [SerializeField, OptionalLabel("メニュー切り替えボタン"), Tooltip("UIのメニューを切り替えるボタン")]
    public Button MenuSwitchButton;

    [SerializeField, OptionalLabel("メニュー切り替え用のオブジェクト"), Tooltip("メニュー切り替え用空のオブジェクト")]
    public GameObject[] MenuObjects;

    // UIは透明の状態か
    private bool _isUiTransparent = false;

    // 現在アクティブ状態になっているメニューのインデックス
    private int _activeMenuIndex = 0;

    // ボタンの通常時のデフォルトのカラー値
    private ColorBlock _defaultButtonColors;

    // テキストのデフォルトのカラー値
    private Color _defaultTextColor;

    private void Start()
    {
        if (UiTransparency == null)
        {
            return;
        }

        // 各デフォルト値を取得
        _defaultButtonColors = UiTransparency.colors;
        _defaultTextColor = UiTransparency.GetComponentInChildren<Text>().color;

        // 対象となるメニューアイテムのみアクティブに
        for (int i = 0; i < MenuObjects.Length; i++)
        {
            var isActiveIndex = (i == _activeMenuIndex);

            MenuObjects[i].SetActive(isActiveIndex);
        }
    }

    // UIの可視・不可視の切り替え
    public void SwitchTransparency()
    {
        if (UiTransparency == null)
        {
            return;
        }

        // コンポーネント取得
        var colors = UiTransparency.colors;
        var textColor = UiTransparency.GetComponentInChildren<Text>();

        // 切り替えた状態に合うようにUIの色情報を変更する
        if (_isUiTransparent)
        {
            // ボタンの色情報を初期値へ
            UiTransparency.colors = _defaultButtonColors;

            // テキストの色を初期値へ
            textColor.color = _defaultTextColor;

            // アクティブ状態だったもののみアクティブに
            for (int i = 0; i < MenuObjects.Length; i++)
            {
                var isActiveIndex = (i == _activeMenuIndex);

                MenuObjects[i].SetActive(isActiveIndex);
            }
        }
        else
        {
            // ボタンの色を透明に
            colors.normalColor = Color.clear;
            colors.pressedColor = Color.clear;
            colors.selectedColor = Color.clear;
            colors.disabledColor = Color.clear;
            colors.highlightedColor = Color.clear;
            UiTransparency.colors = colors;

            // テキストの色を透明に
            textColor.color = Color.clear;

            // 全て透明に
            for (int i = 0; i < MenuObjects.Length; i++)
            {
                MenuObjects[i].SetActive(false);
            }
        }

        // メニュー切り替えはDisable
        MenuSwitchButton.gameObject.SetActive(_isUiTransparent);

        // 状態を切り替え
        _isUiTransparent = !_isUiTransparent;
    }

    // メニューの切り替え
    public void MenuSwitch()
    {
        if (MenuSwitchButton == null)
        {
            return;
        }

        // _activeMenuIndex + 1の値が配列の長さ以上になるなら配列の最初に戻す
        _activeMenuIndex = _activeMenuIndex + 1 >= MenuObjects.Length ? 0 : _activeMenuIndex + 1;

        // 対象となるメニューアイテムのみアクティブに
        for (int i = 0; i < MenuObjects.Length; i++)
        {
            var isActiveIndex = (i == _activeMenuIndex);

            MenuObjects[i].SetActive(isActiveIndex);
        }
    }
}
