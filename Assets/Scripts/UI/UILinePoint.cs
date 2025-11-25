using Radishmouse;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UILinePoint : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Transform _goal;
    [SerializeField] private UILineRenderer _line;
    [SerializeField] private LineConnect _task;

    // 드래그 시작
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        // 올바르지 않은 곳에 드래그 했을 때 돌아갈 위치를 저장해준다
        // 드래그 시작 되었을 때는 드래그 중인 오브젝트의 레이캐스트 타겟을 꺼줘야 오류가 생기지 않는다
        GetComponent<Image>().raycastTarget = false;
    }

    // 드래그 중
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        // 현재 터치되고 있는 좌표를 저장해서 오브젝트가 손가락을 따라갈 수 있도록 오브젝트의 좌표로 넣어준다
        Vector3 currentPos = eventData.position;

        RefreshLine();

        // 이렇게 넣어주지 않으면 오브젝트가 이상한 곳에 위치하길래 조정해주었다
        // 원인을 아시는 분은 댓글로 알려주시면 감사하겠습니다
        //currentPos.z = 90f;
        //currentPos.y -= 160f;
        this.transform.position = currentPos;
    }

    // 드래그 끝
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Vector3 currentPos = eventData.position;

        if (Vector3.Distance(currentPos, _goal.position) > 100f)
        {
            Debug.Log($"from {currentPos} to {_goal.position} is {Vector3.Distance(currentPos, _goal.position)}");
            // 손님 오브젝트에서 음식 오브젝트에 대한 처리를 하고 오브젝트를 제거한다
            // 제거되지 않았다면 올바르지 않은 곳에 드롭된 것이기 때문에 원래 위치로 돌려준다
            this.transform.position = _line.Points[0].position + Vector3.right * 50f;
            // 레이캐스트 타겟도 원래대로 돌려준다
            GetComponent<Image>().raycastTarget = true;
        }
        else
        {
            this.transform.position = _goal.position;
        }
        RefreshLine();

        if (_task != null)
        {
            _task.Check();
        }
    }

    void RefreshLine()
    {
        _line.gameObject.SetActive(false);
        _line.gameObject.SetActive(true);
    }
}
