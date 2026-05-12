using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static FeedbackGraphic;
using static UnityEngine.Rendering.DebugUI;

public class BeatComboTimeline : Timeline
{
    #region Vars

    // BeatTimeline specific 

    float BPS; // Calculate from BPM / 60 seconds per minute;
    int previousBeatIndex = 0; // Used to play code at every beat increment.
    float beatTime = 0f; // Accumulated actual time that has passed
    float aroundBeatApex = 0f; // time between now and the actual current beat point.
    AudioClip tickAudioClip; // Metronome sound effect. 
    bool CanPlayTick = true;
    float tickPoint; // Time in beatTime when tick sound is played. 

    float health = 1f;
    void AddHealth(float value) 
    {
        health += value;
        health = Mathf.Clamp(health, 0f, 1f);
        if (health <= 0f)
            LevelEnd(false);
    }

    LineRenderer healthBar;
    #endregion

    #region Start(), LoadTimeline(), GenerateBeatListSequential() & MakeBeats()
    public override void PlayManagerSetFields(PlayManager.TLFields tlFields)
    {
        this.progressBar = tlFields.progressBar;
        this.healthBar = tlFields.timeRemainingBar;
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

        tickAudioClip = Resources.Load<AudioClip>("Synth_Tick_A_hi");

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


        // BPM based on level data
        //BPM *= 2; // ALTERATION FOR BeatComboTL

        BPS = 60f / BPM;
        tickPoint = -(tickAudioClip.length / BPS) / 2f;

        // Load beatLine
        beatLine = Resources.Load<GameObject>("BeatLine");
        beatLineBlack = Resources.Load<GameObject>("BeatLine BlackVariant");

        // Load TimelineBeat prefab, create beats list and generate TimelineBeats
        timelineBeatPrefab = Resources.Load<GameObject>("TimelineBeatPrefabExp");
        beatList = new List<Beat>();

        // Loop through data and create beats
        GenerateBeatListSequential(ref beatList);

        // Make BeatObjects
        beatObjects = MakeBeats(beatList);
        isLevelLoaded = true;
        GameManager.PauseGame(false);
        NewBeat();
    }

    // this method creates the beat class instances in a list in sequential order that they were created in the array
    void GenerateBeatListSequential(ref List<Beat> beatsList)
    {
        Beat.AddEmptyBeats(ref beatsList, levelPreBeats * 2);

        for (int i = 0; i < beatElementsBank.Length; i++)
        {
            //Beat.ProcessElement(ref beatsList, beatElementsBank[i]);
            beatElementsBank[i].ProcessToComboBeat(ref beatsList);
            //beatElementsBank[i].ProcessToComboBeat(ref beatsList);
            Beat.AddEmptyBeats(ref beatsList, betweenBeats * 2);
        }

        Beat.AddEmptyBeats(ref beatsList, levelPostBeats * 2);
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
            
            if ((i % 2) == 0)
                Instantiate(beatLine, pos1, Quaternion.identity, transform); // Create beatLine guide objects
            //Instantiate(beatLineBlack, pos2, Quaternion.identity, transform); // Create beatLine guide objects

            if (beatList[i].text == string.Empty) continue; // skip if empty string beat class
            GameObject gameObject = Instantiate(timelineBeatPrefab, pos1, Quaternion.identity, transform); // Create the object
            beatObjects[i] = gameObject.GetComponent<TimelineBeatObject>(); // Grab the component
            beatObjects[i].SetBeat(beatList[i]); // Sets text and visual of the timeline beat object. 
        }

        return beatObjects;
    }

    #endregion

    #region Runtime: CheckBeat(), Update()
    public void CheckBeat(string text) 
    {
        if (!base.CheckBeatGuards(text)) { return; }

        // Punish player if inputted during wrong moments
        if ((tlState != TLState.Input || // Quit if can't score now.
            //scoredSuccessfully || // Don't search any longer if scored already.
            currentBeat.text == string.Empty) && // Don't search if currently an empty beat
            text != string.Empty
            )
        /*if ((!duringKeyRecognition || // Quit if can't score now.
            scoredSuccessfully || // Don't search any longer if scored already.
            currentBeat.text == string.Empty) && // Don't search if currently an empty beat
            text != string.Empty
            )*/
        { 
            Debug.Log("Can't input. Wrong time!");
            // punish player code
            totalPoints -= 20;
            AddHealth(-0.2f);

            // Update ScoreDisplay
            scoreDisplay.text = "Score: " + totalPoints.ToString();

            FeedbackGraphic.InitiateFeedback(FeedbackGraphic.Degree.WrongTime);

            //inputField.text = string.Empty;
            InputString.ResetString();
            return;
        }

        // Correct Input
        if (text.Contains(currentBeat.text)) // if text match
        {
            //scoredSuccessfully = true;
            //inputField.text = string.Empty;
            InputString.ResetString();
            float accuracy = 1f - (Mathf.Abs(aroundBeatApex) / maxBeatError); // Accuracy depending on distance to beat. 
            float round = Mathf.Floor(accuracy * 100f) / 100f; // reduce to 2 decimal places
            accuracy = Mathf.Clamp01(round); // Clamp between 0 and 1

            var points = Mathf.RoundToInt(accuracy * 100f);
            Debug.Log("Scored: " + points.ToString());

            // Add to total score. 
            totalPoints += points;
            AddHealth(accuracy * 0.1f);

            // Update ScoreDisplay
            scoreDisplay.text = "Score: " + totalPoints.ToString();

            // Categorize Score, then InitiateFeedback for graphic. 
            FeedbackGraphic.InitiateFeedback(FeedbackGraphic.Degree.Perfect);
            tlState = TLState.After;
        }
    }


    enum TLState { Before, Input, After }
    [SerializeField] TLState tlState = TLState.Before;
    void NewBeat()
    {
        previousBeatIndex = currentBeatIndex;

        currentBeat = beatList[currentBeatIndex]; // The actual current Beat class instance being focused on in the current beat window

        // Update Visuals based on current Beat class
        if (currentBeatIndex < beatList.Count)
        {
            if (currentBeat.text == string.Empty)
            {
                tlState = TLState.After;
            }
            currentKanaTMP.text = beatList[currentBeatIndex].text;
            currentWordTMP.text = beatList[currentBeatIndex].word;

            // Play Sound
            if (currentBeat.clip != null)
            {
                GameManager.AudioManager.PlayOneShot(AudioManager.Source.Kana, currentBeat.clip);
            }
        }
        else
        {
            currentKanaTMP.text = string.Empty;
            currentWordTMP.text = string.Empty;
        }
    }
    void InputStart()
    {

    }
    void InputMiss()
    {
        FeedbackGraphic.InitiateFeedback(FeedbackGraphic.Degree.Miss);

        AddHealth(-0.2f);

        InputString.ResetString();
    }

    void Update()
    {
        if (!base.UpdateGuards()) return;
        beatTime += Time.deltaTime / BPS; // Time counted as amount of beats (float, not discrete int)
        currentBeatIndex = Mathf.FloorToInt(beatTime);  // time as discrete int
        if (currentBeatIndex >= beatList.Count) { LevelEnd(true); return; } // End level when beat index hits end of beatList count

        aroundBeatApex = (beatTime - currentBeatIndex) - 0.5f; // Amount of time between now and the apex of the current beat. 

        levelProgress = beatTime / beatList.Count;

        //float _tickPoint = -(tickAudioClip.length / 2f);
        if (CanPlayTick && aroundBeatApex >= tickPoint)
        {
            // Play Sound
            GameManager.AudioManager.PlayTick();
            Debug.Log(tickAudioClip.length.ToString());

            CanPlayTick = false;
        } else if (aroundBeatApex < tickPoint) { CanPlayTick = true; }

        // Use beatTime, currentBeatIndex, and aroundBeatApex to compute beat states. 
        switch (tlState)
        {
            case TLState.Before:
                if (aroundBeatApex >= -maxBeatError)
                {
                    tlState = TLState.Input;
                    InputStart();
                }
                break;
            case TLState.Input:
                if (aroundBeatApex >= maxBeatError)
                {
                    tlState = TLState.After;
                    InputMiss();
                }
                break;
            case TLState.After:
                if (currentBeatIndex != previousBeatIndex)
                {
                    tlState = TLState.Before;
                    //CanPlayTick = true;
                    NewBeat();
                }
                break;
        }

        

        float TimelinePos = beatTime * BeatDistance;
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
        var healthBarPos = healthBar.GetPosition(1);
        // Reuse cam_left and cam_right

        //var relTime = health;
        var time_rel_x = (cam_right - cam_left) * health;
        healthBarPos.x = time_rel_x + cam_left;
        healthBar.SetPosition(1, healthBarPos);

        Color barColor = Color.Lerp(Color.red, Color.green, health);
        healthBar.startColor = barColor;
        healthBar.endColor = barColor;
    }
    #endregion
}
