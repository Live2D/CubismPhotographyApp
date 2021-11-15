using System.Collections;
using System.Collections.Generic;
using Live2D.Cubism.Core;
using UnityEngine;

public class Live2DModelSpawner : MonoBehaviour
{
    [SerializeField,Tooltip("モデルのプレハブをアタッチする場所")]
    public CubismModel _model;

    // 最後に2本指でタッチした場所の距離
    private float previousDistance;

    // Start is called before the first frame update
    void Start()
    {
        // モデルがセットされていなかった場合
        if (!_model)
        {
           Debug.LogError("[Live2DModelSpawner]: _model is empty!!");
           Application.Quit();
        }

        previousDistance = 0.0f;

        // モデルをシーンへ生成
        Instantiate(_model.gameObject,new Vector3(0,0,0),_model.gameObject.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        // ピンチインアウト処理
        if (Input.touchCount >= 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            if (t2.phase == TouchPhase.Began)
            {
                //Debug.Log("t2TouchBegin!");
            }
        }
    }
}
