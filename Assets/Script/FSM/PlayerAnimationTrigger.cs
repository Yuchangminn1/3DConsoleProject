using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    PlayerStateController player;
    private void Start()
    {
        player = GetComponentInParent<PlayerStateController>();
    }
    void AnimationTriggerOFF()
    {
        player.AnimationTriggerOFF();
    }
}
