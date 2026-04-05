using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;

public class EmailAuthManager : MonoBehaviour
{
    //inputs from user for the email and password, and text to show the status
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    //storing firebase authentication instance
    private FirebaseAuth auth;

    //runs as soon as the scene starts 
    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;


        //checks if status exists if yes, clears it for next status
        if (statusText != null)
            statusText.text = "";
    }

    //creates a new firebase account using email and password 
    public void SignUp()
    {
        //gets email and removes extra spaces, and gets password
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        //if either are empty shows this message
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            SetStatus("Please enter email and password.");
            return;
        }

        //creates new account in firebase 
        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                //if sign up was cancelled show this message
                if (task.IsCanceled)
                {
                    SetStatus("Sign up was canceled.");
                    return;
                }

                //if sign up fails for whatever reason print sign up failed with the error message
                if (task.IsFaulted)
                {
                    SetStatus("Sign up failed: " + task.Exception?.GetBaseException().Message);
                    return;
                }

                //if account created, show this message and loads the scene where the user creates his username
                SetStatus("Account created successfully!");
                SceneManager.LoadScene("CreateUsernameScene");
            });
    }

    //Logs user into the firebase
    public void Login()
    {
        //gets email and password inputs 
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        //if both or one of the two inputs are missing show this message
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            SetStatus("Please enter email and password.");
            return;
        }

        //starts logging in
        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                //if login is cancelled show this message 
                if (task.IsCanceled)
                {
                    SetStatus("Login was canceled.");
                    return;
                }

                //if login fails show this message and the error message 
                if (task.IsFaulted)
                {
                    SetStatus("Login failed: " + task.Exception?.GetBaseException().Message);
                    return;
                }

                //login successful
                SetStatus("Login successful!");
                //loads the next scene
                SceneManager.LoadScene("LevelSelect");
            });
    }

    //sends email to user to reset password
    public void ForgotPassword()
    {
        //gets and checks if email exists, if it doesnt print the message in the status
        string email = emailInput.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            SetStatus("Please enter your email first.");
            return;
        }

        //sends password email through firebase 
        auth.SendPasswordResetEmailAsync(email)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    SetStatus("Password reset was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    SetStatus("Reset failed: " + task.Exception?.GetBaseException().Message);
                    return;
                }

                SetStatus("Password reset email sent.");
            });
    }

    //returns player to main menu through the back button 
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    //prints messages in the console and UI
    private void SetStatus(string message)
    {
        Debug.Log(message);

        if (statusText != null)
            statusText.text = message;
    }
}