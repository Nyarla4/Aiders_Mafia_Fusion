using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YabawiTask : TaskBase
{
    public Button[] Cups;
    [SerializeField] private int _shuffleCount = 3;
    private float _openCloseSpeed = 20f;
    private float _moveSpeed = 5f;
    public override string Name => "Yabawi";

    private Coroutine _shuffleRoutine = null;
    public override void ResetTask()
    {
        StopAllCoroutines();
        _shuffleRoutine = null;

        Cups[0].transform.localPosition = new Vector3(-115f, 0f, 0f);
        Cups[1].transform.localPosition = new Vector3(0f, 0f, 0f);
        Cups[2].transform.localPosition = new Vector3(115f, 0f, 0f);
        Cups[0].transform.GetChild(0).localPosition = Vector3.zero;
        Cups[1].transform.GetChild(0).localPosition = Vector3.zero;
        Cups[2].transform.GetChild(0).localPosition = Vector3.zero;
        
        if (gameObject.activeInHierarchy)
        {
            _shuffleRoutine = StartCoroutine(Shuffle());
        }
    }

    private void OnEnable()
    {
        _shuffleRoutine = StartCoroutine(Shuffle());
    }

    /// <summary>
    /// Shuffle
    /// </summary>
    IEnumerator Shuffle()
    {
        float deltaSpeed = Time.deltaTime * _moveSpeed;

        yield return StartCoroutine(OpenCups());

        //0.25초대기
        yield return new WaitForSeconds(0.25f);

        yield return StartCoroutine(CloseCups());

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
                timer += deltaSpeed;
                Vector3 tar1Pos = target1.transform.localPosition;
                tar1Pos.x = Mathf.Lerp(target1X, target2X, timer / 1f);
                target1.transform.localPosition = tar1Pos;
                Vector3 tar2Pos = target2.transform.localPosition;
                tar2Pos.x = Mathf.Lerp(target2X, target1X, timer / 1f);
                target2.transform.localPosition = tar2Pos;
                yield return null;
            }
        }

        _shuffleRoutine = null;
    }

    private IEnumerator OpenCups()
    {
        float imgY = 0f;
        float deltaSpeed = Time.deltaTime * _openCloseSpeed;

        //컵 올리고
        while (imgY < 100f)
        {
            imgY += deltaSpeed;
            for (int i = 0; i < Cups.Length; i++)
            {
                var cup = Cups[i];
                var pos = cup.transform.GetChild(0).localPosition;
                pos.y = imgY;
                cup.transform.GetChild(0).localPosition = pos;
            }
            yield return null;
        }
        imgY = 100f;
        for (int i = 0; i < Cups.Length; i++)
        {
            var cup = Cups[i];
            var pos = cup.transform.GetChild(0).localPosition;
            pos.y = imgY;
            cup.transform.GetChild(0).localPosition = pos;
        }
    }
    
    private IEnumerator CloseCups()
    {
        float imgY = 0f;
        float deltaSpeed = Time.deltaTime * _openCloseSpeed;

        //컵 내리고
        while (imgY > 0)
        {
            imgY -= deltaSpeed;
            for (int i = 0; i < Cups.Length; i++)
            {
                var cup = Cups[i];
                var pos = cup.transform.GetChild(0).localPosition;
                pos.y = imgY;
                cup.transform.GetChild(0).localPosition = pos;
            }
            yield return null;
        }
        imgY = 0f;
        for (int i = 0; i < Cups.Length; i++)
        {
            var cup = Cups[i];
            var pos = cup.transform.GetChild(0).localPosition;
            pos.y = imgY;
            cup.transform.GetChild(0).localPosition = pos;
        }
    }

    private IEnumerator CheckCups(int index)
    {
        float imgY = 0f;
        float deltaSpeed = Time.deltaTime * _openCloseSpeed;

        //컵 올리고
        while (imgY < 100f)
        {
            imgY += deltaSpeed;
            for (int i = 0; i < Cups.Length; i++)
            {
                var cup = Cups[i];
                var pos = cup.transform.GetChild(0).localPosition;
                pos.y = imgY;
                cup.transform.GetChild(0).localPosition = pos;
            }
            yield return null;
        }
        imgY = 100f;
        for (int i = 0; i < Cups.Length; i++)
        {
            var cup = Cups[i];
            var pos = cup.transform.GetChild(0).localPosition;
            pos.y = imgY;
            cup.transform.GetChild(0).localPosition = pos;
        }

        if (index == 0)
        {
            Completed();
        }
        else
        {
            ResetTask();
        }
    }

    public void PressMatch(int index)
    {
        if (_shuffleRoutine != null)
        {
            return;
        }

        StartCoroutine(CheckCups(index));
    }
}
