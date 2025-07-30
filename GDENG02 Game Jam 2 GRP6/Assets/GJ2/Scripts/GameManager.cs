using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("Video")]
    public VideoPlayer victoryVideo;
    public string mainMenuSceneName = "MainMenu";

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Time.timeScale = 0f; // Pause 
        ShowCursor(true);
        gameOverPanel.SetActive(true);
    }

    public void TriggerVictory()
    {
        if (isGameOver) return;
        isGameOver = true;

        //Time.timeScale = 0f; 
        ShowCursor(true);
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("EndCutscene");

    }

    void OnVictoryVideoEnd(VideoPlayer vp)
    {
        ReturnToMainMenu();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void ShowCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
}