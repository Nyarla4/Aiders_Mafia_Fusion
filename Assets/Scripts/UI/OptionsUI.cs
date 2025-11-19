using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for managing the options UI.
/// </summary>
public class OptionsUI : MonoBehaviour
{
	public Slider masterVol;
	public Slider sfxVol;
	public Slider uiVol;
	public Slider ambienceVol;
	public Slider voiceVol;

	public TMP_InputField nicknameInput;
	public Dropdown regionDropdown;

	private void OnEnable()
	{
		masterVol.SetValueWithoutNotify(AudioManager.GetFloatNormalized(SaveSystem.GetVolumeName(VolumeKind.master)));
		sfxVol.SetValueWithoutNotify(AudioManager.GetFloatNormalized(SaveSystem.GetVolumeName(VolumeKind.sfx)));
		uiVol.SetValueWithoutNotify(AudioManager.GetFloatNormalized(SaveSystem.GetVolumeName(VolumeKind.ui)));
		ambienceVol.SetValueWithoutNotify(AudioManager.GetFloatNormalized(SaveSystem.GetVolumeName(VolumeKind.amb)));
		voiceVol.SetValueWithoutNotify(AudioManager.GetFloatNormalized(SaveSystem.GetVolumeName(VolumeKind.voi)));

		nicknameInput.interactable = regionDropdown.interactable = GameManager.Instance.Runner == null;
	}

	public void SetVolumeMaster(float value)
	{
		AudioManager.SetVolumeMaster(value);
	}

	public void SetVolumeSFX(float value)
	{
		AudioManager.SetVolumeSFX(value);
	}

	public void SetVolumeUI(float value)
	{
		AudioManager.SetVolumeUI(value);
	}

	public void SetVolumeAmbience(float value)
	{
		AudioManager.SetVolumeAmbience(value);
	}

	public void SetVolumeVoice(float value)
	{
		AudioManager.SetVolumeVoice(value);
	}
}
