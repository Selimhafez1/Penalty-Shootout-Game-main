using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

//handles the profile menu where the user can see his username, and delete/logout of account buttons
public class ProfileMenuManager : MonoBehaviour
{
    //text shown on profile
    public TMP_Text profileButtonText;
    //profile panel ui reference
    public GameObject profilePanel;
    //text for status 
    public TMP_Text statusText;

    //fire base authentication and database instances 
    private FirebaseAuth auth;
    private FirebaseFirestore db;

    //doesn't allow account to deletion to run more than once at a time
    private bool isBusy = false;

    //once scene starts it hides the profile panel, sets status text to null, and loads the username of the current user
    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        if (profilePanel != null)
            profilePanel.SetActive(false);

        if (statusText != null)
            statusText.text = "";

        LoadUsername();
    }

    //loads the username of the user from firestore and puts it below the profile icon
    private void LoadUsername()
    {
        //gets current user
        FirebaseUser user = auth.CurrentUser;

        //if no user is logged in, then it just shows "Profile"
        if (user == null)
        {
            if (profileButtonText != null)
                profileButtonText.text = "Profile";
            return;
        }

        //reads firestore document
        db.Collection("users").Document(user.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            //if loading fails it goes back to the "Profile" fallback
            if (task.IsCanceled || task.IsFaulted)
            {
                if (profileButtonText != null)
                    profileButtonText.text = "Profile";
                return;
            }

            //stores the downloaded firestore document snapshot
            DocumentSnapshot snapshot = task.Result;

            //if doc exists and has a username field, and it loads the username stored
            if (snapshot.Exists && snapshot.TryGetValue("username", out string username))
            {
                if (profileButtonText != null)
                    profileButtonText.text = username;
            }
            //it doesn't exist, then go to the fallback
            else
            {
                if (profileButtonText != null)
                    profileButtonText.text = "Profile";
            }
        });
    }

    //pressing the profile button opens the panel by setting active true
    public void OpenProfileMenu()
    {
        if (profilePanel != null)
            profilePanel.SetActive(true);
    }

    //pressing the close in the panel, sets the panel to false
    public void CloseProfileMenu()
    {
        if (profilePanel != null)
            profilePanel.SetActive(false);
    }

    //logout button in the panel logs the user out of firebase and loads the main menu scene
    public void Logout()
    {
        auth.SignOut();
        SceneManager.LoadScene("MainMenu");
    }

    //deletes users firestore data and authentication account 
    public void DeleteAccount()
    {
        //if already deleting it doesn't do anything
        if (isBusy) return;

        //gets current user
        FirebaseUser user = auth.CurrentUser;
        //if hes not logged in
        if (user == null)
        {
            //return the main menu
            SceneManager.LoadScene("MainMenu");
            return;
        }

        //so deletion can't start again
        isBusy = true;
        SetStatus("Deleting account...");

        //stores the uid for the current user and reads the users firestore document
        string uid = user.UserId;
        DocumentReference userDoc = db.Collection("users").Document(uid);

        userDoc.GetSnapshotAsync().ContinueWithOnMainThread(getTask =>
        {
            //if cant read it, it shows this message 
            if (getTask.IsCanceled || getTask.IsFaulted)
            {
                isBusy = false;
                SetStatus("Failed to load account data.");
                //stops process
                return;
            }

            DocumentSnapshot snapshot = getTask.Result;
            //stores lowercase username 
            string usernameLower = null;

            //if doc exists and username has a lower field, then store it
            if (snapshot.Exists && snapshot.TryGetValue("usernameLower", out string savedUsernameLower))
                usernameLower = savedUsernameLower;

            //creates a list to hold delete tasks from firestore
            List<Task> deleteTasks = new List<Task>();

            //adds the task to delete the users main document
            deleteTasks.Add(userDoc.DeleteAsync());

            if (!string.IsNullOrEmpty(usernameLower))
            {
                //deletes username document so username could now be used again
                DocumentReference usernameDoc = db.Collection("usernames").Document(usernameLower);
                deleteTasks.Add(usernameDoc.DeleteAsync());
            }

            //waits until everything is deleted 
            Task.WhenAll(deleteTasks).ContinueWithOnMainThread(deleteFirestoreTask =>
            {
                if (deleteFirestoreTask.IsCanceled || deleteFirestoreTask.IsFaulted)
                {
                    //if it fails it shows this message 
                    isBusy = false;
                    SetStatus("Failed to delete user data.");
                    return;
                }

                //finally deletes the users authentication account 
                user.DeleteAsync().ContinueWithOnMainThread(deleteAuthTask =>
                {
                    isBusy = false;

                    if (deleteAuthTask.IsCanceled || deleteAuthTask.IsFaulted)
                    {
                        SetStatus("Auth account delete failed. Please log in again and try again.");
                        return;
                    }

                    //after deleting it goes back to the main menu scene
                    SceneManager.LoadScene("MainMenu");
                });
            });
        });
    }

    private void SetStatus(string message)
    {
        Debug.Log(message);

        if (statusText != null)
            statusText.text = message;
    }
}