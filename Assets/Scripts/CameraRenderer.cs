using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRenderer : MonoBehaviour
{
    [SerializeField, Tooltip("カメラの画像を映すオブジェクト")]
    public RawImage CameraRawImage;

    [SerializeField, Tooltip("端末を縦にしたときのカメラ映像の回転角度")]
    public float CamPortraitAngle = -90.0f;

    [SerializeField, Tooltip("端末の上を左に向けたときのカメラ映像の回転角度")]
    public float CamLandscapeNormalAngle = 0.0f;

    [SerializeField, Tooltip("端末の上を右に向けたときのカメラ映像の回転角度")]
    public float CamLandscapeInversionAngle = 180.0f;

    private WebCamTexture _webCamTexture;

    // 登録したデバイスのリスト
    private List<WebCamDevice> _webCamDevices;
    
    //現在選択しているカメラ
    private int _selectCamera = 0;

    // 以前のディスプレイ向き
    DeviceOrientation _previousDeviceOrientation;

    // 現在のディスプレイ向き
    DeviceOrientation _currentDeviceOrientation;

    private void Start()
    {
        Application.RequestUserAuthorization(UserAuthorization.WebCam);
        _webCamTexture = new WebCamTexture();
        CameraRawImage.texture = _webCamTexture;
        _webCamDevices = new List<WebCamDevice>();
    }

    // 端末の向きを取得する
    private DeviceOrientation GetDeviceOrientation()
    {
        // デバイスの向きを取得
        DeviceOrientation deviceOrientation = Input.deviceOrientation;

        // 不明な場合はピクセル数から判断
        if (deviceOrientation == DeviceOrientation.Unknown)
        {
            if (Screen.width < Screen.height)
            {
                deviceOrientation = DeviceOrientation.Portrait;
            }
            else
            {
                deviceOrientation = DeviceOrientation.LandscapeLeft;
            }
        }

        return deviceOrientation;
    }

    private void Update()
    {
        var deviceOrientation = GetDeviceOrientation();

        // 前のフレームと状態が同じなら早期リターン
        if (_currentDeviceOrientation == deviceOrientation)
        {
            return;
        }

        var angles = CameraRawImage.GetComponent<RectTransform>().eulerAngles;

        // 現在の状態を保存
        _previousDeviceOrientation = _currentDeviceOrientation;
        _currentDeviceOrientation = deviceOrientation;

        // 画面の回転に合わせてカメラを回転
        switch (_currentDeviceOrientation)
        {
            // 縦長
            case DeviceOrientation.Portrait:
                angles.z = CamPortraitAngle;
                break;
            case DeviceOrientation.PortraitUpsideDown:
                angles.z = -CamPortraitAngle;
                break;

            //横長
            case DeviceOrientation.LandscapeLeft:
                if (_webCamTexture.deviceName.Contains("Back"))
                {
                    angles.z = CamLandscapeNormalAngle;
                }
                else
                {
                    angles.z = CamLandscapeInversionAngle;
                }
                break;
            case DeviceOrientation.LandscapeRight:
                if (_webCamTexture.deviceName.Contains("Back"))
                {
                    angles.z = CamLandscapeInversionAngle;
                }
                else
                {
                    angles.z = CamLandscapeNormalAngle;
                }
                break;

            // 画面を上にしているか下に向けているならば条件分岐する
            case DeviceOrientation.FaceUp:
            case DeviceOrientation.FaceDown:

                // 以前のデバイスの状態から取得
                switch (_previousDeviceOrientation)
                {
                    // 縦長
                    case DeviceOrientation.Portrait:
                        angles.z = CamPortraitAngle;
                        break;
                    case DeviceOrientation.PortraitUpsideDown:
                        angles.z = -CamPortraitAngle;
                        break;
                    //横長
                    case DeviceOrientation.LandscapeLeft:
                        if (_webCamTexture.deviceName.Contains("Back"))
                        {
                            angles.z = CamLandscapeNormalAngle;
                        }
                        else
                        {
                            angles.z = CamLandscapeInversionAngle;
                        }
                        break;
                    case DeviceOrientation.LandscapeRight:
                        if (_webCamTexture.deviceName.Contains("Back"))
                        {
                            angles.z = CamLandscapeInversionAngle;
                        }
                        else
                        {
                            angles.z = CamLandscapeNormalAngle;
                        }
                        break;
                }
                break;
        }

        CameraRawImage.GetComponent<RectTransform>().eulerAngles = angles;
    }

    //カメラの状態を切り替え
    public void CamStateChange()
    {
        if (!_webCamTexture)
        {
            Debug.LogWarning("デバイスが登録されていません");
            return;
        }

        if (_webCamTexture.isPlaying)
        {
            // カメラを停止
            Debug.Log("カメラを停止しました");
            _webCamTexture.Stop();
        }
        else
        {
            // カメラを開始
            Debug.Log("カメラを開始しました");
            _webCamTexture.Play();
        }
    }

    // カメラ切り替え
    public void ChangeCamera()
    {
        //カメラの個数を取得
        var cameras = _webCamDevices.Count;
        if (cameras < 1)
        {
            // カメラが1台しかなかったら実行せず終了
            return;
        }

        _selectCamera++;
        if (_selectCamera >= cameras) _selectCamera = 0;

        if (!_webCamTexture)
        {
            Debug.LogWarning("デバイスが登録されていません");
            return;
        }
        // カメラを停止
        _webCamTexture.Stop();
        //カメラを変更
        _webCamTexture = new WebCamTexture(_webCamDevices[_selectCamera].name);
        Debug.Log($"現在のカメラ: {_webCamDevices[_selectCamera].name}");

        SetCamSize();

        // 別カメラを開始
        _webCamTexture.Play();
    }

    // デバイス情報を更新
    public void Refresh()
    {
        Debug.Log("デバイス情報を一新します");

        // 条件に合うカメラを探す
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            // 条件
            if (WebCamTexture.devices[i].name.Contains("Remote") && !_webCamDevices.Contains(WebCamTexture.devices[i]))
            {
                // 条件に合うカメラがあったら登録
                _webCamDevices.Add(WebCamTexture.devices[i]);
                Debug.Log($"デバイスを登録しました: {WebCamTexture.devices[i].name}");
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
            Debug.Log($"現在のカメラ: {_webCamDevices[0].name}");

            // カメラのサイズを設定
            SetCamSize();

            // 事故防止用にカメラを停止
            _webCamTexture.Stop();
        }
    }

    // カメラのサイズを設定
    private void SetCamSize()
    {
        // 画面の回転に合わせて映像の大きさを修正
        var sizeDelta = CameraRawImage.rectTransform.sizeDelta;

        switch (_currentDeviceOrientation)
        {
            // 縦長
            case DeviceOrientation.Portrait:
            case DeviceOrientation.PortraitUpsideDown:
                sizeDelta.y = Screen.height;
                sizeDelta.x = Screen.width;
                break;

            // 横長
            case DeviceOrientation.LandscapeLeft:
            case DeviceOrientation.LandscapeRight:
                sizeDelta.x = Screen.width;
                sizeDelta.y = Screen.height;
                break;

            // 画面を上にしているか下に向けているならば条件分岐する
            case DeviceOrientation.FaceUp:
            case DeviceOrientation.FaceDown:

                // 以前のデバイスの状態から取得
                switch (_previousDeviceOrientation)
                {
                    // 縦長
                    case DeviceOrientation.Portrait:
                    case DeviceOrientation.PortraitUpsideDown:
                        sizeDelta.y = Screen.height;
                        sizeDelta.x = Screen.width;
                        break;
                    //横長
                    case DeviceOrientation.LandscapeLeft:
                    case DeviceOrientation.LandscapeRight:
                        sizeDelta.x = Screen.width;
                        sizeDelta.y = Screen.height;
                        break;
                }
                break;
        }

        CameraRawImage.rectTransform.sizeDelta = sizeDelta;

        //カメラの映像をテクスチャへ反映
        CameraRawImage.texture = _webCamTexture;
    }
}