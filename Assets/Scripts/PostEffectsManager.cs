/**
* Copyright(c) Live2D Inc. All rights reserved.
*
* Use of this source code is governed by the Live2D Open Software license
* that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PostEffectsManager : MonoBehaviour
{
    [SerializeField, OptionalLabel("Bloomのスライダー"), Tooltip("Bloomの強さを切り替えるスライダー")]
    public Slider BloomSlider;

    [SerializeField, OptionalLabel("ChromaticAberrationのスライダー"), Tooltip("ChromaticAberrationの強さを切り替えるスライダー")]
    public Slider ChromaticAberrationSlider;

    [SerializeField, OptionalLabel("LensDistortionのスライダー"), Tooltip("LensDistortionの強さを切り替えるスライダー")]
    public Slider LensDistortionSlider;

    [SerializeField, OptionalLabel("Vignetteのスライダー"), Tooltip("Vignetteの強さを切り替えるスライダー")]
    public Slider VignetteSlider;

    #region 色調補正のスライダー

    [SerializeField, OptionalLabel("色相のスライダー"), Tooltip("色相を変化させるためのスライダー")]
    public Slider HueSlider;

    [SerializeField, OptionalLabel("彩度のスライダー"), Tooltip("彩度を変化させるためのスライダー")]
    public Slider SaturationSlider;

    [SerializeField, OptionalLabel("輝度のスライダー"), Tooltip("輝度を変化させるためのスライダー")]
    public Slider BrightnessSlider;

    [SerializeField, OptionalLabel("コントラストのスライダー"), Tooltip("コントラストを変化させるためのスライダー")]
    public Slider ContrastSlider;

    [SerializeField, OptionalLabel("チャンネルミキサーのスライダー"), Tooltip("赤、緑、青の順になるように")]
    public Slider[] ChannelMixerSliders;

    #endregion

    // ポストエフェクトのボリューム
    private PostProcessVolume _postProcessVolume;

    // ポストエフェクトのプロファイル（シーン限定）
    private PostProcessProfile _postProcessProfile;

    // ブルーム
    private Bloom _bloom;
    private float _storeBloom;
    
    // 色収差
    private ChromaticAberration _chromaticAberration;
    private float _storeChromaticAberration;

    // レンズ効果
    private LensDistortion _lensDistortion;
    private float _storeLensDistortion;

    // ビネット
    private Vignette _vignette;
    private float _storeVignette;

    #region 色調補正

    // 色調補正
    private ColorGrading _colorGrading;

    // 色相の値
    private float _storeHue;

    // 彩度の値
    private float _storeSaturation;

    // 輝度の値
    private float _storeBrightness;

    // コントラストの値
    private float _storeContrast;

    // チャンネルミキサーの要素の値
    private float[] _storeChannelMixer;

    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();

        if (!_postProcessVolume.isActiveAndEnabled)
        {
            Debug.Log("PostProcessVolumeが適用されていません");
            return;
        }

        _postProcessProfile = _postProcessVolume.profile;

        // ブルーム
        _bloom = _postProcessProfile.GetSetting<Bloom>();
        _storeBloom = _bloom.intensity.value;

        // 色収差
        _chromaticAberration = _postProcessProfile.GetSetting<ChromaticAberration>();
        _storeChromaticAberration = _chromaticAberration.intensity.value;

        // レンズ効果
        _lensDistortion = _postProcessProfile.GetSetting<LensDistortion>();
        _storeLensDistortion = _lensDistortion.intensity.value;

        // ビネット
        _vignette = _postProcessProfile.GetSetting<Vignette>();
        _storeVignette = _vignette.intensity.value;

        // 色調補正
        _colorGrading = _postProcessProfile.GetSetting<ColorGrading>();
        _storeHue = _colorGrading.hueShift.value;
        _storeSaturation = _colorGrading.saturation.value;
        _storeBrightness = _colorGrading.brightness.value;
        _storeContrast = _colorGrading.contrast.value;
        _storeChannelMixer = new []{ _colorGrading.mixerRedOutRedIn.value, _colorGrading.mixerGreenOutGreenIn, _colorGrading.mixerBlueOutBlueIn };
    }

    // ブルームの強さを設定
    public void SetBloomIntensity(float intensity)
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        _bloom.intensity.value = intensity;
    }

    // 色収差の強さを設定
    public void SetChromaticAberrationIntensity(float intensity)
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        _chromaticAberration.intensity.value = intensity;
    }

    // レンズ効果の強さを設定
    public void SetLensDistortionIntensity(float intensity)
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        _lensDistortion.intensity.value = intensity;
    }

    // ビネットの強さを設定
    public void SetVignetteIntensity(float intensity)
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        _vignette.intensity.value = intensity;
    }

    // ポストエフェクトの設定をリセット
    public void ResetPostEffects()
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        // ブルーム
        BloomSlider.value = _bloom.intensity.value = _storeBloom;

        // 色収差
        ChromaticAberrationSlider.value = _chromaticAberration.intensity.value = _storeChromaticAberration;

        // レンズ効果
        LensDistortionSlider.value = _lensDistortion.intensity.value = _storeLensDistortion;

        // ビネット
        VignetteSlider.value = _vignette.intensity.value = _storeVignette;

        // 色調補正
        HueSlider.value = _colorGrading.hueShift.value = _storeHue;
        SaturationSlider.value = _colorGrading.saturation.value = _storeSaturation;
        BrightnessSlider.value = _colorGrading.brightness.value = _storeBrightness;
        ContrastSlider.value = _colorGrading.contrast.value = _storeContrast;
        ChannelMixerSliders[0].value = _colorGrading.mixerRedOutRedIn.value = _storeChannelMixer[0];
        ChannelMixerSliders[1].value = _colorGrading.mixerGreenOutGreenIn.value = _storeChannelMixer[1];
        ChannelMixerSliders[2].value = _colorGrading.mixerBlueOutBlueIn.value = _storeChannelMixer[2];

        Debug.Log("エフェクトを全てリセットしました");
    }

    public void SetHue(float value)
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        _colorGrading.hueShift.value = value;
    }

    public void SetSaturation(float value)
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        _colorGrading.saturation.value = value;
    }

    public void SetBrightness(float value)
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        _colorGrading.brightness.value = value;
    }

    public void SetContrast(float value)
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        _colorGrading.contrast.value = value;
    }

    public void SetChannelMixerRed(float value)
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        _colorGrading.mixerRedOutRedIn.value = value;
    }

    public void SetChannelMixerGreen(float value)
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        _colorGrading.mixerGreenOutGreenIn.value = value;
    }

    public void SetChannelMixerBlue(float value)
    {
        if (!_postProcessVolume.isActiveAndEnabled)
        {
            return;
        }
        _colorGrading.mixerBlueOutBlueIn.value = value;
    }
}
