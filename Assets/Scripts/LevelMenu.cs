using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons; // Assign these in the Inspector, one for each level
    public GameObject levelButtons;

    private void Awake()
    {
        //ButtonsToArray();

        //PlayerPrefs.DeleteAll(); //to reselt all levels
        //PlayerPrefs.Save();

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); // Default to 1 if not set
        Debug.Log("UnlockedLevel = " + unlockedLevel);
        //int maxToEnable = Mathf.Clamp(reachedIndex, 1, buttons.Length); // Ensure we don't exceed the number of buttons available
        for (int i = 0; i < buttons.Length; i++) // Loop through all buttons
        {
            buttons[i].interactable = false; // Disable all buttons initially
        }
        for (int i = 0; i < unlockedLevel; i++)
        {
            buttons[i].interactable = true; // Ensure buttons for unlocked levels are interactable
        }

    }



    public void OpenLevel(int levelId)
    {
        string levelName = "Level " + levelId; // Construct the level name based on the levelId
        SceneManager.LoadSceneAsync(levelName); // Load the scene with the constructed name
    }

    void ButtonsToArray()  //this automatically addds the buttons in the buttons array 
    {
        int childCount = levelButtons.transform.childCount;
        buttons = new Button[childCount];
        for(int i = 0; i < childCount;i++)
        {
            buttons[i] = levelButtons.transform.GetChild(i).GetComponent<Button>();
        }
    }

}
