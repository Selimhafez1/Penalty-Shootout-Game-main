using UnityEngine;

//manages the music in the menu
public class MenuMusicManager : MonoBehaviour
{
    public static MenuMusicManager Instance;

    //array of music tracks that play in the menu
    public AudioClip[] playlist;

    //reference the audio source
    private AudioSource audioSource;

    //stores which song is currently playing and whether music is allowed to play
    private int currentIndex = 0;
    private bool musicAllowed = true;

    //once object is created, it checks if an instance exists, and if it does it deletes the duplicate
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        //keeps object alive across different scenes
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            Debug.LogError("MenuMusicManager: No AudioSource found.");
    }

    //runs once scene starts 
    private void Start()
    {
        Debug.Log("MenuMusicManager Start");

        //checks that the playlist has one song at least
        if (playlist != null && playlist.Length > 0)
        {
            //randomizes which song starts first in the playlist, and also applies the saved music volume before playing the song
            currentIndex = Random.Range(0, playlist.Length);
            ApplySavedVolume();
            PlayCurrentSong();
        }
        else
        {
            Debug.LogError("MenuMusicManager: Playlist is empty.");
        }
    }

    private void Update()
    {
        //if music isn't allowed then it stops the song 
        if (!musicAllowed) return;

        if (audioSource == null || playlist == null || playlist.Length == 0) return;

        //if current song finishes, it plays the next song 
        if (!audioSource.isPlaying)
        {
            PlayNextSong();
        }
    }

    //plays current song in the playlist
    private void PlayCurrentSong()
    {
        if (audioSource == null || playlist == null || playlist.Length == 0) return;
        //stops if music isn't allowed 
        if (!musicAllowed) return;

        //sets song as current audio source clip
        audioSource.clip = playlist[currentIndex];
        Debug.Log("Playing song: " + audioSource.clip.name + " at volume " + audioSource.volume);
        //plays song
        audioSource.Play();
    }

    //moves to the next song in the playlist
    private void PlayNextSong()
    {
        //increases index in playlist 
        currentIndex++;
        if (currentIndex >= playlist.Length)
            //loops back to the first song
            currentIndex = 0;

        //plays the new current song
        PlayCurrentSong();
    }


    //plays music if its allowed, and applies the saved volume before playing
    public void PlayMusic()
    {
        musicAllowed = true;
        ApplySavedVolume();

        if (audioSource != null && !audioSource.isPlaying)
            PlayCurrentSong();
    }

    //stops music from playing 
    public void StopMusic()
    {
        musicAllowed = false;

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    //applies the default volume to the audio source
    public void ApplyVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    //applies the new saved volume to the audio source 
    private void ApplySavedVolume()
    {
        if (AudioSettingsManager.Instance != null)
        {
            ApplyVolume(AudioSettingsManager.Instance.MusicVolume);
        }
        else
        {
            //runs default if no saved volume is found
            Debug.LogWarning("AudioSettingsManager.Instance is null, defaulting music volume to 1");
            ApplyVolume(1f);
        }
    }
}
