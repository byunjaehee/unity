using UnityEngine;

namespace AdvancedCharacterController.Movement.InputManager
//namespace= 클래스 또는 메서드, 변수의 범위를 정의해서 사용할 수 있는 것..! 
// 고급된 캐릭터 컨트롤러 , 움직임에 대한 캐릭터 컨트롤러
{
    [RequireComponent(typeof(Core.AdvancedCharacterController))]
    //RequireComponent=요구되는 컴포넌트 종속성으로 자동으로 추가
    //자동으로 Core.AdvancedCharacterController 컴포넌트 추가
    public class InputManagerMovement : MonoBehaviour
    {
        public string horizontalInput = "Horizontal";
        public string verticalInput = "Vertical";
        //수평 수직 입력
        public bool useRawInput = false;

        public KeyCode jumpKey = KeyCode.Space;
        //Jumpkey를 스페이스 키로 지정

        public float moveSpeed = 7f;

        public float jumpSpeed = 15f;
        // move랑 jump 스피드

        public bool jumpKeyPressed;
        // 이거는 왜 true나 false가 안 나와있을까..?
        private Core.AdvancedCharacterController _characterController;

        private void Awake()
        {
            _characterController = GetComponent<Core.AdvancedCharacterController>();
            //컴포넌트 가져오기
        }

        private void Update()
        {
            jumpKeyPressed = Input.GetKey(jumpKey);
            //jumpkey 눌린거는 스페이스바 눌렸을 때를 의미
        }

        private void FixedUpdate()
        {
            Vector3 velocity = CalculateMovement() * moveSpeed;

            if (jumpKeyPressed && _characterController.IsGrounded) 
            //만약 점프키가 눌렸고 캐릭터 컨트롤러가 바닥에 있다면
            {
                velocity += transform.up * jumpSpeed;
                //윗 방향으로 jumpspeed만큼
            }

            _characterController.Move(velocity);
            //움직임
        }

        private Vector3 CalculateMovement()
        {
            Vector3 velocity = Vector3.zero;
            // 속력을 0으로 만들어줌

            float horizontal;
            float vertical;

            if (useRawInput) //만약 RawInput?이라면
            {
                horizontal = Input.GetAxisRaw(horizontalInput);
                vertical = Input.GetAxisRaw(verticalInput);
                // 수평수직 입력
            }
            else
            {
                horizontal = Input.GetAxis(horizontalInput);
                vertical = Input.GetAxis(verticalInput);
                //아니면 수평수직 입력
            }
            //Raw의 차이점?
            //GetAxis = 부드러운 이동 -1.0f~1.0f
            //GetAxisRaw = 즉시 반응 -1,0,1

            velocity += transform.right * horizontal;
            velocity += transform.forward * vertical;

            //오른쪽 왼쪽 구분

            if (velocity.sqrMagnitude > 1f) // 두 점간의 거리가 1f보다 크다면
                velocity.Normalize();
                //속력 정규화

            return velocity;
        }
    }
}
