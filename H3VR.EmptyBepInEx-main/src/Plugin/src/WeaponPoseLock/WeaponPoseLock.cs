using System;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using FistVR;

namespace AccessibilityOptions
{
    class WeaponPoseLock : MonoBehaviour
    {
        public static WeaponPoseLock instance;

        public Dictionary<Type, Type> LockableWeaponDict = new();   //Contains references to every supported weapon type and its locking class

        public float triggerDuration;
        public LockableWeapon currentlyLockedWeapon;
        public FVRPhysicalObject.FVRPhysicalObjectSize lockedWeaponSize;

        public GameObject lockedWeaponProxy;

        void Awake()
        {
            instance = this;

            triggerDuration = AccessibilityOptionsBase.weaponPoseLockingTriggerDuration.Value;

            //-------------------------------------------------HOOKS

            On.FistVR.GM.InitScene += GM_InitScene;

            On.FistVR.FVRFireArm.Awake += FVRFireArm_Awake;
            On.FistVR.FVRFireArm.FVRUpdate += FVRFireArm_FVRUpdate;
            On.FistVR.FVRFireArm.BeginInteraction += FVRFireArm_BeginInteraction;

            On.FistVR.FVRQuickBeltSlot.MoveContentsInstant += FVRQuickBeltSlot_MoveContentsInstant;
            On.FistVR.FVRQuickBeltSlot.MoveContents += FVRQuickBeltSlot_MoveContents;
            On.FistVR.FVRQuickBeltSlot.MoveContentsCheap += FVRQuickBeltSlot_MoveContentsCheap;

            //-------------------------------------------------DICTIONARY ENTRIES

            #region Dictionary Entries
            //IEnumerable<LockableWeapon> lockableWeaponClasses = ReflectiveEnumerator.GetEnumerableOfType<LockableWeapon>();

            //I would have automated this, but I don't know how
            LockableWeaponDict[typeof(BoltActionRifle)] = typeof(LockableBoltActionRifle);
            LockableWeaponDict[typeof(TubeFedShotgun)] = typeof(LockableTubeFedShotgun);
            LockableWeaponDict[typeof(ClosedBoltWeapon)] = typeof(LockableClosedBoltWeapon);
            LockableWeaponDict[typeof(OpenBoltReceiver)] = typeof(LockableOpenBoltReceiver);
            LockableWeaponDict[typeof(Handgun)] = typeof(LockableHandgun);
            LockableWeaponDict[typeof(BAP)] = typeof(LockableBAP);
            LockableWeaponDict[typeof(Revolver)] = typeof(LockableRevolver);
            LockableWeaponDict[typeof(SingleActionRevolver)] = typeof(LockableSingleActionRevolver);
            LockableWeaponDict[typeof(Flaregun)] = typeof(LockableFlaregun);
            LockableWeaponDict[typeof(BreakActionWeapon)] = typeof(LockableBreakActionWeapon);
            LockableWeaponDict[typeof(Derringer)] = typeof(LockableDerringer);
            LockableWeaponDict[typeof(RevolvingShotgun)] = typeof(LockableRevolvingShotgun);
            LockableWeaponDict[typeof(RollingBlock)] = typeof(LockableRollingBlock);
            LockableWeaponDict[typeof(LeverActionFirearm)] = typeof(LockableLeverActionFirearm); //WIP, may need a cop-out solution
            LockableWeaponDict[typeof(RPG7)] = typeof(LockableRPG7);
            LockableWeaponDict[typeof(RailTater)] = typeof(LockableRailTater);
            LockableWeaponDict[typeof(CarlGustaf)] = typeof(LockableCarlGustaf);
            LockableWeaponDict[typeof(Airgun)] = typeof(LockableAirgun);
            LockableWeaponDict[typeof(SRG)] = typeof(LockableSRG);
            LockableWeaponDict[typeof(SimpleLauncher)] = typeof(LockableSimpleLauncher);
            LockableWeaponDict[typeof(SimpleLauncher2)] = typeof(LockableSimpleLauncher2);
            LockableWeaponDict[typeof(M72)] = typeof(LockableM72);
            LockableWeaponDict[typeof(Minigun)] = typeof(LockableMinigun);  //WIP, locks regardless of magazine state
            #endregion
        }

        private void GM_InitScene(On.FistVR.GM.orig_InitScene orig, GM self)
        {
            //assigns a new gameobject as lockedWeaponProxy if one doesn't exist yet
            orig(self);

            lockedWeaponProxy = Instantiate(new GameObject(), GM.CurrentPlayerBody.gameObject.transform);
            lockedWeaponProxy.name = "lockedWeaponProxy";

            //Refreshes the shot fired event to work in the new scene
            GM.CurrentSceneSettings.ShotFiredEvent -= OnShotFired;
            GM.CurrentSceneSettings.ShotFiredEvent += OnShotFired;
        }

        private void FVRFireArm_Awake(On.FistVR.FVRFireArm.orig_Awake orig, FVRFireArm self)
        {
            orig(self);

            //Check recursively for dictionary classes
            var derived = self.GetType();
            do {
                if (derived != null)
                {
                    LockableWeaponDict.TryGetValue(derived, out Type weaponLockType);
                    if (weaponLockType != null)
                    {
                        self.gameObject.AddComponent(weaponLockType);
                        break;
                    }
                    derived = derived.BaseType; //Gets the child class of derived
                }
            } while (derived != null);
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
                    ///New implementation:
                    ///Create a virtual function to LockableWeapon that is called from here
                    ///For each weapon that needs custom checks, make an override function that inherits from LockableWeapon
                    LockableWeapon curWep = self.GetComponent<LockableWeapon>();

                    if (curWep != null)
                    {
                        curWep.CheckChamberTriggerAmt(curWep.CanFire());
                    }
                }
            }
        }

        //Unlocks weapon upon shooting 
        void OnShotFired(FVRFireArm _firearm)
        {
            if (currentlyLockedWeapon != null && _firearm == currentlyLockedWeapon.thisFirearm && currentlyLockedWeapon.unlocksAfterFiring) currentlyLockedWeapon.UnlockWeapon();
        }

        #region MoveContents
        //To prevent the locked weapon from slotting back into a QB slot
        private void FVRQuickBeltSlot_MoveContentsInstant(On.FistVR.FVRQuickBeltSlot.orig_MoveContentsInstant orig, FVRQuickBeltSlot self, Vector3 dir)
        {
            if (!IsQBWeaponLocked(self)) orig(self, dir);
            //else Debug.Log("Blocked MovecontentsInstant");
        }

        private void FVRQuickBeltSlot_MoveContents(On.FistVR.FVRQuickBeltSlot.orig_MoveContents orig, FVRQuickBeltSlot self, Vector3 dir)
        {
            if (!IsQBWeaponLocked(self)) orig(self, dir);
            //else Debug.Log("Blocked Movecontents");
        }

        private void FVRQuickBeltSlot_MoveContentsCheap(On.FistVR.FVRQuickBeltSlot.orig_MoveContentsCheap orig, FVRQuickBeltSlot self, Vector3 dir)
        {
            if (!IsQBWeaponLocked(self)) orig(self, dir);
            //else Debug.Log("Blocked MovecontentsCheap");
        }

        bool IsQBWeaponLocked(FVRQuickBeltSlot slot)
        {
            if (slot.CurObject != null && slot.CurObject.GetComponent<LockableWeapon>() != null)
            {
                if (currentlyLockedWeapon == slot.GetComponent<LockableWeapon>())
                {
                    //Debug.Log("Blocked locked weapon from holstering");
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}