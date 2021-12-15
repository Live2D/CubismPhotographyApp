/**
* Copyright(c) Live2D Inc. All rights reserved.
*
* Use of this source code is governed by the Live2D Open Software license
* that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
*/

using System.Collections;
using System.Collections.Generic;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.Raycasting;
using UnityEngine;
using UnityEngine.UI;

public class Live2DModelManager : MonoBehaviour
{
    [SerializeField, OptionalLabel("Live2Dモデル"), Tooltip("モデルのプレハブをアタッチする場所")]
    public CubismModel Model;

    [SerializeField, OptionalLabel("モデルの反転ボタン"), Tooltip("モデルの反転ボタンをアタッチする場所")]
    public Button ModelInversionButton;

    [SerializeField, OptionalLabel("モデルの固定化ボタン"), Tooltip("モデルの固定化ボタンをアタッチする場所")]
    public Button ModelLockedButton;

    [SerializeField, OptionalLabel("モデルの最大スケール"), Tooltip("モデルの最大スケールを設定する場所")]
    public float MaxScale = 5.0f;

    #region 定数

    // 処理の対象外とする2点間の距離
    private const float UnprocessedValue = 0.001f;

    // 最小拡大率
    private const float MinScale = 0.5f;

    // 距離から拡大率を設定する際の変換率
    private const float DistanceToScaleRate = 200.0f;

    #endregion

    #region 変数

    // メインカメラの参照
    private Camera _mainCamera;

    // 生成されたモデルのオブジェクト
    private GameObject _modelObject;

    // モデルと光線の衝突判定用コンポーネント
    private CubismRaycaster _modelRaycaster;

    // 光線と衝突したアートメッシュの配列
    // 判定するアートメッシュにはCubismRaycastableが必要
    private CubismRaycastHit[] _raycastHits;

    // タッチし始めた位置
    private Vector3 _touchPosition = Vector3.zero;

    // 最後にタッチし始めた位置
    private Vector3 _previousTouchPosition = Vector3.zero;

    // モデルのスクリーン座標
    private Vector3 _modelScreenPosition = Vector3.zero;

    // 最後に2本指でタッチした時の2点間の距離
    private float _previousDistance = 0.0f;

    // 最後に2本指でタッチした時のベクトル
    private Vector2 _previousVector = Vector2.zero;

    // 2本指でタッチし始めた時の2点間の距離
    private float _beginDistance = 0.0f;

    // ピンチインアウトを始めた時の2点間の距離
    private float _moveDistance = 0.0f;

    // モデルに適用する共通の拡大率
    private float _modelScaleRate = 0.0f;

    // モデルの拡大率（代入・取得用）
    private Vector3 _modelScaleVector3 = Vector3.zero;

    // モデルの角度情報
    private Vector3 _modelEulerAngles = Vector3.zero;

    // モデルが左右反転しているか
    private bool _isModelInversion = false;

    // モデルが固定状態か
    private bool _isModelLocked = false;

    private AnimationClipSampler _sampler;

    #endregion

    // Start is called before the first frame update
    private void Awake()
    {
        // モデルがセットされていなかった場合
        if (Model == null)
        {
           Debug.LogError("[Live2DModelManager]: モデルが入力されていません");
           return;
        }

        // カメラへの参照を取得
        _mainCamera = Camera.main;

        // モデルをシーンへ生成
        _modelObject = Instantiate(Model.gameObject, Vector3.zero, Quaternion.identity);

        // コンポーネントをアタッチ
        _sampler = _modelObject.AddComponent<AnimationClipSampler>();

        // 光線との衝突判定用コンポーネントを取得
        _modelRaycaster = _modelObject.GetComponent<CubismRaycaster>();

        // 衝突したアートメッシュの配列の初期化
        // 保存する個数は任意（今回は4つまで）
        _raycastHits = new CubismRaycastHit[4];

        _modelObject.GetComponent<CubismUpdateController>().Refresh();
    }

    // Update is called once per frame
    private void Update()
    {
        // タッチの判定が無かった場合
        if (Input.touchCount == 0 || _isModelLocked || Model == null)
        {
            return;
        }

        // 指がモデルに触れていた場合
        if(Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            _touchPosition = touch.position;

            // タッチした位置から判定用の光線を飛ばす
            var ray = _mainCamera.ScreenPointToRay(_touchPosition);

            // 光線と衝突したアートメッシュの個数を取得
            var hitCount = _modelRaycaster.Raycast(ray, _raycastHits);

            // モデルと光線が衝突したら処理を始める
            if (hitCount > 0)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    // 移動した移動量を計算
                    var positionDifference = _touchPosition - _previousTouchPosition;

                    // モデルの位置情報をスクリーン座標へ変換
                    _modelScreenPosition = _mainCamera.WorldToScreenPoint(_modelObject.transform.position);

                    // 移動量をモデルのスクリーン座標へ加算
                    _modelScreenPosition.x += positionDifference.x;
                    _modelScreenPosition.y += positionDifference.y;

                    // 移動量を加算した位置情報をモデルオブジェクトへ適用
                    _modelObject.transform.position = _mainCamera.ScreenToWorldPoint(_modelScreenPosition);
                }

                // 最後に観測した位置を保存
                _previousTouchPosition = _touchPosition;
            }
        }

        // ピンチインアウト・回転処理
        if (Input.touchCount >= 2)
        {
            var touch1 = Input.GetTouch(0);
            var touch2 = Input.GetTouch(1);

            // 2本指でタッチし始めた
            if (touch2.phase == TouchPhase.Began)
            {
                // 2点間の距離を取得
                _beginDistance = Vector2.Distance(touch1.position, touch2.position);
                
                // 最後に観測した2点間の距離を保存
                _previousDistance = _beginDistance;

                // 2点間のベクトルを取得
                var beginVector = touch1.position - touch2.position;

                // 最後に観測した2点間のベクトルを保存
                _previousVector = beginVector;
            }
            // ピンチインアウト開始
            else if(touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                // 2点間の距離を取得
                _moveDistance = Vector2.Distance(touch1.position, touch2.position);

                // 2点間の距離が一定値以下だった場合は処理を行わない
                if (_beginDistance < UnprocessedValue || _moveDistance < UnprocessedValue)
                {
                    return;
                }

                #region 拡大率

                // モデルから現在の拡大率を取得
                _modelScaleVector3 = _modelObject.transform.localScale;

                // 取得した拡大率を計算用に変数へ代入
                _modelScaleRate = _modelScaleVector3.x;

                // 距離の差を元に拡大率へ変換して加算
                _modelScaleRate += (_moveDistance - _previousDistance) / DistanceToScaleRate;

                // 最小拡大率と最大拡大率の範囲に収める
                _modelScaleRate = Mathf.Clamp(_modelScaleRate, MinScale, MaxScale);

                // 最後に観測した2点間の距離を保存
                _previousDistance = _moveDistance;

                // 拡大率を適用（Z値は奥行きであるため、固定）
                // 左右反転機能がONならXの値を負にする
                _modelScaleVector3.x = _isModelInversion ? -_modelScaleRate : _modelScaleRate;
                _modelScaleVector3.y = _modelScaleRate;
                _modelObject.transform.localScale = _modelScaleVector3;

                #endregion

                #region 回転

                // モデルから現在の角度情報を取得
                _modelEulerAngles = _modelObject.transform.localEulerAngles;

                // 2点間のベクトルを取得
                var differenceVector = touch1.position - touch2.position;

                // 前フレームからの角度を求める
                var angle = Vector2.Angle(_previousVector, differenceVector);

                // 3次元ベクトルで外積を計算
                var cross = Vector3.Cross(_previousVector, differenceVector);

                // 外積の値が負なら角度に負の値を掛ける
                if (cross.z < 0)
                {
                    angle *= -1;
                }

                // 角度情報をモデルへ適用
                _modelEulerAngles.z += angle;
                _modelObject.transform.localEulerAngles = _modelEulerAngles;

                // 最後に観測した2点間のベクトルを保存
                _previousVector = differenceVector;

                #endregion
            }
        }
    }

    // ボタンを押したら左右反転機能切り替え
    public void ModelInversion()
    {
        if (Model == null)
        {
            return;
        }

        // 機能を切り替える
        _isModelInversion = !_isModelInversion;

        // モデルから現在の拡大率を取得
        _modelScaleVector3 = _modelObject.transform.localScale;
        
        // Xの拡大率を反転
        _modelScaleVector3.x = -_modelScaleVector3.x;
        
        // 拡大率を適用
        _modelObject.transform.localScale = _modelScaleVector3;

        var text = "";

        // 現在の状態に応じてテキストの内容を切り替える
        text = _isModelInversion ? "反転 : ON" : "反転 : OFF";

        ModelInversionButton.GetComponentInChildren<Text>().text = text;
    }

    // モデルの状態をリセットする
    public void ModelReset()
    {
        // モデルがセットされていなかった場合
        if (Model == null)
        {
            return;
        }

        // モデルのトランスフォームを取得
        var modelObjectTransform = _modelObject.transform;

        // モデルの位置情報を初期化
        modelObjectTransform.position = Vector3.zero;

        // モデルの回転情報を初期化
        modelObjectTransform.rotation = Quaternion.identity;

        // モデルの拡大率を初期化
        modelObjectTransform.localScale = Vector3.one;

        // モデルの反転状態を初期化
        _isModelInversion = false;
        ModelInversionButton.GetComponentInChildren<Text>().text = "反転 : OFF";

        Debug.Log("モデルの状態を初期化しました");
    }

    // モデルの状態を固定する
    public void ModelTransformLock()
    {
        if (Model == null)
        {
            return;
        }

        // 機能を切り替える
        _isModelLocked = !_isModelLocked;

        var text = "";

        // 現在の状態に応じてテキストの内容を切り替える
        if (_isModelLocked)
        {
            text = "固定 : ON";
            Debug.Log("モデルを固定化しました");
        }
        else
        {
            text = "固定 : OFF";
            Debug.Log("モデルの固定化を解除しました");
        }

        ModelLockedButton.GetComponentInChildren<Text>().text = text;
    }

    public void SetTimeRate(float timeRate)
    {
        if (_sampler == null)
        {
            return;
        }

        _sampler.SetTimeRate(timeRate);
    }
}
