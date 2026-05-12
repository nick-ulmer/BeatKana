using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static FeedbackGraphic;

public class QueueTimeline : Timeline
{
    #region Vars

    // QueueTimeline Specific
    float timeRemaining;
    float timeMultiplier = 1f;
    float maxTimeRemaining = 2f;

    LineRenderer timeRemainingBar; // Bar that displays time left for a beat in Queue

    #endregion

    #region Start(), LoadTimeline(), GenerateBeatListSequential() & MakeBeats()
    public override void PlayManagerSetFields(PlayManager.TLFields tlFields)
    {
        this.progressBar = tlFields.progressBar;
        this.timeRemainingBar = tlFields.timeRemainingBar;
        this.InputString = tlFields.InputString;
        this.SummaryScreen = tlFields.SummaryScreen;
        this.FeedbackGraphic = tlFields.FeedbackGraphic;
        this.currentWordTMP = tlFields.currentWordTMP;
        this.currentKanaTMP = tlFields.currentKanaTMP;
        this.scoreDisplay = tlFields.scoreDisplay;
    }
    public override void StartGame()
    {
        InputString.Init();
        InputString.UpdateStringEvent += CheckBeat;
        InputString.ResetString();
        currentKanaTMP.text = string.Empty;
        currentWordTMP.text = string.Empty;
        SummaryScreen.Init(); // Just sets its child to inactive and gets itself ready for activation
        SummaryScreen.gameObject.SetActive(false);
        FeedbackGraphic.Init();
        FeedbackGraphic.gameObject.SetActive(false);

        LoadTimeline();
    }

    void LoadTimeline()
    {
        // Grab GameManager level if it has one
        if (GameManager.currentLevel != null)
        {
            levelObject = GameManager.GetLevel();
            Debug.Log("Grabbing GameManager's recorded level");
        }
        else
        {
            Debug.Log("Using stored level");
        }

        // Inititialize level data. 

        levelObject.GetLevelData(
            ref BPM,
            ref BeatDistance,
            ref maxBeatError,
            ref beatElementsBank,

            ref levelPreBeats,
            ref betweenBeats,
            ref levelPostBeats
        );

        // Load beatLine
        beatLine = Resources.Load<GameObject>("BeatLine");
        beatLineBlack = Resources.Load<GameObject>("BeatLine BlackVariant");

        // Load TimelineBeat prefab, create beats list and generate TimelineBeats
        timelineBeatPrefab = Resources.Load<GameObject>("TimelineBeatPrefab");
        beatList = new List<Beat>();

        // Loop through data and create beats
        GenerateBeatListSequential(ref beatList);

        // Make BeatObjects
        beatObjects = MakeBeats(beatList);
        isLevelLoaded = true;
        AdvanceToNextBeat(0);
        GameManager.PauseGame(false);
    }

    // this method creates the beat class instances in a list in sequential order that they were created in the array
    void GenerateBeatListSequential(ref List<Beat> beatsList)
    {
        //Beat.AddEmptyBeats(ref beatsList, levelPreBeats);

        for (int i = 0; i < beatElementsBank.Length; i++)
        {
            beatElementsBank[i].ProcessToBeat(ref beatsList);
        }
    }

    // This method instantiates the beat prefabs into child objects of the Timeline. 
    TimelineBeatObject[] MakeBeats(List<Beat> beatList)
    {
        // TimelineBeat component array to return
        TimelineBeatObject[] beatObjects = new TimelineBeatObject[beatList.Count];

        // Iterate over all of the beat class instances
        for (int i = 0; i < beatList.Count; i++)
        {
            Vector3 offset = Vector3.right * BeatDistance * 0.5f; // Offset by half a BeatDistance (places beat square in the center of the correct bounds)
            Vector3 pos1 = Vector3.right * BeatDistance * i + offset;
            Vector3 pos2 = Vector3.right * BeatDistance * i;
            
            Instantiate(beatLine, pos1, Quaternion.identity, transform); // Create beatLine guide objects
            Instantiate(beatLineBlack, pos2, Quaternion.identity, transform); // Create beatLine guide objects

            if (beatList[i].text == string.Empty) continue; // skip if empty string beat class
            GameObject gameObject = Instantiate(timelineBeatPrefab, pos1, Quaternion.identity, transform); // Create the object
            beatObjects[i] = gameObject.GetComponent<TimelineBeatObject>(); // Grab the component
            beatObjects[i].SetBeat(beatList[i]); 
        }

        return beatObjects;
    }

    #endregion

    #region Runtime: CheckBeat(), Update()
    public void CheckBeat(string text) 
    {
        if (!base.CheckBeatGuards(text)) { return; }

        if (timeRemaining < 0) 
        {
            Debug.Log("No more time for CheckBeat");
            return;
        }

        

        // Correct Input
        if (text.Contains(currentBeat.text)) // if text match
        {
            int excessChars = text.Length - currentBeat.text.Length;
            int subtractPoints = excessChars * 20;
            totalPoints -= subtractPoints;

            //scoredSuccessfully = true;
            InputString.ResetString();

            int points = Mathf.FloorToInt(100f * (timeRemaining / maxTimeRemaining));

            // Add to total score. 
            totalPoints += points;

            // Update ScoreDisplay
            scoreDisplay.text = "Score: " + totalPoints.ToString();

            // Categorize Score, then InitiateFeedback for graphic. 
            FeedbackGraphic.InitiateFeedback(FeedbackGraphic.Degree.Perfect);


            Debug.Log("Scored: " + points.ToString() +" - "+ subtractPoints.ToString()+" = "+ (points-subtractPoints).ToString());

            AdvanceToNextBeat();
        }
    }

    bool CheckEnd() { if (currentBeatIndex >= beatList.Count) { LevelEnd(true); return true; } return false; }
    void AdvanceToNextBeat()
    {
        timeRemaining = maxTimeRemaining;
        currentBeatIndex++;
        if (CheckEnd()) return; // Check once

        while (beatList[currentBeatIndex].text == string.Empty)
        {
            currentBeatIndex++; // Skip empty beats
            if (CheckEnd()) return; // Check once
        }


        levelProgress = (float)currentBeatIndex / beatList.Count;
        currentBeat = beatList[currentBeatIndex];

        // Play Sound
        GameManager.AudioManager.PlayOneShot(AudioManager.Source.Kana, currentBeat.clip);

        InputString.ResetString();
    }
    void AdvanceToNextBeat(int index)
    {
        currentBeatIndex = index - 1;
        AdvanceToNextBeat();
    }

    void Update()
    {
        if (!base.UpdateGuards()) return;

        timeRemaining -= Time.deltaTime * timeMultiplier;
        if (timeRemaining < 0)  { AdvanceToNextBeat(); }


        float TimelinePos = currentBeatIndex * BeatDistance;
        var pos = Vector3.right * (-TimelinePos);
        transform.position = pos;

        // Update progress bar
        var progressBarPos = progressBar.GetPosition(1);

        var cam_left = GameManager.camWorldCorners[0].x;
        var cam_right = GameManager.camWorldCorners[2].x;
        
        var progress_rel_x = (cam_right - cam_left) * levelProgress;
        progressBarPos.x = progress_rel_x + cam_left;
        progressBar.SetPosition(1, progressBarPos);

        // Update timeRemaining bar
        var timeBarPos = timeRemainingBar.GetPosition(1);
        // Reuse cam_left and cam_right

        var relTime = timeRemaining / maxTimeRemaining;
        var time_rel_x = (cam_right - cam_left) * relTime;
        timeBarPos.x = time_rel_x + cam_left;
        timeRemainingBar.SetPosition(1, timeBarPos);

        Color barColor = Color.Lerp(Color.red, Color.green, relTime);
        timeRemainingBar.startColor = barColor;
        timeRemainingBar.endColor = barColor;
    }
    #endregion
}
