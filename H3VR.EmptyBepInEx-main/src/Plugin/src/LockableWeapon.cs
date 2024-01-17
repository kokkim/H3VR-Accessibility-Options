using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    class LockableWeapon : MonoBehaviour
    {
        FVRFireArm thisFirearm;
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

        //this only runs when the trigger is already detected to be pulled in WeaponPoseLock
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

        void FixedUpdate()
        {
            switch (lockState)
            {
                case WeaponLockState.Unlocked:
                    {
                        curTriggerDuration = 0f;
                        if (isValidForPoseLock)
                        {
                            lockState = WeaponLockState.Locking;
                        }

                        break;
                    }
                case WeaponLockState.Locking:
                    {
                        if (!isValidForPoseLock)
                        {
                            lockState = WeaponLockState.Unlocked;
                            break;
                        }

                        curTriggerDuration += Time.fixedDeltaTime;
                        Debug.Log("curTriggerDuration increased to " + curTriggerDuration.ToString("F4"));
                        if (curTriggerDuration > durationForPoseLock)
                        {
                            lockState = WeaponLockState.Locked;
                        }

                        break;
                    }
                case WeaponLockState.Locked:
                    {
                        Debug.Log(thisFirearm.gameObject.name + " is pose locked!");

                        LockWeapon();
                        break;
                    }
            }
        }

        void LockWeapon()
        {
            //To prevent locking two weapons simultaneously
            if (WeaponPoseLock.instance.currentlyLockedWeapon == null)
            {
                WeaponPoseLock.instance.currentlyLockedWeapon = this;
                thisFirearm.SetIsKinematicLocked(true);
                thisFirearm.EndInteraction(thisFirearm.m_hand);

                WeaponPoseLock.instance.lockedWeaponProxy.transform.position = thisFirearm.transform.position;
                WeaponPoseLock.instance.lockedWeaponProxy.transform.rotation = thisFirearm.transform.rotation;
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
            WeaponPoseLock.instance.currentlyLockedWeapon = null;
            lockState = WeaponLockState.Unlocked;
            thisFirearm.SetIsKinematicLocked(false);
        }
    }
}