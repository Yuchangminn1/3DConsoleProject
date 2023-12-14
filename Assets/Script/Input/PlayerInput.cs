using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
public class PlayerInput : MonoBehaviour
{
    PlayerStateController player;
    private void Awake()
    {
        player = GetComponent<PlayerStateController>();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        player.moveInput = context.ReadValue<Vector2>();
        
    }
    public void OnMouseMove(InputAction.CallbackContext context)
    {
        player.mouseInput = context.ReadValue<Vector2>();
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            player.isRunning = true;
        }
        else
        {
            player.isRunning = false;
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed )
        {
            player.jumpButton = true;
            
        }
    }
    public void OnFlashlight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            player.flashlightHolder.SetActive(!player.flashlightHolder.activeSelf);
        }
    }

}

