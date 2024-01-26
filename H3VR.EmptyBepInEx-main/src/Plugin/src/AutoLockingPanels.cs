using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace AccessibilityOptions
{
    class AutoLockingPanels : MonoBehaviour
    {
        string[] panelWhitelist = new string[0];

        public void Hook(string _panelWhitelist)
        {
            On.FistVR.FVRPhysicalObject.Awake += FVRPhysicalObject_Awake;

            //chops up the long string of panel class names and assigns them to an array
            panelWhitelist = _panelWhitelist.Split(' ');
            for (int i = 0; i < panelWhitelist.Length; i++)
            {
                panelWhitelist[i].Trim();
            }
        }

        private void FVRPhysicalObject_Awake(On.FistVR.FVRPhysicalObject.orig_Awake orig, FVRPhysicalObject self)
        {
            orig(self);

            foreach (string whitelistString in panelWhitelist)
            {
                //Get list of components from base FVRPhysicalObject
                Component[] components = self.GetComponents(typeof(Component));
                foreach (Component component in components)
                {
                    //Compare each component to each whitelist string
                    if (component.GetType().Name == whitelistString)
                    {
                        Rigidbody rb = self.GetComponent<Rigidbody>();
                        if (rb != null) self.SetIsKinematicLocked(true);
                        return;
                    }
                }
            }
        }
    }
}