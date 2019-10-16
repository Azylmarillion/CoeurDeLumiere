using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_SoundManager : MonoBehaviour
{
    public static PAF_SoundManager I { get; private set; }

    #region AudioClips
    [Space, Header("AudioClips")]
    [SerializeField] AudioClip steps = null;
    [SerializeField] AudioClip hitPlayer1 = null;
    [SerializeField] AudioClip hitPlayer2 = null;
    [SerializeField] AudioClip fallPlayer1 = null;
    [SerializeField] AudioClip fallPlayer2 = null;
    [SerializeField] AudioClip hitBulb = null;
    [SerializeField] AudioClip hitFruit = null;
    [SerializeField] AudioClip player1Attack = null;
    [SerializeField] AudioClip player2Attack = null;
    [SerializeField] AudioClip[] score = null;
    #endregion

    #region AudioSources
    [Space, Header("AudioSources")]
    [SerializeField] AudioSource plantCoordonates = null;
    [SerializeField] AudioSource player1CrowdCoordonates = null;
    [SerializeField] AudioSource player2CrowdCoordonates = null;
    [SerializeField] AudioSource cameraCoordonates = null;
    [SerializeField] AudioSource music = null;
    #endregion

    GameObject musicObject = null;

    public bool IsReady => music && steps && hitPlayer1 && hitPlayer2 && player2Attack && hitBulb && hitFruit && player1Attack && score.Length > 0 && plantCoordonates && player1CrowdCoordonates && player2CrowdCoordonates && cameraCoordonates;



    private void Awake()
    {
        I = this;
    }


    public void StartMusic()
    {
        if (!IsReady) return;
        music.Play();
    }

    public void StopMusic()
    {
        if (!IsReady) return;
        music.Stop();
    }

    public void PauseMusic()
    {
        if (!IsReady) return;
        if (music.isPlaying) music.Pause();
        else music.Play();
    }

    public void PlaySteps(Vector3 _pos)
    {
        if (!IsReady) return;
        AudioSource.PlayClipAtPoint(steps, _pos);
    }

    public void PlayHitPlayer(Vector3 _pos, bool _isPlayer1)
    {
        if (!IsReady) return;
        AudioSource.PlayClipAtPoint(_isPlayer1 ? hitPlayer1 : hitPlayer2, _pos);
    }

    public void PlayFallSound(Vector3 _pos, bool _isPlayer1)
    {
        if (!IsReady) return;
        AudioSource.PlayClipAtPoint(_isPlayer1 ? fallPlayer1 : fallPlayer2, _pos);
    }

    public void PlayHitBulb(Vector3 _pos)
    {
        if (!IsReady) return;
        AudioSource.PlayClipAtPoint(hitBulb, _pos);
    }

    public void PlayHitFruit(Vector3 _pos)
    {
        if (!IsReady) return;
        AudioSource.PlayClipAtPoint(hitFruit, _pos);
    }

    public void PlayPlayerAttack(Vector3 _pos, bool _isPlayer1)
    {
        if (!IsReady) return;
        AudioSource.PlayClipAtPoint(_isPlayer1 ? player1Attack : player2Attack, _pos);
    }

    public void PlayPlantEat()
    {
        if (!IsReady) return;
        if (plantCoordonates.isPlaying) plantCoordonates.Stop();
        plantCoordonates.Play();
    }

    public void PlayScore(bool _isPlayer1)
    {
        if (!IsReady) return;
        foreach (AudioClip _audio in score)
        {
            if (_isPlayer1) player1CrowdCoordonates.PlayOneShot(_audio);
            else player2CrowdCoordonates.PlayOneShot(_audio);
        }
    }
}
