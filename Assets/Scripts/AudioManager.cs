using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public AudioClip ArrowAudioClip, DeathSoundAudioClip;

    public static AudioManager Instance;

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

    private IEnumerator PlaySound(AudioClip clip)
    {
        GameObject go = ObjectPoolerManager.Instance.AudioPooler.GetPooledObject();
        go.SetActive(true);
        go.GetComponent<AudioSource>().PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        go.SetActive(false);
    }
}
