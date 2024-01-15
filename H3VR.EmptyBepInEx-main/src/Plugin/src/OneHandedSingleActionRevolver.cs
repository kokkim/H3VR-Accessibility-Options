using System;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    class OneHandedSingleActionRevolver : MonoBehaviour
    {
        void Awake()
        {
            On.FistVR.FVRFireArmRound.Chamber += FVRFireArmRound_Chamber;
        }

        private void FVRFireArmRound_Chamber(On.FistVR.FVRFireArmRound.orig_Chamber orig, FVRFireArmRound self, FVRFireArmChamber c, bool makeChamberingSound)
        {
            orig(self, c, makeChamberingSound);
            if (c.Firearm is SingleActionRevolver SAR)
            {
                SAR.AdvanceCylinder();
                SAR.UpdateCylinderRot();
            }
        }

        void OnDestroy()
        {
            On.FistVR.FVRFireArmRound.Chamber -= FVRFireArmRound_Chamber;
        }
    }
}
