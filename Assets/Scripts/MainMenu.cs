using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void Playgame()
    {
        SceneManager.LoadSceneAsync(1); // Load the scene with index 1
    }
    public void Quitgame()
    {
        Application.Quit();
    }

}
