using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//controls level 3 logic
public class Level3Manager : MonoBehaviour
{
    public static Level3Manager Instance;

    //inspector heading and fields for UI elements, level info, and delays
    [Header("UI")]
    public GameObject winPanel;
    public TMP_Text winText;
    public TMP_Text streakText;

    [Header("Level Info")]
    public int currentLevelNumber = 3;
    public int nextLevelNumber = 4;
    public string nextLevelSceneName = "Level_04";

    [Header("Delays")]
    public float restartDelay = 1.2f;
    public float nextShotDelay = 1.0f;

    //sets current streak as 0 at the start of the level
    private int currentStreak = 0;
    private bool levelEnded = false;
    private bool progressSaved = false;

    private void Awake()
    {
        Instance = this;
    }

    //once scene starts, it hides the win panel and updates the streak text
    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        UpdateStreakUI();
    }

    //handles the result of each shot 
    public void HandleResult(string result)
    {
        Debug.Log("Level3Manager received result: " + result);

        //stops if level ended 
        if (levelEnded) return;

        //if the result is a goal it increases the steak
        if (result == "GOAL")
        {
            currentStreak++;
            UpdateStreakUI();

            //once player reaches 3 goals in a row it ends level, saves, and shows the win panel 
            if (currentStreak >= 3)
            {
                CancelInvoke();
                levelEnded = true;
                SaveProgressAndShowWinPanel();
            }

            //if player hasn't reached the 3 goals, it prepares the next shot after the short delay set in the inspector 
            else
            {
                Invoke(nameof(PrepareNextShot), nextShotDelay);
            }
        }

        //if the user doesn't score it cancels invoked methods, ends, and restarts the level (without saving)
        else if (result == "MISSED" || result == "SAVED")
        {
            CancelInvoke();
            levelEnded = true;
            Invoke(nameof(RestartLevel), restartDelay);
        }
    }

    //function for saving progress in the firestore and shows the win panel 
    private void SaveProgressAndShowWinPanel()
    {
        if (progressSaved)
        {
            ShowWinPanel();
            return;
        }

        //if firestore progress manager doesn't exist, still shows the win panel, but progress wont be saved
        if (FirestoreProgressManager.Instance == null)
        {
            Debug.LogError("FirestoreProgressManager is missing.");
            ShowWinPanel();
            return;
        }

        //saves completed level and unlocks the next one in firestore
        FirestoreProgressManager.Instance.CompleteLevelAndUnlockNext(currentLevelNumber, nextLevelNumber, success =>
        {
            if (success)
            {
                //saves progress
                progressSaved = true;
                Debug.Log("Level 3 progress saved. Level 4 unlocked.");
            }
            else
            {
                Debug.LogError("Failed to save Level 3 progress.");
            }
            
            //shows win panel 
            ShowWinPanel();
        });
    }

    //resets round for next shot by finding the LevelRoundResetter 
    private void PrepareNextShot()
    {
        FindFirstObjectByType<LevelRoundResetter>()?.ResetRoundForNextShot();
    }

    //if win panel exists show the win panel
    private void ShowWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        //never gets used!
        if (winText != null)
            winText.text = "Amazing! You scored 3 in a row!";
    }

    //checks current streak reference and updates the streak text
    private void UpdateStreakUI()
    {
        if (streakText != null)
            streakText.text = currentStreak + "/3";
    }

    //loads the next level scene, which is level 4
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevelSceneName);
    }

    //restarts the scene through looking at the active scene and loading it again
    public void RestartLevel()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
}