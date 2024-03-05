using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    class GripAngleOverride : MonoBehaviour
    {
        float overrideGripAngle;

        void Awake()
        {
            overrideGripAngle = AccessibilityOptionsBase.overrideGripAngle.Value;

            On.FistVR.FVRFireArm.Awake += FVRFireArm_Awake;
        }

        private void FVRFireArm_Awake(On.FistVR.FVRFireArm.orig_Awake orig, FVRFireArm self)
        {
            if (self.PoseOverride != null) self.PoseOverride.transform.localRotation = Quaternion.Euler(new Vector3(overrideGripAngle, 0f, 0f));
            if (self.PoseOverride_Touch != null) self.PoseOverride_Touch.transform.localRotation = Quaternion.Euler(new Vector3(overrideGripAngle, 0f, 0f));
            orig(self);
        }
    }
}
