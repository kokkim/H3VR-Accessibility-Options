using System;
using System.Collections;
using System.Text;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    class OneHandedPinnedGrenade : MonoBehaviour
    {
        float pinPullDuration;
        bool pinBeingPulled;
        float pinPullDistance;

        PinnedGrenadeRing? curRing;

        bool hasPinPullReset = true;

        void Awake()
        {
            pinPullDuration = AccessibilityOptionsBase.pinnedGrenadePinPullDuration.Value;
        }

        public void UpdateInteraction_Hooked(PinnedGrenade self, FVRViveHand hand)
        {
            bool pinPullButtonPressed = false, fuseSettingButtonPressed = false;

            //touchpad top/BY button down starts pin pull
            if (hand.IsInStreamlinedMode)
            {
                if (hand.Input.BYButtonDown) pinPullButtonPressed = true;
                //touchpad bottom/AX button advances fuse setting
                if (hand.Input.AXButtonDown) fuseSettingButtonPressed = true;
            }
            else
            {
                Vector2 touchpadAxes = hand.Input.TouchpadAxes;
                if (hand.Input.TouchpadDown && touchpadAxes.magnitude > 0.2f)
                {
                    if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f) pinPullButtonPressed = true;
                    if (Vector2.Angle(touchpadAxes, Vector2.down) <= 45f) fuseSettingButtonPressed = true;
                }
            }

            if (pinPullButtonPressed)
            {
                for (int i = 0; i < self.m_rings.Count; i++)
                {
                    if (!self.m_rings[i].HasPinDetached() && !self.m_rings[i].IsHeld && hasPinPullReset)
                    {
                        curRing = self.m_rings[i];
                        pinPullDistance = curRing.m_posMax - curRing.m_posMin;
                        pinBeingPulled = true;
                        break;
                    }
                }
            }

            //touchpad/AX button up stops pin pull
            if ((hand.IsInStreamlinedMode && hand.Input.BYButtonUp) || (!hand.IsInStreamlinedMode && hand.Input.TouchpadUp))
            {
                if (curRing != null)
                {
                    hasPinPullReset = true;
                    pinBeingPulled = false;
                    curRing = null;
                }
            }

            if (pinBeingPulled) RemoteUpdatePinPos();

            //trigger pull advances Cyber Grenade fuse setting
            if (fuseSettingButtonPressed)
            {
                if (self.FuseCylinder != null && !self.m_isPinPulled) OneHandedIncreaseFuseSetting(self);
            }
        }

        #region fuse length

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
        #endregion

        private void RemoteUpdatePinPos() //Pulls on the grenade pin for a set duration, and eventually pops out
        {
            if (curRing != null && !curRing.HasPinDetached())
            {
                if (curRing.UsesSwapPin && !curRing.m_hasSwapped)
                {
                    curRing.m_hasSwapped = true;
                    curRing.Pin.GetComponent<MeshFilter>().mesh = curRing.SwapPin;
                }

                if (pinPullDuration > 0f)
                {
                    curRing.m_posCurrent += pinPullDistance * (Time.deltaTime / pinPullDuration); //pulls out the pin at a set speed

                    curRing.transform.localPosition = new Vector3(curRing.transform.localPosition.x, curRing.transform.localPosition.y, curRing.m_posCurrent);
                    curRing.Pin.transform.localPosition = new Vector3(curRing.Pin.transform.localPosition.x, curRing.Pin.transform.localPosition.y, curRing.m_posCurrent);

                    if (curRing.m_posCurrent > curRing.m_posMax)
                    {
                        RemoteDetachPin(curRing);
                        return;
                    }
                }
                else
                {
                    RemoteDetachPin(curRing);
                    return;
                }
            }
        }

        private void RemoteDetachPin(PinnedGrenadeRing _ring)   //altered varsion of the DetachPin function in-game, separate to make manual pulls still possible
        {
            if (_ring.m_hasPinDetached)
            {
                return;
            }

            pinBeingPulled = false;
            hasPinPullReset = false;

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