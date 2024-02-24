using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableDerringer : LockableWeapon<Derringer>
    {
        Derringer derringer;

        protected override void Awake()
        {
            base.Awake();
            derringer = thisFirearm as Derringer;
        }

        public override bool CanFire()
        {
            if (derringer != null)
            {
                if ((derringer.HasExternalHammer && !derringer.IsExternalHammerCocked()) || !derringer.m_hasTriggerReset || derringer.GetHingeState() == Derringer.HingeState.Open) return false;
            }
            return true;
        }
    }
}