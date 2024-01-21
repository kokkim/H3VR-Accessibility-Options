using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableClosedBoltWeapon : LockableWeapon<ClosedBoltWeapon>
    {
        ClosedBoltWeapon CBW;
        ClosedBoltWeapon.FireSelectorModeType fireMode;

        protected override void Awake()
        {
            base.Awake();
            CBW = thisFirearm as ClosedBoltWeapon;
        }

        public override bool CanFire()
        {
            if (CBW != null)
            {
                fireMode = CBW.FireSelector_Modes[CBW.m_fireSelectorMode].ModeType;

                //Safe
                if (fireMode == ClosedBoltWeapon.FireSelectorModeType.Safe) return false;

                //Single
                if (fireMode == ClosedBoltWeapon.FireSelectorModeType.Single && !CBW.m_hasTriggerReset) return false;

                //Burst & hyperburst
                if ((fireMode == ClosedBoltWeapon.FireSelectorModeType.Burst && CBW.m_CamBurst == 0) || fireMode == ClosedBoltWeapon.FireSelectorModeType.SuperFastBurst) return false;
            }
            return true;
        }

        public override bool IsBoltMoving()
        {
            if (CBW.Bolt.CurPos == ClosedBolt.BoltPos.Forward || CBW.Bolt.CurPos == ClosedBolt.BoltPos.Locked) return false;
            return true;
        }
    }
}