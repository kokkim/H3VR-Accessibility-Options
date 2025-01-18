using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableSimpleLauncher2 : LockableWeapon<SimpleLauncher2>
    {
        SimpleLauncher2 SL2;

        protected override void Awake()
        {
            base.Awake();
            SL2 = thisFirearm as SimpleLauncher2;
            unlocksAfterFiring = false;
        }

        public override bool CanFire()
        {
            return false;   //Has no traditional trigger (only mortar trigger), so can always be locked
        }
    }
}