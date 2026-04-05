using UnityEngine;
using System.Collections;

//for detecting if the ball went in the goal, was saved, or missed
public class BallOutcomeDetector : MonoBehaviour
{
    //where the result is stored once decided 
    private string decided = null;

    //to track if it touched the keeper
    private bool keeperTouched = false;

    //label in inspector 
    [Header("Goal Confirm Delay")]

    //delaying confirmation of goal
    public float goalConfirmDelay = 0.2f;

    //References the UI script for popups after result 
    public ResultPopupUI popupUI;

    //Audio sources for saves and goals.
    public AudioSource keeperSaveAudio;
    public AudioSource netHitAudio;


    //stores goal confirmation routine
    private Coroutine goalConfirmRoutine;

    //Finalizing the result 
    private void Decide(string result)
    {
        //if reuslt was already decided stop the method
        if (decided != null) return;

        //saves the decided result
        decided = result;
        //for testing purposes before the popup to see the result in the console
        Debug.Log("RESULT: " + result);

        //starts result handling flow 
        StartCoroutine(HandleResultFlow(result));
    }

    //handles delays, popups, and level manager calls when the result is decided 
    private IEnumerator HandleResultFlow(string result)
    {
        //how long it waits to show the popup after the result is decided 
        yield return new WaitForSeconds(0.7f);

        //if popupUI is assigned, it shows the popup based on the result 
        if (popupUI != null)
        {
            if (result == "GOAL") popupUI.ShowGoal();
            else if (result == "SAVED") popupUI.ShowSaved();
            else if (result == "MISSED") popupUI.ShowMissed();
        }

        //waits for a second before it passes the result to the different level managers 
        yield return new WaitForSeconds(1.0f);

        //sends result to level managers (5, 3, and end)
        if (Level5Manager.Instance != null)
            Level5Manager.Instance.HandleResult(result);
        else if (Level3Manager.Instance != null)
        {
            Level3Manager.Instance.HandleResult(result);
        }
        else
        {
            LevelEndManager.Instance?.HandleResult(result);
        }
    }

    //runs once a collision is detected 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if the ball hits the goal wall, the net sound is played
        if (collision.collider.CompareTag("GoalWall"))
        {
            if (netHitAudio != null && netHitAudio.clip != null)
            {
                if (AudioSettingsManager.Instance != null)
                //applies the sfx volume set in settings
                netHitAudio.volume = AudioSettingsManager.Instance.SFXVolume;

                //plays audio once 
                netHitAudio.PlayOneShot(netHitAudio.clip);
            }
            return;
        }

        //if the ball hits the keeper, it plays the save audo
        if (!collision.collider.CompareTag("Keeper")) return;

        if (!keeperTouched && keeperSaveAudio != null && keeperSaveAudio.clip != null)
        {
            if (AudioSettingsManager.Instance != null)
                keeperSaveAudio.volume = AudioSettingsManager.Instance.SFXVolume;

            keeperSaveAudio.PlayOneShot(keeperSaveAudio.clip);
        }   

        //if it touched the keeper, it sets the flag to true
        keeperTouched = true;

        //checks if goalconfirm routine is running
        if (goalConfirmRoutine != null)
        {
            //stops and clear the routine, so that the save is the final decision instead of a goal
            StopCoroutine(goalConfirmRoutine);
            goalConfirmRoutine = null;
        }

        //if no other decision was made, it decides it was only a save.
        if (decided == null)
            Decide("SAVED");
    }


    //runs once a trigger collision is detected (specfiically for goals and outs)
    private void OnTriggerEnter2D(Collider2D other)
    {
        //stops if a decision was already made
        if (decided != null) return;

        //if ball hits the out tags set in the inspector
        if (other.CompareTag("Out"))
        {
            //if it was saved before going out, its a save, otherwise its a miss
            if (keeperTouched) Decide("SAVED");
            else Decide("MISSED");
            return;
        }

        //if the ball hits the goalTargetArea which is tagged as Goal, 
        if (other.CompareTag("Goal"))
        {
            //if keeper had touched it its a save.
            if (keeperTouched)
            {
                Decide("SAVED");
                return;
            }

            //starts goal confirmation routine
            if (goalConfirmRoutine == null)
                goalConfirmRoutine = StartCoroutine(ConfirmGoal());
        }
    }

    //waits a second to decide if it was a goal, to give more time for save detection
    private IEnumerator ConfirmGoal()
    {
        yield return new WaitForSeconds(goalConfirmDelay);
        goalConfirmRoutine = null;

        //if another result was decided during the wait, it stops
        if (decided != null) yield break;

        //if its saved during the wait, its a save, otherwise its a goal
        if (keeperTouched) Decide("SAVED");
        else Decide("GOAL");
    }

    //resets decision and keeper touch flag while also stopping the goal confirmation routine so that it's ready for the next shot
    public void ResetDecision()
    {
        decided = null;
        keeperTouched = false;

        if (goalConfirmRoutine != null)
        {
            StopCoroutine(goalConfirmRoutine);
            goalConfirmRoutine = null;
        }
    }
}