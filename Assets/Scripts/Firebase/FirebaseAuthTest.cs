using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

//testing for authentication
public class FirebaseAuthTest : MonoBehaviour
{
    private FirebaseAuth auth;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;

                auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask =>
                {
                    if (authTask.IsCanceled || authTask.IsFaulted)
                    {
                        Debug.LogError("Anonymous sign-in failed.");
                        return;
                    }

                    FirebaseUser newUser = authTask.Result.User;
                    Debug.Log("Signed in anonymously!");
                    Debug.Log("User ID: " + newUser.UserId);
                });
            }
            else
            {
                Debug.LogError("Firebase dependencies not available: " + task.Result);
            }
        });
    }
}