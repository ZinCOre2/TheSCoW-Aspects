using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHolder : MonoBehaviour
{
    public Animation Animation;
    public AnimationType Type;
    public float CustomDelay;

    public string ConvertAnimationTypeToName(AnimationType animationType)
    {
        string animationName = string.Empty;
        
        switch (animationType)
        {
            case AnimationType.Idle:
                animationName = "Idle";
                break;
            case AnimationType.Move:
                animationName = "Moving";
                break;
            case AnimationType.CastSpell1:
                animationName = "UseSpell";
                break;
            case AnimationType.CastSpell2:
                animationName = "UseSpell";
                break;
            case AnimationType.Attack1:
                animationName = "UseSpell";
                break;
            case AnimationType.Attack2:
                animationName = "UseSpell";
                break;
            case AnimationType.TakeDamage:
                animationName = "TakeHit";
                break;
            case AnimationType.Die:
                animationName = "Death";
                break;
        }

        return animationName;
    }
}
