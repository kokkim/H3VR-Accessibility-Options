using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    class GripAngleOverride : MonoBehaviour
    {
        float overrideGripAngle;

        public void Hook(float _overrideGripAngle)
        {
            overrideGripAngle = _overrideGripAngle;

            On.FistVR.FVRFireArm.Awake += FVRFireArm_Awake;
        }

        private void FVRFireArm_Awake(On.FistVR.FVRFireArm.orig_Awake orig, FVRFireArm self)
        {
            self.PoseOverride.transform.localRotation = Quaternion.Euler(new Vector3(overrideGripAngle, 0f, 0f));
            self.PoseOverride_Touch.transform.localRotation = Quaternion.Euler(new Vector3(overrideGripAngle, 0f, 0f));
            orig(self);
        }
    }
}
