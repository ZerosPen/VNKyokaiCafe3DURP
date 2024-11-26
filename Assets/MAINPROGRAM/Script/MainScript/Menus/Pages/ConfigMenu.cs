using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigMenu : MenuPage
{
    public UI_Items ui;

    // Start is called before the first frame update
    void Start()
    {
        SetAvailableResolutions();

        // Set up button listeners
        ui.fullscreen.onClick.AddListener(SetFullscreen);
        ui.windowed.onClick.AddListener(SetWindowed);
        ui.resolution.onValueChanged.AddListener(SetResolution);
    }

    private void SetAvailableResolutions()
    {
        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();

        for (int i = resolutions.Length - 1; i >= 0; i--)
        {
            options.Add($"{resolutions[i].width} X {resolutions[i].height}");
        }

        ui.resolution.ClearOptions();
        ui.resolution.AddOptions(options);

        // Set the current resolution in the dropdown
        int currentResolutionIndex = options.FindIndex(option => option == $"{Screen.width} X {Screen.height}");
        if (currentResolutionIndex >= 0)
        {
            ui.resolution.value = currentResolutionIndex;
            ui.resolution.RefreshShownValue();
        }
    }

    private void SetFullscreen()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow; // or FullScreenMode.ExclusiveFullScreen
        Screen.fullScreen = true; // Set fullscreen to true
    }

    private void SetWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.fullScreen = false; // Set fullscreen to false
    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution[] resolutions = Screen.resolutions;
        Resolution selectedResolution = resolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
    }

    [System.Serializable]
    public class UI_Items
    {
        [Header("General")]
        public Button fullscreen;
        public Button windowed;
        public TMP_Dropdown resolution;
        public Button muteSound;
        public Slider architectSpeed, autoReaderSpeed;

        [Header("Audio")]
        public Slider musicVolume;
        public Slider sfxVolume;
        public Slider voiceVolume;
    }
}