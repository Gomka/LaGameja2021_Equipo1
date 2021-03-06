using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;

    private void Start()
    {
        PlayerPrefs.DeleteAll();
        playButton.Select();
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
