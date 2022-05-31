using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    AudioSource music;

    private Button bNewGame;
    private Button bResumeGame;
    private Button bAbout;
    private Button bSettings;
    private Button bQuit;

    private Button bCloseAbout;
    private Button bCloseSettings;

    private GameObject pAbout;
    private GameObject pSettings;
    private GameObject pTutorial;

    private Slider sMusic;
    private Slider sSound;


    // Start is called before the first frame update
    void Start()
    {
        // Find the buttons in the scene
        bNewGame = GameObject.Find("ButtonNewGame").GetComponent<Button>();
        bResumeGame = GameObject.Find("ButtonResumeGame").GetComponent<Button>();
        bAbout = GameObject.Find("ButtonAbout").GetComponent<Button>();
        bSettings = GameObject.Find("ButtonSettings").GetComponent<Button>();
        bQuit = GameObject.Find("ButtonQuit").GetComponent<Button>();
        bCloseAbout = GameObject.Find("ButtonCloseAbout").GetComponent<Button>();
        bCloseSettings = GameObject.Find("ButtonCloseSettings").GetComponent<Button>();

        // Find the panels in the scene
        pAbout = GameObject.Find("PanelAbout");
        pSettings = GameObject.Find("PanelSettings");
        pTutorial = GameObject.Find("PanelTutorial");

        // Find the sliders in the scene
        sMusic = GameObject.Find("SliderMusic").GetComponent<Slider>();
        sSound = GameObject.Find("SliderSound").GetComponent<Slider>();

        // Find the audio source
        music = GetComponent<AudioSource>();

        // Add click listeners to the buttons
        bNewGame.onClick.AddListener(() => NewGame());
        bResumeGame.onClick.AddListener(() => ResumeGame());
        bAbout.onClick.AddListener(() => About());
        bSettings.onClick.AddListener(() => Settings());
        bQuit.onClick.AddListener(() => Quit());
        bCloseAbout.onClick.AddListener(() => ShowAboutPanel(false));
        bCloseSettings.onClick.AddListener(() => ShowSettingsPanel(false));

        // Add change listeners to the sliders
        sMusic.onValueChanged.AddListener(delegate { MusicSliderChanged(); });
        sMusic.value = 1f;
        sSound.onValueChanged.AddListener(delegate { SoundSliderChanged(); });

        // Set visibility
        ShowAboutPanel(false);
        ShowSettingsPanel(false);
        ShowTutorialPanel(false);
    }

    // Start a new game
    void NewGame() 
    {
        StartCoroutine(EnterTutorial());
    }

    // Enters the tutorial
    IEnumerator EnterTutorial()
    {
        ShowTutorialPanel(true);
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("NightScene");
    }

    // Resume a saved game
    void ResumeGame() 
    {
        switch (Variables.currentLevel)
        {
            case 1:
                SceneManager.LoadScene("NightScene");
                break;
            case 2:
                SceneManager.LoadScene("DayScene");
                break;
            case 3:
                SceneManager.LoadScene("NightScene");
                break;
        }
    }

    // Show the settings panel
    void Settings() 
    {
        ShowSettingsPanel(true);
    }

    // Show the about panel
    void About() 
    {
        ShowAboutPanel(true);
    }

    // Quit the application
    void Quit() 
    {
        Application.Quit();
        Debug.Log("The Application.Quit call is ignored in the Editor");
    }

    // Change music volume
    void MusicSliderChanged()
    {
        Debug.Log("Music slider changed: " + sMusic.value);
        music.volume = sMusic.value;
        Debug.Log("music volume: " + music.volume);
        
    }

    // Change sound effects volume
    void SoundSliderChanged()
    {
        Debug.Log("Sound slider changed: " + sSound.value * 2f);
    }

    // Helper function
    void ShowAboutPanel(bool visible) 
    {
        pAbout.gameObject.SetActive(visible);
    }

    // Helper function
    void ShowSettingsPanel(bool visible) 
    {
        pSettings.gameObject.SetActive(visible);
    }

    // Helper function
    void ShowTutorialPanel(bool visible) 
    {
        pTutorial.gameObject.SetActive(visible);
    }
}
