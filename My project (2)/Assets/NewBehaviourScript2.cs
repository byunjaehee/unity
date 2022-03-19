using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainCam : MonoBehaviour
{
    public GameObject target; //게임 오브젝트로 target을 설정
    void Start()
    {
        target = GameObject.Find("Player"); //target에 Player이라는 이름을 가진 대상을 부여
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null) //target이 존재한다면
        {
             // Camera.main.transform.position = target.transform.position - Vector3.forward; <-follow target
             Camera.main.transform.position = new Vector3(0,2,-10); // <- fix the position
             // Camera.main.transform.position= 메인 카메라가 추적 대상을 쫓게 되는 예제로 
             // Vector(0,2,-10)으로 변경
        }
        else
        {
            return; //존재하지 않으면 함수를 종료
        }
    }
}