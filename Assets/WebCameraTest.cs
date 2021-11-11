using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCameraTest : MonoBehaviour
{

    public RawImage rawImage;

    WebCamTexture webCamTexture;

    List<WebCamDevice> webCamDevices;
    int selectCamera = 0;

    void Start()
    {
        Application.RequestUserAuthorization(UserAuthorization.WebCam);
        webCamTexture = new WebCamTexture();
        rawImage.texture = webCamTexture;
        webCamDevices = new List<WebCamDevice>();
        Vector3 angles = rawImage.GetComponent<RectTransform>().eulerAngles;
        angles.z = -90;

        rawImage.GetComponent<RectTransform>().eulerAngles = angles;
    }

    //カメラの状態を切り替え
    public void CamStateChange()
    {
        if (!webCamTexture)
        {
            return;
        }

        if (webCamTexture.isPlaying)
        {
            // カメラを停止
            webCamTexture.Stop();
        }
        else
        {
            // カメラを開始
            webCamTexture.Play();
        }
    }

    // カメラ切り替え
    public void ChangeCamera()
    {
        //カメラの個数を取得
        int cameras = webCamDevices.Count;
        if (cameras < 1)
        {
            // カメラが1台しかなかったら実行せず終了
            return;
        }

        selectCamera++;
        if (selectCamera >= cameras) selectCamera = 0;

        if (!webCamTexture)
        {
            return;
        }
        // カメラを停止
        webCamTexture.Stop();
        //カメラを変更
        webCamTexture = new WebCamTexture(webCamDevices[selectCamera].name);
        
        SetCamSize();

        // 別カメラを開始
        webCamTexture.Play();
    }

    // デバイス情報を更新
    public void Refresh()
    {
        // 条件に合うカメラを探す
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            // 条件
            if (WebCamTexture.devices[i].name.Contains("Remote") && !webCamDevices.Contains(WebCamTexture.devices[i]))
            {
                // 条件に合うカメラがあったら登録
                webCamDevices.Add(WebCamTexture.devices[i]);
            }
        }

        // 条件に合うデバイス数が1つでも登録されていたなら処理を始める
        if (webCamDevices.Count >0)
        {
            if (webCamTexture)
            {
                // カメラを停止
                webCamTexture.Stop();
            }

            //カメラを変更
            webCamTexture = new WebCamTexture(webCamDevices[0].name);

            // カメラのサイズを設定
            SetCamSize();

            // 事故防止用にカメラを停止
            webCamTexture.Stop();
        }
    }

    // カメラのサイズを設定
    private void SetCamSize()
    {
        // 縦長の画面に映像の大きさを修正
        rawImage.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);

        //カメラの映像をテクスチャへ反映
        rawImage.texture = webCamTexture;
    }
}