using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

//Uses players shot patters to bias keeper dives in level 5
public class Level5KeeperAI : MonoBehaviour
{
    public static Level5KeeperAI Instance;

    //Settings for whether to use 3 or 5 zone dives (used 5 in the end)
    [Header("Use top-corner dives too?")]
    public bool useFiveZoneDives = false;

    //Animator Trigger names set in the inspector 
    [Header("Trigger names")]
    public string diveLeftTrigger = "DiveLeft";
    public string diveRightTrigger = "DiveRight";
    public string jumpMiddleTrigger = "JumpMiddle";
    public string diveTopRightTrigger = "DiveTopRight";
    public string diveTopLeftTrigger = "DiveTopLeft";

    //Probability tuning in the inspector 
    [Header("Bias tuning")]
    public float baseWeight = 1f;
    public float biasMultiplier = 1f;

    private FirebaseAuth auth;
    private FirebaseFirestore db;

    //stores how many times the player shot in each zone 
    private int topLeft;
    private int topRight;
    private int middle;
    private int bottomLeft;
    private int bottomRight;

    //tracks if shot history loaded successfully
    private bool dataLoaded = false;

    private void Awake()
    {
        Instance = this;
    }

    //once scene starts it loads players shot patterns from firestore 
    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        LoadShotPatterns();
    }

    //for loading shot patterns for each user from firestore
    private void LoadShotPatterns()
    {
        //gets user that's currently logged in from firestore 
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("No logged-in user. Keeper AI will use random dives.");
            dataLoaded = false;
            return;
        }

        //reads user doc from firestore 
        db.Collection("users").Document(user.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Failed to load shot patterns: " + task.Exception);
                dataLoaded = false;
                return;
            }

            //stores snapshot of user doc 
            var snapshot = task.Result;

            if (!snapshot.Exists)
            {
                Debug.Log("No user document found. Keeper AI will use random dives.");
                dataLoaded = false;
                return;
            }

            //resets default values just in case they don't exisit in firestore 
            topLeft = 0;
            topRight = 0;
            middle = 0;
            bottomLeft = 0;
            bottomRight = 0;

            //if it exists it gets the values for each shot zone 
            if (snapshot.TryGetValue("shotPatterns.TopLeft", out int tl)) topLeft = tl;
            if (snapshot.TryGetValue("shotPatterns.TopRight", out int tr)) topRight = tr;
            if (snapshot.TryGetValue("shotPatterns.Middle", out int mid)) middle = mid;
            if (snapshot.TryGetValue("shotPatterns.BottomLeft", out int bl)) bottomLeft = bl;
            if (snapshot.TryGetValue("shotPatterns.BottomRight", out int br)) bottomRight = br;

            //shot pattern data successfully loaded 
            dataLoaded = true;

            //for testing shows the users shot patterns in the console
            Debug.Log($"Level 5 shot patterns loaded: TL={topLeft}, TR={topRight}, M={middle}, BL={bottomLeft}, BR={bottomRight}");
        });
    }

    //chooses which animator trigger to use
    public string GetBiasedDiveTrigger()
    {
        //if no data is loaded then the keeper will dive randomly 
        if (!dataLoaded)
        {
            return GetFallbackRandomTrigger();
        }

        //five zone diving is enabled so this is what it uses 
        if (useFiveZoneDives)
        {
            return GetFiveZoneTrigger();
        }
        else
        {
            return GetThreeZoneTrigger();
        }
    }

    //don't use this one
    private string GetThreeZoneTrigger()
    {
        float leftWeight = baseWeight + (topLeft + bottomLeft) * biasMultiplier;
        float rightWeight = baseWeight + (topRight + bottomRight) * biasMultiplier;
        float middleWeight = baseWeight + middle * biasMultiplier;

        float total = leftWeight + rightWeight + middleWeight;
        float roll = Random.Range(0f, total);

        if (roll < leftWeight)
            return diveLeftTrigger;

        roll -= leftWeight;
        if (roll < rightWeight)
            return diveRightTrigger;

        return jumpMiddleTrigger;
    }

    //chooses which dive to do based on player shot patterns 
    private string GetFiveZoneTrigger()
    {
        //calculates the weights for each dive 
        float tlWeight = baseWeight + topLeft * biasMultiplier;
        float trWeight = baseWeight + topRight * biasMultiplier;
        float midWeight = baseWeight + middle * biasMultiplier;
        float blWeight = baseWeight + bottomLeft * biasMultiplier;
        float brWeight = baseWeight + bottomRight * biasMultiplier;

        //adds all weights together and picks a random number between 0 and the total
        float total = tlWeight + trWeight + midWeight + blWeight + brWeight;
        float roll = Random.Range(0f, total);

        //if the number lands on the top left weight, keeper dives top left
        if (roll < tlWeight)
            return diveTopLeftTrigger;

        //removes the top left weight and checks if it lands on the top right weight, if it does keeper dives top right
        roll -= tlWeight;
        if (roll < trWeight)
            return diveTopRightTrigger;

        //removes the top right weight and checks if it lands on the middle weight, if it does keeper jumps middle
        roll -= trWeight;
        if (roll < midWeight)
            return jumpMiddleTrigger;

        //removes the middle weight and checks if it lands on the bottom left weight, if it does keeper dives bottom left
        roll -= midWeight;
        if (roll < blWeight)
            return diveLeftTrigger;

        //if none matched then keeper dives bottom right
        return diveRightTrigger;
    }

    //random dive selection if shot pattern isn't loaded, 
    private string GetFallbackRandomTrigger()
    {
        if (useFiveZoneDives)
        {
            string[] triggers =
            {
                //list of triggers
                diveTopLeftTrigger,
                diveTopRightTrigger,
                jumpMiddleTrigger,
                diveLeftTrigger,
                diveRightTrigger
            };

            //returns a random trigger from the list 
            return triggers[Random.Range(0, triggers.Length)];
        }
        else
        {
            string[] triggers =
            {
                diveLeftTrigger,
                diveRightTrigger,
                jumpMiddleTrigger
            };

            return triggers[Random.Range(0, triggers.Length)];
        }
    }
}
