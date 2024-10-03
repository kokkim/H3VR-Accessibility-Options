using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableRailTater : LockableWeapon<RailTater>
    {
        RailTater RT;

        protected override void Awake()
        {
            base.Awake();
            RT = thisFirearm as RailTater;
        }

        public override bool CanFire()
        {
            if (RT != null)
            {
                //Single
                if (!RT.Chamber.IsFull || RT.BoltHandle.HandleState != RailTater_Handle.RailTaterHandleState.Forward || RT.BoltHandle.HandleRot != RailTater_Handle.RailTaterHandleRot.Down) return false;
            }
            return true;
        }
    }
}