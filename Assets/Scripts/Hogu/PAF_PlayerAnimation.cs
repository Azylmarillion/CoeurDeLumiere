using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_PlayerAnimation : MonoBehaviour
{
    [SerializeField] private AudioSource audio = null;
    [SerializeField] private GameObject m_vfxObject = null;
    [SerializeField] private PAF_Player m_player = null;

    public void StepSounds()
    {
        // Run FX
        ParticleSystem _system = PAF_GameManager.Instance?.VFXDatas?.StepSmokeFX;
        if (_system) Instantiate(_system.gameObject, transform.position, Quaternion.identity);

        if (!audio)
        {
            audio = GetComponentInParent<AudioSource>();
            if (!audio) return;
        }
        audio.PlayOneShot(PAF_GameManager.Instance?.SoundDatas.GetStepsPlayer(), .25f);
    }

    public void CastTrail()
    {
        if (m_vfxObject == null) return;
        m_vfxObject.SetActive(!m_vfxObject.activeInHierarchy); 
    }

    public void Interact()
    {
        if (!m_player)
        {
            m_player = GetComponentInParent<PAF_Player>();
            if (!m_player) return;
        }

        m_player.Interact();
    }
}
