using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_PlayerAnimation : MonoBehaviour
{
    [SerializeField] private GameObject m_vfxObject = null; 


    public void StepSounds()
    {
        AudioSource _audio = GetComponentInParent<AudioSource>();
        if (_audio) _audio.PlayOneShot(PAF_GameManager.Instance?.SoundDatas.GetStepsPlayer());
    }

    public void CastTrail()
    {
        if (m_vfxObject == null) return;
        m_vfxObject.SetActive(!m_vfxObject.activeInHierarchy); 
    }
}
