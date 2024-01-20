using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    public abstract class LockableWeapon<T> : LockableWeapon where T : MonoBehaviour
    {
        protected T Target { get; private set; }

        protected virtual void Awake()
        {
            Target = GetComponent<T>();     //To assign the correct weapon type to T
        }
    }

    public abstract class LockableWeapon : MonoBehaviour
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

        public abstract bool CheckSafety();

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
    }
}