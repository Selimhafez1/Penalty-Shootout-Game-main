using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

//saves and loads the players level progress in firestore
public class FirestoreProgressManager : MonoBehaviour
{
    public static FirestoreProgressManager Instance;

    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private bool firebaseReady = false;

    //runs once object is created
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //runs when scene starts
    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                db = FirebaseFirestore.DefaultInstance;
                firebaseReady = true;

                Debug.Log("FirestoreProgressManager: Firebase is ready.");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    //for saving completed level and unlocking the next one 
    public void CompleteLevelAndUnlockNext(int completedLevel, int nextLevel, System.Action<bool> onComplete = null)
    {
        //if firebase not ready, print error and return
        if (!firebaseReady)
        {
            Debug.LogError("Firebase is not ready yet.");
            onComplete?.Invoke(false);
            return;
        }

        //gets current user logged in
        FirebaseUser user = auth.CurrentUser;

        //if there's no user logged in, print error and return
        if (user == null)
        {
            Debug.LogError("No logged-in user found.");
            onComplete?.Invoke(false);
            return;
        }

        //opens firestore document for the user 
        DocumentReference docRef = db.Collection("users").Document(user.UserId);

        //dictionary to store updates of progress
        Dictionary<string, object> updates = new Dictionary<string, object>();
        //marks level as completed if completed
        updates["level" + completedLevel + "Completed"] = true;

        //if next level exisits within the 5 levels, it unlocks and saves it for the player
        if (nextLevel <= 5)
            updates["level" + nextLevel + "Unlocked"] = true;

        //saves to firestore
        docRef.SetAsync(updates, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            //tests to see if saving was successful
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to save level progress: " + task.Exception);
                onComplete?.Invoke(false);
                return;
            }

            //returns true is saving was successful
            onComplete?.Invoke(true);
        });
    }

    //checks if a level is unlocked for the player
    public void CheckLevelUnlocked(int levelNumber, System.Action<bool> onResult)
    {
        if (!firebaseReady)
        {
            Debug.LogError("Firebase is not ready yet.");
            onResult?.Invoke(false);
            return;
        }

        //if requesting level 1, its always unlocked so return true
        if (levelNumber == 1)
        {
            onResult?.Invoke(true);
            return;
        }

        //gets user logged in
        FirebaseUser user = auth.CurrentUser;

        if (user == null)
        {
            Debug.LogError("No logged-in user found.");
            onResult?.Invoke(false);
            return;
        }

        //gets firestore document for the user 
        DocumentReference docRef = db.Collection("users").Document(user.UserId);

        //gets snapshot of document 
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            //tests if reading was successful
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to read progress: " + task.Exception);
                onResult?.Invoke(false);
                return;
            }

            //stores the snapshot of the document and checks if it exists
            DocumentSnapshot snapshot = task.Result;

            if (!snapshot.Exists)
            {
                onResult?.Invoke(false);
                return;
            }

            //default value for when the level is unlocked is false 
            bool unlocked = false;
            //creates firestore field name for this levels unlocked status
            string fieldName = "level" + levelNumber + "Unlocked";

            //reads unlocked value from firestore snapshot
            if (snapshot.TryGetValue(fieldName, out unlocked))
                //gets the store unlocked value
                onResult?.Invoke(unlocked);
            else
                //returns false by default 
                onResult?.Invoke(false);
        });
    }
}
