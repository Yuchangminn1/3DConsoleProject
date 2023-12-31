using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : PlayerState
{
    public FallState(PlayerStateController _player, int _stateNum) : base(_player, _stateNum)
    {
        player = _player;
        stateNum = _stateNum;
        ableFly = true;

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
        if (player.GetIsGround())
        {
            player.ChangeState(player.moveState);
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
