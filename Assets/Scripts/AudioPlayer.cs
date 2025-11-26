using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
