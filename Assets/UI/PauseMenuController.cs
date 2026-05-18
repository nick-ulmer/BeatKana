using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UIDocument pauseMenuDoc;
    [SerializeField] private PlayManager playManager;

    private VisualElement _root;

    private Slider _masterSlider;
    private Slider _tickSlider;
    private Slider _kanaSlider;

    private Button _retryButton;
    private Button _mainMenuButton;
    private Button _unpauseButton;

    private bool _isPaused;

    private void Awake()
    {
        if (pauseMenuDoc == null)
            pauseMenuDoc = GetComponent<UIDocument>();

        _root = pauseMenuDoc.rootVisualElement;

        _masterSlider = Q<Slider>("pause-slider-master");
        _tickSlider = Q<Slider>("pause-slider-tick");
        _kanaSlider = Q<Slider>("pause-slider-kana");

        _retryButton = Q<Button>("btn-retry");
        _mainMenuButton = Q<Button>("btn-main-menu");
        _unpauseButton = Q<Button>("btn-unpause");

        WireEvents();

        HidePauseMenu();
    }

    private void WireEvents()
    {
        _masterSlider.RegisterValueChangedCallback(evt =>
        {
            Debug.Log("Pause menu master volume: " + evt.newValue);
        });

        _tickSlider.RegisterValueChangedCallback(evt =>
        {
            Debug.Log("Pause menu metronome volume: " + evt.newValue);
        });

        _kanaSlider.RegisterValueChangedCallback(evt =>
        {
            Debug.Log("Pause menu kana volume: " + evt.newValue);
        });

        _retryButton.clicked += () =>
        {
            Debug.Log("Retry level requested.");
            // Later: reload current level / restart gameplay state.
            SceneManager.LoadScene("Scenes/PlayScene", LoadSceneMode.Single); 
        };

        _mainMenuButton.clicked += () =>
        {
            Debug.Log("Main menu requested.");
            // Later: SceneManager.LoadScene("MainMenu");
            SceneManager.LoadScene("Scenes/MainMenu", LoadSceneMode.Single); 
        };

        _unpauseButton.clicked += () =>
        {
            Debug.Log("Unpause requested.");
            playManager.PauseGame();
            //Unpause();
        };
    }

    public void TogglePause()
    {
        if (_isPaused)
            Unpause();
        else
            Pause();
    }

    public void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        ShowPauseMenu();

        Debug.Log("Game paused.");
    }

    public void Unpause()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        HidePauseMenu();

        Debug.Log("Game unpaused.");
    }

    private void ShowPauseMenu()
    {
        _root.style.display = DisplayStyle.Flex;
    }

    private void HidePauseMenu()
    {
        _root.style.display = DisplayStyle.None;
    }

    private T Q<T>(string name) where T : VisualElement
    {
        T result = _root.Q<T>(name);

        if (result == null)
            Debug.LogError($"PauseMenuController: could not find <{typeof(T).Name}> named '{name}'");

        return result;
    }
}