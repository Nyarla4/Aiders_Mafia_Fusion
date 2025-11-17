using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Adds audio effect triggers to buttons
/// </summary>
public class UIAudio : MonoBehaviour, IPointerEnterHandler
{
    private Button btn;
	private void Awake()
	{
		if (TryGetComponent(out btn))
		{
			btn.onClick.AddListener(() => AudioManager.Play("SFX_Click", AudioManager.MixerTarget.UI));
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!btn || btn.interactable)
			AudioManager.Play("SFX_Hover", AudioManager.MixerTarget.UI);
	}
}
