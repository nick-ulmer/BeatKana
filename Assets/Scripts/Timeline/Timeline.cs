using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Timeline : MonoBehaviour
{

    public abstract void PlayManagerSetFields(PlayManager.TLFields tlFields);
    public abstract void StartGame();

    protected bool isGameOver = false;
    protected bool isLevelLoaded = false;
    protected int currentBeatIndex = 0; // The current beat which is seconds / BPM
    protected float levelProgress = 0f; // Between 0 and 1 for how far the level has progressed
    
    // Level Vars
    [SerializeField] protected Level levelObject; // Optional Level scriptable object. 
    
    protected int BPM = 60;
    protected float BeatDistance = 1.0f;
    protected float maxBeatError = 0.2f; // Can't be equal to or more than 0.25f;
    protected BeatElement[] beatElementsBank; // Assume in order for now. 

    protected int levelPreBeats = 3;
    protected int betweenBeats = 1;
    protected int levelPostBeats = 2;

    // Performance stats
    protected int totalPoints = 0; // Accumulated points
    protected SummaryScreen SummaryScreen;
    

    // Beat Processing and note keeping
    protected Beat currentBeat; // Reference to the current Beat class instance
    protected List<Beat> beatList; /// List of beat classes
    protected GameObject timelineBeatPrefab; // the actual visual beat prefab that reacts to input
    protected TimelineBeatObject[] beatObjects; // the list of beat objects that can be manipulated

    // Visual Section
    protected GameObject beatLine; // Visual guideline made at every beat position. 
    protected GameObject beatLineBlack; // Visual guideline made at every beat position. 
    protected LineRenderer progressBar; // Bar that display how far through the level the player is. 
    //TMP_InputField inputField;
    protected InputString InputString;
    protected TMP_Text currentWordTMP;
    protected TMP_Text currentKanaTMP;
    protected TMP_Text scoreDisplay;
    protected FeedbackGraphic FeedbackGraphic;

    protected bool CheckBeatGuards(string text)
    {
        if (text == string.Empty)
        {
            Debug.Log("Nothing in inputField");
            return false;
        }
        if (GameManager.gamePaused || // pause state stops input  
            isGameOver
            )
        {
            InputString.ResetString();
            Debug.Log("Can't input. Game is paused");
            return false;
        }
        return true;
    }


    protected bool UpdateGuards()
    {
        if (isGameOver) return false;
        if (!isLevelLoaded) return false;
        if (GameManager.gamePaused) return false; // don't update if game is paused
        return true;
    }


    protected void LevelEnd(bool completed)
    {
        isGameOver = true;
        //GameManager.PauseGame(true);

        // Update stats, then save game
        LevelCompletionRecord record = new LevelCompletionRecord(levelObject.LevelName, totalPoints, completed);

        GameManager.PlayerSaveData.RecordLevelResult(record); // Saves the completion record, then determines if 
        GameManager.PlayerSaveData.SaveToJson();

        // level ending transition
        // maybe goto a level success or failure screen with stats
        SummaryScreen.gameObject.SetActive(true);
        SummaryScreen.Activate();
        // switch to main menu with said conclusion screen
    }
}
