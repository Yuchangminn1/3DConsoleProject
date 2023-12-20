using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerStateController _player, int _stateNum) : base(_player, _stateNum)
    {
        //player = _player;
        //stateNum = _stateNum;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        base.Update();

    }
    public override void LateUpdate()
    {
        base.LateUpdate();
        if (player.jumpButton)
        {
            player.ChangeState(player.jumpState);
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void Exit()
    {
        base.Exit();
    }
}
