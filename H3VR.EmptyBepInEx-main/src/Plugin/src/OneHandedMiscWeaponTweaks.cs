using System;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    class OneHandedMiscWeaponTweaks : MonoBehaviour
    {
        void Awake()
        {
            if (AccessibilityOptionsBase.oneHandedSAREnabled.Value)
            {
                On.FistVR.FVRFireArmRound.Chamber += FVRFireArmRound_Chamber;
            }

            if (AccessibilityOptionsBase.oneHandedGrenadesEnabled.Value)
            {
                On.FistVR.PinnedGrenade.Awake += PinnedGrenade_Awake;
                On.FistVR.PinnedGrenade.UpdateInteraction += PinnedGrenade_UpdateInteraction;
                On.FistVR.PinnedGrenade.ReleaseLever += PinnedGrenade_ReleaseLever;
                On.FistVR.PinnedGrenade.IncreaseFuseSetting += PinnedGrenade_IncreaseFuseSetting;

                On.FistVR.FVRCappedGrenade.Start += FVRCappedGrenade_Start;
                On.FistVR.FVRCappedGrenade.FVRFixedUpdate += FVRCappedGrenade_FVRFixedUpdate;
            }

            if (AccessibilityOptionsBase.oneHandedPumpReleaseEnabled.Value)
            {
                On.FistVR.TubeFedShotgunHandle.UpdateHandle += TubeFedShotgunHandle_UpdateHandle;
                On.FistVR.ClosedBoltForeHandle.UpdateInteraction += ClosedBoltForeHandle_UpdateInteraction;
            }

            if (AccessibilityOptionsBase.RPG7MainHandControlsEnabled.Value)
            {
                On.FistVR.RPG7.UpdateInteraction += RPG7_UpdateInteraction;
                On.FistVR.FVRAlternateGrip.BeginInteraction += FVRAlternateGrip_BeginInteraction;
            }
        }

        private void ClosedBoltForeHandle_UpdateInteraction(On.FistVR.ClosedBoltForeHandle.orig_UpdateInteraction orig, ClosedBoltForeHandle self, FVRViveHand hand)
        {
            Vector2 touchpadAxes = hand.Input.TouchpadAxes;
            if (self.Weapon.IsAltHeld)
            {
                bool canUnlock = false;
                if (hand.IsInStreamlinedMode)
                {
                    if (hand.Input.BYButtonDown)
                    {
                        canUnlock = true;
                    }
                }
                else if (hand.Input.TouchpadDown && Vector2.Angle(Vector2.up, touchpadAxes) < 45f && touchpadAxes.magnitude > 0.2f)
                {
                    canUnlock = true;
                }

                if (canUnlock && self.CanSwap)
                {
                    self.AttemptToToggleMode();
                }
            }
        }

        void OnDestroy()
        {
            On.FistVR.FVRFireArmRound.Chamber -= FVRFireArmRound_Chamber;

            On.FistVR.PinnedGrenade.Awake -= PinnedGrenade_Awake;
            On.FistVR.PinnedGrenade.UpdateInteraction -= PinnedGrenade_UpdateInteraction;
            On.FistVR.PinnedGrenade.ReleaseLever -= PinnedGrenade_ReleaseLever;
            On.FistVR.PinnedGrenade.IncreaseFuseSetting -= PinnedGrenade_IncreaseFuseSetting;

            On.FistVR.FVRCappedGrenade.Start -= FVRCappedGrenade_Start;
            On.FistVR.FVRCappedGrenade.FVRFixedUpdate -= FVRCappedGrenade_FVRFixedUpdate;

            On.FistVR.TubeFedShotgunHandle.UpdateHandle -= TubeFedShotgunHandle_UpdateHandle;
            On.FistVR.ClosedBoltForeHandle.UpdateInteraction -= ClosedBoltForeHandle_UpdateInteraction;

            On.FistVR.RPG7.UpdateInteraction -= RPG7_UpdateInteraction;
            On.FistVR.FVRAlternateGrip.BeginInteraction -= FVRAlternateGrip_BeginInteraction;
        }

        #region single-action revolvers
        private void FVRFireArmRound_Chamber(On.FistVR.FVRFireArmRound.orig_Chamber orig, FVRFireArmRound self, FVRFireArmChamber c, bool makeChamberingSound)
        {
            orig(self, c, makeChamberingSound);
            if (c.Firearm is SingleActionRevolver SAR)
            {
                SAR.AdvanceCylinder();
                SAR.UpdateCylinderRot();
            }
        }
        #endregion

        #region pinned grenades
        private void PinnedGrenade_Awake(On.FistVR.PinnedGrenade.orig_Awake orig, PinnedGrenade self)
        {
            self.gameObject.AddComponent<OneHandedPinnedGrenade>();
            orig(self);
        }
        private void PinnedGrenade_UpdateInteraction(On.FistVR.PinnedGrenade.orig_UpdateInteraction orig, PinnedGrenade self, FVRViveHand hand)
        {
            self.GetComponent<OneHandedPinnedGrenade>().UpdateInteraction_Hooked(self, hand);
            orig(self, hand);
        }
        private void PinnedGrenade_ReleaseLever(On.FistVR.PinnedGrenade.orig_ReleaseLever orig, PinnedGrenade self)
        {
            if (self.IsHeld)
            {
                FVRViveHand hand = self.m_hand;
                if (!hand.IsInStreamlinedMode)
                {
                    Vector2 touchpadAxes = hand.Input.TouchpadAxes;
                    if (hand.Input.TouchpadDown)
                    {
                        if (touchpadAxes.magnitude < 0.2f || Vector2.Angle(touchpadAxes, Vector2.up) > 45f)
                        {
                            return;
                        }
                    }
                }
            }
            orig(self);
        }
        private void PinnedGrenade_IncreaseFuseSetting(On.FistVR.PinnedGrenade.orig_IncreaseFuseSetting orig, PinnedGrenade self)
        {
            //this is left deliberately empty to completely overwrite the original input method for it
        }
        #endregion

        #region capped grenades
        private void FVRCappedGrenade_Start(On.FistVR.FVRCappedGrenade.orig_Start orig, FVRCappedGrenade self)
        {
            orig(self);
            self.gameObject.AddComponent<OneHandedCappedGrenade>();
        }
        private void FVRCappedGrenade_FVRFixedUpdate(On.FistVR.FVRCappedGrenade.orig_FVRFixedUpdate orig, FVRCappedGrenade self)
        {
            if (self.IsHeld && !self.m_IsFuseActive)
            {
                self.GetComponent<OneHandedCappedGrenade>().FVRFixedUpdate_Hooked(self);
            }
            orig(self);
        }
        #endregion

        #region pump-actions
        #region TubeFedShotgun
        private void TubeFedShotgunHandle_UpdateHandle(On.FistVR.TubeFedShotgunHandle.orig_UpdateHandle orig, TubeFedShotgunHandle self)
        {
            //still can't detect that a hand is holding a foregrip
            //Check if the weapon's IsHeld isn't active?
            if (WeaponPoseLock.instance.currentlyLockedWeapon != null)
            {
                if (self.Shotgun.IsAltHeld && WeaponPoseLock.instance.currentlyLockedWeapon.thisFirearm == self.Shotgun)
                {
                    foreach (FVRViveHand hand in GM.CurrentMovementManager.Hands)
                    {
                        if (hand.CurrentInteractable == self.Shotgun && hand.OtherHand.CurrentInteractable != self.Shotgun)
                        {
                            if (hand.Input.TriggerFloat >= 0.6f)
                            {
                                self.UnlockHandle();
                            }
                            break;
                        }
                    }
                }
            }
            orig(self);
        }
        #endregion
        #region ClosedBoltWeapon & ClosedBoltForeHandle

        #endregion
        #endregion

        #region RPG7
        private void RPG7_UpdateInteraction(On.FistVR.RPG7.orig_UpdateInteraction orig, RPG7 self, FVRViveHand hand)
        {
            orig(self, hand);

            //Copied from RPG7Foregrip
            bool doHammerCock = false;
            if ((hand.IsInStreamlinedMode && hand.Input.AXButtonDown) || (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown))
            {
                doHammerCock = true;
            }
            if (doHammerCock) self.CockHammer();

            if (hand.Input.TriggerDown) self.Fire();
            self.UpdateTriggerRot(hand.Input.TriggerFloat);
        }

        private void FVRAlternateGrip_BeginInteraction(On.FistVR.FVRAlternateGrip.orig_BeginInteraction orig, FVRAlternateGrip self, FVRViveHand hand)
        {
            if (self is RPG7Foregrip)
            {
                RPG7Foregrip? grip = self as RPG7Foregrip;
                if (grip != null && !grip.RPG.IsHeld)
                {
                    grip.RPG.BeginInteraction(hand);
                    return;
                }
            }
            orig(self, hand);
            return;
        }
        #endregion
    }
}
