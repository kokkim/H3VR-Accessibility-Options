using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableCarlGustaf : LockableWeapon<CarlGustaf>
    {
        CarlGustaf CG;

        protected override void Awake()
        {
            base.Awake();
            CG = thisFirearm as CarlGustaf;
        }

        public override bool CanFire()
        {
            if (CG != null)
            {
                //Single
                if (!CG.Chamber.IsFull || CG.Chamber.IsSpent || CG.CHState != CarlGustaf.ChargingHandleState.Forward || CG.TailLatch.LState != CarlGustafLatch.CGLatchState.Closed) return false;
            }
            return true;
        }
    }
}