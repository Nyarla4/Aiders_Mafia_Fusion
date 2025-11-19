using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI for allowing players to adjust the graphics settings.
/// </summary>
public class GraphicsSettingsUI_Res : MonoBehaviour
{
    public Dropdown graphicsDropdown;

    private List<Resolution> _resolutions = new()
    {
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2048, height = 1152 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 2880, height = 1620 },
        new Resolution { width = 3840, height = 2160 },
    };

    private int _optimalResolutionIndex = 0;

    private void Awake()
    {
        InitGraphicsDropdown();
    }

    public void InitGraphicsDropdown()
    {
        List<string> options = new();
        for (int i = 0; i < _resolutions.Count; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            // 가장 적합한 해상도에 별표를 표기합니다.
            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                _optimalResolutionIndex = i;
                option += " *";
            }
            options.Add(option);
        }

        graphicsDropdown.AddOptions(options);
		graphicsDropdown.onValueChanged.AddListener(index => SetGraphicsResoulution(index));

        graphicsDropdown.value = _optimalResolutionIndex;
    }

    public void SetGraphicsResoulution(int value)
    {
        Resolution resolution = _resolutions[value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        SaveSystem.SaveResolution(value);
        Debug.Log($"Set graphics resoulution to {_resolutions[value]}");
    }
}
