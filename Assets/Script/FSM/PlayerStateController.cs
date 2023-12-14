using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

public class PlayerStateController : MonoBehaviour
{
    public PlayerState currentState;
    StateMachine stateMachine;
    public MoveState moveState;
    public JumpState jumpState;
    public FallState fallState;

    public Vector2 moveInput = Vector2.zero;
    public Vector2 mouseInput = Vector2.zero;

    public Camera playerCamera;
    public GameObject flashlightHolder;

    public bool canMove = true;

    public bool isRunning { get; set; }

    public Animator animator;
    CharacterController characterController;

    Vector3 moveDirection = Vector3.zero;

    float walkingSpeed = 4f;
    float runningSpeed = 8f;
    float jumpSpeed = 20.0f;
    float gravity = 25.0f;
    float lookSpeed = 0.1f;
    float lookXLimit = 45.0f;
    float rotationX = 0;
    float groundCheckDis = 0.3f;
    public bool jumpButton;

    Vector3 nextFixedPosition;
    Vector3 lastFixedPosition;
    [SerializeField]
    LayerMask ground;
    [SerializeField]
    bool isGround;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();

        stateMachine = new StateMachine();
        moveState = new MoveState(this, 0);
        jumpState = new JumpState(this, 1);
        fallState = new FallState(this, 2);
        ChangeState(moveState);

        nextFixedPosition =transform.position;
        lastFixedPosition = transform.position;



        //Ŀ�� �Ⱥ��̰� ���
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        if (UnityEngine.Input.GetKeyDown(KeyCode.C))
        {
            //������ ���� �׽�Ʈ��
            canMove = !canMove;
            Debug.Log($"C {canMove}");
        }
        characterController.Move(Vector3.Lerp(lastFixedPosition,nextFixedPosition, 0.1f)-transform.position);
    }
    private void OnDrawGizmos()
    {
        Vector3 tmp = transform.position;
        tmp.y += groundCheckDis / 2f;
        Gizmos.DrawRay(tmp, Vector3.down * groundCheckDis);
    }
    public void GroundCheck()
    {
         isGround = Physics.Raycast(transform.position,Vector3.down , groundCheckDis, ground);
    }
    public bool GetIsGruond()
    {
        return isGround;
    }
    private void FixedUpdate()
    {
        GroundCheck();
        stateMachine.FixedUpdate();

        if (canMove)
        {
            //xz�� ������ 
            lastFixedPosition = nextFixedPosition;

            float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * moveInput.y : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * moveInput.x : 0;
            moveDirection = (transform.forward * curSpeedX) + (transform.right * curSpeedY);
            moveDirection.y = characterController.velocity.y;
            if (!isGround)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
            if (isGround && jumpButton)
            {
                moveDirection.y = jumpSpeed;
                jumpButton = false;
            }
            if(characterController.velocity.y > 0.15f)
            {
                //Ÿ�̹� �и��� ���� ��ƺ����� �߰���
                jumpButton = false;
            }
            //characterController.Move(moveDirection * Time.deltaTime);
            
            nextFixedPosition += moveDirection * Time.fixedDeltaTime;
            Vector2 tmp1 = new Vector2(nextFixedPosition.x,nextFixedPosition.z);
            Vector2 tmp2 = new Vector2(transform.position.x, transform.position.z);

            if (Vector2.Distance(tmp1, tmp2)>2f) 
            {
                //��ֹ��� ������ next�� last�������� ������ Ŀ���� �ڷ���Ʈ�ϴ°� �����ϰ��� �������
                //last next����ϴ� ���� �ܼ� �ӵ��� �̵��ϸ� �𼭸� �κп��� �������� �̻����浹�� �Ͼ�� 
                //������ ������� �̵��ϰ� https://velog.io/@nagi0101/Unity-%EC%99%84%EB%B2%BD%ED%95%9C-CharacterController-%EA%B8%B0%EB%B0%98-Player%EB%A5%BC-%EC%9C%84%ED%95%B4
                //��������
                nextFixedPosition = transform.position;
                Debug.Log($"������ ũ�׿� {Vector2.Distance(tmp1, tmp2)}");
            }

        }
    }
    private void LateUpdate()
    {
        stateMachine.LateUpdate();

        //xz ��ǲ �ִϸ��̼� 
        if (isRunning && moveInput.y > 0)
        {
            animator.SetFloat("Vertical", 2f);
        }
        else
        {
            animator.SetFloat("Vertical", moveInput.y);
        }
        animator.SetFloat("Horizontal", moveInput.x);

        //ī�޶� ����
        if (mouseInput != null && canMove)
        {
            rotationX += -mouseInput.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, mouseInput.x * lookSpeed, 0);
        }
    }
    public void ChangeState(PlayerState _newState)
    {
        stateMachine.ChangState(_newState);
    }
    
    public void SetState(int _num)
    {
        animator.SetInteger("State", _num);
    }
}
