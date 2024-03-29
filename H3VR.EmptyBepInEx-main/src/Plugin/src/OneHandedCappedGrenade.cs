﻿using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace AccessibilityOptions
{
    class OneHandedCappedGrenade : MonoBehaviour
    {
		FVRCappedGrenadeCap primaryCap, secondaryCap;

		void Awake()
        {
			FVRCappedGrenadeCap[] caps = GetComponentsInChildren<FVRCappedGrenadeCap>();

			for (int i = 0; i < caps.Length; i++)
			{
				if (i > 1)
                {
					Debug.LogError("Too many caps found!");
					break;
                }

				if (caps[i].IsPrimaryCap) primaryCap = caps[i];
				else secondaryCap = caps[i];
            }
        }

        public void FVRFixedUpdate_Hooked(FVRCappedGrenade _self)
        {
			bool doRemoveCap = false;
			FVRViveHand hand = _self.m_hand;

			if (hand.IsInStreamlinedMode)
            {
				if (hand.Input.BYButtonDown) doRemoveCap = true;
			}
            else
            {
				Vector2 touchpadAxes = hand.Input.TouchpadAxes;
				if (hand.Input.TouchpadDown && touchpadAxes.magnitude > 0.2f)
				{
					if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f) doRemoveCap = true;
				}
			}
            if (doRemoveCap)
			{
				if (!_self.IsPrimaryCapRemoved)
				{
					if (primaryCap != null) RemoteCapRemoved(true, _self, primaryCap);
				}
				else if (_self.UsesSecondaryCap && !_self.IsSecondaryCapRemoved)
				{
					if (secondaryCap != null) RemoteCapRemoved(false, _self, secondaryCap);
                }
            }
        }

		//Separate function to still allow for manual cap removal with two hands
        void RemoteCapRemoved(bool _isPrimary, FVRCappedGrenade _self, FVRCappedGrenadeCap _cap)
        {
			if (_isPrimary)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, _self.AudEvent_CapRemovePrimary, base.transform.position);
				_self.IsPrimaryCapRemoved = true;
				if (_self.AudOnUncapped != null) _self.AudOnUncapped.Play();
			}
			else
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, _self.AudEvent_CapRemoveSecondary, base.transform.position);
				_self.IsSecondaryCapRemoved = true;
			}
			if ((_self.UsesSecondaryCap && _self.IsPrimaryCapRemoved && _self.IsSecondaryCapRemoved) || (!_self.UsesSecondaryCap && _self.IsPrimaryCapRemoved))
			{
				_self.m_IsFuseActive = true;
			}

			GameObject original = _isPrimary ? _self.Cap_Primary_Prefab : _self.Cap_Secondary_Prefab;
			GameObject newCap = Instantiate(original, _cap.transform.position, _cap.transform.rotation);
			newCap.GetComponent<FVRPhysicalObject>().RecoverRigidbody();	//base game cap objects don't have rigidbodies by default

			Destroy(_cap.gameObject);
			if (_self.HasPopOutShell)
			{
				for (int i = 0; i < _self.ShellPieces.Count; i++)
				{
					_self.ShellPieces[i].localPosition = _self.ShellPoses[i];
				}
			}
		}
    }
}