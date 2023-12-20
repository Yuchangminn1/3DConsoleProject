using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerStateController : MonoBehaviour
{
    
    // ���� �� ������Ʈ ���� ����
    public PlayerState currentState;
    public MoveState moveState;
    public JumpState jumpState;
    public FallState fallState;
    StateMachine stateMachine;

    // �÷��̾� �Է� ���� ����
    public Vector2 moveInput = Vector2.zero;
    public Vector2 mouseInput = Vector2.zero;

    // ī�޶� �� �ִϸ����� ���� ����
    public Camera playerCamera;
    //public Camera playerAimCamera;
    public GameObject crossHair;
    public GameObject flashlightHolder;
    public Animator animator;

    // �÷��̾� �Է� ����
    public bool canMove = true;
    private bool _isRunning = false;
    public bool IsRunning { get { return _isRunning; } set { _isRunning = value; } } 
    public bool jumpButton { get; set; } = false;
    public bool mouseLButton { get; set; } = false;
    public bool mouseRButton { get; set; } = false;


    // ĳ���� ��Ʈ�ѷ� �� �̵� �ӵ� ���� ����
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

    // ��� �� �̵� ��ġ ���� ����
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

    // ��Ÿ �÷��� �� �׼� ���� ����
    bool animationTrigger { get; set; }
    [SerializeField]
    private InputActionReference moveI, mousemoveI;
    [SerializeField]
    Image corssHair1;


    //�� ����
    float rayDistance = 10f; //�� �����Ÿ���� ����
    [SerializeField]
    LayerMask layerWall; //�� Ȱ � ������ �޴� ��ü
    [SerializeField]
    LayerMask layerEnemy; //�� Ȱ � ������ �޴� ��ü 
    
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

        //Ŀ�� �Ⱥ��̰� ���
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void WeaponInit()
    {
        //                      ������Ʈ ��Ÿ�                           
        bow = new PlayerWeapon(equip[0], 0,10, layerWall, layerEnemy, corssHair1);
        pistol = new PlayerWeapon(equip[1], 1, 10, layerWall, layerEnemy, corssHair1);
        rifle = new PlayerWeapon(equip[2], 2, 10, layerWall, layerEnemy, corssHair1);
        hand = new PlayerWeapon(equip[3], 3, 0.1f, layerWall, layerEnemy, corssHair1);
        corssHair1.enabled = false;
    }

    void Update()
    {
        //�Է°� �ޱ�
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

        //xz ��ǲ �ִϸ��̼� 
        AnimatorSetInputXZ();

        //ī�޶� ����
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
        //������ ���� �׽�Ʈ��
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
        //�÷��̾��� �������� ���
        //xz�� ������ 
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
            //Ÿ�̹� �и��� ���� ��ƺ����� �߰���
            jumpButton = false;
        }
        //characterController.Move(moveDirection * Time.deltaTime);

        nextFixedPosition += moveDirection * Time.fixedDeltaTime;
        Vector2 tmp1 = new Vector2(nextFixedPosition.x, nextFixedPosition.z);
        Vector2 tmp2 = new Vector2(transform.position.x, transform.position.z);

        if (Vector2.Distance(tmp1, tmp2) > 2f)
        {
            //��ֹ��� ������ next�� last�������� ������ Ŀ���� �ڷ���Ʈ�ϴ°� �����ϰ��� �������
            //last next����ϴ� ���� �ܼ� �ӵ��� �̵��ϸ� �𼭸� �κп��� �������� �̻����浹�� �Ͼ�� 
            //������ ������� �̵��ϰ� https://velog.io/@nagi0101/Unity-%EC%99%84%EB%B2%BD%ED%95%9C-CharacterController-%EA%B8%B0%EB%B0%98-Player%EB%A5%BC-%EC%9C%84%ED%95%B4
            //��������
            nextFixedPosition = transform.position;
            Debug.Log($"������ ũ�׿� {Vector2.Distance(tmp1, tmp2)}");
        }
    }
    #endregion
    public void ChangeEquipment(int _num)
    {

        PlayerWeapon _playerWeapon;
        //���� ü���� 
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
    //    //�̰� Ŭ������ �޾Ƽ� �������̵� �ؾ��ҰŰ���
    //    if (currentEquip == equip[3])
    //    {
    //        //�Ǽ��� �ϴ� ����
    //        return;
    //    }
    //    //if (currentEquip == equip[0])
    //    //{
    //    //    //Ȱ
    //    //    ;
    //    //}
    //    else
    //    {
    //        //��
    //        // ���� ��ġ�� ������ ���� �������� ���̸� ���ϴ�.
    //        Vector3 tmp = playerCamera.transform.position;
    //        Ray ray = new Ray(tmp, playerCamera.transform.forward); ;
    //        RaycastHit hitInfo;
    //        Debug.Log("�߻�");
    //        // ���� ���̰� � ��ü�� �浹�Ѵٸ�
    //        if (Physics.Raycast(ray, out hitInfo, rayDistance, layerEnemy))
    //        {
    //            // �浹�� ��ü�� ���� ���
    //            Debug.Log("Hit Monster : " + hitInfo.collider.name);

    //            hitInfo.transform.GetComponent<Enemy>().HitEnemy();
    //            // ���⼭ �߰����� ������ ������ �� �ֽ��ϴ�.
    //            // ���� ���, �浹�� ��ü�� ���� ó���� �����ϰų� Ư�� ������ ������ �� �ֽ��ϴ�.
    //        }
    //        else if (Physics.Raycast(ray, out hitInfo, rayDistance, layerWall))
    //        {
    //            // �浹�� ��ü�� ���� ���
    //            Debug.Log("Hit Wall: ");

    //            // ���⼭ �߰����� ������ ������ �� �ֽ��ϴ�.
    //            // ���� ���, �浹�� ��ü�� ���� ó���� �����ϰų� Ư�� ������ ������ �� �ֽ��ϴ�.
    //        }
    //        else
    //        {
    //            // ���̰� �浹���� �ʾ��� ���� ����
    //        }
    //        // ���̸� �ð������� �׸��� ���� �ּ� ������ �� �ֽ��ϴ�.
    //        Debug.DrawRay(tmp, playerCamera.transform.forward * rayDistance, Color.green);
    //    }
    //}
    //public void OnAim()
    //{
    //    if (currentEquip != equip[3])
    //    {
    //        //�������̰� �Ǽ��� �ƴϸ� 
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
