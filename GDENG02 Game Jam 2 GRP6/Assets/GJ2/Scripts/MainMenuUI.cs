using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuUI : MonoBehaviour
{
    //[SerializeField] private Button playButton;
    //[SerializeField] private Button creditsButton;
    //[SerializeField] private Button exitButton;

    [SerializeField] private GameObject Controls;
    [SerializeField] private GameObject Credits;

    void Start()
    {
        //if (playButton != null)
        //    playButton.onClick.AddListener(OnPlayClicked);

        //if (creditsButton != null)
        //    creditsButton.onClick.AddListener(OnCreditsClicked);

        //if (exitButton != null)
        //    exitButton.onClick.AddListener(OnExitClicked);
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene("LevelDesign");
        Debug.Log("Loading Game Scene: LevelDesign");
        EventBroadcaster.Instance.PostEvent(EventNames.MainMenuEvents.ON_PLAY);
    }

    public void OnControlsClicked()
    {
        Controls.SetActive(true);
    }

    public void OnControlsCloseClicked()
    {
        Controls.SetActive(false);
    }

    public void OnCreditsClicked()
    {
        Credits.SetActive(true);
        EventBroadcaster.Instance.PostEvent(EventNames.MainMenuEvents.ON_CREDITS);
    }

    public void OnCreditsCloseClicked()
    {
        Credits.SetActive(false);
    }

    public void OnExitClicked()
    {
        EventBroadcaster.Instance.PostEvent(EventNames.MainMenuEvents.ON_EXIT);
        Application.Quit();
    }
}