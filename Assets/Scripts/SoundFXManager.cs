using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioClip[] backgroundMusicClip;  // For managing multiple background music tracks
    private List<AudioSource> backgroundMusicSources;
    private List<AudioSource> playingSounds;  // To track all currently playing sound effects
    [SerializeField] private AudioSource soundFXObject;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);  // Ensure only one instance of SoundFXManager exists
        }

        playingSounds = new List<AudioSource>();
        backgroundMusicSources = new List<AudioSource>(); 
    }

    // Play a background music clip on a specific track (0, 1, or 2)
    public void PlayBackgroundMusic(int trackIndex, float volume = 1f, bool loop = true)
    {
        if (trackIndex < 0 || trackIndex >= backgroundMusicClip.Length)
        {
            Debug.LogError("Invalid track index.");
            return;
        }
        AudioSource audioSource;
        AudioClip clip = backgroundMusicClip[trackIndex];
        if (trackIndex >= backgroundMusicSources.Count)
        {
            audioSource = Instantiate(soundFXObject, Vector3.zero, Quaternion.identity);
            audioSource.clip = clip;
        } else
        {
            audioSource = backgroundMusicSources[trackIndex];
            // Stop any currently playing music on this track
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        // Set up the new track
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = loop;

        // Start playing the music
        audioSource.Play();
       
    }

    // Stop a specific background track (0, 1, or 2)
    public void StopBackgroundMusic(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= backgroundMusicSources.Count)
        {
            Debug.LogError("Invalid track index.");
            return;
        }

        AudioSource audioSource = backgroundMusicSources[trackIndex];
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Stop all background music tracks
    public void StopAllBackgroundMusic()
    {
        foreach (var audioSource in backgroundMusicSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    // Fade out a specific background track (0, 1, or 2) over a set duration
    public IEnumerator FadeOutBackgroundMusic(int trackIndex, float fadeDuration)
    {
        if (trackIndex < 0 || trackIndex >= backgroundMusicSources.Count)
        {
            Debug.LogError("Invalid track index.");
            yield break;
        }

        AudioSource audioSource = backgroundMusicSources[trackIndex];
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();  // Stop music after fade-out
    }

    // Fade in a specific background track (0, 1, or 2) over a set duration
    public IEnumerator FadeInBackgroundMusic(int trackIndex, float fadeDuration, float targetVolume = 1f)
    {
        if (trackIndex < 0 || trackIndex >= backgroundMusicSources.Count)
        {
            Debug.LogError("Invalid track index.");
            yield break;
        }

        AudioSource audioSource = backgroundMusicSources[trackIndex];
        audioSource.volume = 0f;  // Start with volume 0
        audioSource.loop = true;

        audioSource.Play();

        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;  // Ensure the final volume is set
    }

    // Play a single sound effect clip
    public IEnumerator PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume = 1f)
    {
        // Spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        // Assign the audioClip
        audioSource.clip = audioClip;

        // Assign the volume
        audioSource.volume = volume;

        // Play the audioClip
        audioSource.Play();

        // Add to playing list
        playingSounds.Add(audioSource);

        // Get length of audioClip
        float audioClipLength = audioClip.length;

        // Destroy the gameObject after the length of the audioClip
        yield return new WaitForSeconds(audioClipLength);

        // Remove from playing list and destroy
        playingSounds.Remove(audioSource);
        Destroy(audioSource.gameObject);
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
        playingSounds.Add(audioSource);

        // get length of audioClip
        float audioClipLength = randomClip.length;

        // destroy the gameObject after the length of the audioClip
        playingSounds.Remove(audioSource);
        Destroy(audioSource.gameObject, audioClipLength);
    }

    // Stop all currently playing sound effects
    public void StopAllSoundFX()
    {
        foreach (var audioSource in playingSounds)
        {
            audioSource.Stop();
            Destroy(audioSource.gameObject);  // Optionally destroy the sound effect after stopping
        }
        playingSounds.Clear();
    }
}
