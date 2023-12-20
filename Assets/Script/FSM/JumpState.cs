using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerStateController _player, int _stateNum) : base(_player, _stateNum)
    {
        //player = _player;
        //stateNum = _stateNum;
        ableFly = true;
    }

    public override void Enter()
    {
        base.Enter();
        player.AnimationTriggerON();
    }
    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate()
    {

        if (player.AnimationTrigger())
        {
            return;
        }
        //애니메이션트리거 후 변경할려고 
        base.LateUpdate();

        if (player.GetIsGround())
        {
            player.ChangeState(player.moveState);
        }
        else
        {
            player.ChangeState(player.fallState);
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
