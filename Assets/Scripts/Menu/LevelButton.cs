using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{

    
    [Header("Level & Text: Please Input")]
    [SerializeField] Level level; // This is the level to be loaded and save data referenced to in the menu
    [SerializeField] string displayText; 
    
    [Space(10)]
    [Header("Visual")]
    [SerializeField] Image image;
    [SerializeField] TMP_Text label;


    Button button;

    void Start()
    {
        MainMenuManager MainMenuManager = GetComponentInParent<MainMenuManager>();
        button = GetComponent<Button>();
        //image = GetComponent<Image>();
        //button.onClick.AddListener(() => MainMenuManager.GoToLevelButton(level));
        button.onClick.AddListener(() => MainMenuManager.ActivatePlayLevelInfo(this.level));
        label.text = displayText;

        if (GameManager.PlayerSaveData.IsLevelLocked(level))
        {
            ColorUtility.TryParseHtmlString("#79633F", out Color color);
            image.color = color;
            //image.color = Color.grey;
            
        }
        else if (!GameManager.PlayerSaveData.IsLevelCompleted(level.LevelName))
        {
            ColorUtility.TryParseHtmlString("#BECC6B", out Color color);
            image.color = color;
        } 
        else 
        {
            ColorUtility.TryParseHtmlString("#CCA76B", out Color color);
            image.color = color;
        }

    }
}
