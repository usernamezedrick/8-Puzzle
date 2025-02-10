using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace

public class MainMenuManager : MonoBehaviour
{
    public AudioSource backgroundMusic; // Drag the AudioSource (music) here
    public Button muteButton; // Drag the Button here
    public TMP_Text muteButtonText; // Drag the TMP Text component inside the button

    private bool isMuted = false;

    void Start()
    {
        // Load saved mute state
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        UpdateMuteState();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void OpenChallenges()
    {
        SceneManager.LoadScene("Challenges");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        UpdateMuteState();
    }

    private void UpdateMuteState()
    {
        backgroundMusic.mute = isMuted;
        muteButtonText.text = isMuted ? "Unmute" : "Mute";
    }
}
