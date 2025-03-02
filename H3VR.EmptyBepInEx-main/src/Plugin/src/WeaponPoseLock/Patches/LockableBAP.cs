﻿using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableBAP : LockableWeapon<BAP>
    {
        BAP bap;

        protected override void Awake()
        {
            base.Awake();
            bap = thisFirearm as BAP;
        }

        public override bool CanFire()
        {
            if (bap != null)
            {
                //Safe
                if (bap.FireSelector_Modes[bap.m_fireSelectorMode].ModeType == BAP.FireSelectorModeType.Safe) return false;
            }
            return true;
        }
    }
}