using Fusion;
using Fusion.Addons.FSM;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State entered when a gameplay session starts.
/// </summary>
public class PlayStateBehaviour : StateBehaviour
{
    [SerializeField, Tooltip("The amoount of time, in seconds, that an impostor has to wait before being able to kill a crewmate.")]
    float initialKillTimer = 30;

    protected override void OnEnterState()
    {
        // Sets the initial position for all players
        PlayerRegistry.ForEach(
            obj => obj.Controller.cc.SetPosition(GameManager.Instance.mapData.GetSpawnPosition(obj.Index)));

        //각 팀 설정 및 팀의 왕 설정
        // Defines who is the impostier and sets the number of emergency meeting uses, but only if we were previously in the main game.
        if (Machine.PreviousState is PregameStateBehaviour)
        {
            //팀 당 인원수
            int personPerTeam = PlayerRegistry.Count / 2;

            //반은 B, 나머지는 A
            PlayerObject[] objs = PlayerRegistry.GetRandom(personPerTeam);
            foreach (PlayerObject p in objs)
            {
                p.Controller.Team = Teams.B;
                p.Controller.IsKing = false;
            }

            PlayerRegistry.ForEachWhere(
                p => p.Controller.Team != Teams.B,
                p => { p.Controller.Team = Teams.A; p.Controller.IsKing = false; });

            PlayerRegistry.GetRandomWhere(p => p.Controller.Team == Teams.A).Controller.IsKing = true;


            PlayerRegistry.GetRandomWhere(p => p.Controller.Team == Teams.B).Controller.IsKing = true;

            //PlayerRegistry.ForEach(pObj =>
            //{
            //	pObj.Controller.EmergencyMeetingUses = GameManager.Instance.Settings.numEmergencyMeetings;
            //});
        }

        // Defines the tasks for each player
        PlayerRegistry.ForEach(p => p.Controller.DefineTasks(GameManager.Instance.GetRandomTasks(GameManager.Instance.Settings.numTasks)));

        // Sets the kill timer for each suspect
        PlayerRegistry.ForEachWhere(
            p => p,
            p => p.Controller.KillTimer = TickTimer.CreateFromSeconds(GameManager.Instance.Runner, initialKillTimer));
    }

    protected override void OnEnterStateRender()
    {
        GameManager.im.gameUI.CloseOverlay(3);//Machine.PreviousState is VotingResultsStateBehaviour ? 0 : 

        // Renders the players' local tasks.
        if (Machine.PreviousState is PregameStateBehaviour)
        {
            GameManager.Instance.mapData.hull.SetActive(false);
            GameManager.im.gameUI.InitGame();

            GameManager.Instance.taskDisplayList.Clear();
            foreach (TaskStation playerTask in PlayerRegistry.GetPlayer(Runner.LocalPlayer).Controller.tasks)
                GameManager.Instance.taskDisplayList.Add(playerTask);

            TaskBase[] foundTasks = FindObjectsByType<TaskBase>(FindObjectsSortMode.None);

            foreach (TaskBase task in foundTasks)
            {
                GameManager.Instance.taskDisplayAmounts.Add(task, (byte)GameManager.Instance.TaskCount(task));
            }

            GameManager.im.gameUI.InitTaskUI();

            GameManager.vm.SetTalkChannel(VoiceManager.NONE);
        }
    }
}
