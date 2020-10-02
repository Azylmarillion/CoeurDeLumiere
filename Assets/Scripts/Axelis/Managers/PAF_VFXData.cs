using UnityEngine;

[CreateAssetMenu(fileName = "VFXData", menuName = "ScriptableObjects/VFXDataScriptableObject", order = 1)]
public class PAF_VFXData : ScriptableObject
{
    [SerializeField] private ParticleSystem m_hitFX = null; 
    [SerializeField] private ParticleSystem m_splashFX = null; 
    [SerializeField] private ParticleSystem m_fruitFX = null;
    [SerializeField] private ParticleSystem m_bulbFX = null;
    [SerializeField] private GameObject m_confusedFX = null;
    [SerializeField] private ParticleSystem m_stepSmokeFX = null;

    public ParticleSystem HitFX => m_hitFX;
    public ParticleSystem SplashFX => m_splashFX;
    public ParticleSystem FruitFX => m_fruitFX;
    public ParticleSystem BulbFX => m_bulbFX;
    public GameObject ConfusedFX => m_confusedFX;
    public ParticleSystem StepSmokeFX => m_stepSmokeFX;
}
