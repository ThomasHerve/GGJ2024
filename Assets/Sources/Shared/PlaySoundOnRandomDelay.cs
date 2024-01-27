using UnityEngine;
using System.Collections;

public class PlaySoundOnRandomDelay : MonoBehaviour
{
    [SerializeField]
    public AudioClip[] sounds;
    [SerializeField]
    public float minDelay = 1.0f;
    [SerializeField]
    public float maxDelay = 5.0f;
    [SerializeField]
    public AudioSource audioSource;

    void Start()
    {
        StartCoroutine(PlayRandomSoundWithDelay());
    }

    IEnumerator PlayRandomSoundWithDelay()
    {
        while (true)
        {
            // Wait for a random delay between minDelay and maxDelay
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            // Play a random sound from the array
            if (sounds.Length > 0)
            {
                int randomIndex = Random.Range(0, sounds.Length);
                audioSource.PlayOneShot(sounds[randomIndex]);
            }
        }
    }
}

