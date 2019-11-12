using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VFXData", menuName = "ScriptableObjects/VFXDataScriptableObject", order = 1)]
public class PAF_VFXData : ScriptableObject
{
    [SerializeField] private ParticleSystem m_hitFX = null; 
    [SerializeField] private ParticleSystem m_splashFX = null; 
    [SerializeField] private ParticleSystem m_fruitFX = null;


    public ParticleSystem HitFX { get { return m_hitFX; } }
    public ParticleSystem SplashFX { get { return m_splashFX; } }
    public ParticleSystem FruitFX { get { return m_fruitFX; } }
}
