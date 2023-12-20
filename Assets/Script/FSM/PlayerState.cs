using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerStateController player;
    protected int stateNum;
    protected bool ableFly = false;
    public PlayerState(PlayerStateController _player, int _stateNum)
    {
        player = _player;
        stateNum = _stateNum;
    }
    public virtual void Enter()
    {
        player.SetState(stateNum);
        Debug.Log("Enter");
    }
    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {
        
    }
    public virtual void LateUpdate()
    {
        if (!ableFly)
        {
            if (!player.GetIsGround())
            {
                player.ChangeState(player.fallState);
            }
        }
    }
    public virtual void Exit()
    {

    }
}
