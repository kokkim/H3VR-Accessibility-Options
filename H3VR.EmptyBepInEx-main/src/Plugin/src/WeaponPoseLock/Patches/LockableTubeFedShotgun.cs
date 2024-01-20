using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableTubeFedShotgun : LockableWeapon<TubeFedShotgun>
    {
        public override bool CheckSafety()
        {
            TubeFedShotgun? TFS = thisFirearm as TubeFedShotgun;

            if (TFS != null && TFS.HasSafety && TFS.IsSafetyEngaged)
            {
                return true;
            }
            return false;
        }
    }
}