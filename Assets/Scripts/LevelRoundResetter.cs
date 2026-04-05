using UnityEngine;

//resets the round by resetting every component involved in the round
public class LevelRoundResetter : MonoBehaviour
{
    //references to all the components that need to be reset for the next shot
    public Rigidbody2D ballRb;
    public Transform ball;
    public Transform ballStartPoint;

    public BallTapAim2D tapAim;
    public ShootWithTiming shootScript;
    public BallOutcomeDetector outcomeDetector;
    public TimingBarController timingBar;
    public ResultPopupUI popupUI;

    //what actually resets the round
    public void ResetRoundForNextShot()
    {
        //stops ball movement
        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }

        //resets ball position to the starting point
        if (ball != null && ballStartPoint != null)
        {
            ball.position = ballStartPoint.position;
        }

        //clears the aim, resets the shoot script, outcome, timing bar, and popups
        if (tapAim != null)
            tapAim.ClearAim();

        if (shootScript != null)
            shootScript.ResetForNextShot();

        if (outcomeDetector != null)
            outcomeDetector.ResetDecision();

        if (timingBar != null)
            timingBar.ResetBar();

        if (popupUI != null)
            popupUI.HideAll();
    }
}