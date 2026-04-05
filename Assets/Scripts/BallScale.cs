using UnityEngine;

//Makes the ball look smaller as it goes towards the goal to make it look like its going further away and vice versa
public class BallDepthScaler2D : MonoBehaviour
{
    [Header("References")]
    public BoxCollider2D goalTargetArea; 

    [Header("Scaling")]
    //balls scale at start 
    public float startScale = 1.0f;

    //balls scale when it reaches the goalTargetArea (I later changed it to 0.35 in the inspector)
    public float goalScale  = 0.45f;

    //how smooth the ball scales 
    [Header("Smoothing")]
    public float smoothSpeed = 12f;

    //takes the balls y position at the start
    private float startY;
    
    //the y position where the ball stops scaling
    private float clampY; 

    //starts once the object starts
    void Start()
    {
        //saves balls starting y position
        startY = transform.position.y;

        //the bottom of the goaltargetarea is where the ball stops scaling
        if (goalTargetArea != null)
            clampY = goalTargetArea.bounds.min.y;
        else
            //if no goalTargetArea is assigned, there's a fall back so it doesn't infitely scale.
            clampY = startY + 5f;
    }

    //runs every frame so that the ball scales smoothly 
    void Update()
    {
        //uses current y position to scale the ball, but clamps so it doesn't keep on scaling after a the point assigned (goalTargetArea).
        float yForScale = Mathf.Min(transform.position.y, clampY);

        //converts y position to a 0-1 value
        float t = Mathf.InverseLerp(startY, clampY, yForScale);
        //confirms it always stays between these two values just in case
        t = Mathf.Clamp01(t);

        //makes scaling look more natural, by easing the t value
        float eased = Mathf.SmoothStep(0f, 1f, t);
        //calculates the target scale which is between the starting point scale and the goal scale
        float targetScale = Mathf.Lerp(startScale, goalScale, eased);

        //creates the scale value wanted for the ball
        Vector3 desired = new Vector3(targetScale, targetScale, 1f);
        //smoothly scales the current scale to the target scale
        transform.localScale = Vector3.Lerp(transform.localScale, desired, Time.deltaTime * smoothSpeed);
    }
}