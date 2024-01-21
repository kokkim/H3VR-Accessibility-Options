using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableBoltActionRifle : LockableWeapon<BoltActionRifle>
    {
        BoltActionRifle BAR;

        protected override void Awake()
        {
            base.Awake();
            BAR = thisFirearm as BoltActionRifle;
        }

        public override bool CanFire()
        {
            if (BAR != null)
            {
                //Safe
                if (BAR.FireSelector_Modes[BAR.m_fireSelectorMode].ModeType == BoltActionRifle.FireSelectorModeType.Safe) return false;
            }
            return true;
        }
    }
}