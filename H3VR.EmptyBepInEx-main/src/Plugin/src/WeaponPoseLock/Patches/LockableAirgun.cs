using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableAirgun : LockableWeapon<Airgun>
    {
        Airgun AG;

        protected override void Awake()
        {
            base.Awake();
            AG = thisFirearm as Airgun;
        }

        public override bool CanFire()
        {
            if (AG != null)
            {
                //Single
                if (!AG.Chamber.IsFull || AG.Abarrel.m_isBreachOpen || !AG.m_isHammerCocked) return false;
            }
            return true;
        }
    }
}