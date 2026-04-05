using UnityEngine;
using TMPro;

//same process as level 2 and 3 but with different messages for level 4
public class L4TutorialManager : MonoBehaviour
{
    public static L4TutorialManager Instance;

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
        tutorialText.text = "You're surprising me! But can you only score bottom corner shots? Prove it to me! Anything other than green timing won't save you this time HA HA!";
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