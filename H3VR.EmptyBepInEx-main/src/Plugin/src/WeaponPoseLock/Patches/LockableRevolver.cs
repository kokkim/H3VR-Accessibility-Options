using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableRevolver : LockableWeapon<Revolver>
    {
        Revolver revolver;

        protected override void Awake()
        {
            base.Awake();
            revolver = thisFirearm as Revolver;
        }

        public override bool CanFire()
        {
            if (revolver != null)
            {
                //trigger not reset or cylinder arm open
                if (revolver.m_hasTriggerCycled || !revolver.isCylinderArmLocked) return false;
            }
            return true;
        }
    }
}