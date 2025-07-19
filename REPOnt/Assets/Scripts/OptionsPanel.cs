using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    private Resolution[] resolutions;

    void Start() { Initialize(); }
    private void Initialize()
    {
        resolutions = Screen.resolutions;
        masterSlider.value = PlayerPrefs.GetFloat("AudioMaster", 1f);

        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resText = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(resText);
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = isFullscreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        qualityDropdown.ClearOptions();
        List<string> qualityOptions = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(qualityOptions);
        int savedQualityIndex = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qualityDropdown.value = savedQualityIndex;
        qualityDropdown.RefreshShownValue();
        qualityDropdown.onValueChanged.AddListener(SetQuality);

        audioMixer.SetFloat("Volume", masterSlider.value);

        SetResolution(savedResolutionIndex);
        SetFullscreen(isFullscreen);
        QualitySettings.SetQualityLevel(savedQualityIndex);
        QualitySettings.vSyncCount = 1;
    }

    private void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("AudioMaster", masterSlider.value);
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        PlayerPrefs.Save();
    }
}
