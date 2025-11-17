using TMPro;
using UnityEngine;
using static GameManager;

/// <summary>
/// Class that displays task item UI and when it's been completed.
/// </summary>
public class TaskItemUI : MonoBehaviour
{
    public TMP_Text taskText;
	public Infos Info { get; private set; }

    public void SetTask(Infos info)
    {
		Info = info;
		taskText.text = Info.InfoValue();
	}

	//public void SetCount(int numFinished, int numTotal)
	//{
	//	string count = (numTotal > 1) ? $" ({numFinished}/{numTotal})" : "";
	//	taskText.text = Info.Name + count;
	//	//if (PlayerMovement.Local.IsSuspect)
	//	//	taskText.color = new Color32(230, 140, 150, 255);
	//	//else if (numFinished == numTotal)
	//		taskText.color = Color.green;
	//}
}
