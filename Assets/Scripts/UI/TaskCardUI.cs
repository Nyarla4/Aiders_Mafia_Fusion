using UnityEngine;
using UnityEngine.UI;
using static CardMatchTask;

public class TaskCardUI : MonoBehaviour
{
    public Image ImageUI;
    public CardKind Kind;    

    public void OpenImage()
    {
        var color = ImageUI.color;
        color.a = 1f;
        ImageUI.color = color;
        //ImageUI.color = Color.white; 추후 이미지 교체시 이걸로 수정
    }

    public void CloseImage()
    {
        var color = ImageUI.color;
        color.a = 0f;
        ImageUI.color = color;
        //ImageUI.color = Color.clear; 위에 설명있음
    }
}
