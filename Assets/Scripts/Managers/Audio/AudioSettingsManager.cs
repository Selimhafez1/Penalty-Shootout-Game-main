using UnityEngine;

//for saving and applying music and sound effect volume settings
public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance;

    //default is full volume for both music and sfx
    public float MusicVolume { get; private set; } = 1f; 
    public float SFXVolume { get; private set; } = 1f;

    //to save and load using PlayerPrefs
    private const string MusicKey = "MusicVolume";
    private const string SFXKey = "SFXVolume";

    //checks, deletes duplicates, and loads saved volumes on awake
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolumes();
    }

    //cleans up the singleton reference after it's destoryed
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    //Loads the saved volumes from PlayerPrefs
    private void LoadVolumes()
    {
        MusicVolume = PlayerPrefs.GetFloat(MusicKey, 1f);
        SFXVolume = PlayerPrefs.GetFloat(SFXKey, 1f);
    }

    //sets and saves the music volume 
    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(MusicKey, MusicVolume);
        PlayerPrefs.Save();

        //checks if menumusicmanager exists and then applies the new music volume.
        if (MenuMusicManager.Instance != null)
            MenuMusicManager.Instance.ApplyVolume(MusicVolume);
    }

    //sets and saves the sfx volume
    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(SFXKey, SFXVolume);
        PlayerPrefs.Save();
    }
}
