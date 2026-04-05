using UnityEngine;

public class StartMenuMusicOnMenuLoad : MonoBehaviour
{
    //if music manager exists play or resume the music
    private void Start()
    {
        if (MenuMusicManager.Instance != null)
            MenuMusicManager.Instance.PlayMusic();
    }
}
