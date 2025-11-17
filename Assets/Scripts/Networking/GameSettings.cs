using Fusion;
using UnityEngine;

public enum Teams
{
	none = -1,
	A = 0,
	B = 1,
}

/// <summary>
/// Network struct that handles various game settings such as number of impostors, if collision between players should occur, walk speed and more.
/// </summary>
[System.Serializable]
public struct GameSettings : INetworkStruct
{
	public const byte TEAMS = 2;
	public const byte MIN_MEETINGS = 0;
	public const byte MAX_MEETINGS = 9;
	public const byte MIN_TASKS = 4;
	public const byte MAX_TASKS = 10;
	public const ushort MIN_DISCUSSION = 5;
	public const ushort MAX_DISCUSSION = 120;
	public const ushort MIN_VOTING = 15;
	public const ushort MAX_VOTING = 300;
	public const byte MIN_WALK_SPEED = 4;
	public const byte MAX_WALK_SPEED = 12;

	//[Range(MIN_IMPOSTERS,MAX_IMPOSTERS)]
	//public byte numImposters;
	//[Range(MIN_TEAMS,MAX_TEAMS)]
	//public byte numTeams;
	[Range(MIN_MEETINGS,MAX_MEETINGS)]
	public byte numEmergencyMeetings;
	[Range(MIN_TASKS,MAX_TASKS)]
	public byte numTasks;
	[Range(MIN_DISCUSSION, MAX_DISCUSSION)]
	public ushort discussionTime;
	[Range(MIN_VOTING, MAX_VOTING)]
	public ushort votingTime;
	[Range(MIN_WALK_SPEED, MAX_WALK_SPEED)]
	public byte walkSpeed;
	public bool playerCollision;

	public static GameSettings Default => new GameSettings
	{
		//numImposters = 1,
		//numTeams = 2,
		numEmergencyMeetings = 1,
		numTasks = 6,
		discussionTime = 15,
		votingTime = 30,
		walkSpeed = 6,
		playerCollision = false
	};
}
