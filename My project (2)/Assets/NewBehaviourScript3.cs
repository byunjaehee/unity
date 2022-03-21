using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private float bossHP = 100.0f; //bossHP 100으로 할당
    private float bossSpeed = 50.0f; //bossSpeed 50으로 할당
    private bool canAttack = true; //canAttack은 true로 설정
    private bool firstPattern = true; //firstPatter은 true로 설정

    public GameObject bossAttack; //bossAttack 이라는 게임 오브젝트 변수 선언
    public GameObject target; //target이라는 게임 오브젝트 변수 선언
    private Rigidbody2D bossRigidbody; //bossRigidbody라는 리지드바디 설정

    // Start is called before the first frame update
    void Start()
    {
        bossRigidbody = GetComponent<Rigidbody2D>();
        //bossRigidbody에 rigidbody 컴포넌트 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        target = GameObject.Find("Player"); //traget은 "Player"라는 게임 오브젝트
        Attack();
        Move();
        //Attack, Move 메서드 실행

        if(bossHP <= 0 ) //만약 bossHP가 0과 같거나 작으면
        {
            BossDie(); //BossDie 메서드 실행
        }
    }
    void Attack()
    {
        if(canAttack == true && target != null) 
        //canAttack이 true상태고 target이 존재한다면
        {
            StartCoroutine(bossAttackDelay());
            //매개변수 bossAttackDelay를 사용해 StartCoroutine 메서드 실행
        }
    }
    void Move()
    {
        if(bossRigidbody.velocity.y == 0) //만약 bossRigidbody의 y축의 속력이 0이라면
        {
            int randomMove = Random.Range(-1,2); 
            // randomMove를 -1과 2 사이 값 하나로 지정 (-1,0,1) 방향 설정
            int randomJump = Random.Range(0,100);
             // randomJump를 0~100사이 값 하나로 지정 (0~99) 
            Vector2 BossRandomMove = new Vector2(randomMove,0);
            // BossRandomMove를 (randomMove,0)로 변경
            Vector2 BossRandomJump = new Vector2(0 , 10.0f);
            // BossRandomJump를 (0 , 10.0f)로 변경
            bossRigidbody.AddForce(BossRandomMove.normalized * bossSpeed);
            //bossRigidbody에 정규화된 값인 BossRandomMove에 bossSpeed(50.0f)를 곱한 값의 힘을 더함

            if(randomJump == 1 && bossRigidbody.velocity.y == 0) 
            //만약 randumJump의 값이 1이며 bossRigidbody의 y축의 속력이 0이라면
            {
                bossRigidbody.AddForce(BossRandomJump * bossSpeed);
                //bossRigidbody에 bossRandomJump에 bossSpeed(50.0f)를 곱한 값의 힘을 더함
            }
        }
        if(bossHP == 50 && firstPattern == true)
        //bossHP가 50이고 firstPatter이 ture일 때
        {
            transform.position = target.transform.position + new Vector3(0,2,0);
            //위치를 타겟위치에 (0,2,0) 더한 값으로 변경
            firstPattern = false;
            //firstPatter을 false로 변경
        }
        if(bossRigidbody.velocity.y < 0) 
        // bossRigidbody의 y축의 속력이 0 보다 작다면
        {
            bossRigidbody.AddForce(new Vector2(0,-100.0f)); 
            // bossRigidbody에 (0,-100.0f)의 힘을 더함
        }
    }
    void BossDie()
    {
        Destroy(gameObject); //게임 오브젝트 파괴
    }
    
    IEnumerator bossAttackDelay() //코루틴 반환값(?) IEnumerator
    {
        canAttack = false;
        //canAttack을 false 상태로 변경
        yield return new WaitForSeconds(0.5f); 
        // 코루틴 반환값으로 WiatForSeconds (0,5f) 반환
        GameObject bossOnAttack = Instantiate(bossAttack,transform.position,Quaternion.identity); // spawn boss attack after 2 seconds
        //bossOnAttack 게임 오브젝트 bossAttack을 transform.position 위치로 , Quaternio.identit 회전값으로 생성
        //Instantiate = 게임중 게임 오브젝트 생성
        canAttack = true;
        //canAttack을 true 상태로 변경
    }
    private void OnTriggerEnter2D(Collider2D other) //충돌(접촉)이 일어난 순간
     {
        if(other.tag == "PlayerAttack") //만약 부딪친 오브젝트의 태그가 "PlayerAttack"이라면
        {
            bossHP -= 1; //bossHP가 1씩 계속 줄어듦
            Debug.Log("Boss HP:" + bossHP);
            //Boss HP의 값 콘솔창에서 보여주기
        }
    }
}
