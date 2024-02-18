using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

/// <summary>
/// BROKEN ATM UNTIL ANTON FIXES Flaregun.FVRUpdate()!
/// </summary>

namespace AccessibilityOptions
{
    public class LockableFlaregun : LockableWeapon<Flaregun>
    {
        Flaregun FG;

        protected override void Awake()
        {
            base.Awake();
            FG = thisFirearm as Flaregun;
        }

        public override bool CanFire()
        {
            if (FG != null)
            {
                if (!FG.m_isHammerCocked) Debug.Log("a");
                if (!FG.m_isTriggerReset) Debug.Log("b");
                if (FG.m_hingeState == Flaregun.HingeState.Open) Debug.Log("c");

                if (!FG.m_isHammerCocked || !FG.m_isTriggerReset || FG.m_hingeState == Flaregun.HingeState.Open) return false;
            }
            return true;
        }
    }
}