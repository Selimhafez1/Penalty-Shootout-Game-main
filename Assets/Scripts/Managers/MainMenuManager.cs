using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //once player presses the play button, it loads the login scene 
    public void PlayGame()
    {
        SceneManager.LoadScene("LoginScene");
    }
}