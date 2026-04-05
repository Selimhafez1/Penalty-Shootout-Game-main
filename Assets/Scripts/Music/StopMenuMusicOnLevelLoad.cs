using UnityEngine;

//stops music when gameplay starts
public class StopMenuMusicOnLevelLoad : MonoBehaviour
{
    //if menumusicmanager exists stop the music
    private void Start()
    {
        if (MenuMusicManager.Instance != null)
        {
            MenuMusicManager.Instance.StopMusic();
        }
    }
}
