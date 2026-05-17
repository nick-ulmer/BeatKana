using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    #region Stored Level Data
    private static Dictionary<string, Level> _allLevelData;
    public static void LoadLevelData() // Lazy initialization
    {
        if (_allLevelData != null) return; // already initialized

        _allLevelData = new Dictionary<string, Level>();
        Level[] levels = Resources.LoadAll<Level>("ScriptableObjects/Levels/");
        foreach (Level level in levels)
        {
            _allLevelData.Add(level.LevelName, level);
        }
    }
    public static bool TryGetLevelByName(string levelName, out Level level)
    {
        return _allLevelData.TryGetValue(levelName, out level);
    }
    #endregion

    #region Define structs & enums
    [Serializable] 
    public struct MenuFields
    {
        public string name;
        public string description;
        public MenuFields(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }

    [Serializable]
    public struct DifficultyFields
    {
        public int BPM;
        public float BeatDistance;
        public float maxBeatError;
        public bool hideKana;
        public bool mute;
        public DifficultyFields(int BPM, float BeatDistance, float maxBeatError, bool hideKana, bool mute)
        {
            this.BPM = BPM;
            this.BeatDistance = BeatDistance;
            this.maxBeatError =maxBeatError;
            this.hideKana = hideKana;
            this.mute = mute;
        }
    }
    [Serializable]
    public struct BeatCounts
    {
        public int levelPreBeats;
        public int betweenBeats;
        public int levelPostBeats;
        public BeatCounts(int levelPreBeats, int betweenBeats, int levelPostBeats)
        {
            this.levelPreBeats = levelPreBeats;
            this.betweenBeats = betweenBeats;
            this.levelPostBeats = levelPostBeats;
        }
    } 
    [Serializable]
    public enum LevelType { Beat, Queue, BeatCombo } 
    [Serializable] 
    public struct Prereqs
    {
        public int minScore;
        public Level[] requiredLevels;
        public Prereqs(int minScore, Level[] requiredLevels)
        {
            this.minScore = minScore;
            this.requiredLevels = requiredLevels;
        }
    }
    #endregion

    // Name used to assist in saved data referencing
    // Used as an ID to make sure that where ever a level is being loaded (the button, the level, etc), it can also use the proper saved data there. 
    public string LevelName = string.Empty; // Technically also an ID
    public LevelType levelType = LevelType.Beat; 
    public MenuFields menuFields = new MenuFields("<line-height=65%>愛 あい Love", "Learn Love!");
    public DifficultyFields difficulty = new DifficultyFields(60, 1.0f, 0.2f, false, false); 
    public BeatElement[] beatElementsBank; // Assume in order for now. 
    public BeatCounts beatCounts = new BeatCounts(3, 1, 2); // Only valid for LevelType.Beat
    public Prereqs prereqs = new Prereqs(0, new Level[0]); // Determines which levels or score are necessary to access level

    [TextArea(3, 8)] 
    [SerializeField] string Notes = "This is a notes section used to detail a level's purpose and what it introduces to the player.";

    #region Methods
    public void GetLevelData(
        ref DifficultyFields difficulty,
        ref BeatElement[] beatElementsBank,
        ref BeatCounts beatCounts
        )
    {
        var _ = this.Notes;
        difficulty = this.difficulty;
        beatElementsBank = this.beatElementsBank;
        beatCounts = this.beatCounts;
    }

    // method WITHOUT Level Name
    public void GetLevelData(
        ref int BPM,
        ref float BeatDistance,
        ref float maxBeatError,
        ref BeatElement[] beatElementsBank,

        ref int levelPreBeats,
        ref int betweenBeats,
        ref int levelPostBeats
        )
    {
        BPM = this.difficulty.BPM;
        BeatDistance = this.difficulty.BeatDistance;
        maxBeatError = this.difficulty.maxBeatError;

        beatElementsBank = this.beatElementsBank;

        levelPreBeats = this.beatCounts.levelPreBeats;
        betweenBeats = this.beatCounts.betweenBeats;
        levelPostBeats = this.beatCounts.levelPostBeats;
    }

    // method WITH Level Name
    public void GetLevelData(
        ref string LevelName,
        ref int BPM,
        ref float BeatDistance,
        ref float maxBeatError,
        ref BeatElement[] beatElementsBank,

        ref int levelPreBeats,
        ref int betweenBeats,
        ref int levelPostBeats
        )
    {
        LevelName = this.LevelName;
        GetLevelData(
                ref BPM,
                ref BeatDistance,
                ref maxBeatError,
                ref beatElementsBank,

                ref levelPreBeats,
                ref betweenBeats,
                ref levelPostBeats
            );
    }
    #endregion
}
