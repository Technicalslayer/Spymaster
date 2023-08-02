using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLookAroundState : HeroIdleState
{
    public HeroLookAroundState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName)
    {
    }
}
