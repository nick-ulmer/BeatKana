using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private VisualElement mainMenu;
    private VisualElement levelMenu;
    private VisualElement settingsMenu;

    private Button playButton;
    private Button endlessButton;
    private Button guidesButton;
    private Button settingsButton;
    private Button quitButton;

    private Button level1Button;
    private Button level2Button;
    private Button level3Button;
    private Button level4Button;
    private Button levelBackButton;

    private Button settingsBackButton;

    private Slider masterVolumeSlider;
    private Slider musicVolumeSlider;
    private Slider sfxVolumeSlider;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        mainMenu = root.Q<VisualElement>("main-menu");
        levelMenu = root.Q<VisualElement>("level-menu");
        settingsMenu = root.Q<VisualElement>("settings-menu");

        playButton = root.Q<Button>("play-button");
        endlessButton = root.Q<Button>("endless-button");
        guidesButton = root.Q<Button>("guides-button");
        settingsButton = root.Q<Button>("settings-button");
        quitButton = root.Q<Button>("quit-button");

        level1Button = root.Q<Button>("level-1-button");
        level2Button = root.Q<Button>("level-2-button");
        level3Button = root.Q<Button>("level-3-button");
        level4Button = root.Q<Button>("level-4-button");
        levelBackButton = root.Q<Button>("level-back-button");

        settingsBackButton = root.Q<Button>("settings-back-button");

        masterVolumeSlider = root.Q<Slider>("master-volume-slider");
        musicVolumeSlider = root.Q<Slider>("music-volume-slider");
        sfxVolumeSlider = root.Q<Slider>("sfx-volume-slider");

        playButton.clicked += ShowLevelMenu;
        endlessButton.clicked += StartEndlessMode;
        guidesButton.clicked += OpenGuides;
        settingsButton.clicked += ShowSettingsMenu;
        quitButton.clicked += QuitGame;

        level1Button.clicked += () => StartLevel("Level1");
        level2Button.clicked += () => StartLevel("Level2");
        level3Button.clicked += () => StartLevel("Level3");
        level4Button.clicked += () => StartLevel("Level4");
        levelBackButton.clicked += ShowMainMenu;

        settingsBackButton.clicked += ShowMainMenu;

        masterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);
        musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
        sfxVolumeSlider.RegisterValueChangedCallback(OnSfxVolumeChanged);

        ShowMainMenu();
    }

    private void OnDisable()
    {
        playButton.clicked -= ShowLevelMenu;
        endlessButton.clicked -= StartEndlessMode;
        guidesButton.clicked -= OpenGuides;
        settingsButton.clicked -= ShowSettingsMenu;
        quitButton.clicked -= QuitGame;

        levelBackButton.clicked -= ShowMainMenu;
        settingsBackButton.clicked -= ShowMainMenu;

        masterVolumeSlider.UnregisterValueChangedCallback(OnMasterVolumeChanged);
        musicVolumeSlider.UnregisterValueChangedCallback(OnMusicVolumeChanged);
        sfxVolumeSlider.UnregisterValueChangedCallback(OnSfxVolumeChanged);

        /*
         * Note:
         * The level buttons used lambda expressions, so they are not cleanly unsubscribed here.
         * This is acceptable for a simple persistent menu object, but for stricter cleanup,
         * use named methods instead of lambdas.
         */
    }

    private void ShowMainMenu()
    {
        ShowOnly(mainMenu);
    }

    private void ShowLevelMenu()
    {
        ShowOnly(levelMenu);
    }

    private void ShowSettingsMenu()
    {
        ShowOnly(settingsMenu);
    }

    private void ShowOnly(VisualElement menuToShow)
    {
        mainMenu.AddToClassList("hidden");
        levelMenu.AddToClassList("hidden");
        settingsMenu.AddToClassList("hidden");

        menuToShow.RemoveFromClassList("hidden");
    }

    private void StartLevel(string sceneName)
    {
        Debug.Log($"Starting level: {sceneName}");

        // Uncomment when your scenes exist and are added to Build Settings.
        // SceneManager.LoadScene(sceneName);
    }

    private void StartEndlessMode()
    {
        Debug.Log("Starting endless mode");

        // SceneManager.LoadScene("Endless");
    }

    private void OpenGuides()
    {
        Debug.Log("Opening guides");

        // Later, you could add a guides panel and call ShowOnly(guidesMenu).
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game");

#if UNITY_WEBGL
        Debug.Log("Application.Quit() does not work in WebGL.");
#else
        Application.Quit();
#endif
    }

    private void OnMasterVolumeChanged(ChangeEvent<float> evt)
    {
        float volume = evt.newValue;
        Debug.Log($"Master volume: {volume}");

        AudioListener.volume = volume;
    }

    private void OnMusicVolumeChanged(ChangeEvent<float> evt)
    {
        float volume = evt.newValue;
        Debug.Log($"Music volume: {volume}");

        // Example:
        // audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
    }

    private void OnSfxVolumeChanged(ChangeEvent<float> evt)
    {
        float volume = evt.newValue;
        Debug.Log($"SFX volume: {volume}");

        // Example:
        // audioMixer.SetFloat("SfxVolume", Mathf.Log10(volume) * 20f);
    }
}