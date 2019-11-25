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
            PlayerPrefs.SetFloat("ContrastIntensity", value); 
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
            PlayerPrefs.SetFloat("SaturationIntensity", value);
        }
    }



    [Header("Option Sliders")]
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
        //Get Saturation Player Prefs
        if (PlayerPrefs.HasKey("SaturationIntensity"))
        {
            m_settings.basic.saturation = PlayerPrefs.GetFloat("SaturationIntensity"); 
        }
        else
        {
            PlayerPrefs.SetFloat("SaturationIntensity", m_settings.basic.saturation); 
        }

        //Get Contrast Player Prefs
        if (PlayerPrefs.HasKey("ContrastIntensity"))
        {
            m_settings.basic.contrast = PlayerPrefs.GetFloat("ContrastIntensity");
        }
        else
        {
            PlayerPrefs.SetFloat("ContrastIntensity", m_settings.basic.contrast);
        }
        optionPPProfile.colorGrading.settings = m_settings;
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