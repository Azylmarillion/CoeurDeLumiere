using UnityEngine.UI; 
using UnityEngine;
using UnityEngine.PostProcessing;

public class PAF_PostProcessOptions : MonoBehaviour
{
    #region Fields / Properties
    [Space]
    [SerializeField]
    PostProcessingProfile optionPPProfile = null;
    ColorGradingModel.Settings m_settings; 

    //Contrast Properties
    [SerializeField] private float m_contrastIntensity = 1.0f; 
    public float ContrastIntensity
    {
        get { return m_contrastIntensity;  }
        set
        {
            m_contrastIntensity = value;
            if( optionPPProfile != null)
            {
                m_settings.basic.contrast = m_contrastIntensity;
                optionPPProfile.colorGrading.settings = m_settings;
            }
        }
    }

    //Exposure Properties
    [SerializeField] private float m_exposureIntensity = 1.0f;
    public float ExposureIntensity
    {
        get { return m_exposureIntensity; }
        set
        {
            m_exposureIntensity = value;
            if(optionPPProfile != null)
            {
                m_settings.basic.postExposure = m_contrastIntensity;
                optionPPProfile.colorGrading.settings = m_settings;
            }
        }
    }    
    
    //Saturation Properties
    [SerializeField] private float m_saturationIntensity = 1.0f;
    public float SaturationIntensity
    {
        get { return m_saturationIntensity; }
        set
        {
            m_saturationIntensity = value;
            
            if(optionPPProfile != null)
            {
                m_settings.basic.saturation = SaturationIntensity;
                optionPPProfile.colorGrading.settings = m_settings;
            }

        }
    }



    [Header("Option Sliders")]
    [SerializeField] private Slider m_exposureSlider = null;
    [SerializeField] private Slider m_contrastSlider = null;
    [SerializeField] private Slider m_saturationSlider = null;
    #endregion

    #region Methods
    #region Original Methods   
    void Init()
    {
        /*
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
        */

        m_settings = optionPPProfile.colorGrading.settings;
        if(m_exposureSlider)m_exposureSlider.value = m_settings.basic.postExposure;
        if (m_contrastSlider) m_contrastSlider.value = m_settings.basic.contrast;
        if (m_saturationSlider) m_saturationSlider.value = m_settings.basic.saturation; 
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