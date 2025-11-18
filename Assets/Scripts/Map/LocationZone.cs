using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour that handles the changing the display name of the room the player has entered.
/// </summary>
[RequireComponent(typeof(Collider))]
public class LocationZone : MonoBehaviour
{
	public string displayName;

	public List<string> PlayerLog { get; private set; } = new();

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.parent && other.transform.parent.TryGetComponent(out PlayerObject pObj))
		{
			PlayerLog.Add(pObj.Nickname.ToString());
			if(pObj == PlayerObject.Local)
            {
				GameManager.im.gameUI.SetRoomText(displayName);
				pObj.Controller.Location = this;
            }
		}
	}
}
