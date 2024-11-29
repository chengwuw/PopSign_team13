using UnityEngine;

public class IndianToggleHandler : MonoBehaviour
{
    public GameObject checkmark; // Checkmark for visual feedback
    private int useIndianLevels = 0; // Tracks whether Indian levels are enabled

    void Start()
    {
        // Initialize the checkmark state from PlayerPrefs
        useIndianLevels = PlayerPrefs.GetInt("UseIndianLevels", 0);
        UpdateCheckmark();
    }

	public void ButtonClick()
	{
	}
    public void ToggleSelection()
    {
        // Toggle the state between 0 and 1
        useIndianLevels = 1 - useIndianLevels;

        // Update the checkmark visibility
        UpdateCheckmark();

        // Save the state in PlayerPrefs
        PlayerPrefs.SetInt("UseIndianLevels", useIndianLevels);
        PlayerPrefs.Save();

        Debug.Log("Use Indian Levels: " + (useIndianLevels == 1 ? "Enabled" : "Disabled"));
        // Debug.Log("Toggled to use path: " + GetLevelSetPath());
    }

    private void UpdateCheckmark()
    {
        // Enable or disable the checkmark based on the current state
        if (checkmark != null)
        {
            checkmark.SetActive(useIndianLevels == 1);
        }
    }

    // public string GetLevelSetPath()
    // {
    //     // Return the appropriate level set path
    //     return useIndianLevels == 1 ? "IndianLevels/" : "Levels/";
    // }
}