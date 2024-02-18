using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

/// <summary>
/// WIP!
/// </summary>

namespace AccessibilityOptions
{
    public class LockableBreakActionWeapon : LockableWeapon<BreakActionWeapon>
    {
        BreakActionWeapon BAW;

        protected override void Awake()
        {
            base.Awake();
            BAW = thisFirearm as BreakActionWeapon;
        }

        public override bool CanFire()
        {
            if (BAW != null)
            {
                if (!BAW.IsLatched || !BAW.HasTriggerReset) return false;
                /*if (BAW.UsesManuallyCockedHammers)
                {
                    for (int i = 0; i < BAW.Barrels.Length; i++)
                    {
                        BAW.curre
                        if (BAW.Barrels[i].m_isHammerCocked)
                    }
                    return false;
                }*/
            }
            return true;
        }
    }
}