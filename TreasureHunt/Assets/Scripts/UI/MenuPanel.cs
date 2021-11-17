using UnityEngine;

public class MenuPanel : MonoBehaviour 
{
    private void Awake()
    {
        AudioManager.Instance.StopBGM();
    }

    public void OnCloseButtonClick()
    {
        Application.Quit();
    }

    public void OnStartButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
}
