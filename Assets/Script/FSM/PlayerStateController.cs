using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerStateController : MonoBehaviour
{
    
    // 상태 및 스테이트 관련 변수
    public PlayerState currentState;
    public MoveState moveState;
    public JumpState jumpState;
    public FallState fallState;
    StateMachine stateMachine;

    // 플레이어 입력 관련 변수
    public Vector2 moveInput = Vector2.zero;
    public Vector2 mouseInput = Vector2.zero;

    // 카메라 및 애니메이터 관련 변수
    public Camera playerCamera;
    //public Camera playerAimCamera;
    public GameObject crossHair;
    public GameObject flashlightHolder;
    public Animator animator;

    // 플레이어 입력 변수
    public bool canMove = true;
    private bool _isRunning = false;
    public bool IsRunning { get { return _isRunning; } set { _isRunning = value; } } 
    public bool jumpButton { get; set; } = false;
    public bool mouseLButton { get; set; } = false;
    public bool mouseRButton { get; set; } = false;


    // 캐릭터 컨트롤러 및 이동 속도 관련 변수
    CharacterController characterController;
    float walkingSpeed = 4f;
    float runningSpeed = 8f;
    float jumpSpeed = 20.0f;
    float gravity = 25.0f;
    float lookSpeed = 0.1f;
    float lookXLimit = 45.0f;
    float rotationX = 0;
    float groundCheckDis = 0.3f;
    Vector3 moveDirection = Vector3.zero;

    // 장비 및 이동 위치 관련 변수
    public List<GameObject> equip;
    PlayerWeapon bow;
    PlayerWeapon pistol;
    PlayerWeapon rifle;
    PlayerWeapon hand;
    public PlayerWeapon currentEquip;
    Vector3 nextFixedPosition;
    Vector3 lastFixedPosition;
    [SerializeField]
    LayerMask ground;
    [SerializeField]
    bool isGround = false;
    bool isAiming = false;

    // 기타 플래그 및 액션 관련 변수
    bool animationTrigger { get; set; }
    [SerializeField]
    private InputActionReference moveI, mousemoveI;
    [SerializeField]
    Image corssHair1;


    //총 관련
    float rayDistance = 10f; //총 사정거리라고 생각
    [SerializeField]
    LayerMask layerWall; //총 활 등에 영향을 받는 물체
    [SerializeField]
    LayerMask layerEnemy; //총 활 등에 영향을 받는 물체 
    
    private void Awake()
    {
        
        //foreach (GameObject _gameObject in equip)
        //{
        //    _gameObject.SetActive(false);
        //}
        stateMachine = new StateMachine();
        moveState = new MoveState(this, 0);
        jumpState = new JumpState(this, 1);
        fallState = new FallState(this, 2);
        nextFixedPosition = transform.position;
        lastFixedPosition = transform.position;

        //if (crossHair != null)
        //{
        //    crossHair.SetActive(false);
        //    //playerAimCamera.enabled = false;

        //}
    }

    void Start()
    {
        WeaponInit();

        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();

        ChangeState(moveState);
        ChangeEquipment(3);

        //커서 안보이고 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void WeaponInit()
    {
        //                      오브젝트 사거리                           
        bow = new PlayerWeapon(equip[0], 0,10, layerWall, layerEnemy, corssHair1);
        pistol = new PlayerWeapon(equip[1], 1, 10, layerWall, layerEnemy, corssHair1);
        rifle = new PlayerWeapon(equip[2], 2, 10, layerWall, layerEnemy, corssHair1);
        hand = new PlayerWeapon(equip[3], 3, 0.1f, layerWall, layerEnemy, corssHair1);
        corssHair1.enabled = false;
    }

    void Update()
    {
        //입력값 받기
        moveInput = moveI.action.ReadValue<Vector2>();
        mouseInput = mousemoveI.action.ReadValue<Vector2>();

        stateMachine.Update();
        SetCanMove();

        characterController.Move(Vector3.Lerp(lastFixedPosition, nextFixedPosition, 0.1f) - transform.position);
    }



    private void FixedUpdate()
    {
        GroundCheck();
        stateMachine.FixedUpdate();
        PlayerMovement();
    }

    private void LateUpdate()
    {
        stateMachine.LateUpdate();

        //xz 인풋 애니메이션 
        AnimatorSetInputXZ();

        //카메라 무브
        CameraMove();
    }
    #region InputMove
    private void CameraMove()
    {
        if (mouseInput != null && canMove)
        {
            rotationX += -mouseInput.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, mouseInput.x * lookSpeed, 0);
        }
    }
    private void SetCanMove()
    {
        //움직임 막기 테스트용
        if (UnityEngine.Input.GetKeyDown(KeyCode.C))
        {
            canMove = !canMove;
            Debug.Log($"C {canMove}");
        }
    }
    private void PlayerMovement()
    {
        if (!canMove)
        {
            return;
        }
        //플레이어의 움직임을 계산
        //xz축 움직임 
        lastFixedPosition = nextFixedPosition;

        float curSpeedX = canMove ? (IsRunning ? runningSpeed : walkingSpeed) * moveInput.y : 0;
        float curSpeedY = canMove ? (IsRunning ? runningSpeed : walkingSpeed) * moveInput.x : 0;
        moveDirection = (transform.forward * curSpeedX) + (transform.right * curSpeedY);
        moveDirection.y = characterController.velocity.y;
        if (!isGround)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        if (isGround && jumpButton)
        {
            stateMachine.LateUpdate();
            moveDirection.y = jumpSpeed;
            jumpButton = false;
        }
        if (characterController.velocity.y > 0.15f)
        {
            //타이밍 밀리는 점프 잡아볼려고 추가함
            jumpButton = false;
        }
        //characterController.Move(moveDirection * Time.deltaTime);

        nextFixedPosition += moveDirection * Time.fixedDeltaTime;
        Vector2 tmp1 = new Vector2(nextFixedPosition.x, nextFixedPosition.z);
        Vector2 tmp2 = new Vector2(transform.position.x, transform.position.z);

        if (Vector2.Distance(tmp1, tmp2) > 2f)
        {
            //장애물에 막히면 next와 last포지션의 간격이 커져서 텔러포트하는걸 방지하고자 만들었음
            //last next사용하는 이유 단순 속도로 이동하면 모서리 부분에서 슈퍼점프 이상한충돌이 일어나서 
            //포지션 계산으로 이동하게 https://velog.io/@nagi0101/Unity-%EC%99%84%EB%B2%BD%ED%95%9C-CharacterController-%EA%B8%B0%EB%B0%98-Player%EB%A5%BC-%EC%9C%84%ED%95%B4
            //참조했음
            nextFixedPosition = transform.position;
            Debug.Log($"오차가 크네용 {Vector2.Distance(tmp1, tmp2)}");
        }
    }
    #endregion
    public void ChangeEquipment(int _num)
    {

        PlayerWeapon _playerWeapon;
        //무기 체인지 
        if (_num== 0)
        {
            _playerWeapon = bow;
        }
        else if (_num == 1)
        {
            _playerWeapon = pistol;
        }
        else if (_num == 2)
        {
            _playerWeapon = rifle;
        }
        else if (_num == 3)
        {
            _playerWeapon = hand;
        }
        else
        {
            return;
        }
        if (currentEquip != _playerWeapon)
        {
            if(currentEquip != null)
            {
                currentEquip.Unequip();
            }
            //currentEquip.SetActive(false);
            currentEquip = _playerWeapon;
            //currentEquip = equip[_num];
            currentEquip.Equip();
            //currentEquip.SetActive(true);
            animator.SetInteger("Weapon", currentEquip.weaponNum);
        }
    }
    public void FireWeapon() => currentEquip.Fire
        (playerCamera.transform.position,playerCamera.transform.forward);
    public void OnAim() => currentEquip.OnAim();
    public void OffAim() => currentEquip.OffAim();

    //public void FireWeapon()
    //{
    //    //이건 클래스로 받아서 오버라이드 해야할거같음
    //    if (currentEquip == equip[3])
    //    {
    //        //맨손은 일단 생략
    //        return;
    //    }
    //    //if (currentEquip == equip[0])
    //    //{
    //    //    //활
    //    //    ;
    //    //}
    //    else
    //    {
    //        //총
    //        // 현재 위치와 전방을 향한 방향으로 레이를 쏩니다.
    //        Vector3 tmp = playerCamera.transform.position;
    //        Ray ray = new Ray(tmp, playerCamera.transform.forward); ;
    //        RaycastHit hitInfo;
    //        Debug.Log("발사");
    //        // 만약 레이가 어떤 물체와 충돌한다면
    //        if (Physics.Raycast(ray, out hitInfo, rayDistance, layerEnemy))
    //        {
    //            // 충돌한 물체의 정보 출력
    //            Debug.Log("Hit Monster : " + hitInfo.collider.name);

    //            hitInfo.transform.GetComponent<Enemy>().HitEnemy();
    //            // 여기서 추가적인 동작을 수행할 수 있습니다.
    //            // 예를 들어, 충돌한 물체에 대한 처리를 진행하거나 특정 동작을 수행할 수 있습니다.
    //        }
    //        else if (Physics.Raycast(ray, out hitInfo, rayDistance, layerWall))
    //        {
    //            // 충돌한 물체의 정보 출력
    //            Debug.Log("Hit Wall: ");

    //            // 여기서 추가적인 동작을 수행할 수 있습니다.
    //            // 예를 들어, 충돌한 물체에 대한 처리를 진행하거나 특정 동작을 수행할 수 있습니다.
    //        }
    //        else
    //        {
    //            // 레이가 충돌하지 않았을 때의 동작
    //        }
    //        // 레이를 시각적으로 그리기 위해 주석 해제할 수 있습니다.
    //        Debug.DrawRay(tmp, playerCamera.transform.forward * rayDistance, Color.green);
    //    }
    //}
    //public void OnAim()
    //{
    //    if (currentEquip != equip[3])
    //    {
    //        //조준중이고 맨손이 아니면 
    //        isAiming = true;
    //        crossHair.SetActive(true);

    //        //if (playerAimCamera.enabled == false)
    //        //{
    //        //    playerAimCamera.enabled = true;
    //        //    isAiming = true;
    //        //    crossHair.SetActive(true);
    //        //}
    //    }
    //}
    //public void OffAim()
    //{
    //    isAiming = false;
    //    crossHair.SetActive(false);
    //    //if (playerAimCamera.enabled == true)
    //    //{
    //    //    playerAimCamera.enabled = false;
    //    //    isAiming = false;
    //    //    crossHair.SetActive(false);
    //    //}
    //}
    #region Animator
    private void AnimatorSetInputXZ()
    {
        if (IsRunning && moveInput.y > 0)
        {
            animator.SetFloat("Vertical", 2f);
        }
        else
        {
            animator.SetFloat("Vertical", moveInput.y);
        }
        animator.SetFloat("Horizontal", moveInput.x);
    }
    public void AnimationTriggerON()
    {
        animationTrigger = true;
    }
    public void AnimationTriggerOFF()
    {
        animationTrigger = false;
    }
    public bool AnimationTrigger()
    {
        return animationTrigger;
    }
    public void SetState(int _num)
    {
        animator.SetInteger("State", _num);
    }
    #endregion
    // Update is called once per frame
    #region GroundCheck
    private void OnDrawGizmos()
    {
        Vector3 tmp = transform.position;
        tmp.y += groundCheckDis / 2f;
        Gizmos.DrawRay(tmp, Vector3.down * groundCheckDis);
    }
    public void GroundCheck()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, groundCheckDis, ground);
    }
    public bool GetIsGround()
    {
        return isGround;
    }
    #endregion
    public void ChangeState(PlayerState _newState)
    {
        stateMachine.ChangState(_newState);
    }
    public void ToggleFlashlight()
    {
        flashlightHolder.SetActive(!flashlightHolder.activeSelf);
    }
}
