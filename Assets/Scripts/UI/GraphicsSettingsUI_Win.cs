using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI for allowing players to adjust the graphics settings.
/// </summary>
public class GraphicsSettingsUI_Win : MonoBehaviour
{
    public Dropdown graphicsDropdown;

    private List<FullScreenMode> _fullScreenModes = new()
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.MaximizedWindow,
        FullScreenMode.Windowed,
    };

    private void Awake()
    {
        InitGraphicsDropdown();
    }

    public void InitGraphicsDropdown()
    {
        List<string> options = new();
        for (int i = 0; i < _fullScreenModes.Count; i++)
        {
            string option = "";
            switch (_fullScreenModes[i])
            {
                case FullScreenMode.ExclusiveFullScreen:
                    option = "전체화면";
                    break;
                case FullScreenMode.FullScreenWindow:
                    option = "테두리 없는 전체화면";
                    break;
                case FullScreenMode.MaximizedWindow:
                    option = "최대화된 창";
                    break;
                case FullScreenMode.Windowed:
                    option = "창 모드";
                    break;
                default:
                    break;
            }
            options.Add(option);
        }
        
        graphicsDropdown.AddOptions(options);
		graphicsDropdown.onValueChanged.AddListener(index => SetGraphicsWindow(index));

        graphicsDropdown.value = graphicsDropdown.options.Count - 1;
    }

    public void SetGraphicsWindow(int value)
    {
        Screen.fullScreenMode = _fullScreenModes[value];
        SaveSystem.SaveFullScreen(value);
		Debug.Log($"Set graphics window to {_fullScreenModes[value]}");
    }
}
