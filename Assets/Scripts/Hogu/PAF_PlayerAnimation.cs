using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_PlayerAnimation : MonoBehaviour
{
    public void StepSounds()
    {
        AudioSource _audio = GetComponentInParent<AudioSource>();
        if (_audio) _audio.PlayOneShot(PAF_GameManager.Instance?.SoundDatas.GetStepsPlayer());
    }
}
