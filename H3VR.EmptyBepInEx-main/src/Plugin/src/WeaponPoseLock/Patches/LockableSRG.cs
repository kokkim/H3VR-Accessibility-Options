using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableSRG : LockableWeapon<SRG>
    {
        SRG srg;

        protected override void Awake()
        {
            base.Awake();
            srg = thisFirearm as SRG;
        }

        public override bool CanFire()
        {
            if (srg != null)
            {
                //Single
                if (srg.Magazine == null || !srg.Chamber.IsFull || !srg.m_canFire) return false;
            }
            return true;
        }
    }
}