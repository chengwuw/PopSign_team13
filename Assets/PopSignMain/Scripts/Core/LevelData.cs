using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Xml;

public enum ModeGame
{
    Vertical=0,
    Rounded,
}
public class LevelData
{
    public static LevelData Instance;

    public static int[] map = new int[11 * 70];

    //List of mission in this map
    public static List<Mission> requestMissions = new List<Mission>();
    public static ModeGame mode = ModeGame.Vertical;
    private static float limitAmount = 40;

    public static float LimitAmount
    {
        get { return LevelData.limitAmount; }
        set
        {
            LevelData.limitAmount = value;
            if( value < 0 ) LevelData.limitAmount = 0;
        }
    }
    private static bool startReadData;
    public static Dictionary<int, BallColor> colorsDict = new Dictionary<int, BallColor>();
    static int key;
    public static int colors;
    public static int star1;
    public static int star2;
    public static int star3;

    // public static void LoadDataFromXML(int currentLevel)
    // {
    //     requestMissions.Clear();
    //     TextAsset textReader = Resources.Load( "AlternateLevels/" + currentLevel + ".txt") as TextAsset;
    //     ProcessGameDataFromXML( textReader );

    // }

    // public static void LoadDataFromLocal(int currentLevel)
    // {
    //     requestMissions.Clear();
    //     //Read data from text file
    //     TextAsset mapText = Resources.Load("AlternateLevels/" + currentLevel) as TextAsset;
    //     ProcessGameDataFromString(mapText.text);
    // }
    public static void LoadDataFromXML(int currentLevel)
    {
        // Find the handlers for the toggles
        TopicSetToggleHandler topicHandler = GameObject.FindObjectOfType<TopicSetToggleHandler>();
        LevelSetToggleHandler levelHandler = GameObject.FindObjectOfType<LevelSetToggleHandler>();

        // Ensure handlers are not null
        if (topicHandler == null || levelHandler == null)
        {
            Debug.LogError("Toggle handlers are not set in the scene.");
            return;
        }

        // Determine which toggle is active
        string dataPath;
        if (topicHandler.GetLevelSetPath() == "TopicLevels/")
        {
            // Topic levels toggle is active
            dataPath = topicHandler.GetLevelSetPath();
        }
        else if (levelHandler.GetLevelSetPath() == "AlternateLevels/")
        {
            // Level set toggle is active
            dataPath = levelHandler.GetLevelSetPath();
        }
        else
        {
            Debug.Log("No valid toggle handler or path detected. Defaulting to 'Levels/'.");
            dataPath = "Levels/";
        }

        // Load the data from the chosen path
        string fullPath = dataPath + currentLevel + ".txt";
        TextAsset textReader = Resources.Load(fullPath) as TextAsset;

        if (textReader == null)
        {
            Debug.LogError($"Failed to load data from path: {fullPath}");
            return;
        }

        // Process the loaded XML data
        ProcessGameDataFromXML(textReader);
    }

    // public static void LoadDataFromLocal(int currentLevel)
    // {
    //     LevelSetToggleHandler toggleHandler = GameObject.FindObjectOfType<LevelSetToggleHandler>();
    //     string levelSetPath = toggleHandler.GetLevelSetPath();
    //     TextAsset mapText = Resources.Load(levelSetPath + currentLevel) as TextAsset;
    //     ProcessGameDataFromString(mapText.text);
    // }
    public static void LoadDataFromLocal(int currentLevel)
    {
        // Find the toggle handlers
        TopicSetToggleHandler topicHandler = GameObject.FindObjectOfType<TopicSetToggleHandler>();
        LevelSetToggleHandler levelHandler = GameObject.FindObjectOfType<LevelSetToggleHandler>();

        // Ensure handlers are not null
        if (topicHandler == null || levelHandler == null)
        {
            Debug.LogError("Toggle handlers are not set in the scene.");
            return;
        }

        // Determine which path to use based on active toggle
        string dataPath;
        if (topicHandler.GetLevelSetPath() == "TopicLevels/")
        {
            // Use the path for Topic Levels
            dataPath = topicHandler.GetLevelSetPath();
        }
        else if (levelHandler.GetLevelSetPath() == "AlternateLevels/")
        {
            // Level set toggle is active
            dataPath = levelHandler.GetLevelSetPath();
        }
        else
        {
            Debug.Log("No valid toggle handler or path detected. Defaulting to 'Levels/'.");
            dataPath = "Levels/";
        }

        // Construct the full path
        string fullPath = dataPath + currentLevel;

        // Load the map text asset from Resources
        TextAsset mapText = Resources.Load(fullPath) as TextAsset;

        if (mapText == null)
        {
            Debug.LogError($"Failed to load data from path: {fullPath}");
            return;
        }

        // Process the loaded data as a string
        ProcessGameDataFromString(mapText.text);
    }

    public static void LoadDataFromURL(int currentLevel)
    {
        //Read data from your server, if you want
    }
    static void ProcessGameDataFromString(string mapText)
    {
        //Structure of text file like this:
        //1st: Line start with "GM". This is game mode line (0-Move Limit, 1-Time Limit)
        //2nd: Line start with "LMT" is limit amount of play time (time of move or seconds depend on game mode)
        //Ex: LMT 20  mean player can move 20 times or 20 seconds, depend on game mode
        //3rd: Line start with "MNS" is missions line. This is amount ofScore/Block/Ring/...
        //Ex: MNS 10000/24/0' mean user need get 1000 points, 24 block, and not need to get rings.
        //4th:Map lines: This is an array of square types.
        //First thing is split text to get all in arrays of text
        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int mapLine = 0;
        foreach (string line in lines)
        {
            //check if line is game mode line
            if (line.StartsWith("GM="))
            {
                //Replace GM to get mode number,
                string modeString = line.Replace("GM=", string.Empty).Trim();
                //then parse it to interger
                int modeNum = int.Parse(modeString);
                //Assign game mode
                mode = (ModeGame)modeNum;
            }
            else if (line.StartsWith("LMT="))
            {
                //Replace LTM to get limit number,
                string amountString = line.Replace("LMT=", string.Empty).Trim();
                //then parse it to interger and assign to limitAmount
                limitAmount = int.Parse(amountString);
            }
            //check third line to get missions
            else if (line.StartsWith("MNS"))
            {
                //Replace 'MNS' to get mission numbers
                string missionString = line.Replace("MNS", string.Empty).Trim();
                //Split again to get mission numbers
                string[] missionNumbers = missionString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < missionNumbers.Length; i++)
                {
                    //Set scores of mission and mission type
                    int amount = int.Parse(missionNumbers[i].Trim());
                    MissionType type = (MissionType)i;
                    if (amount > 0)
                        requestMissions.Add(new Mission(amount, type));
                }
            }
            else if (line.StartsWith("data="))
            {
                startReadData = true;
            }
            else if (startReadData)//Maps
            {
                //Split lines again to get map numbers
                string[] squareTypes = line.Replace("\r", string.Empty).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < squareTypes.Length; i++)
                {
                    int value = int.Parse(squareTypes[i].Trim());
                    //if (!colorsDict.ContainsValue((BallColor)mapValue) && mapValue != 9)
                    //    colorsDict.Add(key++, (BallColor)mapValue);

                    map[mapLine * creatorBall.columns + i] = value;
                }
                mapLine++;
            }
        }
    }

    static void ProcessGameDataFromXML( TextAsset xmlString )
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml( xmlString.text );
        XmlNodeList elemList = doc.GetElementsByTagName("property");
        foreach (XmlElement element in elemList)
        {
            if (element.GetAttribute("name") == "GM") mode = (ModeGame)int.Parse(element.GetAttribute("value"));
            if (element.GetAttribute("name") == "LMT") limitAmount = int.Parse(element.GetAttribute("value"));
            if (element.GetAttribute("name") == "COLORS") colors = int.Parse(element.GetAttribute("value"));
            foreach (Mission mission in requestMissions)
            {

                if (element.GetAttribute("name") == "STAR1") star1 = 5;//int.Parse(element.GetAttribute("value"));
                    if (element.GetAttribute("name") == "STAR2") star2 = 10;//int.Parse(element.GetAttribute("value"));
                    if (element.GetAttribute("name") == "STAR3") star3 = 15; //int.Parse(element.GetAttribute("value"));   

            }

            //    Debug.Log(element.GetAttribute("value"));
        }

        elemList = doc.GetElementsByTagName("tile");
        colorsDict.Clear();
        key = 0;

        for (int i = 0; i < creatorBall.rows; i++)
        {
            for (int j = 0; j < creatorBall.columns; j++)
            {
                XmlElement element = (XmlElement)elemList[i * creatorBall.columns + j];
                int value = int.Parse(element.GetAttribute("gid"));

                if (!colorsDict.ContainsValue((BallColor)value) && value > 0 && value < (int)BallColor.random)
                {
                        colorsDict.Add(key, (BallColor)value);
                        key++;

                }

                map[i * creatorBall.columns + j] = value;
            }

        }


        //random colors
        if (colorsDict.Count == 0)
        {
            //add constant colors
            colorsDict.Add(0, BallColor.yellow);
            colorsDict.Add(1, BallColor.red);

            //add random colors
            List<BallColor> randomList = new List<BallColor>();
            randomList.Add(BallColor.blue);
            randomList.Add(BallColor.green);
            if( mode != ModeGame.Rounded )
                randomList.Add(BallColor.violet);
            for (int i = 0; i < colors-2; i++)
            {
                BallColor randCol = BallColor.yellow;
                while (colorsDict.ContainsValue(randCol))
                {
                    randCol = randomList[UnityEngine.Random.Range(0, randomList.Count)];
                }
                colorsDict.Add(2 + i, randCol);

            }

        }
        //foreach (XmlElement element in elemList)
        //{
        //    Debug.Log(element.GetAttribute("gid"));
        //}

    }

    public static int GetScoreTarget(int currentLevel)
    {
        LoadDataFromLocal(currentLevel);
        return GetMission(MissionType.Stars).amount;
    }

    public static Mission GetMission(MissionType type)
    {
        return requestMissions.Find(obj => obj.type == type);
    }

    public static Target GetTarget(int levelNumber)
    {
        LoadLevel(levelNumber);
        return (Target)LevelData.mode;
    }
    

    // public static bool LoadLevel(int currentLevel)
    // {
    //     // Get the level set path dynamically
    //     LevelSetToggleHandler toggleHandler = GameObject.FindObjectOfType<LevelSetToggleHandler>();
    //     string levelSetPath = toggleHandler != null ? toggleHandler.GetLevelSetPath() : "Levels/";

    //     // Log the path being used
    //     Debug.Log("Loading level from: " + levelSetPath + currentLevel);

    //     // Read the level data
    //     TextAsset mapText = Resources.Load(levelSetPath + currentLevel) as TextAsset;
    //     if (mapText == null)
    //     {
    //         Debug.LogError($"Level file not found: {levelSetPath + currentLevel}");
    //         return false; // Return false if the file is missing
    //     }

    //     Debug.Log("Load level data: " + (mapText.text));
    //     // Process the level data
    //     ProcesDataFromString(mapText.text);
    //     return true;
    // }

    public static bool LoadLevel(int currentLevel)
    {
        // Find the toggle handlers
        TopicSetToggleHandler topicHandler = GameObject.FindObjectOfType<TopicSetToggleHandler>();
        LevelSetToggleHandler levelHandler = GameObject.FindObjectOfType<LevelSetToggleHandler>();

        // Ensure handlers are not null and determine the active path
        string levelSetPath;
        if (topicHandler.GetLevelSetPath() == "TopicLevels/")
        {
            levelSetPath = topicHandler.GetLevelSetPath(); // Use Topic Levels path
        }
        else if (levelHandler.GetLevelSetPath() == "AlternateLevels/")
        {
            levelSetPath = levelHandler.GetLevelSetPath(); // Use Levels path
        }
        else
        {
            Debug.Log("No valid toggle handler or path detected. Defaulting to 'Levels/'.");
            levelSetPath = "Levels/"; // Default to Levels path if no handler is set
        }

        // Log the path being used
        Debug.Log("Loading level from: " + levelSetPath + currentLevel);

        // Load the level data
        TextAsset mapText = Resources.Load(levelSetPath + currentLevel) as TextAsset;
        if (mapText == null)
        {
            Debug.LogError($"Level file not found: {levelSetPath + currentLevel}");
            return false; // Return false if the file is missing
        }

        // Log and process the level data
        Debug.Log("Loaded level data: " + mapText.text);
        ProcesDataFromString(mapText.text); // Corrected method name from `ProcesDataFromString`
        return true; // Return true if the level loads successfully
    }

    public static Target loadLevelByTextAsset(TextAsset tx)
    {
        ProcesDataFromString(tx.text);
        return (Target)LevelData.mode;
    }

    static void ProcesDataFromString(string mapText)
    {
        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        LevelData.colorsDict.Clear();
		//POPSign comment next 2 lines because this values is never used
        //int mapLine = 0;
        //int key = 0;
        foreach (string line in lines)
        {
            if (line.StartsWith("MODE "))
            {
                string modeString = line.Replace("MODE", string.Empty).Trim();
                LevelData.mode = (ModeGame)int.Parse(modeString);
            }
        }
    }
}
