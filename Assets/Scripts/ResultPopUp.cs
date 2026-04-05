using TMPro;
using UnityEngine;

//handles the popups that show GOAL, MISSED, AND SAVED
public class ResultPopupUI : MonoBehaviour
{
    //references to the text, animator, and the canvas group used to show/hide popup
    public TMP_Text label;
    public Animator anim;
    public CanvasGroup canvasGroup;

    //on awake hide the popups
    private void Awake()
    {
        HideAll();
    }

    //shows the pop up with each text and animation trigger
    public void ShowGoal()
    {
        ShowPopup("GOAL", "GOAL");
    }

    public void ShowSaved()
    {
        ShowPopup("SAVED", "SAVED");
    }

    public void ShowMissed()
    {
        ShowPopup("MISSED", "MISSED");
    }

    //makes sure the result text pops up and animates properly
    private void ShowPopup(string text, string triggerName)
    {
        //if references exist it sets popup text and makes it visible
        if (label != null)
            label.text = text;

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        //if reference exists, it clears it and starts it again using the new trigger passed through the method 
        if (anim != null)
        {
            anim.ResetTrigger("GOAL");
            anim.ResetTrigger("SAVED");
            anim.ResetTrigger("MISSED");
            anim.SetTrigger(triggerName);
        }
    }

    //hides the pop up and sets it back to its default idle state
    public void HideAll()
    {
        //label text is null and canvas group sets it to invisible
        if (label != null)
            label.text = "";

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        //plays the idle anim
        if (anim != null)
        {
            anim.ResetTrigger("GOAL");
            anim.ResetTrigger("SAVED");
            anim.ResetTrigger("MISSED");
            anim.Play("Idle", 0, 0f);
        }
    }
}