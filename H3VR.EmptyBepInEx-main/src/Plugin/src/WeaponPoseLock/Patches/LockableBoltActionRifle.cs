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

        public override bool CheckSafety()
        {
            if (BAR != null && BAR.HasFireSelectorButton)
            {
                if ((BAR.FireSelector_Modes.Length > 0 && BAR.FireSelector_Modes[BAR.m_fireSelectorMode].ModeType == BoltActionRifle.FireSelectorModeType.Safe)
                || (BAR.FireSelector_Modes_Secondary.Length > 0 && BAR.FireSelector_Modes_Secondary[BAR.m_fireSelectorMode].ModeType == BoltActionRifle.FireSelectorModeType.Safe))
                {
                    return true;
                }
            }
            return false;
        }
    }
}