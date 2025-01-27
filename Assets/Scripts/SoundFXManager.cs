using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject;
    private bool keepPlaying = true;
    private List<AudioSource> playing;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume = 1f)
    {
        // spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClip;

        // assign the volume
        audioSource.volume = volume;

        // play the audioClip
        audioSource.Play();

        // get length of audioClip
        float audioClipLength = audioClip.length;

        // destroy the gameObject after the length of the audioClip
        Destroy(audioSource.gameObject, audioClipLength);

    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume = 1f)
    {
        int randomIndex = Random.Range(0, audioClips.Length);
        AudioClip randomClip = audioClips[randomIndex];
        // spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = randomClip;

        // assign the volume
        audioSource.volume = volume;

        // play the audioClip
        audioSource.Play();

        // get length of audioClip
        float audioClipLength = randomClip.length;

        // destroy the gameObject after the length of the audioClip
        Destroy(audioSource.gameObject, audioClipLength);

    }

    public IEnumerator PlayBackgroundMusic(AudioClip audioclip, float volume = 1f)
    {
        while (keepPlaying)
        {
            Debug.Log("Playing music");
            // spawn in gameObject
            AudioSource audioSource = Instantiate(soundFXObject, Vector3.zero, Quaternion.identity);

            //assign the audioClip
            audioSource.clip = audioclip;

            // assign the volume
            audioSource.volume = volume;

            // play the audioClip
            audioSource.Play();

            // get length of audioClip
            float audioClipLength = audioclip.length;

            // destroy the gameObject after the length of the audioClip
            Destroy(audioSource.gameObject, audioClipLength);

            // wait for the length of the audioClip
            yield return new WaitForSeconds(audioClipLength);
        }
    }

    public void StopMusic()
    {
        keepPlaying = false;
        // stop musics playing
    }

}
