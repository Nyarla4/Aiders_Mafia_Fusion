using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;

/// <summary>
/// State that handles displaying the win screen.
/// </summary>
public class WinStateBehaviour : StateBehaviour
{
	[Networked, Tooltip("If true, the crew won; otherwise, the impostor(s) did.")]
	public NetworkBool teamAWin { get; set; }

	float stateLimit = 3f;

	protected override void OnFixedUpdate()
	{
		if (Machine.StateTime < stateLimit)
			return;

		Machine.ForceActivateState(Machine.GetState<PregameStateBehaviour>());
	}

	protected override void OnEnterStateRender()
	{
		if (teamAWin)
		//	GameManager.im.gameUI.CrewmateWinOverlay();
			GameManager.im.gameUI.WinOverlay(Teams.A);
		else
		//	GameManager.im.gameUI.ImpostorWinOverlay();
			GameManager.im.gameUI.WinOverlay(Teams.B);

		GameManager.vm.SetTalkChannel(VoiceManager.GLOBAL);
	}

	protected override void OnExitStateRender()
	{
		GameManager.im.gameUI.CloseOverlay();
	}
}
