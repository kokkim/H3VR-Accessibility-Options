using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableClosedBoltWeapon : LockableWeapon<ClosedBoltWeapon>
    {
        ClosedBoltWeapon CBW;

        protected override void Awake()
        {
            base.Awake();
            CBW = thisFirearm as ClosedBoltWeapon;
        }

        public override bool CheckSafety()
        {
            if (CBW != null && CBW.HasFireSelectorButton)
            {
                if ((CBW.FireSelector_Modes.Length > 0 && CBW.FireSelector_Modes[CBW.m_fireSelectorMode].ModeType == ClosedBoltWeapon.FireSelectorModeType.Safe)
                || (CBW.FireSelector_Modes2.Length > 0 && CBW.FireSelector_Modes2[CBW.m_fireSelectorMode].ModeType == ClosedBoltWeapon.FireSelectorModeType.Safe))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool IsBoltMoving()
        {
            if (CBW.Bolt.CurPos == ClosedBolt.BoltPos.Forward || CBW.Bolt.CurPos == ClosedBolt.BoltPos.Locked) return false;
            return true;
        }
    }
}