using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroIdleState : HeroState
{
    public HeroIdleState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName)
    {
    }
}
