using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    public class LockableHandgun : LockableWeapon<Handgun>
    {
        Handgun HG;
        Handgun.FireSelectorModeType fireMode;

        protected override void Awake()
        {
            base.Awake();
            HG = thisFirearm as Handgun;
        }

        public override bool CanFire()
        {
            if (HG != null)
            {
                fireMode = HG.FireSelectorModes[HG.m_fireSelectorMode].ModeType;

                //Safe
                if (fireMode == Handgun.FireSelectorModeType.Safe) return false;

                //Single
                if (fireMode == Handgun.FireSelectorModeType.Single && !HG.HasTriggerReset) return false;

                //Burst & hyperburst
                if (fireMode == Handgun.FireSelectorModeType.Burst && HG.m_CamBurst == 0) return false;
            }
            return true;
        }

        public override bool IsBoltMoving()
        {
            if (HG.Slide.CurPos == HandgunSlide.SlidePos.Forward || HG.Slide.CurPos == HandgunSlide.SlidePos.Locked) return false;
            return true;
        }
    }
}