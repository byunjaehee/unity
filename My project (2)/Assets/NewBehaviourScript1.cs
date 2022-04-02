using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stair : MonoBehaviour
{
    public Sprite newdoorSprite; //그래픽 오브젝트= sprite newdoor 스프라이트 
    public Sprite olddoorSprite; //olddoor 스프라이트
    public GameObject InTarget; // 게임 오브젝트 InTarget
    public GameObject OutTarget; // 게임 오브젝트 OutTarget
    
    public GameObject PlayerTarget; // 게임 오브젝트 PlayerTarget

    public Transform InTarget_Trans; //위치 InTarget_Trans
    public Transform OutTarget_Trans; //위치 OutTarget_Trans

    SpriteRenderer olddoor; //Sprite 렌더링 olddoor
    
    void Start()  
  
    {
        InTarget_Trans = GetComponent<Transform>(); 
        //InTarget_Trans에 Transform 컴포넌트 가져오기
        OutTarget_Trans = GetComponent<Transform>();
        //OutTarget_Trans에 Transform 컴포넌트 가져오기

        InTarget = this.gameObject;
        // InTarget은 stair이라는 오브젝트로 설정
        InTarget_Trans = InTarget.transform;
        //InTraget_Trans는 InTarget의 위치값으로 설정 (transform 컴포넌트가 Intarget의 위치값)
        OutTarget_Trans = OutTarget.transform;
        //OutTraget_Trans는 OuntTarget의 위치값으로 설정 (transform 컴포넌트가 Outtarget의 위치값)
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other) //충돌(접촉)이 일어난 동안
    {
        if(other.tag == "Player" && Input.GetKeyDown(KeyCode.E)) // Player Push Portal
        //부딪친 오브젝트의 태그가 Player이고 키 E를 눌렀다면
        {
            PlayerTarget = other.gameObject; // Get Player
            //PlayerTarget을 부딪친 오브젝트로 바꾼다
            PlayerTarget.transform.SetPositionAndRotation(OutTarget_Trans.position,new Quaternion(0.0f,0.0f,0.0f,0.0f));
            //PlayerTarget의 위치와 방향을 OutTarget 포지션, (0.0f,0.0f,0.0f,0.0f) 위치로 바꾼다
        }
    }
    void OnTriggerEnter2D (Collider2D other) //충돌(접촉)이 일어났을 순간
    {
		if (other.gameObject.tag=="Player") //부딪친 오브젝트의 태그가 Player이라면
    {
            changedoorsprite();//changedoorsprite() 메서드 실행
		}
    }
    void OnTriggerExit2D(Collider2D other) //충돌(접촉)이 일어나고 분리되는 순간
    { 
        if (other.gameObject.tag=="Player") //부딪친 오브젝트의 태그가 Player이라면
        {
            changedoorspriteback(); //changedoorspriteback() 메서드 실행
		  }
    }

    void changedoorsprite() 
    {
      olddoor = this.gameObject.GetComponent<SpriteRenderer>();
      // olddoor에 stair 오브젝트의 SpriteRender 컴포넌트 할당
        olddoor.sprite = newdoorSprite;
        // olddoor sprite에 newdoorSprite 할당
    }
    void changedoorspriteback() 
    {
      olddoor = this.gameObject.GetComponent<SpriteRenderer>();
      //olddoor에 stair 오브젝트의 SpriteRender 컴포넌트 할당
        olddoor.sprite = olddoorSprite;
        // olddoor sprite에 olddoorSpirte할당
    }
}
