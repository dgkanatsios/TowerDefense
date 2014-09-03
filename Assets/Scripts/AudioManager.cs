using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public AudioClip ArrowAudioClip, DeathSoundAudioClip;

    /// <summary>
    /// Basic singleton implementation
    /// </summary>
    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void PlayArrowSound()
    {
        StartCoroutine(PlaySound(ArrowAudioClip));
    }

    public void PlayDeathSound()
    {
        StartCoroutine(PlaySound(DeathSoundAudioClip));
    }

    //coroutine is used since we also want to deactivate it after the sound is played
    private IEnumerator PlaySound(AudioClip clip)
    {
        //get an object from the pooler, activate it, play the sound
        //wait for sound completion and then deactivate the object
        GameObject go = ObjectPoolerManager.Instance.AudioPooler.GetPooledObject();
        go.SetActive(true);
        go.GetComponent<AudioSource>().PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        go.SetActive(false);
    }
}
