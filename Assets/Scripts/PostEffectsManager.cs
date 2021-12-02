using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PostEffectsManager : MonoBehaviour
{
    [SerializeField, CustomLabel("Bloomのスライダー"), Tooltip("Bloomの強さを切り替えるスライダー")]
    public Slider BloomSlider;

    [SerializeField, CustomLabel("ChromaticAberrationのスライダー"), Tooltip("ChromaticAberrationの強さを切り替えるスライダー")]
    public Slider ChromaticAberrationSlider;

    [SerializeField, CustomLabel("LensDistortionのスライダー"), Tooltip("LensDistortionの強さを切り替えるスライダー")]
    public Slider LensDistortionSlider;

    [SerializeField, CustomLabel("Vignetteのスライダー"), Tooltip("Vignetteの強さを切り替えるスライダー")]
    public Slider VignetteSlider;

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

    // 色調調整
    private ColorGrading _colorGrading;
    private float[] _storeColorGradingParameters;

    // レンズ効果
    private LensDistortion _lensDistortion;
    private float _storeLensDistortion;

    // ビネット
    private Vignette _vignette;
    private float _storeVignette;

    // Start is called before the first frame update
    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();

        if (_postProcessVolume == null)
        {
            return;
        }

        _postProcessProfile = _postProcessVolume.profile;

        // ブルーム
        _bloom = _postProcessProfile.GetSetting<Bloom>();
        _storeBloom = _bloom.intensity.value;

        // 色収差
        _chromaticAberration = _postProcessProfile.GetSetting<ChromaticAberration>();
        _storeChromaticAberration = _chromaticAberration.intensity.value;

        _colorGrading = _postProcessProfile.GetSetting<ColorGrading>();

        // レンズ効果
        _lensDistortion = _postProcessProfile.GetSetting<LensDistortion>();
        _storeLensDistortion = _lensDistortion.intensity.value;

        // ビネット
        _vignette = _postProcessProfile.GetSetting<Vignette>();
        _storeVignette = _vignette.intensity.value;
    }

    // ブルームの強さを設定
    public void SetBloomIntensity(float intensity)
    {
        var intensityParameter = _bloom.intensity;
        intensityParameter.value = intensity;
        _bloom.intensity = intensityParameter;
    }

    // 色収差の強さを設定
    public void SetChromaticAberrationIntensity(float intensity)
    {
        var intensityParameter = _chromaticAberration.intensity;
        intensityParameter.value = intensity;
        _chromaticAberration.intensity = intensityParameter;
    }

    // レンズ効果の強さを設定
    public void SetLensDistortionIntensity(float intensity)
    {
        var intensityParameter = _lensDistortion.intensity;
        intensityParameter.value = intensity;
        _lensDistortion.intensity = intensityParameter;
    }

    // ビネットの強さを設定
    public void SetVignetteIntensity(float intensity)
    {
        var intensityParameter = _vignette.intensity;
        intensityParameter.value = intensity;
        _vignette.intensity = intensityParameter;
    }

    // ポストエフェクトの設定をリセット
    public void ResetPostEffects()
    {
        // ブルーム
        BloomSlider.value = _bloom.intensity.value = _storeBloom;

        // 色収差
        ChromaticAberrationSlider.value = _chromaticAberration.intensity.value = _storeChromaticAberration;

        // レンズ効果
        LensDistortionSlider.value = _lensDistortion.intensity.value = _storeLensDistortion;

        // ビネット
        VignetteSlider.value = _vignette.intensity.value = _storeVignette;

        Debug.Log("エフェクトをリセットしました");
    }
}
