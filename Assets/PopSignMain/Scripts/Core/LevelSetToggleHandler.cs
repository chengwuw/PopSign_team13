using UnityEngine;

public class LevelSetToggleHandler : MonoBehaviour
{
    public GameObject checkmark; // Checkmark for visual feedback
    private int useAlternateLevels = 0; // Tracks whether alternate levels are enabled

    void Start()
    {
        // Initialize the checkmark state from PlayerPrefs
        useAlternateLevels = PlayerPrefs.GetInt("UseAlternateLevels", 0);
        UpdateCheckmark();
    }

	public void ButtonClick()
	{
	}
    public void ToggleSelection()
    {
        // Toggle the state between 0 and 1
        useAlternateLevels = 1 - useAlternateLevels;

        // Update the checkmark visibility
        UpdateCheckmark();

        // Save the state in PlayerPrefs
        PlayerPrefs.SetInt("UseAlternateLevels", useAlternateLevels);
        PlayerPrefs.Save();

        Debug.Log("Use Alternate Levels: " + (useAlternateLevels == 1 ? "Enabled" : "Disabled"));
        Debug.Log("Toggled to use path: " + GetLevelSetPath());
    }

    private void UpdateCheckmark()
    {
        // Enable or disable the checkmark based on the current state
        if (checkmark != null)
        {
            checkmark.SetActive(useAlternateLevels == 1);
        }
    }

    public string GetLevelSetPath()
    {
        // Return the appropriate level set path
        return useAlternateLevels == 1 ? "AlternateLevels/" : "Levels/";
    }
}