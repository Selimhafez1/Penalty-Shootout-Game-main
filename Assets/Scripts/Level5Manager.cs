using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//manages the logic of level 5
public class Level5Manager : MonoBehaviour
{
    public static Level5Manager Instance;

    //UI, lev,el info, and delay fields for inspector
    [Header("UI")]
    public GameObject winPanel;
    public TMP_Text winText;
    public TMP_Text streakText;

    [Header("Level Info")]
    public int currentLevelNumber = 5;

    //level 6 was never used since 5 is the last level
    public int nextLevelNumber = 6; 

    //final scene is main menu
    public string nextLevelSceneName = "MainMenu"; 

    [Header("Delays")]
    public float restartDelay = 1.2f;
    public float nextShotDelay = 1.0f;

    //sets current streak to 0
    private int currentStreak = 0;

    //tracks if level ended
    private bool levelEnded = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    //on start it hides the win panel and sets the streak text (0/5)
    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        UpdateStreakUI();
    }

    //similar to level 3 but with 5 goals in a row instead of 3, if the player misses or gets saved it restarts the level after a delay
    //if they score 5 in a row it shows the win panel and allows them to move on to the next level
    public void HandleResult(string result)
    {
        if (levelEnded) return;

        if (result == "GOAL")
        {
            currentStreak++;
            UpdateStreakUI();

            if (currentStreak >= 5)
            {
                CancelInvoke();
                levelEnded = true;
                ShowWinPanel();
            }
            else
            {
                Invoke(nameof(PrepareNextShot), nextShotDelay);
            }
        }
        else if (result == "MISSED" || result == "SAVED")
        {
            CancelInvoke();
            levelEnded = true;
            Invoke(nameof(RestartLevel), restartDelay);
        }
    }

    //prepares next shot by finding the LevelRoundResetter
    private void PrepareNextShot()
    {
        FindFirstObjectByType<LevelRoundResetter>()?.ResetRoundForNextShot();
    }

    //shows win panel
    private void ShowWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        //dont have a win text 
        if (winText != null)
            winText.text = "Incredible! You scored 5 in a row!";
    }

    //checks current streak reference and updates the streak text
    private void UpdateStreakUI()
    {
        if (streakText != null)
            streakText.text = currentStreak + "/5";
    }

    //loads the next level which is assigned as the main menu since level 5 is the last level
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevelSceneName);
    }

    //restarts the level by loading the active scene again
    public void RestartLevel()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
}
