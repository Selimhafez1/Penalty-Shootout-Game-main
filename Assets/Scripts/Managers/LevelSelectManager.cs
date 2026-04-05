using UnityEngine;
using UnityEngine.SceneManagement;

//handles level select scene buttons 
public class LevelSelectManager : MonoBehaviour
{
    //loads levels 1, 2, 3, 4, and 5 when their respective buttons are clicked and also has a button to go back to the main menu
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level_01");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level_02");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level_03");
    }

    public void LoadLevel4()
    {
        SceneManager.LoadScene("Level_04");
    }

    public void LoadLevel5()
    {
        SceneManager.LoadScene("Level_05");
    }

    //never used in the end
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}