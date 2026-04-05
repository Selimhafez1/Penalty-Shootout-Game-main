using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//handles svaing and and reserving unique usernames
public class UsernameSetupManager : MonoBehaviour
{
    //where player types his unique username
    public TMP_InputField usernameInput;
    public TMP_Text statusText;

    private FirebaseAuth auth;
    private FirebaseFirestore db;

    //stops saving from runnning more than once at the same time
    private bool isSaving = false;

    //once scene starts gets default instances and clears status texts
    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        if (statusText != null)
            statusText.text = "";
    }

    //validates, reserves, and saves username to firestore
    public void SaveUsername()
    {
        if (isSaving) return;

        string rawUsername = usernameInput.text.Trim();

        //if no username is written asks the user to write one
        if (string.IsNullOrEmpty(rawUsername))
        {
            SetStatus("Please enter a username.");
            return;
        }

        //forces user between these limits
        if (rawUsername.Length < 3 || rawUsername.Length > 15)
        {
            SetStatus("Username must be 3 to 15 characters.");
            return;
        }


        //letters, numbers, underscore only are allowed
        if (!Regex.IsMatch(rawUsername, @"^[A-Za-z0-9_]+$"))
        {
            SetStatus("Use only letters, numbers, and underscore.");
            return;
        }

        //creates lowercase version for username checking
        string usernameLower = rawUsername.ToLowerInvariant();

        //gets the currently logged in user
        FirebaseUser user = auth.CurrentUser;
        //if he doesnt exist then
        if (user == null)
        {
            SetStatus("No logged-in user found.");
            return;
        }

        //if saving is working this will print
        isSaving = true;
        SetStatus("Saving username...");

        //gets users firestore doc and doc used for reserving name
        DocumentReference userDoc = db.Collection("users").Document(user.UserId);
        DocumentReference usernameDoc = db.Collection("usernames").Document(usernameLower);

        //firestore transaction for username check and save
        db.RunTransactionAsync<bool>(transaction =>
        {
            //reads document
            return transaction.GetSnapshotAsync(usernameDoc).ContinueWith<Task<bool>>(task =>
            {
                //if it fails to read
                if (task.IsFaulted || task.IsCanceled)
                    throw task.Exception?.GetBaseException() ?? new System.Exception("Failed to check username.");

                //stores the username read
                DocumentSnapshot usernameSnapshot = task.Result;

                //if username exists tells the user
                if (usernameSnapshot.Exists)
                    throw new System.Exception("That username is already taken.");

                //creates data to reserve username
                Dictionary<string, object> usernameReservation = new Dictionary<string, object>
                {
                    //which owner owns the username
                    { "uid", user.UserId },
                    //the username owned
                    { "username", rawUsername }
                };

                //creates data to store in users doc
                Dictionary<string, object> userData = new Dictionary<string, object>
                {
                    //chosen username
                    { "username", rawUsername },
                    //lowercase username
                    { "usernameLower", usernameLower },

                    //progress fields for each user start as...
                    { "level2Unlocked", false },
                    { "level3Unlocked", false },
                    { "level4Unlocked", false },
                    { "level5Unlocked", false }
                };

                //reserved in usernames collection
                transaction.Set(usernameDoc, usernameReservation);
                //saves users data
                transaction.Set(userDoc, userData, SetOptions.MergeAll);

                return Task.FromResult(true);
            }).Unwrap();
        }).ContinueWithOnMainThread(task =>
        {
            //saving is finished
            isSaving = false;

            //if failed to save print this 
            if (task.IsCanceled || task.IsFaulted)
            {
                string errorMessage = "Failed to save username.";

                //if theres a deep error print it instead
                if (task.Exception != null && task.Exception.GetBaseException() != null)
                    errorMessage = task.Exception.GetBaseException().Message;

                SetStatus(errorMessage);
                return;
            }

            //all goes well this prints and opens the next scene
            SetStatus("Username saved!");
            SceneManager.LoadScene("LevelSelect");
        });
    }

    //changes the messages in console and UI
    private void SetStatus(string message)
    {
        Debug.Log(message);

        if (statusText != null)
            statusText.text = message;
    }
}