using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YabawiTask : TaskBase
{
	public Button[] Cups;
	[SerializeField] private int _shuffleCount = 3;
	public override string Name => "Yabawi";

	public override void ResetTask()
	{
		StopAllCoroutines();
		Cups[0].transform.localPosition = new Vector3(-115f, 0f, 0f);
		Cups[1].transform.localPosition = new Vector3(0f, 0f, 0f);
		Cups[2].transform.localPosition = new Vector3(115f, 0f, 0f);
		Cups[0].transform.GetChild(0).localPosition = Vector3.zero;
		Cups[1].transform.GetChild(0).localPosition = Vector3.zero;
		Cups[2].transform.GetChild(0).localPosition = Vector3.zero;
		//foreach (Image img in patternStageLights)
		//	img.color = stageLightOff;
		//foreach (Image img in patternSquares)
		//	img.color = patternSquareOff;
		//foreach (Image img in matchStageLights)
		//	img.color = stageLightOff;
		//foreach (Button btn in matchButtons)
		//	btn.interactable = false;
		//pattern.Clear();
		//match.Clear();

		if (gameObject.activeInHierarchy)
			StartCoroutine(Shuffle());
	}

	private void OnEnable()
	{
		StartCoroutine(Shuffle());
	}

	/// <summary>
	/// Shuffle
	///		
	/// </summary>
	IEnumerator Shuffle()
	{
		float imgY = 0f;
		
		//컵 올리고
        while (imgY < 100f)
        {
			imgY += Time.deltaTime;
            for (int i = 0; i < Cups.Length; i++)
            {
				var cup = Cups[i];
				var pos = cup.transform.GetChild(0).localPosition;
				pos.y = imgY;
				cup.transform.GetChild(0).localPosition = pos;
			}
		}
		imgY = 100f;
		for (int i = 0; i < Cups.Length; i++)
		{
			var cup = Cups[i];
			var pos = cup.transform.GetChild(0).localPosition;
			pos.y = imgY;
			cup.transform.GetChild(0).localPosition = pos;
		}

		//0.25초대기
		yield return new WaitForSeconds(0.25f);

		//컵 내리고
		while (imgY > 0)
		{
			imgY -= Time.deltaTime;
			for (int i = 0; i < Cups.Length; i++)
			{
				var cup = Cups[i];
				var pos = cup.transform.GetChild(0).localPosition;
				pos.y = imgY;
				cup.transform.GetChild(0).localPosition = pos;
			}
		}
		imgY = 0f;
		for (int i = 0; i < Cups.Length; i++)
		{
			var cup = Cups[i];
			var pos = cup.transform.GetChild(0).localPosition;
			pos.y = imgY;
			cup.transform.GetChild(0).localPosition = pos;
		}

		//_shuffleCount 회 셔플
		for (int i = 0; i < _shuffleCount; i++)
        {
			//섞을 대상
			int notTarget = Random.Range(0, 3);
			int target1Idx = -1;
			int target2Idx = -1;
            switch (notTarget)
            {
				case 0:
					target1Idx = 1;
					target2Idx = 2;
                    break;
				case 1:
					target1Idx = 0;
					target2Idx = 2;
                    break;
				case 2:
					target1Idx = 0;
					target2Idx = 1;
                    break;
            }
			Button target1 = Cups[target1Idx];
			float target1X = target1.transform.localPosition.x;
			Button target2 = Cups[target2Idx];
			float target2X = target2.transform.localPosition.x;

			float timer = 0;
            while (timer < 1f)
            {
				timer += Time.deltaTime;
				Vector3 tar1Pos = target1.transform.localPosition;
				tar1Pos.x = Mathf.Lerp(target1X, target2X, timer / 1f);
				target1.transform.localPosition = tar1Pos;
				Vector3 tar2Pos = target2.transform.localPosition;
				tar2Pos.x = Mathf.Lerp(target2X, target1X, timer / 1f);
				target2.transform.localPosition = tar2Pos;
			}
        }
		//match.Clear();
		//
		//foreach (Image img in matchStageLights)
		//	img.color = stageLightOff;
		//
		//foreach (Button btn in matchButtons)
		//	btn.interactable = false;
		//
		//yield return new WaitForSeconds(0.75f);
		//
		//pattern.Add((byte)Random.Range(0, 9));
		//patternStageLights[pattern.Count - 1].color = stageLightOn;
		//
		//foreach (byte b in pattern)
		//{
		//	patternSquares[b].color = patternSquareOn;
		//	AudioManager.Play("simonUI", AudioManager.MixerTarget.UI, null, pitches[b]);
		//	yield return new WaitForSeconds(0.4f);
		//	patternSquares[b].color = patternSquareOff;
		//	yield return new WaitForSeconds(0.1f);
		//}
		//
		//foreach (Button btn in matchButtons)
		//	btn.interactable = true;
	}

	public void PressMatch(int index)
	{
		if(index == 0)
        {
			Completed();
        }
        else
        {
			ResetTask();
		}

		//match.Add(index);
		//AudioManager.Play("simonUI", AudioManager.MixerTarget.UI, null, pitches[index]);
		//matchStageLights[match.Count - 1].color = stageLightOn;
		//
		//if (index != pattern[match.Count - 1])
		//	StartCoroutine(WrongInput());
		//else if (match.Count == pattern.Count)
		//{
		//	if (pattern.Count == 5)
		//		StartCoroutine(DelayCompleted());
		//	else
		//		StartCoroutine(ShowPattern());
		//}
	}

	IEnumerator WrongInput()
	{
		//foreach (Button btn in matchButtons)
		//	btn.interactable = false;
		//
		//foreach (Image img in patternSquares)
		//	img.color = patternSquareWrong;

		yield return new WaitForSeconds(1);

		ResetTask();
	}

	IEnumerator DelayCompleted()
	{
		//foreach (Image img in patternSquares)
		//	img.color = patternSquareOn;
		yield return new WaitForSeconds(0.5f);
		Completed();
	}
}
