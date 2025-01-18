using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableSimpleLauncher : LockableWeapon<SimpleLauncher>
    {
        SimpleLauncher SL;

        protected override void Awake()
        {
            base.Awake();
            SL = thisFirearm as SimpleLauncher;
            unlocksAfterFiring = false;     //to make the whizzbanger not fall to the ground after firing
        }

        public override bool CanFire()
        {
            return false;   //Has no trigger, so can always be locked
        }
    }
}