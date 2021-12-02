using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostEffectsManager : MonoBehaviour
{
    // ポストエフェクトのボリューム
    private PostProcessVolume _postProcessVolume;

    // ポストエフェクトのプロファイル（シーン限定）
    private PostProcessProfile _postProcessProfile;

    // ブルーム
    private Bloom _bloom;
    
    // 色収差
    private ChromaticAberration _chromaticAberration;

    // 色調調整
    private ColorGrading _colorGrading;

    // レンズ効果
    private LensDistortion _lensDistortion;

    // ビネット
    private Vignette _vignette;

    // Start is called before the first frame update
    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();

        if (_postProcessVolume == null)
        {
            return;
        }

        _postProcessProfile = _postProcessVolume.profile;
        _bloom = _postProcessProfile.GetSetting<Bloom>();
        _chromaticAberration = _postProcessProfile.GetSetting<ChromaticAberration>();
        _colorGrading = _postProcessProfile.GetSetting<ColorGrading>();
        _lensDistortion = _postProcessProfile.GetSetting<LensDistortion>();
        _vignette = _postProcessProfile.GetSetting<Vignette>();
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

    // Update is called once per frame
    private void Update()
    {
        
    }
}
