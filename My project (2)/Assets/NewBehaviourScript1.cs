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
    // Start is called before the first frame update
    void Start()
    {
        InTarget_Trans = GetComponent<Transform>();
        OutTarget_Trans = GetComponent<Transform>();

        InTarget = this.gameObject;
        InTarget_Trans = InTarget.transform;
        OutTarget_Trans = OutTarget.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other) {
        if(other.tag == "Player" && Input.GetKeyDown(KeyCode.E)) // Player Push Portal
        {
            PlayerTarget = other.gameObject; // Get Player
            PlayerTarget.transform.SetPositionAndRotation(OutTarget_Trans.position,new Quaternion(0.0f,0.0f,0.0f,0.0f));
        }
    }
    void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag=="Player") {
            changedoorsprite();
		}
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag=="Player") {
            changedoorspriteback();
		  }
    }

    void changedoorsprite() {
      olddoor = this.gameObject.GetComponent<SpriteRenderer>();
        olddoor.sprite = newdoorSprite;
    }
    void changedoorspriteback() {
      olddoor = this.gameObject.GetComponent<SpriteRenderer>();
        olddoor.sprite = olddoorSprite;
    }
}
