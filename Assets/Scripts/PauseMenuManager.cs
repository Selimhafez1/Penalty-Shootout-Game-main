using UnityEngine;
using UnityEngine.SceneManagement;

//handles the pause menu in game
public class PauseMenuManager : MonoBehaviour
{
    //references the pause panel
    public GameObject pausePanel;
    //name of the scene to load when player clicks the button to go back to the level select menu
    public string levelSelectSceneName = "LevelSelect";

    //tracks whether the game is currently paused or not
    private bool isPaused = false;

    //once scene starts it hides the pause panel
    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        //game runs at normal speed at the start of the level
        Time.timeScale = 1f;
        //paused state always starts as false
        isPaused = false;
    }

    //pauses the game by showing the pause panel
    public void PauseGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        //sets game speed to 0 so pauses the game as well
        Time.timeScale = 0f;
        //marks game as paused 
        isPaused = true;
    }

    //resume button hides the pause panel and sets the speed of the game back to normal which resumes it, and is paused is false again
    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    //for the back to menu button, leaves the scene and goes to the level select scene
    public void GoToLevelsMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSelectSceneName);
    }
}