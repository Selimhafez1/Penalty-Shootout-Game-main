using UnityEngine;
using UnityEngine.EventSystems;

public class BallTapAim2D : MonoBehaviour, IPointerDownHandler
{
    //reference to the ball transform so it can know where the ball starts from
    public Transform ball;

    //references the timing bar so that it could start after the player taps to aim
    public TimingBarController timingBar;

    //Stores aim direction
    public Vector2 AimDirection { get; private set; } = Vector2.up;

    //tracks to see if player already aimed or not
    public bool HasAim { get; private set; } = false;

    //heading in inspector for shot tracking variables 
    [Header("Shot Zone Tracking")]

    //for enabling and disabling shot tracking 
    public bool enableShotTracking = false;

    //X range to see if its middle or not
    public float middleXThreshold = 2.25f;
    //Y range to see if its top or bottom
    public float topYThreshold = 0.85f;

    //stores the current shot zone as a string (TopLeft, TopRight, BottomLeft, BottomRight, Middle, or "" if not selected yet)
    public string CurrentShotZone { get; private set; } = "";

    //runs when the player aims
    public void OnPointerDown(PointerEventData eventData)
    {
        //converts where the player tapped to a world position
        Vector2 world = Camera.main.ScreenToWorldPoint(eventData.position);

        //finds the direction to the tap from the ball 
        Vector2 dir = world - (Vector2)ball.position;
        //if tap is too close to the ball, ignore it
        if (dir.sqrMagnitude < 0.001f) return;

        //prevents aiming downwards which could cause issues.
        if (dir.y < 0f) dir.y = 0.05f;

        //normalizes and stores aim direction
        AimDirection = dir.normalized;
        //sets that the player aimed
        HasAim = true;

        //tells the listed tutorial managers that the player aimed so that the tutorials could progress
        TutorialManager.Instance?.OnGoalTapped();
        L2TutorialManager.Instance?.OnGoalTapped();

        //if the shot tracking is checked in the inspector, it checks where the player aimed and prints the result in the conssole (for testing)
        if (enableShotTracking)
        {
            CurrentShotZone = GetShotZone(world);
            Debug.Log("Selected shot zone: " + CurrentShotZone);
        }

        //checks that timing bar is assigned and starts after the player aims
        if (timingBar != null)
            timingBar.StartBar();
    }

    //This decides which shot zone the player aimed for 
    private string GetShotZone(Vector2 targetPoint)
    {
        //stores x and y positions of the tap
        float x = targetPoint.x;
        float y = targetPoint.y;

        //checks if the tap is in the middle or top half
        bool isMiddle = Mathf.Abs(x) <= middleXThreshold;
        bool isTop = y >= topYThreshold;

        //returns middle if in middle threshold
        if (isMiddle)
            return "Middle";

        //if its on the left and it's in the top half its top left, if not in the top half its bottom left
        if (x < 0f)
            return isTop ? "TopLeft" : "BottomLeft";
        
        //same here for the right side
        else
            return isTop ? "TopRight" : "BottomRight";
    }

    //Clears the aim information 
    public void ClearAim()
    {
        HasAim = false;
        CurrentShotZone = "";
    }
}
