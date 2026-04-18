using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //to target 90 frames per sec 
    private void Awake()
    {
        Application.targetFrameRate = 90;
    }

    //once player presses the play button, it loads the login scene 
    public void PlayGame()
    {
        SceneManager.LoadScene("LoginScene");
    }
}