using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

//everytime the player shoots it saves and gets the zone it shot in
public class ShotPatternTracker : MonoBehaviour
{
    //
    public static ShotPatternTracker Instance;

    private FirebaseAuth auth;
    private FirebaseFirestore db;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //on start it gets the firebase auth and firestoe data
    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
    }

    //saves the shotzone to firestore
    public void TrackShotZone(string zoneName)
    {
        FirebaseUser user = auth.CurrentUser;

        //if user isnt logged in the shot pattern isnt saved
        if (user == null)
        {
            Debug.LogWarning("No logged-in user. Shot pattern not saved.");
            return;
        }

        //gets firestore doc for this user
        DocumentReference userDoc = db.Collection("users").Document(user.UserId);

        //creates dictionary of fields to update
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            //increases the shot zone count with each shot
            { "shotPatterns." + zoneName, FieldValue.Increment(1) }
        };

        //updates on main thread
        userDoc.UpdateAsync(updates).ContinueWithOnMainThread(task =>
        {
            //if saving fails it prints this message
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to save shot pattern: " + task.Exception);
                return;
            }
            //for testing 
            Debug.Log("Tracked shot zone: " + zoneName);
        });
    }
}
