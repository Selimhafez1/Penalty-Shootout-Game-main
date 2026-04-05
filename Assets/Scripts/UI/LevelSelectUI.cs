using UnityEngine;
using UnityEngine.UI;

//controls which levels are visually unlocked and interactable in the level select scene based on player progress
public class LevelSelectUI : MonoBehaviour
{
    //makes inner class visible
    [System.Serializable]

    //stores data for each level button including its number, button component, visual image, and locked/unlocked sprites
    public class LevelButtonData
    {
        public int levelNumber;
        public Button button;
        public Image visualImage;
        public Sprite lockedSprite;
        public Sprite unlockedSprite;
    }

    //array of level button data
    public LevelButtonData[] levels;

    //runs once scene starts
    private void Start()
    {
        //loops through the array, through each level button
        foreach (var level in levels)
        {
            //if button is null, skip to next level button
            if (level.button == null) continue;

            //if level one its always unlocked 
            if (level.levelNumber == 1)
            {
                SetupButton(level, true);
            }
            //for every other level...
            else
            {
                //sets button as locked 
                SetupButton(level, false);

                //checks if firestore progress manager exists 
                if (FirestoreProgressManager.Instance != null)
                {
                    //stores current level number and data in local variables 
                    int capturedLevel = level.levelNumber;
                    LevelButtonData capturedData = level;

                    //checks if the level is unlocked in the firestore
                    FirestoreProgressManager.Instance.CheckLevelUnlocked(capturedLevel, isUnlocked =>
                    {
                        //updates the button based on whether the level is unlocked or not
                        SetupButton(capturedData, isUnlocked);
                    });
                }
            }
        }
    }

    //updates the button to make it interactable and change its visual based on whether the level is unlocked or not
    private void SetupButton(LevelButtonData data, bool isUnlocked)
    {
        if (data.button != null)
            data.button.interactable = isUnlocked;

        if (data.visualImage != null)
            data.visualImage.sprite = isUnlocked ? data.unlockedSprite : data.lockedSprite;
    }
}