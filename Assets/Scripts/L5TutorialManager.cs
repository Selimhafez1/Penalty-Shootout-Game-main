using UnityEngine;
using TMPro;

//same process as level 2, 3 and 4 but with different messages for level 5
public class L5TutorialManager : MonoBehaviour
{
    public static L5TutorialManager Instance;

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
            Instance = null;
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
        tutorialText.text = "You might think you're smart! But I'm always one step ahead! TAP HERE!";
    }

    private void ShowTapGoalMessage()
    {
        tutorialStep = 1;
        tutorialText.text = "I've been keeping track of your shots, I know what you like and what you don't! You'll never be able to score 5 in a row now!";
    }

    public void OnPlayerShot()
    {
        if (tutorialStep != 1) return;

        tutorialStep = 2;
        tutorialPanel.SetActive(false);
    }
}