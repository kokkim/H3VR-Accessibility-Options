using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableRPG7 : LockableWeapon<RPG7>
    {
        RPG7 RPG;

        protected override void Awake()
        {
            base.Awake();
            RPG = thisFirearm as RPG7;
        }

        public override bool CanFire()
        {
            if (RPG != null)
            {
                //Single
                if (!RPG.m_isHammerCocked) return false;
            }
            return true;
        }
    }
}