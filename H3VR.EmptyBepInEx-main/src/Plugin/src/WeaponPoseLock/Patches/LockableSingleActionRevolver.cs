using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableSingleActionRevolver : LockableWeapon<SingleActionRevolver>
    {
        SingleActionRevolver SAR;
        Handgun.FireSelectorModeType fireMode;

        protected override void Awake()
        {
            base.Awake();
            SAR = thisFirearm as SingleActionRevolver;
        }

        public override bool CanFire()
        {
            if (SAR != null)
            {
                //hammer down or loading gate open
                if (!SAR.m_isHammerCocked || SAR.m_isStateToggled) return false;
            }
            return true;
        }
    }
}