using UnityEngine;
using UnityEngine.UI;

//Attached to the timing bar UI element.
//The idea is that the user must time the shot directly in the center of the bar for maximum power and accuracy.
public class TimingBarController : MonoBehaviour
{
    //Referece to the slider component in UI canvas.
    public Slider timingSlider;

    //How fast the slider moves back and forth.
    public float speed = 1.8f;   

    //Makes the center of the bar (the perfect timing place).   
    public float center = 0.5f;

    //Closer to the center, the more perfect the shot is
    //0.06 is 0.44 and 0.56 on the bar for my center.
    public float perfectWindow = 0.06f;

    // Yellow area size (must be bigger than perfectWindow).
    // Example: 0.18 means yellow is between 0.32 and 0.68 if center is 0.5.
    public float goodWindow = 0.24f;

    // MUST match GenerateTimingGradient
    public float edgeFade = 0.12f;   // red<->yellow fade thickness outside goodWindow
    public float innerFade = 0.10f;  // yellow<->green fade thickness outside perfectWindow

    // Minimum timing score when you're right at the edge of the visual "yellowish" region.
    // If this is 0, that region becomes guaranteed miss again.
    // Try 0.15–0.30.
    [Range(0f, 1f)]

    public float minTimingInOuterFade = 0.2f;
    //Tells other scripts is the timing bar is running.
    public bool IsRunning { get; private set; } = false;

    //Direction of the movement so moving right to left or left to right.
    private float dir = 1f;

    //If it's not assigned, it returns to the center.
    public float Value => timingSlider != null ? timingSlider.value : center;

    //Called automatically by Unity when the scene starts.
    void Start()
    {
        //If the slider reference exists, run the setup.
        if (timingSlider != null)
        {
            //Slider range from 0-1.
            timingSlider.minValue = 0f;
            timingSlider.maxValue = 1f;

            //starts slider in the center.
            timingSlider.value = center;
        }
    }

    //Called automatically by Unity every frame.
    void Update()
    {
        //if reference is missing, do nothing.
        if (!IsRunning || timingSlider == null) return;

        //moves slider value each frame.
        //value += dir * speed * deltaTime.
        //deltaTime is what keeps it dependant on frame rate.
        timingSlider.value += dir * speed * Time.deltaTime;

        //once it reaches the end, it goes back the other way.
        if (timingSlider.value >= 1f)
        {
            timingSlider.value = 1f;
            dir = -1f;
        }

        //once it reaches the end, it goes back the other way.
        else if (timingSlider.value <= 0f)
        {
            timingSlider.value = 0f;
            dir = 1f;
        }
    }

    //Starts timing bar movement.
    public void StartBar()
    {
        //Enables movement.
        IsRunning = true;

        //Always starts moving to the right first.
        dir = 1f;

        //After each attempt restarts to the center.
        if (timingSlider != null) timingSlider.value = center;
    }

    //Stops the bar and returns how good the timing was as a value from 0.0 to 1.0.
    public float StopAndGetTiming01()
{
    IsRunning = false;

    float dist = Mathf.Abs(Value - center);

    float innerBoundary = perfectWindow + innerFade;
    float outerBoundary = goodWindow + edgeFade;

    //perfect zone
    if (dist <= perfectWindow)
        return 1f;

    //inner fade (green to yellow): smoothly drop 1 -> 0.7
    if (dist <= innerBoundary)
    {
        float k = Mathf.InverseLerp(perfectWindow, innerBoundary, dist); 
        //at the end of inner fade, timing becomes 0.7 (feels okay but not perfect)
        return Mathf.Lerp(1f, 0.7f, Mathf.SmoothStep(0f, 1f, k));
    }

    //yellow zone (solid): goes down from 0.7 to 0.4 for good window
    if (dist <= goodWindow)
    {
        float u = Mathf.InverseLerp(innerBoundary, goodWindow, dist); 
        return Mathf.Lerp(0.7f, 0.4f, Mathf.SmoothStep(0f, 1f, u));
    }

    //outer fade (yellow to red): goes down from 0.4 to minTimingInOuterFade
    if (dist <= outerBoundary)
    {
        float v = Mathf.InverseLerp(goodWindow, outerBoundary, dist); // 0..1
        return Mathf.Lerp(0.4f, minTimingInOuterFade, Mathf.SmoothStep(0f, 1f, v));
    }

    //fully red (guaranteed miss region)
    return 0f;
}


    //returns true if the timing is within the perfect window.
    public bool IsPerfect()
    {
        return Mathf.Abs(Value - center) <= perfectWindow;
    }

    //resets bar back to default state
    public void ResetBar()
    {
        IsRunning = false;
        dir = 1f;

        if (timingSlider != null)
        timingSlider.value = center;
    }
}
