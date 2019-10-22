using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObjects/SoundDataScriptableObject", order = 1)]
public class SoundData : ScriptableObject
{
    [SerializeField] AudioClip[] hitPlayer = null;
    [SerializeField] AudioClip[] steps = null;
    [SerializeField] AudioClip[] hitBulb = null;
    [SerializeField] AudioClip[] hitFruit = null;
    [SerializeField] AudioClip[] hitNone = null;
    [SerializeField] AudioClip[] hitWall = null;
    [SerializeField] AudioClip[] fallPlayer = null;


    public AudioClip GetHitPlayer()
    {
        if (hitPlayer.Length > 0) return hitPlayer[Random.Range(0, hitPlayer.Length)];
        return null;
    }
    public AudioClip GetFallPlayer()
    {
        if (fallPlayer.Length > 0) return fallPlayer[Random.Range(0, fallPlayer.Length)];
        return null;
    }
    public AudioClip GetStepsPlayer()
    {
        if (steps.Length > 0) return steps[Random.Range(0, steps.Length)];
        return null;
    }
    public AudioClip GetHitWall()
    {
        if (hitWall.Length > 0) return hitWall[Random.Range(0, hitWall.Length)];
        return null;
    }
    public AudioClip GetHitFruit()
    {
        if (hitFruit.Length > 0) return hitFruit[Random.Range(0, hitFruit.Length)];
        return null;
    }
    public AudioClip GetHitBulb()
    {
        if (hitBulb.Length > 0) return hitBulb[Random.Range(0, hitBulb.Length)];
        return null;
    }
    public AudioClip GetHitNone()
    {
        if (hitNone.Length > 0) return hitNone[Random.Range(0, hitNone.Length)];
        return null;
    }
}
