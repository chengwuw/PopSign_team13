using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectLevels : MonoBehaviour
{
    int latestFile;
    public GameObject levelPrefab;
    public Vector2 startPosition;
    public Vector2 offset;
    public int countInRow = 4;
    public int countInColumn = 4;
    public Button backButton;
    public Button nextButton;
    public Text pageText;
    int firstShownLevelInGrid;
    int currPage = 1;
    int totalPage;
    int itemsInGrid = 16;

    private string levelSetPath; // Path to the selected level set

    // Use this for initialization
    void Start()
    {
        pageText = GameObject.Find("Page Text").GetComponent<Text>();

        // Dynamically determine the level set to use based on the toggle state
        DetermineLevelSetPath();

        GenerateGrid();
    }

    void DetermineLevelSetPath()
    {
        // Find both toggle handlers
        TopicSetToggleHandler topicHandler = FindObjectOfType<TopicSetToggleHandler>();
        LevelSetToggleHandler levelHandler = FindObjectOfType<LevelSetToggleHandler>();

        // Determine the active level set path
        if (topicHandler.GetLevelSetPath() == "TopicLevels/")
        {
            levelSetPath = topicHandler.GetLevelSetPath(); // Use Topic Levels path
        }
        else if (levelHandler.GetLevelSetPath() == "AlternateLevels/")
        {
            levelSetPath = levelHandler.GetLevelSetPath(); // Use Alternate Levels path
        }
        else
        {
            Debug.Log("No valid toggle handler detected. Defaulting to 'Levels/'.");
            levelSetPath = "Levels/"; // Default path
        }


        // LevelSetToggleHandler toggleHandler = FindObjectOfType<LevelSetToggleHandler>();
        // levelSetPath = toggleHandler.GetLevelSetPath();

        // Debug.Log("Using level set path: " + levelSetPath);

    }


    void GenerateGrid(int genfrom = 0)
    {
        int l = 0;
        int posCounter = 0;
        ClearLevels();
        firstShownLevelInGrid = genfrom;
        latestFile = GetLastLevel();
        totalPage = (latestFile / itemsInGrid);
        if (latestFile % itemsInGrid != 0)
        {
            totalPage += 1;
        }
        SetPage(currPage);
        for (l = genfrom; l < latestFile; l++)
        {
            GameObject level = Instantiate(levelPrefab) as GameObject;
            level.GetComponent<Level>().number = l + 1;
            level.transform.SetParent(transform);
            level.transform.localPosition = startPosition + Vector2.right * (posCounter % countInRow) * offset.x + Vector2.down * (posCounter / countInColumn) * offset.y;
            level.transform.localScale = Vector2.one;
            if (posCounter + 1 >= countInRow * countInColumn) break;
            posCounter++;
        }
        // if (genfrom == 0) backButton.gameObject.SetActive(false);
        // else if (genfrom > 0) backButton.gameObject.SetActive(true);
        // if (l + 1 >= latestFile) nextButton.gameObject.SetActive(false);
        // else nextButton.gameObject.SetActive(true);

        backButton.gameObject.SetActive(genfrom > 0);
        nextButton.gameObject.SetActive(l + 1 < latestFile);
    }

    void ClearLevels()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
    }

    public void Next()
    {
        GenerateGrid(firstShownLevelInGrid + countInRow * countInColumn);
        SetPage(currPage + 1);
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
    }

    public void Back()
    {
        GenerateGrid(firstShownLevelInGrid - countInRow * countInColumn);
        SetPage(currPage - 1);
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
    }

    public void SetPage(int page)
    {
        currPage = page;
        pageText.text = string.Format("PAGE {0}/{1}", currPage, totalPage);
    }

    public int GetLastLevel()
    {
        TextAsset mapText = null;

        // Use the selected level set directory
        for (int i = 1; i < 50000; i++)
        {
            mapText = Resources.Load(levelSetPath + i) as TextAsset;
            if (mapText == null)
            {
                PlayerPrefs.SetInt("NumLevels", i - 1);
                PlayerPrefs.Save();
                return i - 1;
            }
        }
        return 0;
    }
}