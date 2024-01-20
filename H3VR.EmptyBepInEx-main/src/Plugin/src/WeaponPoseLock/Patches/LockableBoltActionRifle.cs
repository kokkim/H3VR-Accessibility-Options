using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableBoltActionRifle : LockableWeapon<BoltActionRifle>
    {
        public override bool CheckSafety()
        {
            BoltActionRifle? BAR = thisFirearm as BoltActionRifle;

            if (BAR != null && BAR.HasFireSelectorButton)
            {
                if (BAR.FireSelector_Modes[BAR.m_fireSelectorMode].ModeType == BoltActionRifle.FireSelectorModeType.Safe)
                {
                    return true;
                }
            }
            return false;
        }
    }
}