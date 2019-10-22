//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PAF_SoundManager : MonoBehaviour
//{
//    public static PAF_SoundManager I { get; private set; }

//    [SerializeField, Range(0, 100)] int soundLevel = 50; 

//    #region AudioClips
//    [Space, Header("AudioClips")]
//    [SerializeField] AudioClip[] hitPlayer = null;
//    [SerializeField] AudioClip[] steps = null;
//    [SerializeField] AudioClip[] hitBulb = null;
//    [SerializeField] AudioClip[] hitFruit = null;
//    [SerializeField] AudioClip[] hitNone = null;
//    [SerializeField] AudioClip[] hitWall = null;
//    [SerializeField] AudioClip[] fallPlayer = null;
//    [SerializeField] AudioClip[] fruitBounce = null;
//    [SerializeField] AudioClip[] bulbExplode = null;
//    [SerializeField] AudioClip[] dalleFalling = null;
//    [SerializeField] AudioClip moveMenu = null;
//    [SerializeField] AudioClip selectMenu = null;
//    #endregion

//    #region AudioSources
//    [Space, Header("AudioSources")]
//    [SerializeField] AudioSource plantCoordonates = null;
//    [SerializeField] AudioSource player1CrowdCoordonates = null;
//    [SerializeField] AudioSource player2CrowdCoordonates = null;
//    [SerializeField] AudioSource music = null;
//    #endregion
    


//    private void Awake()
//    {
//        I = this;
//    }


//    public void StartMusic()
//    {
//        if (!music) return;
//        music.Play();
//    }

//    public void StopMusic()
//    {
//        if (!music) return;
//        music.Stop();
//    }

//    public void PauseMusic()
//    {
//        if (!music) return;
//        if (music.isPlaying) music.Pause();
//        else music.Play();
//    }

//    public void PlaySteps(Vector3 _pos)
//    {
//        if (steps.Length <= 0) return;
//        int _rnd = Random.Range(0, steps.Length);
//        AudioSource.PlayClipAtPoint(steps[_rnd], _pos, soundLevel / 100f);
//    }

//    public void PlayHitPlayer(Vector3 _pos)
//    {
//        if (hitPlayer.Length <= 0) return;
//        int _rnd = Random.Range(0, hitPlayer.Length);
//        AudioSource.PlayClipAtPoint(hitPlayer[_rnd], _pos, soundLevel / 100f);
//    }

//    public void PlayFallSound(Vector3 _pos)
//    {
//        if (fallPlayer.Length <= 0) return;
//        AudioSource.PlayClipAtPoint(fallPlayer[Random.Range(0, fallPlayer.Length)], _pos, soundLevel / 100f);
//    }

//    public void PlayFruitBounce(Vector3 _pos)
//    {
//        if (fruitBounce.Length <= 0) return;
//        AudioSource.PlayClipAtPoint(fruitBounce[Random.Range(0, fruitBounce.Length)], _pos, soundLevel / 100f);
//    }

//    public void PlayPlayerAttack(Vector3 _pos, AttackType _touched)
//    {
//        AudioClip _clip = null;
//        switch (_touched)
//        {
//            case AttackType.Fruit:
//                if (hitFruit.Length <= 0) break;
//                _clip = hitFruit[Random.Range(0, hitFruit.Length)];
//                break;
//            case AttackType.Player:
//                if (hitPlayer.Length <= 0) break;
//                _clip = hitPlayer[Random.Range(0, hitPlayer.Length)];
//                break;
//            case AttackType.Bulb:
//                if (hitBulb.Length <= 0) break;
//                _clip = hitBulb[Random.Range(0, hitBulb.Length)];
//                break;
//            case AttackType.None:
//                if (hitNone.Length <= 0) break;
//                _clip = hitNone[Random.Range(0, hitNone.Length)];
//                break;
//            case AttackType.Wall:
//                if (hitWall.Length <= 0) break;
//                _clip = hitWall[Random.Range(0, hitWall.Length)];
//                break;
//            default:
//                break;
//        }
//        if(_clip) AudioSource.PlayClipAtPoint(_clip, _pos, soundLevel / 100f);
//    }

//    public void PlayBulbExplode(Vector3 _pos)
//    {
//        if (bulbExplode.Length <= 0) return;
//        AudioSource.PlayClipAtPoint(bulbExplode[Random.Range(0, bulbExplode.Length)], _pos, soundLevel / 100f);
//    }
    
//    public void PlayDalleFalling(Vector3 _pos)
//    {
//        if (dalleFalling.Length <= 0) return;
//        AudioSource.PlayClipAtPoint(dalleFalling[Random.Range(0, dalleFalling.Length)], _pos, soundLevel / 100f);
//    }

//    public void PlayPlantEat()
//    {
//        if (!plantCoordonates) return;
//        if (plantCoordonates.isPlaying) plantCoordonates.Stop();
//        plantCoordonates.Play();
//    }

//    public void PlayScore(bool _isPlayer1)
//    {
//        if (!player1CrowdCoordonates) return;
//        if (_isPlayer1) player1CrowdCoordonates.Play();
//        else player2CrowdCoordonates.Play();
//    }

//    public void PlayMenuMove()
//    {
//        if (!moveMenu || !music) return;
//        AudioSource.PlayClipAtPoint(moveMenu, music.transform.position);
//    }

//    public void PlayMenuSelected()
//    {
//        if (!selectMenu || !music) return;
//        AudioSource.PlayClipAtPoint(selectMenu, music.transform.position);
//    }

//}

//public enum AttackType
//{
//    Fruit,
//    Player,
//    Bulb,
//    None,
//    Wall
//}