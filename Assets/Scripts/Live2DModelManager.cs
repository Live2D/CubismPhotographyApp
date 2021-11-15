using System.Collections;
using System.Collections.Generic;
using Live2D.Cubism.Core;
using UnityEngine;

public class Live2DModelManager : MonoBehaviour
{
    [SerializeField, Tooltip("モデルのプレハブをアタッチする場所")]
    public CubismModel Model;

    #region 定数

    // 処理の対象外とする2点間の距離
    private const float UnprocessedDistance = 0.001f;

    // 最小拡大率
    private const float MinScale = 0.5f;

    // 最大拡大率
    private const float MaxScale = 2.0f;

    // 距離から拡大率を設定する際の変換率
    private const float DistanceToScaleRate = 200.0f;

    #endregion

    #region 変数

    // 生成されたモデルのオブジェクト
    private GameObject _modelObject;

    // 最後に2本指でタッチした時の2点間の距離
    private float _previousDistance;

    // 2本指でタッチし始めた時の2点間の距離
    private float _beginDistance;

    // ピンチインアウトを始めた時の2点間の距離
    private float _moveDistance;

    // モデルに適用する共通の拡大率
    private float _modelScaleRate;

    // モデルの拡大率（代入・取得用）
    private Vector3 _modelScaleVector3;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // モデルがセットされていなかった場合
        if (!Model)
        {
           Debug.LogError("[Live2DModelManager]: Model is empty!!");
           Application.Quit();
        }

        // 変数の初期化
        _previousDistance = 0.0f;
        _beginDistance = 0.0f;
        _moveDistance = 0.0f;
        _modelScaleRate = 0.0f;
        _modelScaleVector3 = Vector3.zero;

        // モデルをシーンへ生成
        _modelObject = Instantiate(Model.gameObject,new Vector3(0,0,0),Model.gameObject.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                //Debug.Log("TouchBegin!");
            }
        }

        // ピンチインアウト処理
        if (Input.touchCount >= 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // 2本指でタッチし始めた
            if (touch2.phase == TouchPhase.Began)
            {
                Debug.Log("開始");

                // 2点間の距離を取得
                _beginDistance = Vector2.Distance(touch1.position, touch2.position);
                
                // 最後に観測した2点間の距離を保存
                _previousDistance = _beginDistance;
            }
            // ピンチインアウト開始
            else if(touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                // 2点間の距離を取得
                _moveDistance = Vector2.Distance(touch1.position, touch2.position);

                // 2点間の距離が一定値以下だった場合は処理を行わない
                if (_beginDistance < UnprocessedDistance || _moveDistance < UnprocessedDistance)
                {
                    return;
                }
                else
                {
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
                }

                // 拡大率を適用（Z値は奥行きであるため、固定）
                _modelObject.transform.localScale = new Vector3(_modelScaleRate, _modelScaleRate, 1.0f);
            }
        }
    }
}
