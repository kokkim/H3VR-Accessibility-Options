using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableTubeFedShotgun : LockableWeapon<TubeFedShotgun>
    {
        TubeFedShotgun? TFS;

        protected override void Awake()
        {
            base.Awake();
            TFS = thisFirearm as TubeFedShotgun;
        }

        public override bool CanFire()
        {
            if (TFS != null && TFS.HasSafety && TFS.IsSafetyEngaged) return false;
            return true;
        }

        public override bool IsBoltMoving()
        {
            if (TFS.Mode == TubeFedShotgun.ShotgunMode.PumpMode) return false;   //manual actions don't need bolt pos check to lock
            if (TFS.Bolt.CurPos == TubeFedShotgunBolt.BoltPos.Forward || TFS.Bolt.CurPos == TubeFedShotgunBolt.BoltPos.Locked) return false;
            return true;
        }
    }
}