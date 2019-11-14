using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PAF_MainMenuMusic : MonoBehaviour
{
    private AudioSource m_audioSource = null; 

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        PAF_GameManager.OnStartCinematic += StopMusic; 
        PAF_GameManager.OnGameEnd += StartMusic;
    }

    private void OnDestroy()
    {
        PAF_GameManager.OnStartCinematic -= StopMusic;
        PAF_GameManager.OnGameEnd -= StartMusic;
    }

    private void StartMusic(int _scoreP1, int _scoreP2) => m_audioSource.Play();

    private void StopMusic() => m_audioSource.Stop();
}
