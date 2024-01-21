using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableOpenBoltReceiver : LockableWeapon<OpenBoltReceiver>
    {
        OpenBoltReceiver OBR;
        OpenBoltReceiver.FireSelectorModeType fireMode;

        protected override void Awake()
        {
            base.Awake();
            OBR = thisFirearm as OpenBoltReceiver;
        }

        public override bool CanFire()
        {
            if (OBR != null)
            {
                fireMode = OBR.FireSelector_Modes[OBR.m_fireSelectorMode].ModeType;

                //Safe
                if (fireMode == OpenBoltReceiver.FireSelectorModeType.Safe) return false;

                //Single
                if (fireMode == OpenBoltReceiver.FireSelectorModeType.Single && OBR.m_hasTriggerCycled) return false;

                //Burst & hyperburst
                if ((fireMode == OpenBoltReceiver.FireSelectorModeType.Burst && OBR.m_CamBurst == 0) || fireMode == OpenBoltReceiver.FireSelectorModeType.SuperFastBurst) return false;
            }
            return true;
        }

        public override bool IsBoltMoving()
        {
            if (OBR.Bolt.CurPos == OpenBoltReceiverBolt.BoltPos.Forward || OBR.Bolt.CurPos == OpenBoltReceiverBolt.BoltPos.Locked) return false;
            return true;
        }
    }
}