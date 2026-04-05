using UnityEngine;
using UnityEngine.UI;

//settings menu panel and the two different audio sliders for music and sfx
public class SettingsMenuUI : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider musicSlider;
    public Slider sfxSlider;

    //on start the panel is hidden
    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (AudioSettingsManager.Instance != null)
        {
            //if music and sfx slider references exist, it loads the saved volumes
            if (musicSlider != null)
                musicSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.MusicVolume);

            if (sfxSlider != null)
                sfxSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.SFXVolume);
        }
    }

    //for the settings button, it opens the settings panel
    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    //for the close button it hides the settings panel
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }


    //when the music slider value is changed it sends the new music volume to the audio settings manager
    public void OnMusicSliderChanged(float value)
    {
        //for testing
        Debug.Log("Music slider changed: " + value);

        if (AudioSettingsManager.Instance != null)
            AudioSettingsManager.Instance.SetMusicVolume(value);
    }

    //when the sfx slider value is changed it sends the new sfx volume to the audio settings manager
    public void OnSFXSliderChanged(float value)
    {
        //again for testing
        Debug.Log("SFX slider changed: " + value);

        if (AudioSettingsManager.Instance != null)
            AudioSettingsManager.Instance.SetSFXVolume(value);
    }
}
