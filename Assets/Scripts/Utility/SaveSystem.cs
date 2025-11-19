using System;
using UnityEngine;

public enum VolumeKind
{
    master,
    sfx,
    ui,
    amb,
    voi,
}

public class SaveSystem : MonoBehaviour
{
    private static string _settingFolderPath = "Setting";
    private static string _settingFilePath = "Config.ini";

    private static string _audioSection = "Audio";
    public static readonly string _audioMasterKey = "MasterVol";
    public static readonly string _audioSfxKey = "SFXVol";
    public static readonly string _audioUIKey = "UIVol";
    public static readonly string _audioAmbienceKey = "AmbienceVol";
    public static readonly string _audioVoiceKey = "VoiceVol";

    private static string _resoulutionSection = "Resoulution";
    private static string _resoulutionKey = "resoulution";
    private static string _fullScreenKey = "fullScreen";

    private static string _goldSection = "Gold";
    private static string _goldKey = "savedGold";

    private static string _shopSection = "Shop";

    public static string ErrorKey { get; } = "Error";

    public static void SaveVolume(float volume, VolumeKind kind)
    {
        switch (kind)
        {
            case VolumeKind.master:
                IniFile.WriteFile(_settingFolderPath, _settingFilePath, _audioSection, _audioMasterKey, volume.ToString());
                break;
            case VolumeKind.sfx:
                IniFile.WriteFile(_settingFolderPath, _settingFilePath, _audioSection, _audioSfxKey, volume.ToString());
                break;
            case VolumeKind.ui:
                IniFile.WriteFile(_settingFolderPath, _settingFilePath, _audioSection, _audioUIKey, volume.ToString());
                break;
            case VolumeKind.amb:
                IniFile.WriteFile(_settingFolderPath, _settingFilePath, _audioSection, _audioAmbienceKey, volume.ToString());
                break;
            case VolumeKind.voi:
                IniFile.WriteFile(_settingFolderPath, _settingFilePath, _audioSection, _audioVoiceKey, volume.ToString());
                break;
            default:
                break;
        }
    }

    public static float LoadVolume(float defaultVolume, VolumeKind kind)
    {
        string key = "";

        switch (kind)
        {
            case VolumeKind.master:
                key = _audioMasterKey;
                break;
            case VolumeKind.sfx:
                key = _audioSfxKey;
                break;
            case VolumeKind.ui:
                key = _audioUIKey;
                break;
            case VolumeKind.amb:
                key = _audioAmbienceKey;
                break;
            case VolumeKind.voi:
                key = _audioVoiceKey;
                break;
        }

        var value = IniFile.ReadFile(_settingFolderPath, _settingFilePath, _audioSection, key);
        if (float.TryParse(value, out var volume))
        {
            return volume;
        }

        return defaultVolume;
    }

    public static string GetVolumeName(VolumeKind kind)
    {
        switch (kind)
        {
            case VolumeKind.master:
                return _audioMasterKey;
            case VolumeKind.sfx:
                return _audioSfxKey;
            case VolumeKind.ui:
                return _audioUIKey;
            case VolumeKind.amb:
                return _audioAmbienceKey;
            case VolumeKind.voi:
                return _audioVoiceKey;
            default:
                return "";
        }
    }

    public static void SaveResolution(int resolutionIndex)
    {
        IniFile.WriteFile(_settingFolderPath, _settingFilePath, _resoulutionSection, _resoulutionKey, resolutionIndex.ToString());
    }

    public static int LoadResolutionIndex(int optimalResolutionIndex)
    {
        var value = IniFile.ReadFile(_settingFolderPath, _settingFilePath, _resoulutionSection, _resoulutionKey);
        if (int.TryParse(value, out var index))
        {
            return index;
        }

        return optimalResolutionIndex;
    }

    public static void SaveFullScreen(int windowModeIndex)
    {
        IniFile.WriteFile(_settingFolderPath, _settingFilePath, _resoulutionSection, _fullScreenKey, windowModeIndex.ToString());
    }

    public static int LoadFullScreen(int initializedIndex)
    {
        var value = IniFile.ReadFile(_settingFolderPath, _settingFilePath, _resoulutionSection, _fullScreenKey);
        if (int.TryParse(value, out var index))
        {
            return index;
        }

        return initializedIndex;
    }

    public static void SaveGold(int currentGold)
    {
        IniFile.WriteFile(_settingFolderPath, _settingFilePath, _goldSection, _goldKey, currentGold.ToString());
    }

    public static int LoadGold()
    {
        var value = IniFile.ReadFile(_settingFolderPath, _settingFilePath, _goldSection, _goldKey);
        if (int.TryParse(value, out var gold))
        {
            return gold;
        }

        return 0;
    }

    public static void SaveBuy(string key)
    {
        int count = LoadBought(key);
        IniFile.WriteFile(_settingFolderPath, _settingFilePath, _shopSection, key, (count + 1).ToString());
    }

    public static int LoadBought(string key)
    {
        if (key == ErrorKey)
        {
            return 0;
        }

        var value = IniFile.ReadFile(_settingFolderPath, _settingFilePath, _shopSection, key);
        if (int.TryParse(value, out var gold))
        {
            return gold;
        }

        return 0;
    }

    //public static void SaveCharacter(Characters character)
    //{
    //    IniFile.WriteFile(_settingFolderPath, _settingFilePath, _characterSection, _characterKey, character.ToString());
    //}
    //
    //public static Characters LoadCharacter()
    //{
    //    var value = IniFile.ReadFile(_settingFolderPath, _settingFilePath, _characterSection, _characterKey);
    //    if(Enum.TryParse(typeof(Characters), value, out var result))
    //    {
    //        return (Characters)result;
    //    }
    //
    //    return Characters.None;
    //}
    //
    //public static void SaveCharacterShapeIndex(int index)
    //{
    //    IniFile.WriteFile(_settingFolderPath, _settingFilePath, _characterSection, _characterShapeKey, index.ToString());
    //}
    //
    //public static int LoadCharacterShapeIndex()
    //{
    //    var value = IniFile.ReadFile(_settingFolderPath, _settingFilePath, _characterSection, _characterShapeKey);
    //    if (int.TryParse(value, out var index))
    //    {
    //        return index;
    //    }
    //    return 0;
    //}
    //
    //public static int LoadPickCount()
    //{
    //    int result = 3;
    //
    //    if (LoadBought(_pickCountKey) > 0)
    //    {
    //        result++;
    //    }
    //
    //    return result;
    //}
    //
    //public static void SaveDifficulty(Difficulty difficulty)
    //{
    //    IniFile.WriteFile(_settingFolderPath, _settingFilePath, _difficultySection, _difficultyKey, difficulty.ToString());
    //}
    //
    //public static Difficulty LoadDifficulty()
    //{
    //    var value = IniFile.ReadFile(_settingFolderPath, _settingFilePath, _difficultySection, _difficultyKey);
    //    if (Enum.TryParse(value, out Difficulty index))
    //    {
    //        return index;
    //    }
    //
    //    return Difficulty.Easy;
    //}
    //
    //public static bool CheckDifficulty()
    //{
    //    var value = IniFile.ReadFile(_settingFolderPath, _settingFilePath, _difficultySection, _difficultyKey);
    //    if (Enum.TryParse(value, out Difficulty index))
    //    {
    //        return true;
    //    }
    //
    //    return false;
    //}
}
