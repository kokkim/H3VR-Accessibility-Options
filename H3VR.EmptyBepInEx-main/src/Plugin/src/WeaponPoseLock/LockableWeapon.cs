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

            thisFirearm = GetComponent<FVRFireArm>();
            chambers = thisFirearm.GetChambers();
            durationForPoseLock = WeaponPoseLock.instance.triggerDuration;
        }

        void OnDestroy()
        {
            if (WeaponPoseLock.instance.currentlyLockedWeapon == this)
            {
                UnlockWeapon();
            }
        }
    }

    public abstract class LockableWeapon : MonoBehaviour
    {
        public FVRFireArm thisFirearm;
        protected List<FVRFireArmChamber> chambers;

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

        /// <summary>
        /// Returns true on one of two conditions:
        /// 1. If the trigger is held, has the weapon's trigger reset?
        /// 2. When the trigger is pulled, will the weapon's striker/firing pin get released?
        /// </summary>
        public abstract bool CanFire();

        public virtual bool IsBoltMoving()
        {
            //always returns false for single-fire weapons
            return false;
        }

        /// <summary>
        /// Checks trigger pull amount and the current chamber's load state
        /// If trigger is held for long enough, Update() locks it
        /// </summary>
        /// <param name="_isFiring"></param>
        public void CheckChamberTriggerAmt(bool _isFiring)
        {
            if (thisFirearm.m_hand?.Input.TriggerFloat >= 0.6f)
            {
                if (!_isFiring)
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
                            //Debug.Log("Unlocked -> Locking");
                        }

                        break;
                    }
                case WeaponLockState.Locking:
                    {
                        if (!isValidForPoseLock || thisFirearm.m_hand?.Input.TriggerFloat < 0.6f || IsBoltMoving())
                        {
                            lockState = WeaponLockState.Unlocked;
                            //Debug.Log("Locking -> Unlocked");
                            break;
                        }

                        curTriggerDuration += Time.deltaTime;
                        if (curTriggerDuration > durationForPoseLock)
                        {
                            lockState = WeaponLockState.Locked;
                            //Debug.Log("Locking -> Locked");
                        }

                        break;
                    }
                case WeaponLockState.Locked:
                    {
                        if ((WeaponPoseLock.instance.currentlyLockedWeapon != null && WeaponPoseLock.instance.currentlyLockedWeapon != this))
                        {
                            WeaponPoseLock.instance.currentlyLockedWeapon.UnlockWeapon();
                        }

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

        public virtual void LockWeapon()
        {
            thisFirearm.m_hand.Buzz(thisFirearm.m_hand.Buzzer.Buzz_BeginInteraction);

            WeaponPoseLock.instance.currentlyLockedWeapon = this;

            thisFirearm.IsKinematicLocked = true;
            thisFirearm.RootRigidbody.isKinematic = true;
            WeaponPoseLock.instance.lockedWeaponSize = thisFirearm.Size;
            thisFirearm.Size = FVRPhysicalObject.FVRPhysicalObjectSize.CantCarryBig;

            WeaponPoseLock.instance.lockedWeaponProxy.transform.position = thisFirearm.transform.position;
            WeaponPoseLock.instance.lockedWeaponProxy.transform.rotation = thisFirearm.transform.rotation;
        }

        public virtual void UnlockWeapon()
        {
            isValidForPoseLock = false;
            WeaponPoseLock.instance.currentlyLockedWeapon = null;
            lockState = WeaponLockState.Unlocked;

            thisFirearm.IsKinematicLocked = false;
            thisFirearm.RootRigidbody.isKinematic = false;
            thisFirearm.Size = WeaponPoseLock.instance.lockedWeaponSize;
            //Debug.Log("Locked -> Unlocked");
        }
    }
}