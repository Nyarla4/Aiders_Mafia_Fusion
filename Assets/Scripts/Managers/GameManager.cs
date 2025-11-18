using Fusion;
using Fusion.Sockets;
using Helpers.Bits;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager that handles the main gameplay loop of Impostor.
/// </summary>
public class GameManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static GameManager Instance { get; private set; }
	public static GameState State { get; private set; }
    public static ResourcesManager rm { get; private set; }
	public static InterfaceManager im { get; private set; }
	public static VoiceManager vm { get; private set; }

	public FusionBootstrap starter;

	public UIScreen pauseScreen;
	public UIScreen optionsScreen;

	public MapData preGameMapData;

	public MapData mapData;

	[Networked]
	public PlayerObject MeetingCaller { get; set; }

	[Networked]
	public PlayerObject MeetingContext { get; set; }

	[Networked]
	public PlayerObject VoteResult { get; set; }

	[Networked, OnChangedRender(nameof(GameSettingsChanged))]
	public GameSettings Settings { get; set; } = GameSettings.Default;

	//[Networked, OnChangedRender(nameof(TasksCompletedChanged))]
	//public byte TasksCompleted { get; set; }

	public List<TaskStation> taskDisplayList;
	public readonly Dictionary<TaskBase, byte> taskDisplayAmounts = new Dictionary<TaskBase, byte>();

	[Networked, Tooltip("Is the meeting screen currently active.")]
	public NetworkBool MeetingScreenActive { get; set; }

	[Networked, Tooltip("Is the vocting screen currently active.")]
	public NetworkBool VotingScreenActive { get; set; }

	private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            rm = GetComponent<ResourcesManager>();
            im = GetComponent<InterfaceManager>();
			vm = GetComponent<VoiceManager>();
			State = GetComponent<GameState>();
        }
        else
        {
			Destroy(gameObject);
        }
    }

	public override void Spawned()
	{
		base.Spawned();
		vm.Init(
			Runner.GetComponent<Photon.Voice.Unity.VoiceConnection>(),
			Runner.GetComponentInChildren<Photon.Voice.Unity.Recorder>()
		);

		Runner.AddCallbacks(this);

		UpdateGameSettingHUD(Settings);
	}

	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		base.Despawned(runner, hasState);
		runner.RemoveCallbacks(this);
		starter.Shutdown();
	}

	public override void Render()
	{
		// Updates the displays ping ever 100 ticks.
		if (Runner.Tick % 100 == 0)
			im.gameUI.pingText.text = $"Ping: {1000 * Runner.GetPlayerRtt(Runner.LocalPlayer):N0}ms";
	}

	public void Server_StartGame()
	{
		if (State.ActiveState is not PregameStateBehaviour) return;

		if (PlayerRegistry.Count < 4)
		{
			Debug.LogWarning($"It's recommended to play with at least 4 people!");
		}

		State.Server_SetState<PlayStateBehaviour>();
	}

	/// <summary>
	/// Method called when a player has murdered, not ejected from a vote.
	/// </summary>
	public static void OnPlayerKilled()
	{
		PlayerObject deadKing = PlayerRegistry.GetRandomWhere(p => p.Controller.IsKing && p.Controller.IsDead);

		if (deadKing != null)
		{
			State.winState.teamAWin = deadKing.Controller.Team ==Teams.B;
			State.Server_DelaySetState<WinStateBehaviour>(1);
		}
	}

	//public void CallMeeting(PlayerRef source, NetworkObject context, Tick tick)
	//{
	//	// Only the state authority can call this.
	//	if (!HasStateAuthority)
	//		return;
	//
	//	PlayerObject caller = PlayerRegistry.GetPlayer(source);
	//
	//	if (context == null)
	//	{
	//		if (caller.Controller.EmergencyMeetingUses > 0)
	//		{
	//			caller.Controller.EmergencyMeetingUses--;
	//			MeetingContext = null;
	//			MeetingCaller = caller;
	//			State.Server_SetState<MeetingStateBehaviour>();
	//
	//			im.gameUI.meetingUI.Server_SetTimer(tick);
	//		}
	//		else
	//		{
	//			Debug.Log($"{caller.Nickname} is out of emergency meeting calls");
	//		}
	//	}
	//	else if (context.TryGetBehaviour(out DeadPlayer body))
	//	{
	//		MeetingContext = PlayerRegistry.GetPlayer(body.Ref);
	//		MeetingCaller = caller;
	//	
	//		State.Server_SetState<MeetingStateBehaviour>();
	//	
	//		im.gameUI.meetingUI.Server_SetTimer(tick);
	//		
	//		// Only the state authority can despawn objects.
	//		if (HasStateAuthority)
	//			Runner.Despawn(body.Object);
	//	}
	//}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	void Rpc_CompleteTask(PlayerRef player, TaskStation completedTask)
	{
		PlayerMovement curPlayer = PlayerRegistry.GetPlayer(player).Controller;

		// Removes the task from the complete players list
		curPlayer.tasks.Remove(completedTask);

		List<TaskStation> taskList = new List<TaskStation>(FindObjectsByType<TaskStation>(FindObjectsSortMode.None));
		List<TaskStation> targetTasks = taskList.FindAll(f => f != completedTask && !curPlayer.tasks.Contains(f));
		int r = UnityEngine.Random.Range(0, targetTasks.Count);
		TaskStation targetTask = targetTasks[r];
		curPlayer.tasks.Add(targetTask);

		//TasksCompleted++;

		//if (TasksCompleted == Settings.numTasks * (PlayerRegistry.Count - Settings.numImposters))
		//{
		//	Debug.Log("All Tasks Completed - Crew Wins");
		//	State.winState.teamAWin = true;
		//	State.Server_SetState<WinStateBehaviour>();
		//}
		//else
		{
			//Debug.Log($"{TasksCompleted} tasks completed");
		}
	}

	public void CompleteTask()
	{
		TaskStation task = PlayerMovement.Local.activeInteractable as TaskStation;
		Debug.LogWarning(task);
		if (taskDisplayList.Remove(task))
		{
			im.gameUI.UpdateTaskUI();
			Rpc_CompleteTask(Runner.LocalPlayer, task);

			SetInfo();
		}
	}

	public void SetInfo()
    {
		var player = Runner.LocalPlayer;
		PlayerMovement curPlayer = PlayerRegistry.GetPlayer(player).Controller;

		Infos info = new();

		//중복 정보 체크
		bool duplicated = true;

		while (duplicated)
		{
			switch (UnityEngine.Random.Range(0, 3))
			{
				case 0:
					info = GetNormalInfos(player);
					break;
				case 1:
					info = GetAdvancedInfo(player);
					break;
				case 2:
					info = GetConfirmedInfo(player);
					break;
				default:
					break;
			}
			duplicated = curPlayer.GotInfos.IndexOf(info) > -1;
		}

		curPlayer.GotInfos.Add(info);
		im.gameUI.SetInfoUI(info);
	}

	public int TaskCount(TaskBase taskBase)
	{
		int count = 0;
		for (int i = 0; i < taskDisplayList.Count; i++)
		{
			if (taskDisplayList[i].taskUI.task == taskBase)
				count++;
		}
		return count;
	}

	public List<string> TaskNames()
	{
		List<string> taskNameList = new List<string>();
		for (int i = 0, len = taskDisplayList.Count; i < len; i++)
        {
			taskNameList.Add(taskDisplayList[i].taskUI.task.Name);
        }

		return taskNameList;
	}

	public static void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	void UpdateGameSettingHUD(GameSettings settings)
    {
		//im.imposterCountDisplay.text = $"{settings.numImposters}";
		im.tasksCountDisplay.text = $"{settings.numTasks}";
		im.emergencyMeetingsDisplay.text = $"{settings.numEmergencyMeetings}";
		im.discussionTimeDisplay.text = $"{settings.discussionTime}s";
		im.votingTimeDisplay.text = $"{settings.votingTime}s";
		im.walkSpeedDisplay.text = $"{settings.walkSpeed}";
		im.playerCollisionDisplay.text = settings.playerCollision ? "Yes" : "No";
	}

	void GameSettingsChanged()
	{
		GameSettings settings = Settings;

		UpdateGameSettingHUD(settings);

		if (Instance.Runner.IsServer)
		{
			int playerLayer = LayerMask.NameToLayer("Player");
			PlayerRegistry.ForEach(p =>
			{
				p.Controller.cc.SetCollisionLayerMask(p.Controller.cc.Settings.CollisionLayerMask.value.OverrideBit(playerLayer, settings.playerCollision));
				p.Controller.Speed = settings.walkSpeed;
			});
		}
	}

	void TasksCompletedChanged()
	{
		//GameManager self = this;
		//float pct = self.TasksCompleted /
		//	(float)(self.Settings.numTasks * (PlayerRegistry.Count - Instance.Settings.numImposters));
		//
		//im.gameUI.totalTaskBarFill.fillAmount = pct;
	}

	void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
	{
		UIScreen.CloseAll();
	}

	void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, Fusion.Sockets.NetAddress remoteAddress, Fusion.Sockets.NetConnectFailedReason reason) { }
	void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
	void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
	void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
	void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
	void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
	void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
	void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
	void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
	void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
	void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
	void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
	void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
	void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
	void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
	void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
	void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

	public enum InfoTypes
    {
		TeamCheck,
		KingCheck,
		KingPlace,
		KillTarget,
    }

	public class Infos
    {
		public InfoTypes Type;
		public PlayerObject Player1 = null;//팀체크에서 누가, 왕체크에서 누가, 킬타겟에서 죽이는 사람
		public PlayerObject Player2 = null;//킬타겟에서 죽여야할 사람
		public Teams Team;//왕위치에서 어떤팀의 왕인지
		public LocationZone Location;//왕위치에서 왕의 위치
		public string InfoValue()
        {
			string result = "";
            switch (Type)
            {
                case InfoTypes.TeamCheck:
					result = $"{Player1.Nickname} is {Player1.Controller.Team.ToString()}";
                    break;
                case InfoTypes.KingCheck:
					result = $"{Player1.Nickname} is {(Player1.Controller.IsKing?"King":"not King")}";
					break;
                case InfoTypes.KingPlace:
					result = $"King of {Team.ToString()} is at {Location.displayName}";
					break;
                case InfoTypes.KillTarget:
					result = $"{Player1.Nickname} have to kill {Player1.Nickname}";
					break;
            }
			return result;
		}
    }

	public Infos GetNormalInfos(PlayerRef userRef)
    {
		Infos info = new();
		PlayerObject user = PlayerRegistry.GetPlayer(userRef);
		List<PlayerObject> others = PlayerRegistry.OtherPlayers(userRef);
		PlayerObject target = others[UnityEngine.Random.Range(0, others.Count)];

		switch (UnityEngine.Random.Range(0, 3))
		{
			case 0:
				info.Type = InfoTypes.TeamCheck;
				info.Player1 = target;
				break;
			case 1:
				info.Type = InfoTypes.KingCheck;
				info.Player1 = target;
				break;
			case 2:
				Teams team = UnityEngine.Random.Range(0, 2) == 0 ? Teams.A : Teams.B;
				info.Type = InfoTypes.KingPlace;
				info.Team = team;
				info.Location = PlayerRegistry.GetRandomWhere(p => p.Controller.Team == team && p.Controller.IsKing).Controller.Location;
				break;
			default:
				break;
		}

		return info;

	}

	public Infos GetAdvancedInfo(PlayerRef userRef)
    {
		Infos info = new();
		PlayerObject user = PlayerRegistry.GetPlayer(userRef);

		PlayerObject killer = PlayerRegistry.GetRandom();
		PlayerObject killed = PlayerRegistry.GetRandomWhere(p => p.Controller.IsKing && p.Controller.Team != killer.Controller.Team);

		info.Type = InfoTypes.KillTarget;
		info.Player1 = killer;
		info.Player2 = killed;

		return info;

	}
	
	public Infos GetConfirmedInfo(PlayerRef userRef)
    {
		Infos info = new();
		PlayerObject user = PlayerRegistry.GetPlayer(userRef);

		info.Type = InfoTypes.TeamCheck;
		info.Player1 = user;

		return info;
	}
}
