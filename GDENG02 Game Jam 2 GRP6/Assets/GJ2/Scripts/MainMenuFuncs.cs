using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFuncs : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBroadcaster.Instance.AddObserver(EventNames.MainMenuEvents.ON_PLAY, LoadGame);
        EventBroadcaster.Instance.AddObserver(EventNames.MainMenuEvents.ON_CREDITS, ShowCredits);
        EventBroadcaster.Instance.AddObserver(EventNames.MainMenuEvents.ON_EXIT, ExitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    void OnDestroy()
    {
        EventBroadcaster.Instance.RemoveObserver(EventNames.MainMenuEvents.ON_PLAY);
        EventBroadcaster.Instance.RemoveObserver(EventNames.MainMenuEvents.ON_CREDITS);
        EventBroadcaster.Instance.RemoveObserver(EventNames.MainMenuEvents.ON_EXIT);
    }

    public void LoadGame()
    {
        Debug.Log("Loading Game Scene: LevelDesign");
        SceneManager.LoadScene("LevelDesign");
        
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
