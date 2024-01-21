using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableOpenBoltReceiver : LockableWeapon<OpenBoltReceiver>
    {
        OpenBoltReceiver OBR;

        protected override void Awake()
        {
            base.Awake();
            OBR = thisFirearm as OpenBoltReceiver;
        }

        public override bool CheckSafety()
        {
            if (OBR != null && OBR.HasFireSelectorButton)
            {
                if ((OBR.FireSelector_Modes.Length > 0 && OBR.FireSelector_Modes[OBR.m_fireSelectorMode].ModeType == OpenBoltReceiver.FireSelectorModeType.Safe)
                || (OBR.FireSelector_Modes2.Length > 0 && OBR.FireSelector_Modes2[OBR.m_fireSelectorMode].ModeType == OpenBoltReceiver.FireSelectorModeType.Safe))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool IsBoltMoving()
        {
            if (OBR.Bolt.CurPos == OpenBoltReceiverBolt.BoltPos.Forward || OBR.Bolt.CurPos == OpenBoltReceiverBolt.BoltPos.Locked) return false;
            return true;
        }
    }
}