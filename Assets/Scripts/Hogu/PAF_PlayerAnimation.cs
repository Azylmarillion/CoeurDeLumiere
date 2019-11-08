using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_PlayerAnimation : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_system = null; 


    public void StepSounds()
    {
        AudioSource _audio = GetComponentInParent<AudioSource>();
        if (_audio) _audio.PlayOneShot(PAF_GameManager.Instance?.SoundDatas.GetStepsPlayer());
    }

    public void CastTrail()
    {
        if (m_system == null) return;
        m_system.Play(); 
    }
}
