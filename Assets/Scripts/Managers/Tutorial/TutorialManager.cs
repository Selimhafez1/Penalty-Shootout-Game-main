using UnityEngine;
using TMPro;

//tutorial manager for first level same process as previous tutorial managers
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public GameObject tutorialPanel;
    public TMP_Text tutorialText;

    private int tutorialStep = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        ShowWelcomeMessage();
    }

    private void Update()
    {
        // Step 0: waiting for first tap anywhere on screen
        if (tutorialStep == 0 && Input.GetMouseButtonDown(0))
        {
            ShowTapGoalMessage();
        }
    }

    private void ShowWelcomeMessage()
    {
        tutorialStep = 0;
        tutorialPanel.SetActive(true);
        tutorialText.text = "Welcome to the tutorial, let me teach you how to play. TAP HERE!";
    }

    private void ShowTapGoalMessage()
    {
        tutorialStep = 1;
        tutorialText.text = "Tap anywhere in the goal! This is how you aim your shot!";
    }

    public void OnGoalTapped()
    {
        if (tutorialStep != 1) return;

        tutorialStep = 2;
        tutorialText.text = "Press the shoot button! DON'T forget to always time green. Or at least try to :)";
    }

    public void OnPlayerShot()
    {
        if (tutorialStep != 2) return;

        tutorialStep = 3;
        tutorialPanel.SetActive(false);
    }
}