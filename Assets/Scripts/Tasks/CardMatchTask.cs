using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 짝 맞추기
/// </summary>
public class CardMatchTask : TaskBase
{
	public enum CardKind
	{
		none,
		SWH_1,
		SWH_2,
		NSW,
		YHZ,
		BYZ_1,
		BYZ_2,
	}

	public override string Name => "Match Cards";
	public List<Button> buttons;
	int lastPushed = 0;
	CardKind lastChecked = CardKind.none;

	Button curButton;
	TaskCardUI cardUi;

	Coroutine checkCo = null;

	//하나 오픈, 다른거 오픈
	//같은거면 interactable false유지 및 이미지 유지
	//다른거면 interactable true로 수정 및 이미지 복구
	//전부 interactable이 false면 컴플리트
	public void PushButton(int number)
	{
        if (checkCo != null)
        {
			return;
        }

		//지금 누른 버튼 interactable false 처리
		curButton = buttons[number - 1];
		curButton.interactable = false;

		cardUi = null;

		if (curButton.TryGetComponent(out cardUi))
		{
			cardUi.OpenImage();
		}

		//0.5초 정도 대기 필요
		checkCo = StartCoroutine(CardCheck(number));
	}

	public override void ResetTask()
	{
		lastPushed = 0;
		foreach (Button button in buttons.ToList().OrderBy(b => Random.value))
		{
			if(button.TryGetComponent<TaskCardUI>(out var ui))
            {
				ui.CloseImage();
            }
			button.interactable = true;
			button.transform.SetAsFirstSibling();
		}
	}

	IEnumerator CardCheck(int number)
    {
		yield return new WaitForSeconds(0.5f);

		//이전 선택이 없는 경우
		if (lastChecked == CardKind.none)
		{
			lastChecked = cardUi.Kind;
			lastPushed = number - 1;
		}
		//이전 선택이 이번 선택과 같은 경우
		else if (lastChecked == cardUi.Kind)
		{
			lastChecked = CardKind.none;
		}
		//이전 선택이 이번 선택과 다른 경우
		else
		{
			buttons[lastPushed].interactable = true;
			buttons[lastPushed].GetComponent<TaskCardUI>().CloseImage();

			curButton.interactable = true;
			cardUi.CloseImage();

			lastChecked = CardKind.none;
		}

		if (buttons.FindIndex(f => f.interactable) < 0)
		{
			Completed();
		}

		checkCo = null;
	}
}
