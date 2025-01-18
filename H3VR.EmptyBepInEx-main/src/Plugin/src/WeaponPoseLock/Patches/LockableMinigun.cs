using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableMinigun : LockableWeapon<Minigun>
    {
        Minigun MG;

        protected override void Awake()
        {
            base.Awake();
            MG = thisFirearm as Minigun;
            skipChamberCheck = true;
        }

        public override bool CanFire()
        {
            if (MG != null)
            {
                MinigunBox minigunBox = (MinigunBox)MG.Magazine;
                if (minigunBox == null || (minigunBox != null && !minigunBox.HasAmmo()))
                {
                    return false;
                }
            }
            return true;
        }
    }
}