using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableBAP : LockableWeapon<BAP>
    {
        BAP BAP;

        protected override void Awake()
        {
            base.Awake();
            BAP = thisFirearm as BAP;
        }

        public override bool CanFire()
        {
            if (BAP != null)
            {
                //Safe
                if (BAP.FireSelector_Modes[BAP.m_fireSelectorMode].ModeType == BAP.FireSelectorModeType.Safe) return false;
            }
            return true;
        }
    }
}