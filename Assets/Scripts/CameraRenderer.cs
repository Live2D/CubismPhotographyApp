using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRenderer : MonoBehaviour
{
    [SerializeField, Tooltip("カメラの画像を映すオブジェクト")]
    public RawImage CameraRawImage;

    WebCamTexture _webCamTexture;

    List<WebCamDevice> _webCamDevices;
    int _selectCamera = 0;

    void Start()
    {
        Application.RequestUserAuthorization(UserAuthorization.WebCam);
        _webCamTexture = new WebCamTexture();
        CameraRawImage.texture = _webCamTexture;
        _webCamDevices = new List<WebCamDevice>();
        Vector3 angles = CameraRawImage.GetComponent<RectTransform>().eulerAngles;
        angles.z = -90;

        CameraRawImage.GetComponent<RectTransform>().eulerAngles = angles;
    }

    //カメラの状態を切り替え
    public void CamStateChange()
    {
        if (!_webCamTexture)
        {
            return;
        }

        if (_webCamTexture.isPlaying)
        {
            // カメラを停止
            _webCamTexture.Stop();
        }
        else
        {
            // カメラを開始
            _webCamTexture.Play();
        }
    }

    // カメラ切り替え
    public void ChangeCamera()
    {
        //カメラの個数を取得
        int cameras = _webCamDevices.Count;
        if (cameras < 1)
        {
            // カメラが1台しかなかったら実行せず終了
            return;
        }

        _selectCamera++;
        if (_selectCamera >= cameras) _selectCamera = 0;

        if (!_webCamTexture)
        {
            return;
        }
        // カメラを停止
        _webCamTexture.Stop();
        //カメラを変更
        _webCamTexture = new WebCamTexture(_webCamDevices[_selectCamera].name);
        
        SetCamSize();

        // 別カメラを開始
        _webCamTexture.Play();
    }

    // デバイス情報を更新
    public void Refresh()
    {
        // 条件に合うカメラを探す
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            // 条件
            if (WebCamTexture.devices[i].name.Contains("Remote") && !_webCamDevices.Contains(WebCamTexture.devices[i]))
            {
                // 条件に合うカメラがあったら登録
                _webCamDevices.Add(WebCamTexture.devices[i]);
            }
        }

        // 条件に合うデバイス数が1つでも登録されていたなら処理を始める
        if (_webCamDevices.Count >0)
        {
            if (_webCamTexture)
            {
                // カメラを停止
                _webCamTexture.Stop();
            }

            //カメラを変更
            _webCamTexture = new WebCamTexture(_webCamDevices[0].name);

            // カメラのサイズを設定
            SetCamSize();

            // 事故防止用にカメラを停止
            _webCamTexture.Stop();
        }
    }

    // カメラのサイズを設定
    private void SetCamSize()
    {
        // 縦長の画面に映像の大きさを修正
        CameraRawImage.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);

        //カメラの映像をテクスチャへ反映
        CameraRawImage.texture = _webCamTexture;
    }
}