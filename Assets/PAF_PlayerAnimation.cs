using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_PlayerAnimation : MonoBehaviour
{
    public void StepSounds()
    {
        PAF_SoundData _data = (PAF_SoundData)Resources.Load("Data/Sounds");
        AudioSource _audio = GetComponentInParent<AudioSource>();
        if (_data && _audio) _audio.PlayOneShot(_data.GetStepsPlayer());
    }
}
