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
            self.gameObject.AddComponent<LockableWeapon>();
        }

        private void FVRFireArm_FVRUpdate(On.FistVR.FVRFireArm.orig_FVRUpdate orig, FVRFireArm self)
        {
            orig(self);
            if (self.m_hand.Input.TriggerFloat >= 0.8f)
            {
                if (self.IsHeld && !self.IsAltHeld)
                {
                    ///checks numerous weapon types if they have their safeties enabled
                    ///Afterwards checks their chambers and triggers in that weapon's LockableWeapon class
                    ///
                    if (self is BoltActionRifle BAR)
                    {

                    }
                    else if (self is TubeFedShotgun TFS)
                    {

                    }
                }
            }
        }
    }
}
