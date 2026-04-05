using UnityEngine;
using UnityEngine.EventSystems;

//handles shot when shoot button is pressed 
public class ShootWithTiming : MonoBehaviour, IPointerClickHandler
{
    //reference to keeper animator 
    public Animator keeperAnimator;

    //trigger names like they are in the animator
    public string[] keeperDiveTriggers = { "DiveLeft", "DiveRight", "JumpMiddle", "DiveTopRight", "DiveTopLeft" };

    //so force could be applied to the ball
    public Rigidbody2D ballRb;

    //references the aim script so that the shot direction could be read
    public BallTapAim2D tapAim;

    //references timing script so timing can effect power and accuracy
    public TimingBarController timingBar;

    //kick sound audio source
    public AudioSource shootAudio;


    //min and max shot power.
    public float minPower = 4f;
    public float maxPower = 9f;

    //maximum angle error for normal shots that are inaccurate
    public float maxAngleErrorDegrees = 14f;


    //miss directions for red misses 
    //min and max horizontal push for red miss
    public float redMissXMin = 1.2f;
    public float redMissXMax = 2.0f;
    
    //min and max upward value for red miss
    public float redMissYMin = 0.15f;
    public float redMissYMax = 0.45f;

    //heading in inspector for tuning accuracy
    [Header("Accuracy tuning")]

    //green shots are near perfect shots
    public float greenErrorDegrees = 4f;    
    
    //yellow near the green has some error too it
    public float yellowMinError = 10f;        
    //yellow near orange has a lot of error to it
    public float yellowMaxError = 25f;      
    //orange between yellow and red is almost always a miss, but not always guaranteed  
    public float orangeMaxError = 55f;        

    //tracks if player took the shot
    private bool hasShot = false;

    //runs once the player presses the shoot button
    public void OnPointerClick(PointerEventData eventData)
    {
        //if player already shot return
        if (hasShot) return;
        //return if no aim chosen
        if (tapAim == null || !tapAim.HasAim) return;
        //return if timing bar isn't running
        if (timingBar == null || !timingBar.IsRunning) return;
        //if ball rigid body is missing stop as well
        if (ballRb == null) return;

        //gets a timing value from the timing bar as number between 0-1
        float timing01 = timingBar.StopAndGetTiming01();

        //calculates shot power based on timing 
        float power = Mathf.Lerp(minPower, maxPower, timing01);

        //gets aim direction already selected by the player
        Vector2 dir = tapAim.AimDirection;

        //sees how far the timing bar stopped from the center
        float dist = Mathf.Abs(timingBar.Value - timingBar.center);
        //if stopped in red, it runs isfullred
        bool isFullRed = dist > (timingBar.goodWindow + timingBar.edgeFade);

        //runs after its in the red zone timing
        if (isFullRed)
        {
            //chooses the side the miss goes towards
            float side = Mathf.Abs(dir.x) < 0.05f ? (Random.value < 0.5f ? -1f : 1f) : Mathf.Sign(dir.x);

            //random horizontal/upward miss strength 
            dir = new Vector2(
                side * Random.Range(redMissXMin, redMissXMax),
                Random.Range(redMissYMin, redMissYMax)
            
            //normalizes direction missed
            ).normalized;

            //uses max power for stronger misses
            power = maxPower;
        }
        else
        {
            //stores error angles to appy
            float error;

            //if timing near perfect
            if (timing01 >= 0.85f)
            {
                //very small error angle
                error = greenErrorDegrees;
            }
            //if timing is in the yellow zone
            else if (timing01 >= 0.4f)
            {
                //converts timing to a value in the yellow range between 0-1
                float t = Mathf.InverseLerp(0.4f, 0.85f, timing01);
                //blends from worst to better yellow error
                error = Mathf.Lerp(yellowMaxError, yellowMinError, t);
            }

            //runs if near the edge between yellow and red
            else
            {
                //converts timing to a value in the ornage range between 0-1
                float t = Mathf.InverseLerp(0.0f, 0.4f, timing01);
                //blends from terrible error to better yellow error
                error = Mathf.Lerp(orangeMaxError, yellowMaxError, t);
            }

            //random error from negative and positive
            float randomError = Random.Range(-error, error);
            //rotates the aim by that error
            dir = (Vector2)(Quaternion.Euler(0, 0, randomError) * dir);
        }

        //resets the velocity before it applies the force
        ballRb.linearVelocity = Vector2.zero;
        ballRb.angularVelocity = 0f;

        //if shoot sound exists play it once button is pressed (also consider sfx volume)
        if (shootAudio != null)
        {
            if (AudioSettingsManager.Instance != null)
                shootAudio.volume = AudioSettingsManager.Instance.SFXVolume;

            shootAudio.Play();
        }


        //shoots the ball using everything calculated above
        ballRb.AddForce(dir.normalized * power, ForceMode2D.Impulse);

        //has shot is now true
        hasShot = true;

        //checks if shot tracking is enabled (levels 1, 3 and 5) and stores these shot zones
        if (tapAim != null && tapAim.enableShotTracking && !string.IsNullOrEmpty(tapAim.CurrentShotZone))
        {
            ShotPatternTracker.Instance?.TrackShotZone(tapAim.CurrentShotZone);
        }

        //tells all level tutorials that the player has shot
        TutorialManager.Instance?.OnPlayerShot();
        L2TutorialManager.Instance?.OnPlayerShot();
        L3TutorialManager.Instance?.OnPlayerShot();
        L4TutorialManager.Instance?.OnPlayerShot();
        L5TutorialManager.Instance?.OnPlayerShot();

        //triggers random animation trigger
        TriggerRandomKeeperDive();
        //resets aim after shot
        tapAim.ClearAim();
    }

    //chooses the triggers
    private void TriggerRandomKeeperDive()
    {
        //if animator is missing nothing happens
        if (keeperAnimator == null)
        {
            //testing
            Debug.LogError("keeperAnimator is NULL");
            return;
        }

        //if in level 5 gets the trigger chosen from the Level5KeeperAI and triggers it
        if (Level5KeeperAI.Instance != null)
        {
            string chosenTrigger = Level5KeeperAI.Instance.GetBiasedDiveTrigger();
            Debug.Log("Level 5 keeper chose trigger: " + chosenTrigger);
            keeperAnimator.SetTrigger(chosenTrigger);
            return;
        }

        //chooses random fallback trigger if it exists, if not then it stops
        if (keeperDiveTriggers == null || keeperDiveTriggers.Length == 0) return;

        int index = Random.Range(0, keeperDiveTriggers.Length);
        Debug.Log("Fallback random keeper trigger: " + keeperDiveTriggers[index]);
        keeperAnimator.SetTrigger(keeperDiveTriggers[index]);
    }



    //resets so player can shoot again 
    public void ResetForNextShot()
    {
        hasShot = false;
    }
}