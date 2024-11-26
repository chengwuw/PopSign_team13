using UnityEngine;

public class TopicSetToggleHandler : MonoBehaviour
{
    public GameObject checkmark; // Checkmark for visual feedback
    private int useTopicLevels = 0; // Tracks whether topic levels are enabled

    void Start()
    {
        // Initialize the checkmark state from PlayerPrefs
        useTopicLevels = PlayerPrefs.GetInt("UseTopicLevels", 0);
        UpdateCheckmark();
    }

	public void ButtonClick()
	{
	}
    public void ToggleSelection()
    {
        // Toggle the state between 0 and 1
        useTopicLevels = 1 - useTopicLevels;

        // Update the checkmark visibility
        UpdateCheckmark();

        // Save the state in PlayerPrefs
        PlayerPrefs.SetInt("UseTopicLevels", useTopicLevels);
        PlayerPrefs.Save();

        Debug.Log("Use Topic Levels: " + (useTopicLevels == 1 ? "Enabled" : "Disabled"));
        Debug.Log("Toggled to use path: " + GetLevelSetPath());
    }

    private void UpdateCheckmark()
    {
        // Enable or disable the checkmark based on the current state
        if (checkmark != null)
        {
            checkmark.SetActive(useTopicLevels == 1);
        }
    }

    public string GetLevelSetPath()
    {
        // Return the appropriate level set path
        return useTopicLevels == 1 ? "TopicLevels/" : "Levels/";
    }
}