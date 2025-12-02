using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int levelIndex;       // set in Inspector
    [SerializeField] private GameObject[] starsUI; // assign 3 star images under this button

    private void Start()
    {
        UpdateStars();
    }

    public void UpdateStars()
    {
        int starsCollected = PlayerPrefs.GetInt("Level" + levelIndex + "_Stars", 0);

        // Enable stars based on progress
        for (int i = 0; i < starsUI.Length; i++)
        {
            starsUI[i].SetActive(i < starsCollected);
        }
    }
}
