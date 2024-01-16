using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    class LockableWeapon : MonoBehaviour
    {
        List<FVRFireArmChamber> chambers;

        void Awake()
        {
            chambers = GetComponent<FVRFireArm>().GetChambers();
        }

        //this only runs when the trigger is already detected to be pulled in WeaponPoseLock
        void CheckChamberTrigger(FVRFireArm self)
        {
            foreach(FVRFireArmChamber chamber in chambers)
            {
                if (!chamber.IsFull || (chamber.IsFull && chamber.IsSpent))
                {
                    Debug.Log("Valid for locking!");
                }
            }
        }
    }
}
