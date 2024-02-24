using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableRollingBlock : LockableWeapon<RollingBlock>
    {
        RollingBlock RB;

        protected override void Awake()
        {
            base.Awake();
            RB = thisFirearm as RollingBlock;
        }

        public override bool CanFire()
        {
            if (RB != null)
            {
                if (RB.m_state != RollingBlock.RollingBlockState.HammerBackBreachClosed || !RB.Chamber.IsFull || RB.Chamber.IsSpent) return false;
            }
            return true;
        }
    }
}