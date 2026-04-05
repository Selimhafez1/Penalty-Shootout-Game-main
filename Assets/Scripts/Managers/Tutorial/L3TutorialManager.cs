using UnityEngine;
using TMPro;

//controls tutorial messages for level 3 (same process as level 2 but with different messages)
public class L3TutorialManager : MonoBehaviour
{
    public static L3TutorialManager Instance;

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

    private void ShowWelcomeMessage()
    {
        tutorialStep = 0;
        tutorialPanel.SetActive(true);
        tutorialText.text = "Are you sure you got this? Prove it to me! Score 3 in a row to complete the level!";
    }

    public void OnPlayerShot()
    {
        if (tutorialStep == 0)
        {
            tutorialStep = 1;
            tutorialPanel.SetActive(false);
        }
    }
}