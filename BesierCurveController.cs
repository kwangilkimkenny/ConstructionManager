using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BesierCurveController : MonoBehaviour
{

    // 보간의 진행도를 실시간으로 조작해서 변경해 보기 위한 변수
    [Range(0f, 1f)] // 0 ~ 1 범위에서만 조작되게 제한하기 위한 설정
    public float m_rate = 0f;

    // 시작지점 / 종료지점 오브젝트를 받아서 Transform 를 얻어오기 위한 변수
    public Transform m_p_Start;
    public Transform m_p_End;

    // 중간지점 오브젝트를 받아서 Transform 을 얻기 위한 배열
    public Transform[] m_pos_Num = new Transform[2];

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = BezierCurve(0.02f, 0.02f);
    }

    // 베지어곡선 계산 공식에 따라 계산 후 최종 위치를 리턴
    Vector3 BezierCurve(float startWidth, float endWidth)
    {
        // 전체 지점의 위치 정보를 저장하기 위한 위치리스트(배열)
        List<Vector3> t_pointList = new List<Vector3>();

        // 시작지점 위치값을 위치리스트 에 추가
        t_pointList.Add(m_p_Start.position);

        // 중간지점 위치값을 위치리스트 에 추가
        foreach (var T in m_pos_Num)
        {
            t_pointList.Add(T.position);
        }

        // 종료지점 위치값을 위치리스트에 추가
        t_pointList.Add(m_p_End.position);

        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = true;
        lineRenderer.positionCount = t_pointList.Count;

        lineRenderer.SetPositions(t_pointList.ToArray());


        //// 오브젝트의 위치를 계산하기 위한 계산 시작
        //// 위치가 1개 남을때 까지 반복문 으로 계산
        //while (t_pointList.Count > 1)
        //{
        //    //선형보간으로 계산된 결과값을 저장하기 위한 결과값리스트(배열)
        //    List<Vector3> t_resultList = new List<Vector3>();

        //    for (int i = 0; i < t_pointList.Count - 1; i++)
        //    {
        //        // 현재 지점과 다음 지점의 선형보간을 통한 위치값 계산
        //        Vector3 result = Vector3.Lerp(t_pointList[i], t_pointList[i + 1], m_rate);

        //        // 계산된 위치 값을 결과값리스트에 대입
        //        t_resultList.Add(result);
        //    }
        //    // 위치리스트를 결과값 리스트로 변경
        //    t_pointList = t_resultList;
        //}

        

        return t_pointList[0];
    }
}
