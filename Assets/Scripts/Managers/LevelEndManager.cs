using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//level end manager that handles winning and losing conditions for the levels 
public class LevelEndManager : MonoBehaviour
{
    public static LevelEndManager Instance;

    public GameObject winPanel;
    public TMP_Text winText;

    [Header("Level Info")]
    public int currentLevelNumber = 1;
    public int nextLevelNumber = 2;
    public string nextLevelSceneName = "Level_02";

    [Header("Restart Delay")]
    public float restartDelay = 1.5f;

    private bool levelEnded = false;
    private bool progressSaved = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    //no difficult win condition, the player just has to score a goal
    public void HandleResult(string result)
    {
        if (levelEnded) return;
        levelEnded = true;

        if (result == "GOAL")
        {
            SaveProgressAndShowWinPanel();
        }
        else if (result == "MISSED" || result == "SAVED")
        {
            Invoke(nameof(RestartLevel), restartDelay);
        }
    }

    private void SaveProgressAndShowWinPanel()
    {
        if (progressSaved)
        {
            ShowWinPanel();
            return;
        }

        if (FirestoreProgressManager.Instance == null)
        {
            Debug.LogError("FirestoreProgressManager is missing.");
            ShowWinPanel();
            return;
        }

        FirestoreProgressManager.Instance.CompleteLevelAndUnlockNext(currentLevelNumber, nextLevelNumber, success =>
        {
            if (success)
            {
                progressSaved = true;
                Debug.Log("Level progress saved successfully.");
            }
            else
            {
                Debug.LogError("Failed to save level progress.");
            }

            ShowWinPanel();
        });
    }

    private void ShowWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        if (winText != null)
            winText.text = "Level Complete!";
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevelSceneName);
    }

    public void RestartLevel()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
}