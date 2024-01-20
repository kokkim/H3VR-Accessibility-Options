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

            On.FistVR.GM.InitScene += GM_InitScene;

            On.FistVR.FVRFireArm.Awake += FVRFireArm_Awake;
            On.FistVR.FVRFireArm.FVRUpdate += FVRFireArm_FVRUpdate;
            On.FistVR.FVRFireArm.BeginInteraction += FVRFireArm_BeginInteraction;

            On.FistVR.FVRQuickBeltSlot.MoveContentsInstant += FVRQuickBeltSlot_MoveContentsInstant;
            On.FistVR.FVRQuickBeltSlot.MoveContents += FVRQuickBeltSlot_MoveContents;
            On.FistVR.FVRQuickBeltSlot.MoveContentsCheap += FVRQuickBeltSlot_MoveContentsCheap;
        }

        private void GM_InitScene(On.FistVR.GM.orig_InitScene orig, GM self)
        {
            //assigns a new gameobject as lockedWeaponProxy if one doesn't exist yet
            orig(self);

            lockedWeaponProxy = Instantiate(new GameObject(), GM.CurrentPlayerBody.gameObject.transform);
            lockedWeaponProxy.name = "lockedWeaponProxy";
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

                if (thisLockableWeapon != null && thisLockableWeapon == currentlyLockedWeapon)
                {
                    if (thisLockableWeapon.thisFirearm.IsAltHeld && thisLockableWeapon.thisFirearm.m_hand.OtherHand.m_currentInteractable != thisLockableWeapon.thisFirearm)
                    {
                        return;
                    }
                    thisLockableWeapon.UnlockWeapon();
                }
            }
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

        #region MoveContents
        //To prevent the locked weapon from slotting back into a QB slot
        private void FVRQuickBeltSlot_MoveContentsInstant(On.FistVR.FVRQuickBeltSlot.orig_MoveContentsInstant orig, FVRQuickBeltSlot self, Vector3 dir)
        {
            if (!IsQBWeaponLocked(self)) orig(self, dir);
            else Debug.Log("Blocked MovecontentsInstant");
        }

        private void FVRQuickBeltSlot_MoveContents(On.FistVR.FVRQuickBeltSlot.orig_MoveContents orig, FVRQuickBeltSlot self, Vector3 dir)
        {
            if (!IsQBWeaponLocked(self)) orig(self, dir);
            else Debug.Log("Blocked Movecontents");
        }

        private void FVRQuickBeltSlot_MoveContentsCheap(On.FistVR.FVRQuickBeltSlot.orig_MoveContentsCheap orig, FVRQuickBeltSlot self, Vector3 dir)
        {
            if (!IsQBWeaponLocked(self)) orig(self, dir);
            else Debug.Log("Blocked MovecontentsCheap");
        }

        bool IsQBWeaponLocked(FVRQuickBeltSlot slot)
        {
            if (slot.CurObject != null && slot.CurObject.GetComponent<LockableWeapon>() != null)
            {
                if (currentlyLockedWeapon == slot.GetComponent<LockableWeapon>())
                {
                    Debug.Log("Blocked locked weapon from holstering");
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}