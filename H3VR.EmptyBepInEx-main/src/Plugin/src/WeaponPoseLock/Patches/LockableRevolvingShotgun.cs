using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableRevolvingShotgun : LockableWeapon<RevolvingShotgun>
    {
        RevolvingShotgun RS;
        RevolvingShotgun.FireSelectorModeType fireMode;

        protected override void Awake()
        {
            base.Awake();
            RS = thisFirearm as RevolvingShotgun;
        }

        public override bool CanFire()
        {
            if (RS != null)
            {
                fireMode = RS.FireSelector_Modes[RS.m_fireSelectorMode].ModeType;

                //Safe
                if (fireMode == RevolvingShotgun.FireSelectorModeType.Safe) return false;

                //Single
                if (fireMode == RevolvingShotgun.FireSelectorModeType.Single && RS.m_hasTriggerCycled) return false;

                //Full auto
                if (fireMode == RevolvingShotgun.FireSelectorModeType.FullAuto && RS.m_hasTriggerCycled && (!RS.Chambers[RS.CurChamber].IsFull || RS.Chambers[RS.CurChamber].IsSpent)) return false;
            }
            return true;
        }
    }
}