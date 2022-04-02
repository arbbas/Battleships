using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// Author: Adam Bassett
/// </summary>

public class OptionsScreen : MonoBehaviour
{
    public Toggle fullscreenToggle, vsyncToggle; // tick boxes in options menu
    public List<ResItem> resolutions = new List<ResItem>(); // list for res values
    private int selectedResolution; // stores selected res

    public Text resolutionLabel;

    public AudioMixer mixer;

    public Text mastLabel, musicLabel, sfxLabel;
    public Slider mastSlider, musicSlider, sfxSlider;

    public GameObject ObjectMusicTrack;
    private AudioSource AudioSource;

    public GameObject ObjectStartSFX;
    private AudioSource audioSource2;

    // Start is called before the first frame update
    void Start()
    {

        ObjectMusicTrack = GameObject.FindWithTag("GameMusic"); // make sure that the music isn't stopped as we navigate around the game
        AudioSource = ObjectMusicTrack.GetComponent<AudioSource>();

        ObjectStartSFX = GameObject.FindWithTag("sfx1");
        audioSource2 = ObjectStartSFX.GetComponent<AudioSource>();

        fullscreenToggle.isOn = Screen.fullScreen; // fullscreen checker

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

        float vol = 0f; // storage of the current volume levels when the game is quit
        mixer.GetFloat("MasterVol", out vol);
        mastSlider.value = vol;
        mixer.GetFloat("MusicVol", out vol);
        musicSlider.value = vol;
        mixer.GetFloat("SFXVol", out vol);
        sfxSlider.value = vol;

        mastLabel.text = Mathf.RoundToInt(mastSlider.value + 80).ToString(); // to change master label

        musicLabel.text = Mathf.RoundToInt(mastSlider.value + 80).ToString(); // to change music label

        sfxLabel.text = Mathf.RoundToInt(mastSlider.value + 80).ToString(); // to change sfx label

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

    /// <summary>
    /// Method to set the master volume using the UI slider
    /// </summary>
    public void SetMasterVolume()
    {
        mastLabel.text = Mathf.RoundToInt(mastSlider.value + 80).ToString();

        mixer.SetFloat("MasterVol", mastSlider.value);

        PlayerPrefs.SetFloat("MasterVol", mastSlider.value);
    }

    /// <summary>
    /// Method to set the music volume
    /// </summary>
    public void SetMusicVolume()
    {
        musicLabel.text = Mathf.RoundToInt(mastSlider.value + 80).ToString();

        mixer.SetFloat("MusicVol", musicSlider.value);

        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
    }

    /// <summary>
    /// Method to set the SFX volume
    /// </summary>
    public void SetSFXVolume()
    {
        sfxLabel.text = Mathf.RoundToInt(mastSlider.value + 80).ToString();

        mixer.SetFloat("SFXVol", sfxSlider.value);

        PlayerPrefs.SetFloat("SFXVol", sfxSlider.value);
    }
}

/// <summary>
/// resolution values for the menus
/// </summary>
[System.Serializable]
public class ResItem
{
    public int horizontal, vertical; //fields for res values
}