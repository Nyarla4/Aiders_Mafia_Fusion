using Radishmouse;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Task in which players must match drag-and-drop a set of sliders to match predefined positions.
/// </summary>
public class LineConnect : TaskBase
{
	public override string Name => "Connect";

	public Transform[] StartPoints;
	public Transform[] EndPoints;
	public UILineRenderer [] Lines;
	public float ErrorMargin = 0.02f;

	public override void ResetTask()
	{
		foreach (Transform start in StartPoints.ToList().OrderBy(b => Random.value))
		{
			start.transform.SetAsFirstSibling();
		}

		foreach (Transform end in EndPoints.ToList().OrderBy(b => Random.value))
		{
			end.transform.SetAsFirstSibling();
		}

        foreach (UILineRenderer line in Lines)
        {
			line.Points[1].position = line.Points[0].position + Vector3.right * 50f;
			line.Points[1].GetComponent<Image>().raycastTarget = true;
			line.gameObject.SetActive(false);
			line.gameObject.SetActive(true);
		}
	}

	public void Check()
	{
		for (int i = 0; i < EndPoints.Length; i++)
		{
			if(Vector3.Distance(EndPoints[i].position, Lines[i].Points[1].position) >= 50f)
            {
				return;
			}
		}

		Completed();
	}
}
