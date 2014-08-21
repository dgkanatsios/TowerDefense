using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public AudioClip ArrowAudioClip, DeathSoundAudioClip;

    public void PlayArrow()
    {
        GameObject go = new GameObject("arrowSound");
        go.AddComponent<AudioSource>();
        go.GetComponent<AudioSource>().PlayOneShot(ArrowAudioClip);
        Destroy(go, ArrowAudioClip.length);
    }

    public void PlayDeathSound()
    {
        GameObject go = new GameObject("deathSound");
        go.AddComponent<AudioSource>();
        go.GetComponent<AudioSource>().PlayOneShot(DeathSoundAudioClip);
        Destroy(go, DeathSoundAudioClip.length);
    }
}
