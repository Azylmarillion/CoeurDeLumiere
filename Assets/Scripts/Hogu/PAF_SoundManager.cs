using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_SoundManager : MonoBehaviour
{
    public static PAF_SoundManager I { get; private set; }

    #region AudioClips
    [Space, Header("AudioClips")]
    [SerializeField] AudioClip[] hitPlayer1 = null;
    [SerializeField] AudioClip[] hitPlayer2 = null;
    [SerializeField] AudioClip[] player1Attack = null;
    [SerializeField] AudioClip[] player2Attack = null;
    [SerializeField] AudioClip[] score = null;
    [SerializeField] AudioClip[] steps = null;
    [SerializeField] AudioClip fallPlayer1 = null;
    [SerializeField] AudioClip fallPlayer2 = null;
    [SerializeField] AudioClip hitBulb = null;
    [SerializeField] AudioClip hitFruit = null;
    [SerializeField] AudioClip hitNone = null;
    [SerializeField] AudioClip hitWall = null;
    [SerializeField] AudioClip fruitBounce = null;
    [SerializeField] AudioClip bulbExplode = null;
    #endregion

    #region AudioSources
    [Space, Header("AudioSources")]
    [SerializeField] AudioSource plantCoordonates = null;
    [SerializeField] AudioSource player1CrowdCoordonates = null;
    [SerializeField] AudioSource player2CrowdCoordonates = null;
    [SerializeField] AudioSource cameraCoordonates = null;
    [SerializeField] AudioSource music = null;
    #endregion
    

    public bool IsReady => music && steps.Length > 0 && hitPlayer1.Length > 0 && hitPlayer2.Length > 0 && player2Attack.Length > 0 && hitBulb && hitFruit && fruitBounce && player1Attack.Length > 0 && score.Length > 0 && plantCoordonates && player1CrowdCoordonates && player2CrowdCoordonates && cameraCoordonates && hitNone && hitWall && bulbExplode;



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
        int _rnd = Random.Range(0, steps.Length);
        AudioSource.PlayClipAtPoint(steps[_rnd], _pos);
    }

    public void PlayHitPlayer(Vector3 _pos, bool _isPlayer1)
    {
        if (!IsReady) return;
        StartCoroutine(PlayClips(_isPlayer1 ? hitPlayer2 : hitPlayer1, _pos));
    }

    public void PlayFallSound(Vector3 _pos, bool _isPlayer1)
    {
        if (!IsReady) return;
        AudioSource.PlayClipAtPoint(_isPlayer1 ? fallPlayer1 : fallPlayer2, _pos);
    }

    public void PlayFruitBounce(Vector3 _pos)
    {
        if (!IsReady) return;
        AudioSource.PlayClipAtPoint(fruitBounce, _pos);
    }

    public void PlayPlayerAttack(Vector3 _pos, AttackType _touched, bool _isPlayer1)
    {
        if (!IsReady) return;
        AudioClip _clip = null;
        switch (_touched)
        {
            case AttackType.Fruit:
                _clip = hitFruit;
                break;
            case AttackType.Player:
                StartCoroutine(PlayClips(_isPlayer1 ? hitPlayer2 : hitPlayer1, _pos));
                break;
            case AttackType.Bulb:
                _clip = hitBulb;
                break;
            case AttackType.None:
                _clip = hitNone;
                break;
            case AttackType.Wall:
                _clip = hitWall;
                break;
            default:
                break;
        }
        if(_clip) AudioSource.PlayClipAtPoint(_clip, _pos);
    }

    public void PlayBulbExplode(Vector3 _pos)
    {
        if (!IsReady) return;
        AudioSource.PlayClipAtPoint(bulbExplode, _pos);
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
        StartCoroutine(PlayClips(score, transform.position));
    }

    IEnumerator PlayClips(AudioClip[] _clips, Vector3 _pos)
    {
        for (int i = 0; i < _clips.Length; i++)
        {
            AudioSource.PlayClipAtPoint(_clips[i], _pos);
            yield return new WaitForSeconds(_clips[i].length);
        }
    }

}

public enum AttackType
{
    Fruit,
    Player,
    Bulb,
    None,
    Wall
}