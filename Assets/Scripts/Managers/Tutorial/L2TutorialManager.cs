using UnityEngine;
using TMPro;


//for controlling tutroial messages for level 2
public class L2TutorialManager : MonoBehaviour
{
    public static L2TutorialManager Instance;

    public GameObject tutorialPanel;
    public TMP_Text tutorialText;

    //current step of the tutorial starts at 0
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
        //shows first tutorial message on start of scene 
        ShowWelcomeMessage();
    }

    private void Update()
    {
        
        //waits for any tap on the screen at step 0
        if (tutorialStep == 0 && Input.GetMouseButtonDown(0))
        {
            //shows the message after  
            ShowTapGoalMessage();
        }
    }

    private void ShowWelcomeMessage()
    {
        //sets tutorial step to 0
        tutorialStep = 0;
        //confirms the panel is visible 
        tutorialPanel.SetActive(true);
        //displays the first message 
        tutorialText.text = "Now let me show you how to shoot in the middle of the goal! TAP HERE!";
    }

    private void ShowTapGoalMessage()
    {
        //sets tutorial step to 1
        tutorialStep = 1;
        //displays the second message 
        tutorialText.text = "Tap anywhere in the middle of the goal!";
    }

    //once player taps the goal 
    public void OnGoalTapped()
    {
        //stops if the player hasn't reached the right step yet to tap the goal
        if (tutorialStep != 1) return;

        //sets tutorial step to 2
        tutorialStep = 2;
        //displays the third message
        tutorialText.text = "Press the shoot button! Find the sunny side of the problem";
    }

    //called once player shoots and moves it to the finished step which hides the panel
    public void OnPlayerShot()
    {
        if (tutorialStep != 2) return;

        tutorialStep = 3;
        tutorialPanel.SetActive(false);
    }
}