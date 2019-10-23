using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_Crowd : MonoBehaviour
{
    [SerializeField] bool team1 = true;

    [SerializeField] AudioSource audioSource = null;
    [SerializeField] PAF_SoundData soundData = null;


    public void ScorePoint()
    {
        if(audioSource && soundData)
        {
            AudioClip _clip = soundData.GetCrowdCheer();
            if (_clip) audioSource.PlayOneShot(_clip);
        }
        // play FX & Animation 
    }


}
