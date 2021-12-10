/**
* Copyright(c) Live2D Inc. All rights reserved.
*
* Use of this source code is governed by the Live2D Open Software license
* that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
*/

using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using UnityEngine;

public class AnimationClipSampler : MonoBehaviour, ICubismUpdatable
{
    [SerializeField, OptionalLabel("Live2Dアニメーション"), Tooltip("任意のLive2D Motionのアニメーションクリップ")]
    public AnimationClip AnimationClip;

    [SerializeField, Range(0.0f, 1.0f), OptionalLabel("アニメーションの時間"), Tooltip("適用するモーションの時間割合")]
    public float AnimationTimeRate;

    // モデル
    private CubismModel _model;

    // 有効になった時
    private void Start()
    {
        // モデルをLive2DModelManagerから取得
        _model = GetComponent<CubismModel>();

        // CubismUpdateControllerを保持している場合はTrueを返す
        HasUpdateController = (GetComponent<CubismUpdateController>() != null);
    }

    // 適用するモーションの時間を指定する
    public void SetTimeRate(float timeRate)
    {
        AnimationTimeRate = timeRate;
    }

    private void LateUpdate()
    {
        if (HasUpdateController)
        {
            return;
        }

        // CubismUpdateControllerを保持していない場合のみ処理を行う
        OnLateUpdate();
    }

    // 自身の更新順序を渡す
    public int ExecutionOrder
    {
        get { return CubismUpdateExecutionOrder.CubismFadeController; }
    }

    public bool NeedsUpdateOnEditing { get; }

    // CubismUpdateControllerを保持しているか？
    public bool HasUpdateController { get; set; }

    // LateUpdate時に処理
    public void OnLateUpdate()
    {
        // モデル、もしくはAnimationClipが空なら処理しない
        if (_model == null || AnimationClip == null)
        {
            return;
        }

        AnimationClip.SampleAnimation(_model.gameObject, AnimationTimeRate * AnimationClip.length);
    }
}
