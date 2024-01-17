using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    class WeaponPoseLock : MonoBehaviour
    {
        public static WeaponPoseLock instance;

        float triggerDuration;
        public LockableWeapon? currentlyLockedWeapon;

        public GameObject lockedWeaponProxy;

        public void Hook(float _triggerDuration)
        {
            instance = this;

            triggerDuration = _triggerDuration;

            On.FistVR.FVRPlayerBody.Start += FVRPlayerBody_Start;
            On.FistVR.FVRFireArm.Awake += FVRFireArm_Awake;
            On.FistVR.FVRFireArm.FVRUpdate += FVRFireArm_FVRUpdate;
            On.FistVR.FVRFireArm.BeginInteraction += FVRFireArm_BeginInteraction;
        }

        private void FVRPlayerBody_Start(On.FistVR.FVRPlayerBody.orig_Start orig, FVRPlayerBody self)
        {
            //assigns a new gameobject as lockedWeaponProxy if one doesn't exist yet
            orig(self);
            lockedWeaponProxy ??= Instantiate(new GameObject(), GM.CurrentPlayerBody.gameObject.transform);
        }

        private void FVRFireArm_Awake(On.FistVR.FVRFireArm.orig_Awake orig, FVRFireArm self)
        {
            orig(self);
            self.gameObject.AddComponent<LockableWeapon>().durationForPoseLock = triggerDuration;
        }

        //To check if the player grabs the currently locked weapon
        private void FVRFireArm_BeginInteraction(On.FistVR.FVRFireArm.orig_BeginInteraction orig, FVRFireArm self, FVRViveHand hand)
        {
            orig(self, hand);
            if (currentlyLockedWeapon != null)
            {
                LockableWeapon thisLockableWeapon = self.gameObject.GetComponent<LockableWeapon>();
                if (thisLockableWeapon == currentlyLockedWeapon)
                {
                    thisLockableWeapon.UnlockWeapon();
                }
            }
        }

        private void FVRFireArm_FVRUpdate(On.FistVR.FVRFireArm.orig_FVRUpdate orig, FVRFireArm self)
        {
            orig(self);
            if (self.m_hand != null)
            {
                if (self.m_hand?.Input.TriggerFloat >= 0.6f && !self.IsAltHeld)
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
