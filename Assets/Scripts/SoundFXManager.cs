using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioClip[] backgroundMusicClip;  // For managing multiple background music tracks
    private AudioSource[] backgroundMusicSources;
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
        backgroundMusicSources = new AudioSource[3];  // Three tracks for background music
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
        if (backgroundMusicSources[trackIndex] == null)
        {
            audioSource = Instantiate(soundFXObject, Vector3.zero, Quaternion.identity);
            audioSource.clip = clip;
            backgroundMusicSources[trackIndex] = audioSource;
            playingSounds.Add(audioSource);
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
        if (trackIndex < 0 || trackIndex >= 3)
        {
            Debug.LogError("Invalid track index.");
            return;
        }

        AudioSource audioSource = backgroundMusicSources[trackIndex];
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            playingSounds.Remove(audioSource);
            Destroy(audioSource.gameObject); // Destroy it
            backgroundMusicSources[trackIndex] = null;
        }
    }

    // Stop all background music tracks
    public void StopAllBackgroundMusic()
    {
        foreach (var audioSource in backgroundMusicSources)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
                playingSounds.Remove(audioSource);
                Destroy(audioSource.gameObject); // Destroy it
            }
        }
        backgroundMusicSources = new AudioSource[3];  // Reset the array
    }

    public void StartFadeOutBackgroundMusic(int trackIndex, float fadeDuration)
    {
        StartCoroutine(FadeOutBackgroundMusic(trackIndex, fadeDuration));
    }

    // Fade out a specific background track (0, 1, or 2) over a set duration
    private IEnumerator FadeOutBackgroundMusic(int trackIndex, float fadeDuration)
    {
        if (trackIndex < 0 || trackIndex >= 3)
        {
            Debug.LogError("Invalid track index.");
            yield break;
        }
        if (backgroundMusicSources[trackIndex] == null)
        {
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
        playingSounds.Remove(audioSource);
        Destroy(audioSource.gameObject); // Destroy it
        backgroundMusicSources[trackIndex] = null;
    }

 
    public void StartFadeInBackgroundMusic(int trackIndex, float fadeDuration, float targetVolume = 1f)
    {
        StartCoroutine(FadeInBackgroundMusic(trackIndex, fadeDuration, targetVolume));
    }

    private IEnumerator FadeInBackgroundMusic(int trackIndex, float fadeDuration, float targetVolume = 1f)
    {
        if (trackIndex < 0 || trackIndex >= backgroundMusicClip.Length)
        {
            Debug.LogError("Invalid track index.");
            yield break;
        }

        // Instantiate a new AudioSource if it doesn't already exist
        AudioSource audioSource;
        if (backgroundMusicSources[trackIndex] == null)
        {
            audioSource = Instantiate(soundFXObject, Vector3.zero, Quaternion.identity);
            backgroundMusicSources[trackIndex] = audioSource; // Add to the list
            playingSounds.Add(audioSource);
        }
        else
        {
            audioSource = backgroundMusicSources[trackIndex];
        }

        // Set the clip and other properties
        audioSource.clip = backgroundMusicClip[trackIndex];
        audioSource.volume = 0f;  // Start with volume at 0
        audioSource.loop = true;

        // Play the audio
        audioSource.Play();

        // Gradually increase the volume to the target volume over the fade duration
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t / fadeDuration);
            yield return null;
        }

        // Ensure the final volume is set correctly
        audioSource.volume = targetVolume;
    }


    // Play a single sound effect clip
    public IEnumerator PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume = 1f)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        playingSounds.Add(audioSource);

        float audioClipLength = audioClip.length;

        yield return new WaitForSeconds(audioClipLength);

        playingSounds.Remove(audioSource);
        Destroy(audioSource.gameObject);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume = 1f)
    {
        int randomIndex = Random.Range(0, audioClips.Length);
        AudioClip randomClip = audioClips[randomIndex];
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = randomClip;

        audioSource.volume = volume;

        audioSource.Play();
        playingSounds.Add(audioSource);

        float audioClipLength = randomClip.length;

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
