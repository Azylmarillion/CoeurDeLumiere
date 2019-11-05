using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_Crowd : MonoBehaviour
{
    [SerializeField] bool team1 = true;

    [SerializeField] AudioSource audioSource = null;

    public void ScorePoint()
    {
        if(audioSource)
        {
            AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetCrowdCheer();
            if (_clip) audioSource.PlayOneShot(_clip);
        }
        // play FX & Animation 
    }


}
