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

    public ParticleSystem HitFX { get { return m_hitFX; } }
    public ParticleSystem SplashFX { get { return m_splashFX; } }
    public ParticleSystem FruitFX { get { return m_fruitFX; } }
    public ParticleSystem BulbFX { get { return m_bulbFX; } }
    public GameObject ConfusedFX { get { return m_confusedFX; } }
    public ParticleSystem StepSmokeFX { get { return m_stepSmokeFX; } }
}
