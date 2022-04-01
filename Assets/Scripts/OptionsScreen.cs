using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: Adam Bassett
/// </summary>

public class OptionsScreen : MonoBehaviour
{
    public Toggle fullscreenToggle, vsyncToggle; // tick boxes in options menu
    public List<ResItem> resolutions = new List<ResItem>(); // list for res values
    private int selectedResolution; // stores selected res

    public Text resolutionLabel;

    // Start is called before the first frame update
    void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;

        if(QualitySettings.vSyncCount == 0) //vsync off here
        {
            vsyncToggle.isOn = false;   
        }
        else
        {
            vsyncToggle.isOn = true; 
        }

        /// Checks to see what the resolution is at the start of the game
        bool foundRes = false;
        for(int i = 0; i < resolutions.Count; i++)
        {
            if (Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                foundRes = true;
                selectedResolution = i;
                UpdateResolutionLabel();
            }
        }

        // prevents system from crashing if users system does not comply with settings
        if (!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;
            newRes.vertical = Screen.height;

            resolutions.Add(newRes);
            selectedResolution = resolutions.Count - 1;
            UpdateResolutionLabel();
        }

    }

    /// <summary>
    /// Covers the resolution decrease button
    /// </summary>
    public void ResLeft()
    {
        selectedResolution--;
        if(selectedResolution < 0) // 0 = 1920 x 1080
        {
            selectedResolution = 0;
        }

        UpdateResolutionLabel();
    }

    public void ResRight()
    {
        selectedResolution++;
        if(selectedResolution > resolutions.Count - 1) // max 2 which is 854 x 480
        {
            selectedResolution = resolutions.Count - 1;
        }

        UpdateResolutionLabel(); // changes the text on the label to be the selected res
    }

    public void UpdateResolutionLabel()
    {
        resolutionLabel.text = resolutions[selectedResolution].horizontal.ToString() + "X" + resolutions[selectedResolution].vertical.ToString(); //access text box to change res values
    }

    public void ApplyGraphics()
    {
        //Screen.fullScreen = fullscreenToggle.isOn;

        if (vsyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1; //on
        }
        else
        {
            QualitySettings.vSyncCount = 0; //off
        }

        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullscreenToggle.isOn); // changes res and fullscreen when apply clicked
    }
}

[System.Serializable]
public class ResItem
{
    public int horizontal, vertical; //fields for res values
}