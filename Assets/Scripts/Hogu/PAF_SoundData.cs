using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObjects/SoundDataScriptableObject", order = 1)]
public class PAF_SoundData : ScriptableObject
{
    [SerializeField] AudioClip[] steps = null;
    [SerializeField] AudioClip[] hitPlayer = null;
    [SerializeField] AudioClip[] hitBulb = null;
    [SerializeField] AudioClip[] hitFruit = null;
    [SerializeField] AudioClip[] hitNone = null;
    [SerializeField] AudioClip[] hitWall = null;
    [SerializeField] AudioClip[] fallPlayer = null;
    [SerializeField] AudioClip[] fruitBounce = null;
    [SerializeField] AudioClip[] fruitSpawn = null;
    [SerializeField] AudioClip[] bulbExplode = null;
    [SerializeField] AudioClip[] dalleFalling = null;
    [SerializeField] AudioClip[] plantEating = null;
    [SerializeField] AudioClip[] plantGloups = null;
    [SerializeField] AudioClip highScore = null;
    [SerializeField] AudioClip selectMenu = null;
    [SerializeField] AudioClip crowdCheer = null;
    [SerializeField] AudioClip whistle = null; 
    [SerializeField] AudioClip ticTic = null; 

    public AudioClip GetHitPlayer() => hitPlayer[Random.Range(0, hitPlayer.Length)];

    public AudioClip GetFallPlayer() => fallPlayer[Random.Range(0, fallPlayer.Length)];

    public AudioClip GetStepsPlayer() => steps[Random.Range(0, steps.Length)];

    public AudioClip GetHitWall() => hitWall[Random.Range(0, hitWall.Length)];

    public AudioClip GetHitFruit() => hitFruit[Random.Range(0, hitFruit.Length)];

    public AudioClip GetHitBulb() => hitBulb[Random.Range(0, hitBulb.Length)];

    public AudioClip GetHitNone() => hitNone[Random.Range(0, hitNone.Length)];

    public AudioClip GetFruitBounce() => fruitBounce[Random.Range(0, fruitBounce.Length)];

    public AudioClip GetBulbExploding() => bulbExplode[Random.Range(0, bulbExplode.Length)];

    public AudioClip GetDalleFalling() => dalleFalling[Random.Range(0, dalleFalling.Length)];

    public AudioClip GetPlantEating() => plantEating[Random.Range(0, plantEating.Length)];

    public AudioClip GetSelectMenu() => selectMenu;

    public AudioClip GetCrowdCheer() => crowdCheer;

    public AudioClip GetFruitSpawn() => fruitSpawn[Random.Range(0, fruitSpawn.Length)];

    public AudioClip GetflowerGloups() => plantGloups[Random.Range(0, plantGloups.Length)];

    public AudioClip GetHighscoreSound() => highScore;

    public AudioClip GetTicTic() => ticTic;

    public AudioClip GetWhistle() => whistle;
}
