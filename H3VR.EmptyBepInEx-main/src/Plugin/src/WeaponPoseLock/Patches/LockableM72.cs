using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableM72 : LockableWeapon<M72>
    {
        M72 m72;

        protected override void Awake()
        {
            base.Awake();
            m72 = thisFirearm as M72;
        }

        public override bool CanFire()
        {
            if (m72 != null)
            {
                //Check for full or spent?
                if (m72.m_isSafetyEngaged || m72.TState != M72.TubeState.Rear || !m72.Chamber.IsFull) return false;
            }
            return true;
        }
    }
}