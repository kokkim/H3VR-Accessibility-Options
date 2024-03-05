using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableLeverActionFirearm : LockableWeapon<LeverActionFirearm>
    {
        LeverActionFirearm LAF;

        protected override void Awake()
        {
            base.Awake();
            LAF = thisFirearm as LeverActionFirearm;
        }

        public override bool CanFire()
        {
            if (LAF != null)
            {
                //spinning, lever not locked back, hammer(s) not cocked
                if (LAF.m_isSpinning || LAF.m_curLeverPos != LeverActionFirearm.ZPos.Rear || !LAF.m_isHammerCocked || (LAF.UsesSecondChamber && !LAF.m_isHammerCocked2)) return false;
            }
            return true;
        }

        public override void LockWeapon()
        {
            base.LockWeapon();
            LAF.m_curLeverPos = LeverActionFirearm.ZPos.Forward;
            LAF.m_tarLeverRot = LAF.LeverAngleRange.x;
        }

        public override void UnlockWeapon()
        {
            base.UnlockWeapon();
            LAF.m_curLeverPos = LeverActionFirearm.ZPos.Rear;
            LAF.m_tarLeverRot = LAF.LeverAngleRange.y;
        }
    }
}