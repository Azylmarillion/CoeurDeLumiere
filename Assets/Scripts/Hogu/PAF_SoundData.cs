using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObjects/SoundDataScriptableObject", order = 1)]
public class PAF_SoundData : ScriptableObject
{
    [SerializeField] AudioClip[] steps = null;// plugged
    [SerializeField] AudioClip[] hitPlayer = null; // plugged
    [SerializeField] AudioClip[] hitBulb = null;// plugged
    [SerializeField] AudioClip[] hitFruit = null;// plugged
    [SerializeField] AudioClip[] hitNone = null;// plugged
    [SerializeField] AudioClip[] hitWall = null;// plugged
    [SerializeField] AudioClip[] fallPlayer = null;// plugged
    [SerializeField] AudioClip[] fruitBounce = null;// plugged
    [SerializeField] AudioClip[] fruitSpawn = null;// plugged
    [SerializeField] AudioClip[] bulbExplode = null;// plugged
    [SerializeField] AudioClip[] dalleFalling = null;// plugged
    [SerializeField] AudioClip[] plantEating = null;// plugged
    [SerializeField] AudioClip moveMenu = null;
    [SerializeField] AudioClip selectMenu = null;
    [SerializeField] AudioClip crowdCheer = null;// plugged



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
    public AudioClip GetFruitBounce()
    {
        if (fruitBounce.Length > 0) return fruitBounce[Random.Range(0, fruitBounce.Length)];
        return null;
    }
    public AudioClip GetBulbExploding()
    {
        if (bulbExplode.Length > 0) return bulbExplode[Random.Range(0, bulbExplode.Length)];
        return null;
    }
    public AudioClip GetDalleFalling()
    {
        if (dalleFalling.Length > 0) return dalleFalling[Random.Range(0, dalleFalling.Length)];
        return null;
    }
    public AudioClip GetPlantEating()
    {
        if (plantEating.Length > 0) return plantEating[Random.Range(0, plantEating.Length)];
        return null;
    }
    public AudioClip GetMoveMenu()
    {
        if (moveMenu) return moveMenu;
        return null;
    }
    public AudioClip GetSelectMenu()
    {
        if (selectMenu) return selectMenu;
        return null;
    }
    public AudioClip GetCrowdCheer()
    {
        if (crowdCheer) return crowdCheer;
        return null;
    }
    public AudioClip GetFruitSpawn()
    {
        if (fruitSpawn.Length > 0) return fruitSpawn[Random.Range(0, fruitSpawn.Length)];
        return null;
    }
}
