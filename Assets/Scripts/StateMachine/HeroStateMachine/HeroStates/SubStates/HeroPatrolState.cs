using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPatrolState : HeroIdleState
{
    public HeroPatrolState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName)
    {
    }
}
