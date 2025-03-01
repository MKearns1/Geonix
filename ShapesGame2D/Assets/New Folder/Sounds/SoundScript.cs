using UnityEngine;
using UnityEngine.Audio;

public class SoundScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    public float volume;
    public bool Loop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = Loop;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource != null)
        {
            if (!audioSource.isPlaying)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
