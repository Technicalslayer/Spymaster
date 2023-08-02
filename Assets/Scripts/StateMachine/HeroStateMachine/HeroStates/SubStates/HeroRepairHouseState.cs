using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroRepairHouseState : HeroIdleState
{
    public HeroRepairHouseState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName)
    {
    }
}
