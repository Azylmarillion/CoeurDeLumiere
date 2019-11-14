using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PAF_Whistle : MonoBehaviour
{
    private AudioSource m_source = null; 

    private void Awake()
    {
        PAF_GameManager.OnGameStart += Whistle;
        PAF_GameManager.OnGameEnd += Whistle;
        m_source = GetComponent<AudioSource>(); 
    }

    private void OnDestroy()
    {
        PAF_GameManager.OnGameStart -= Whistle;
        PAF_GameManager.OnGameEnd -= Whistle;
    }

    private void Whistle()
    {
        if (!m_source) return;
        AudioClip _clip = PAF_GameManager.Instance.SoundDatas.Whistle;
        if (!_clip) return;
        m_source.PlayOneShot(_clip, 1.0f); 
    }

    private void Whistle(int _playerOneScore, int _playerTwoScore)
    {
        if (!m_source) return;
        AudioClip _clip = PAF_GameManager.Instance.SoundDatas.Whistle;
        if (!_clip) return;
        m_source.PlayOneShot(_clip, 1.0f);
    }
}
