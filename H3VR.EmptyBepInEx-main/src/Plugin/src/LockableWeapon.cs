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
        bool isValidForPoseLock, poseLockEnabled;

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
                    Debug.Log("Safety is engaged, valid for locking!");
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
                        Debug.Log("Safety not engaged but chamber safe, valid for locking!");
                        isValidForPoseLock = true;
                        return;
                    }
                }
            }

            Debug.Log("No safety or safety off, un-pulled trigger or loaded chamber!");
            isValidForPoseLock = false;
        }

        void Update()
        {
            if (isValidForPoseLock)
            {
                if (!poseLockEnabled)
                {
                    curTriggerDuration += Time.deltaTime;
                    Debug.Log("curTriggerDuration increased to " + curTriggerDuration.ToString("F4"));

                if (curTriggerDuration > durationForPoseLock)
                    {
                        poseLockEnabled = true;
                    }
                }
            }
            else
            {
                poseLockEnabled = false;
                curTriggerDuration = 0f;
            }

            if (poseLockEnabled)
            {
                Debug.Log(thisFirearm.gameObject.name + " is pose locked!");
            }
        }
    }
}