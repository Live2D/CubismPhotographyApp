using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRenderer : MonoBehaviour
{
    [SerializeField, CustomLabel("カメラ映像スクリーン"), Tooltip("カメラの画像を映すオブジェクト")]
    public RawImage CameraRawImage;

    [SerializeField, CustomLabel("外カメラ角度（縦）"), Tooltip("端末を縦にしたときの外カメラ映像の回転角度")]
    public float CamPortraitAngleOutCam = -90.0f;

    [SerializeField, CustomLabel("内カメラ角度（縦）"), Tooltip("端末を縦にしたときの内カメラ映像の回転角度")]
    public float CamPortraitAngleInCam = -90.0f;

    [SerializeField, CustomLabel("外カメラ角度（横）"), Tooltip("端末の外側のカメラ映像の回転角度")]
    public float CamLandscapeAngleOutCam = 0.0f;

    [SerializeField, CustomLabel("内カメラ角度（横）"), Tooltip("端末の内側のカメラ映像の回転角度")]
    public float CamLandscapeAngleInCam = 180.0f;

    [SerializeField, CustomLabel("判定用のカメラ名"), Tooltip("iOSは「Back」、Androidは「1」に設定（鍵括弧は要りません）")]
    public string JudgeCamName = "Back";

    // 映像反転用定数
    private static float _inversionAngle = 180.0f;

    private WebCamTexture _webCamTexture;

    // 登録したデバイスのリスト
    private List<WebCamDevice> _webCamDevices;
    
    //現在選択しているカメラ
    private int _selectCamera = 0;

    // 以前のディスプレイ向き
    private DeviceOrientation _previousDeviceOrientation;

    // 現在のディスプレイ向き
    private DeviceOrientation _currentDeviceOrientation;

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

        // 現在の状態を保存
        _previousDeviceOrientation = _currentDeviceOrientation;
        _currentDeviceOrientation = deviceOrientation;

        // 画面の回転に合わせて映像の大きさや角度を修正
        SetCameraRawImageProperty();
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

        //カメラの映像をテクスチャへ反映
        CameraRawImage.texture = _webCamTexture;

        // 画面の回転に合わせて映像の大きさや角度を修正
        SetCameraRawImageProperty();

        // 別カメラを開始
        _webCamTexture.Play();
    }

    // デバイス情報を更新
    public void Refresh()
    {
        Debug.Log("デバイス情報を更新します");

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

            //カメラの映像をテクスチャへ反映
            CameraRawImage.texture = _webCamTexture;

            // 画面の回転に合わせて映像の大きさや角度を修正
            SetCameraRawImageProperty();

            // 事故防止用にカメラを停止
            _webCamTexture.Stop();
        }
    }

    // 画面の回転に合わせて映像の大きさや角度を修正
    private void SetCameraRawImageProperty()
    {
        // カメラのサイズを設定
        var sizeDelta = CameraRawImage.rectTransform.sizeDelta;
        var angles = CameraRawImage.rectTransform.eulerAngles;

        // 画面の回転に合わせてサイズと映像の角度を変更
        switch (_currentDeviceOrientation)
        {
            // 縦長
            case DeviceOrientation.Portrait:
                angles.z = _webCamTexture.deviceName.Contains(JudgeCamName)
                    ? CamPortraitAngleOutCam
                    : CamPortraitAngleInCam;

                switch (_previousDeviceOrientation)
                {
                    case DeviceOrientation.FaceUp:
                    case DeviceOrientation.FaceDown:
                    case DeviceOrientation.PortraitUpsideDown:
                        sizeDelta.x = Screen.height;
                        sizeDelta.y = Screen.width;
                        break;
                    default:
                        sizeDelta.x = Screen.width;
                        sizeDelta.y = Screen.height;
                        break;
                }
                break;
            case DeviceOrientation.PortraitUpsideDown:
                angles.z = _webCamTexture.deviceName.Contains(JudgeCamName)
                    ? CamPortraitAngleOutCam
                    : CamPortraitAngleInCam;
                angles.z += _inversionAngle;

                switch (_previousDeviceOrientation)
                {
                    case DeviceOrientation.FaceUp:
                    case DeviceOrientation.FaceDown:
                    case DeviceOrientation.Portrait:
                        sizeDelta.x = Screen.height;
                        sizeDelta.y = Screen.width;
                        break;
                    default:
                        sizeDelta.x = Screen.width;
                        sizeDelta.y = Screen.height;
                        break;
                }
                break;

            //横長
            case DeviceOrientation.LandscapeLeft:
                angles.z = _webCamTexture.deviceName.Contains(JudgeCamName)
                    ? CamLandscapeAngleOutCam
                    : CamLandscapeAngleInCam;

                // 以前のデバイスの状態から取得
                switch (_previousDeviceOrientation)
                {
                    case DeviceOrientation.FaceUp:
                    case DeviceOrientation.FaceDown:
                    case DeviceOrientation.LandscapeRight:
                        sizeDelta.x = Screen.width;
                        sizeDelta.y = Screen.height;
                        break;
                    default:
                        sizeDelta.x = Screen.height;
                        sizeDelta.y = Screen.width;
                        break;
                }
                break;
            case DeviceOrientation.LandscapeRight:
                angles.z = _webCamTexture.deviceName.Contains(JudgeCamName)
                    ? CamLandscapeAngleOutCam
                    : CamLandscapeAngleInCam;
                angles.z += _inversionAngle;

                // 以前のデバイスの状態から取得
                switch (_previousDeviceOrientation)
                {
                    case DeviceOrientation.FaceUp:
                    case DeviceOrientation.FaceDown:
                    case DeviceOrientation.LandscapeLeft:
                        sizeDelta.x = Screen.width;
                        sizeDelta.y = Screen.height;
                        break;
                    default:
                        sizeDelta.x = Screen.height;
                        sizeDelta.y = Screen.width;
                        break;
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
                        angles.z = _webCamTexture.deviceName.Contains(JudgeCamName)
                            ? CamPortraitAngleOutCam
                            : CamPortraitAngleInCam;

                        sizeDelta.x = Screen.height;
                        sizeDelta.y = Screen.width;
                        break;
                    case DeviceOrientation.PortraitUpsideDown:
                        angles.z = _webCamTexture.deviceName.Contains(JudgeCamName)
                            ? CamPortraitAngleOutCam
                            : CamPortraitAngleInCam;
                        angles.z += _inversionAngle;

                        sizeDelta.x = Screen.height;
                        sizeDelta.y = Screen.width;
                        break;
                    //横長
                    case DeviceOrientation.LandscapeLeft:
                        angles.z = _webCamTexture.deviceName.Contains(JudgeCamName)
                            ? CamLandscapeAngleOutCam
                            : CamLandscapeAngleInCam;

                        sizeDelta.x = Screen.width;
                        sizeDelta.y = Screen.height;
                        break;
                    case DeviceOrientation.LandscapeRight:
                        angles.z = _webCamTexture.deviceName.Contains(JudgeCamName)
                            ? CamLandscapeAngleOutCam
                            : CamLandscapeAngleInCam;
                        angles.z += _inversionAngle;

                        sizeDelta.x = Screen.width;
                        sizeDelta.y = Screen.height;
                        break;
                }
                break;
        }

        CameraRawImage.rectTransform.eulerAngles = angles;

        // サイズの変更
        CameraRawImage.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}