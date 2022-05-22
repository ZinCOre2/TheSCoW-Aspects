using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMethods : MonoBehaviour
{
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void EditDeck()
    {
        SceneManager.LoadScene(1);
    }
    
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
