using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    class OneHandedGrenades : MonoBehaviour
    {
        bool hasTriggerReset = true;

        void Awake()
        {
            On.FistVR.PinnedGrenade.UpdateInteraction += PinnedGrenade_UpdateInteraction;
            On.FistVR.PinnedGrenade.IncreaseFuseSetting += PinnedGrenade_IncreaseFuseSetting;
        }

        private void PinnedGrenade_UpdateInteraction(On.FistVR.PinnedGrenade.orig_UpdateInteraction orig, PinnedGrenade self, FVRViveHand hand)
        {
            if (hand.Input.TriggerFloat < 0.1f) hasTriggerReset = true;

            //touchpad/button press pulls pin
            if ((hand.IsInStreamlinedMode && hand.Input.BYButtonDown) || (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown))
            {
                for (int i = 0; i < self.m_rings.Count; i++)
                {
                    if (!self.m_rings[i].HasPinDetached() && !self.m_rings[i].IsHeld)
                    {
                        RemoteDetachPin(self.m_rings[i]);
                        break;
                    }
                }
            }

            //trigger pull advances Cyber Grenade fuse setting
            if (hand.Input.TriggerFloat >= 0.8f && hasTriggerReset)
            {
                hasTriggerReset = false;
                if (self.FuseCylinder != null && !self.m_isPinPulled) OneHandedIncreaseFuseSetting(self);
            }
            orig(self, hand);
        }
        private void PinnedGrenade_IncreaseFuseSetting(On.FistVR.PinnedGrenade.orig_IncreaseFuseSetting orig, PinnedGrenade self)
        {
            //this is left deliberately empty to completely overwrite the original input method for it
        }

        private void OneHandedIncreaseFuseSetting(PinnedGrenade grenade)
        {
            if (grenade.m_fuseCylinderSetting < 4)
            {
                grenade.m_fuseCylinderSetting++;
            }
            else
            {
                grenade.m_fuseCylinderSetting = 0;
            }
            grenade.m_fuseTarYRotation = (float)grenade.m_fuseCylinderSetting * 24f - 48f;
        }

        private void RemoteDetachPin(PinnedGrenadeRing _ring)   //altered varsion of the DetachPin function in-game, separate to make manual pulls still possible
        {
            if (_ring.m_hasPinDetached)
            {
                return;
            }
            _ring.m_hasPinDetached = true;
            _ring.Pin.RootRigidbody = _ring.Pin.gameObject.AddComponent<Rigidbody>();
            _ring.Pin.RootRigidbody.mass = 0.02f;
            _ring.transform.SetParent(_ring.Pin.transform);
            _ring.Pin.enabled = true;
            SM.PlayCoreSound(FVRPooledAudioType.GenericClose, _ring.G.AudEvent_Pinpull, _ring.G.transform.position);
            _ring.GetComponent<Collider>().enabled = false;
            _ring.enabled = false;
        }
    }
}