using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip ClickSFX;
    public int LevelToLoad;
    public GameObject ContinueButton;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            ContinueButton.SetActive(true);
            LevelToLoad = PlayerPrefs.GetInt("CurrentLevel", 1);
        }
        else
        {
            ContinueButton.SetActive(false);
            LevelToLoad = 1;
        }
    }

    public void StartGame()
    {
        _audioSource.PlayOneShot(ClickSFX);

        PlayerPrefs.DeleteKey("CurrentLevel");

        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        _audioSource.PlayOneShot(ClickSFX);

        Application.Quit();
    }

    public void ContinueGame()
    {
        _audioSource.PlayOneShot(ClickSFX);

        SceneManager.LoadScene(LevelToLoad);
    }
}
