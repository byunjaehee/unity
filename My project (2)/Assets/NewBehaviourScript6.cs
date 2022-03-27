using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
//CharacterController이라는 컴포넌트 생성(?)


public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    public float sneakSpeed = 2.5f;
    public float runSpeed = 8.0f;
    public float crouchWalkSpeed = 3.5f;
    public float crouchRunSpeed = 6.5f;
    public float crouchSneakSpeed = 1f;
    public float jumpSpeed = 6.0f;

    //움직임 속도 설정

    public bool limitDiagonalSpeed = true;
    public bool toggleRun = false;
    public bool toggleSneak = false;
    public bool airControl = true; // strafing / b-hop
    public bool firstPerson = false;
    public bool crouching = false; //crouching=웅크림

    //상태 

    public float gravity = 10.0f;
    public float fallingDamageLimit = 10.0f;

    private Vector3 moveDirection;

    private bool grounded;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
	private bool punching;
    private bool playerControl;
    private Animator anim;


    // Use this for initialization
    void Start()
    {
        moveDirection = Vector3.zero;
        grounded = false;
        playerControl = false;
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? 0.6701f : 1.0f;

        //움직임 코드
        //만약 X 입력이 0.0f가 아니고 Y의 입력이 0.0f가 아니면서 limitDiagonalSpeed이라면 0.06701f,아니면 1.0f
    

        anim.SetFloat("BlendX", (inputX * 2));
        anim.SetFloat("BlendY", (inputY * 2));

        //SetFloat은 실수 값 설정
        //SetFloat("Parameter 이름", Parameter의 float 값을 변경할 데이터)
        //BlendX와 BlendY의 값을 inputX,Y값의 각각 2배로 설정

 
        anim.SetBool("Walking", (anim.GetFloat("BlendX") != 0 || anim.GetFloat("BlendY") != 0));

		anim.SetBool("Punching", Input.GetButton("Fire1"));

        //Setbool로 뒤의 값을 true로 하는지 false로 하는지 모르겠다

 



        if (grounded) //만약 grounded라면? (grounded=true)라는 뜻? 
        {

            if (falling) 
            // 그리고 떨어지고 있다면
            {
                falling = false;
                //falling 상태를 false로 바꾸고
                if (myTransform.position.y < (fallStartLevel - fallingDamageLimit))
                //만약 나의 위치의 높이가 떨어지는 구간에서 fallingDamageLimit (10.0f)값을 뺀것보다 작다면
                {
                    FallingDamageAlert(fallStartLevel - myTransform.position.y);
                    //FallingDamageAlert ()메서드 실행 매개변수는 fallStartLevel - myTransform.position.y
                }
            }

            if (!toggleRun)
            //toggleRun이 true라면
            {
                bool running = Input.GetButton("Run");
                // running은 "Run" 버튼이 눌렸을 때
                speed = running ? runSpeed : walkSpeed;
                // running이 true라면 speed 값에 runSpeed, 아니면 walkSpeed로 할당
            
                anim.SetBool("Running", running);
                //"Running"을 running(true)로 변경

            } 

            else
            {
                anim.SetBool("Running", true);
                //"Running" 을 true로 변경
            }

            if (!toggleSneak)
            //만약 toggleSneak가 true라면
            {
                bool sneaking = Input.GetButton("Sneak");
                // Sneaking은 "Sneak" 버튼이 눌렸을 때
                speed = sneaking ? sneakSpeed : speed;
                //sneaking이 true라면 speed에 sneakSpeed, 아니면 walkSpeed를 할당
                anim.SetBool("Sneaking", sneaking);
                //"Sneaking"을 sneaking(true)로 변경
            }

            if (crouching)
            //만약 crouching이 true라면
            {
                speed = Input.GetButton("Run") ? crouchRunSpeed : crouchWalkSpeed;
                //"Run"이 true라면 speed에 crouchRunSpeed , 아니면 crouchWalkSpeed 할당
                speed = Input.GetButton("Sneak") ? crouchSneakSpeed : speed;
                ////"Sneak"가 true라면 speed에 crouchRunSpeed , 아니면 speed 할당


            }

            //print(speed);
            moveDirection = new Vector3(inputX * inputModifyFactor, 0, inputY * inputModifyFactor);
            moveDirection = myTransform.TransformDirection(moveDirection) * speed;

            //움직임 방향 변화

            if (!Input.GetButton("Jump"))
            //만약 "Jump"키 입력이 없다면
            {
                anim.SetBool("Jump", false);
                //"Jump" 상태 false로 변경
            }
            else 
            {
                moveDirection.y = jumpSpeed;
                //움직임의 높이 jumpSpeed로 설정
                
                anim.SetBool("Jump", true);
                //"Jump" 상태 true로 변경
            }
        }
        else
        {
            if (!falling)
            //falling이 false라면
            {
                falling = true;
                //falling을 true로 변경
                fallStartLevel = myTransform.position.y;
                //fallStartLevel을 현재 위치의 높이로 변경
            }

            if (airControl && playerControl)
            //airControl과 playerControl이 true라면
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = myTransform.TransformDirection(moveDirection);
                //방향 변경
            }
        }

        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        moveDirection.y -= gravity * Time.deltaTime;

        
    }

    void Update()
    {
        if (toggleRun && grounded && Input.GetButtonDown("Run"))
        //만약 toggleRun, grounded가 true이고 Run이라는 입력이 있다면
            speed = (speed == walkSpeed ? runSpeed : walkSpeed);
            //walkSpeed가 true라면 speed 값을 runSpeed, 아니면 walkSpeed로 할당

        if (Input.GetButtonUp("Crouch"))
        //만약 Crouch 버튼이 눌렸다면
        {
            crouching = !crouching;
            //crouching false 상태를 true로 변경?
            anim.SetBool("Crouch", crouching);
            //Crouch를 true로 변경 .. 

        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //print(hit.point);
    }

    void FallingDamageAlert(float fallDistance)
    {
        print("Ouch! Fell " + fallDistance + " units!");
    }
}