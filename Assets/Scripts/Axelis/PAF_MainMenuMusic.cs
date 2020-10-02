using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PAF_MainMenuMusic : MonoBehaviour
{
    public static PAF_MainMenuMusic Instance = null;

    private AudioSource m_audioSource = null; 

    private void Awake()
    {
        Instance = this;
        m_audioSource = GetComponent<AudioSource>();
    }

    public void StartMusic() => m_audioSource.Play();

    public void StopMusic() => m_audioSource.Stop();
}
