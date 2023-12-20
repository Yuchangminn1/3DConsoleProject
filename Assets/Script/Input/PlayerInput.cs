using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInput : MonoBehaviour
{
    PlayerStateController player;
    [SerializeField]
    private InputActionReference run, jump, flashlight, mouseButtonL, mouseButtonR;

    [SerializeField]
    private InputActionReference[] equip;
    private void Awake()
    {
    }

    void Start()
    {
        player = GetComponent<PlayerStateController>();

        run.action.Enable();
        run.action.started += ctx => OnRunStarted();
        run.action.canceled += ctx => OnRunCanceled();

        jump.action.Enable();
        jump.action.started += ctx => OnJumpStarted();

        flashlight.action.Enable();
        flashlight.action.started += ctx => OnFlashlightStarted();

        mouseButtonL.action.Enable();
        mouseButtonL.action.started += ctx => OnMouseLStarted();

        mouseButtonR.action.Enable();
        mouseButtonR.action.started += ctx => OnMouseRStarted();
        mouseButtonR.action.canceled += ctx => ONMouseRCanceled();


        for (int i = 0; i < equip.Length; i++)
        {
            equip[i].action.Enable();
        }
        equip[0].action.started += ctx => OnChangeEquipment(0);
        equip[1].action.started += ctx => OnChangeEquipment(1);
        equip[2].action.started += ctx => OnChangeEquipment(2);
        equip[3].action.started += ctx => OnChangeEquipment(3);
        equip[4].action.started += ctx => OnChangeEquipment(4);


    }

    private void OnRunStarted()
    {
        player.IsRunning = true;
    }

    private void OnRunCanceled()
    {
        player.IsRunning = false;
    }

    private void OnJumpStarted()
    {
        player.jumpButton = true;
    }
    private void OnFlashlightStarted()
    {
        player.ToggleFlashlight();
    }
    private void OnMouseLStarted()
    {
        Debug.Log("마우스 왼쪽 클릭");
        player.FireWeapon();
    }
    private void OnMouseRStarted()
    {
        player.OnAim();
    }
    private void ONMouseRCanceled()
    {
        player.OffAim();
    }
    private void OnChangeEquipment(int _num)
    {
        Debug.Log(_num);
        player.ChangeEquipment(_num);
    }
}

//void OnRun(InputAction.CallbackContext context)
//{
//    if (context.ReadValue<float>() > 0.0f)
//    {
//        player.IsRunning = true;
//    }

//}
//void OffRun(InputAction.CallbackContext context)
//{
//    if (context.ReadValue<float>() > 0.0f)
//    {
//        player.IsRunning = false;
//    }

//}
//void OnLeftShiftPerformed(InputAction.CallbackContext context)
//{
//    if (context.ReadValue<float>() > 0.0f)
//    {
//         왼쪽 Shift 키가 눌렸을 때의 동작
//        player.isRunning = true;
//    }
//    else
//    {
//        player.isRunning = false;
//    }
//}
//void OnLeftShiftCanceled(InputAction.CallbackContext context)
//{
//    if (context.ReadValue<float>() == 0.0f)
//    {
//         왼쪽 Shift 키가 떼졌을 때의 동작
//        Debug.Log("Left Shift Key Released");
//    }
//}
//private void OnEnable()
//{
//    runI.action.performed += PerformedRun;
//    runI.action.ReadValue<bool>();
//}
//private void OnDisable()
//{
//    runI.action.performed -= PerformedRun;

//}
//private void PerformedRun(InputAction.CallbackContext obj)
//{
//    player.isRunning = true;
//}







//public void OnMove(InputAction.CallbackContext context)
//{
//    player.moveInput = context.ReadValue<Vector2>();

//}
//public void OnMouseMove(InputAction.CallbackContext context)
//{
//    player.mouseInput = context.ReadValue<Vector2>();
//}
//public void OnRun(InputAction.CallbackContext context)
//{
//    if (context.performed)
//    {
//        player.isRunning = true;
//    }
//    else
//    {
//        player.isRunning = false;
//    }
//}
//public void OnJump(InputAction.CallbackContext context)
//{
//    if (context.performed)
//    {
//        player.jumpButton = true;
//    }
//}
//public void OnFlashlight(InputAction.CallbackContext context)
//{
//    if (context.performed)
//    {
//        player.flashlightHolder.SetActive(!player.flashlightHolder.activeSelf);
//    }
//}
//public void OnEquip1(InputAction.CallbackContext context)
//{
//    if (context.performed)
//    {
//        player.ChangeEquip(1);
//    }
//}
//public void OnEquip2(InputAction.CallbackContext context)
//{
//    if (context.performed)
//    {
//        player.ChangeEquip(2);
//    }
//}
//public void OnEquip3(InputAction.CallbackContext context)
//{
//    if (context.performed)
//    {
//        player.ChangeEquip(3);
//    }
//}
//public void OnEquip4(InputAction.CallbackContext context)
//{
//    if (context.performed)
//    {
//        player.ChangeEquip(4);
//    }
//}
//public void OnEquip5(InputAction.CallbackContext context)
//{
//    if (context.performed)
//    {
//        player.ChangeEquip(5);
//    }
//}
//public void OnMouseButtonL(InputAction.CallbackContext context)
//{
//    if (context.performed)
//    {
//        player.mouseButtonL = true;
//    }
//    else
//    {
//        player.mouseButtonL = false;
//    }
//}
//public void OnMouseButtonR(InputAction.CallbackContext context)
//{
//    if (context.performed)
//    {
//        player.mouseButtonR = true;
//    }
//    else
//    {
//        player.mouseButtonR = false;
//    }
//}
//}

