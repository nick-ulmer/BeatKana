using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    [Header("GameManager Room Initializables")]
    [SerializeField] AudioManager AudioManager;
    [SerializeField] Camera Camera;

    [Header("UI Documents")]
    [SerializeField] UIDocument mainMenuDoc;
    [SerializeField] UIDocument settingsDoc;
    [SerializeField] UIDocument chapterSelectDoc;
    [SerializeField] UIDocument levelSelectDoc;

    [SerializeField] Chapter[] chapters;

    readonly Stack<UIDocument> _history = new Stack<UIDocument>();

    VisualElement _mainMenuRoot;
    VisualElement _settingsRoot;
    VisualElement _chapterSelectRoot;
    VisualElement _levelSelectRoot;

    void Awake()
    {
        GameManager.AudioManager = AudioManager;
        GameManager.cam = Camera;

        _mainMenuRoot = mainMenuDoc.rootVisualElement;
        _settingsRoot = settingsDoc.rootVisualElement;
        _chapterSelectRoot = chapterSelectDoc.rootVisualElement;
        _levelSelectRoot = levelSelectDoc.rootVisualElement;

        HideAllScreens();

        WireMainMenu();
        WireSettings();
        WireChapterSelect();
        WireLevelSelect();

        ShowScreen(mainMenuDoc);
    }

    void WireMainMenu()
    {
        Q<Button>(_mainMenuRoot, "btn-play").clicked += () => ShowScreen(chapterSelectDoc);
        Q<Button>(_mainMenuRoot, "btn-settings").clicked += () => ShowScreen(settingsDoc);

        Button exitButton = _mainMenuRoot.Q<Button>("btn-exit");
        if (exitButton != null)
            exitButton.clicked += () => Application.Quit();

        Q<Button>(_mainMenuRoot, "btn-endless").clicked += () => Debug.Log("Endless: placeholder");
        Q<Button>(_mainMenuRoot, "btn-guide").clicked += () => Debug.Log("Guide: placeholder");
    }
    void WireSettings()
    {
        Q<Button>(_settingsRoot, "btn-back").clicked += GoBack;

        Q<Slider>(_settingsRoot, "slider-master").RegisterValueChangedCallback(
            evt => Debug.Log("Master volume: " + evt.newValue));

        Q<Slider>(_settingsRoot, "slider-tick").RegisterValueChangedCallback(
            evt => Debug.Log("Tick volume: " + evt.newValue));

        Q<Slider>(_settingsRoot, "slider-kana").RegisterValueChangedCallback(
            evt => Debug.Log("Kana volume: " + evt.newValue));
    }
    void WireChapterSelect()
    {
        Q<Button>(_chapterSelectRoot, "btn-back").clicked += GoBack;

        VisualElement chapterStrip = Q<VisualElement>(_chapterSelectRoot, "chapter-strip");
        chapterStrip.Clear();

        for (int i = 0; i < chapters.Length; i++)
        {
            int chapterIndex = i;
            Chapter chapter = chapters[i];

            Button chapterButton = new Button
            {
                name = $"btn-chapter-{i + 1}",
                text = chapter.Title
            };

            chapterButton.AddToClassList("chapter-rect");

            chapterButton.clicked += () =>
            {
                Debug.Log($"Selected chapter {chapterIndex + 1}: {chapter.Title}");
                ShowScreen(levelSelectDoc);
                PopulateLevelSelect(chapter.Levels);
            };

            chapterStrip.Add(chapterButton);
        }
    }
    void WireLevelSelect()
    {
        Q<Button>(_levelSelectRoot, "btn-back").clicked += GoBack;
        Q<Button>(_levelSelectRoot, "btn-play-level").clicked += PlaySelectedLevel;
    }

    void PopulateLevelSelect(Level[] levels)
    {
        ScrollView levelScroll = Q<ScrollView>(_levelSelectRoot, "level-scroll");
        VisualElement levelStrip = Q<VisualElement>(_levelSelectRoot, "level-strip");
        levelStrip.Clear();

        for (int i = levels.Length - 1; i >= 0; i--)
        {
            int levelIndex = i;
            Level level = levels[i];

            Button levelButton = new Button
            {
                name = $"btn-level-{i + 1}",
                text = "" // important: don't use built-in text if adding custom content
            };

            levelButton.AddToClassList("level-rect");

            VisualElement textColumn = new VisualElement();
            textColumn.AddToClassList("level-vertical-text");

            foreach (char c in level.menuFields.name)
            {
                if (c == ' ')
                {
                    VisualElement space = new VisualElement();
                    space.AddToClassList("vertical-text-space");
                    textColumn.Add(space);
                }
                else
                {
                    Label charLabel = new Label(c.ToString());
                    charLabel.AddToClassList("vertical-text-char");
                    textColumn.Add(charLabel);
                }
            }

            levelButton.Add(textColumn);

            levelButton.clicked += () =>
            {
                SelectLevel(levels[levelIndex], levelIndex);
            };

            levelStrip.Add(levelButton);
        }

        if (levels.Length > 0)
            SelectLevel(levels[0], 0);

        levelScroll.schedule.Execute(() =>
        {
            levelScroll.horizontalScroller.value = levelScroll.horizontalScroller.highValue;
        });
    }

    string ToVerticalText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        return string.Join("\n", text.ToCharArray());
    }

    Level _selectedLevel;
    void SelectLevel(Level level, int index)
    {
        _selectedLevel = level;

        Label title = Q<Label>(_levelSelectRoot, "detail-title");
        Label subtitle = Q<Label>(_levelSelectRoot, "detail-subtitle");
        Label description = Q<Label>(_levelSelectRoot, "detail-description");

        /*title.text = level.displayName;       // change field names
        subtitle.text = level.subtitle;        // change field names
        description.text = level.description;  // change field names*/

        title.text = level.LevelName;       // change field names
        subtitle.text = "level subtitle";        // change field names
        description.text = "level description";  // change field names
    }

    void PlaySelectedLevel()
    {
        if (_selectedLevel == null)
        {
            Debug.LogWarning("No level selected.");
            return;
        }

        Debug.Log("Play level: " + _selectedLevel.name);

        // Later:
        // SceneManager.LoadScene(_selectedLevel.sceneName);

        GameManager.SetLevel(_selectedLevel);
        SceneManager.LoadScene("Scenes/PlayScene", LoadSceneMode.Single);
    }

    void ShowScreen(UIDocument doc)
    {
        HideAllScreens();

        doc.rootVisualElement.style.display = DisplayStyle.Flex;

        if (_history.Count == 0 || _history.Peek() != doc)
            _history.Push(doc);
    }

    void HideAllScreens()
    {
        _mainMenuRoot.style.display = DisplayStyle.None;
        _settingsRoot.style.display = DisplayStyle.None;
        _chapterSelectRoot.style.display = DisplayStyle.None;
        _levelSelectRoot.style.display = DisplayStyle.None;
    }

    void GoBack()
    {
        if (_history.Count <= 1)
            return;

        _history.Pop();

        UIDocument previous = _history.Pop();
        ShowScreen(previous);
    }

    T Q<T>(VisualElement root, string name) where T : VisualElement
    {
        T result = root.Q<T>(name);

        if (result == null)
            Debug.LogError($"UIManager: could not find <{typeof(T).Name}> named '{name}'");

        return result;
    }

    [Serializable] struct Chapter
    {
        public string Title;
        public string Description;
        public Level[] Levels;
    }
}