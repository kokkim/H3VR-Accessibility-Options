using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    class WeaponPoseLock : MonoBehaviour
    {
        float triggerDuration;

        public void Hook(float _triggerDuration)
        {
            triggerDuration = _triggerDuration;

            On.FistVR.FVRFireArm.Awake += FVRFireArm_Awake;
            On.FistVR.FVRFireArm.FVRUpdate += FVRFireArm_FVRUpdate;
        }

        private void FVRFireArm_Awake(On.FistVR.FVRFireArm.orig_Awake orig, FVRFireArm self)
        {
            orig(self);
            self.gameObject.AddComponent<LockableWeapon>().durationForPoseLock = triggerDuration;
        }

        private void FVRFireArm_FVRUpdate(On.FistVR.FVRFireArm.orig_FVRUpdate orig, FVRFireArm self)
        {
            orig(self);
            if (self.m_hand != null)
            {
                if (!self.IsAltHeld)
                {
                    ///checks numerous weapon types if they have their safeties enabled
                    ///Afterwards checks their chambers and triggers in that weapon's LockableWeapon class
                    ///
                    bool isSafetyEnabled = false;
                    if (self is BoltActionRifle BAR)
                    {
                        if (BAR.HasFireSelectorButton)
                        {
                            if (BAR.FireSelector_Modes[BAR.m_fireSelectorMode].ModeType == BoltActionRifle.FireSelectorModeType.Safe)
                            {
                                isSafetyEnabled = true;
                            }
                        }
                    }
                    else if (self is TubeFedShotgun TFS)
                    {
                        if (TFS.HasSafety && TFS.IsSafetyEngaged)
                        {
                            isSafetyEnabled = true;
                        }
                    }
                    else
                    {
                        //if the current firearm is not any of the ones specified, it is excluded
                        return;
                    }

                    self.GetComponent<LockableWeapon>().CheckChamberTrigger(isSafetyEnabled);
                }
            }
        }
    }
}
