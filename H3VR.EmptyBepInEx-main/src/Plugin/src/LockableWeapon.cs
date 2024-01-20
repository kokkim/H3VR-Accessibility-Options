using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

/// <summary>
/// PROBABLY NEEDS COMPLETE REWRITE
/// New idea: use quickbelt slots?
/// - Can still operate bolts and slides
/// - Can still load magazines
/// When player triggers the lock, script-wise the player lets go, but the quickbelted weapon's pos is overwritten to the let-go position relative to the player
/// What if there is no quickbelt slot to use?
/// - Make a new quickbelt slot just for this purpose
/// - Use a custom method for storing and moving the weapon
///     - Probably better, causes fewer edgecase issues with scene saving/loading
///     - Still hook onto quickbelt code to move the weapon?
/// </summary>


namespace AccessibilityOptions
{
    class LockableWeapon : MonoBehaviour
    {
        public FVRFireArm thisFirearm;
        List<FVRFireArmChamber> chambers;

        public float durationForPoseLock;

        float curTriggerDuration;
        bool isValidForPoseLock;

        enum WeaponLockState
        {
            Unlocked,
            Locking,
            Locked
        };

        WeaponLockState lockState;

        void Awake()
        {
            chambers = GetComponent<FVRFireArm>().GetChambers();
            thisFirearm = GetComponent<FVRFireArm>();
        }

        void OnDestroy()
        {
            if (WeaponPoseLock.instance.currentlyLockedWeapon == this)
            {
                UnlockWeapon();
            }
        }

        public void CheckChamberTrigger(bool isSafetyEngaged)
        {
            if (thisFirearm.m_hand?.Input.TriggerFloat >= 0.6f)
            {
                if (isSafetyEngaged)
                {
                    isValidForPoseLock = true;
                    return;
                }
                else
                {
                    bool chamberSafe = true;
                    foreach (FVRFireArmChamber chamber in chambers)
                    {
                        if (chamber.IsFull && !chamber.IsSpent)
                        {
                            chamberSafe = false;
                            break;
                        }
                    }
                    if (chamberSafe)
                    {
                        isValidForPoseLock = true;
                        return;
                    }
                }
            }

            isValidForPoseLock = false;
        }

        void Update()
        {
            switch (lockState)
            {
                case WeaponLockState.Unlocked:
                    {
                        curTriggerDuration = 0f;
                        if (isValidForPoseLock)
                        {
                            lockState = WeaponLockState.Locking;
                            Debug.Log("Unlocked -> Locking");
                        }

                        break;
                    }
                case WeaponLockState.Locking:
                    {
                        if (!isValidForPoseLock && thisFirearm.m_hand?.Input.TriggerFloat < 0.6f)
                        {
                            lockState = WeaponLockState.Unlocked;
                            Debug.Log("Locking -> Unlocked");
                            break;
                        }

                        curTriggerDuration += Time.deltaTime;
                        if (curTriggerDuration > durationForPoseLock)
                        {
                            lockState = WeaponLockState.Locked;
                            Debug.Log("Locking -> Locked");
                        }

                        break;
                    }
                case WeaponLockState.Locked:
                    {
                        if (WeaponPoseLock.instance.currentlyLockedWeapon == null) LockWeapon();
                        else
                        {
                            if (!thisFirearm.RootRigidbody.isKinematic) thisFirearm.RootRigidbody.isKinematic = true;
                            if (thisFirearm.transform.parent != null) thisFirearm.transform.SetParent(null);

                            thisFirearm.gameObject.transform.position = WeaponPoseLock.instance.lockedWeaponProxy.transform.position;
                            thisFirearm.gameObject.transform.rotation = WeaponPoseLock.instance.lockedWeaponProxy.transform.rotation;
                        }
                        break;
                    }
            }
        }

        //old implementation
        void LockWeapon()
        {
            //To prevent locking two weapons simultaneously
            if (WeaponPoseLock.instance.currentlyLockedWeapon == null)
            {
                WeaponPoseLock.instance.currentlyLockedWeapon = this;

                thisFirearm.IsPivotLocked = true;       //Unnecessary?
                thisFirearm.RootRigidbody.isKinematic = true;

                WeaponPoseLock.instance.lockedWeaponProxy.transform.position = thisFirearm.transform.position;
                WeaponPoseLock.instance.lockedWeaponProxy.transform.rotation = thisFirearm.transform.rotation;
            }
        }

        public void UnlockWeapon()
        {
            isValidForPoseLock = false;
            WeaponPoseLock.instance.currentlyLockedWeapon = null;
            lockState = WeaponLockState.Unlocked;
            Debug.Log("Locked -> Unlocked");
            thisFirearm.IsPivotLocked = false;      //Unnecessary?
            thisFirearm.IsKinematicLocked = false;
        }

        //old implementation
        /*void LockWeapon()
        {
            //To prevent locking two weapons simultaneously
            if (WeaponPoseLock.instance.currentlyLockedWeapon == null)
            {
                WeaponPoseLock.instance.currentlyLockedWeapon = this;
                thisFirearm.SetIsKinematicLocked(true);
                thisFirearm.EndInteraction(thisFirearm.m_hand);

                if (WeaponPoseLock.instance.lockedWeaponProxy == null)
                {
                    Debug.LogError("lockedWeaponProxy is not set up!");
                    return;
                }
                WeaponPoseLock.instance.lockedWeaponProxy.transform.position = thisFirearm.transform.position;
                WeaponPoseLock.instance.lockedWeaponProxy.transform.rotation = thisFirearm.transform.rotation;
                thisFirearm.SetParentage(WeaponPoseLock.instance.lockedWeaponProxy.transform);
            }

            if (WeaponPoseLock.instance.currentlyLockedWeapon == this)
            {
                //Weapon locking code here
                thisFirearm.gameObject.transform.position = WeaponPoseLock.instance.lockedWeaponProxy.transform.position;
                thisFirearm.gameObject.transform.rotation = WeaponPoseLock.instance.lockedWeaponProxy.transform.rotation;
            }
        }

        public void UnlockWeapon()
        {
            isValidForPoseLock = false;
            WeaponPoseLock.instance.currentlyLockedWeapon = null;
            lockState = WeaponLockState.Unlocked;
            Debug.Log("Locked -> Unlocked");
            thisFirearm.SetIsKinematicLocked(false);

            thisFirearm.SetParentage(null);
        }*/
    }
}