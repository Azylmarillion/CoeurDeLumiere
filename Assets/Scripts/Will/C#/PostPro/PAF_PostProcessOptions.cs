using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PAF_PostProcessOptions : MonoBehaviour
{
    #region Fields / Properties
    [Space]
    [SerializeField]
        PostProcessingProfile optionPPProfile = null;
    //Contrast Properties
        ColorGradingModel.Settings contrast;
        public float ContrastIntensity = 1f;
    //Exposure Properties
        ColorGradingModel.Settings exposure;
        public float ExposureIntensity = 0f;
    //Saturation Properties
        ColorGradingModel.Settings saturation;
        public float SaturationIntensity = 1f;
    #endregion

    #region Methods
    #region Original Methods   
    void Init()
    {
        //Contrast
        contrast = optionPPProfile.colorGrading.settings;
        contrast.basic.saturation = ContrastIntensity;
        optionPPProfile.colorGrading.settings = contrast;
        //Exposure
        exposure = optionPPProfile.colorGrading.settings;
        exposure.basic.saturation = ExposureIntensity;
        optionPPProfile.colorGrading.settings = exposure;
        //Saturation
        saturation = optionPPProfile.colorGrading.settings;
        saturation.basic.saturation = SaturationIntensity;
        optionPPProfile.colorGrading.settings = saturation;
    }

    public void SetSaturationValue()
    {
        SaturationIntensity = Mathf.Clamp(SaturationIntensity, 0, 2);
        
    }
    #endregion

    #region Unity Methods
    void Start()
    {
        Init();
    }
    #endregion
    #endregion
}