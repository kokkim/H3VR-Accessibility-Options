using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace AccessibilityOptions
{
	/// <summary>
	/// TODO:
	/// - Unlock weapon upon detecting a gunshot
	///		- Use the OnShotFired event
	/// </summary>

	[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
	[BepInProcess("h3vr.exe")]
	public class AccessibilityOptionsBase : BaseUnityPlugin
	{
		//Wrist menu
		public static ConfigEntry<bool> oneHandedWristMenuEnabled;
		public static ConfigEntry<float> verticalPointerOffset;
		public static ConfigEntry<Color> pointerColor;
		public static ConfigEntry<float> pointerScale;

		//Option panels
		public static ConfigEntry<bool> lockPanelsAutomatically;
		public static ConfigEntry<string> autoLockPanelWhitelist;

		//Weapon pose locking
		public static ConfigEntry<bool> weaponPoseLockingEnabled;
		public static ConfigEntry<float> weaponPoseLockingTriggerDuration;

		//Individual weapon tweaks
		public static ConfigEntry<bool> oneHandedSAREnabled;

		public static ConfigEntry<bool> oneHandedGrenadesEnabled;
		public static ConfigEntry<float> pinnedGrenadePinPullDuration;

		private const string ASSET_BUNDLE_NAME = "accessibilityoptions";
		AssetBundle bundle;

		public AccessibilityOptionsBase()
		{
			string pluginPath = Path.GetDirectoryName(Info.Location);
			bundle = AssetBundle.LoadFromFile(Path.Combine(pluginPath, ASSET_BUNDLE_NAME));

			//WRIST MENU CONFIG-------------------------------------------------------------------------------------
			oneHandedWristMenuEnabled = Config.Bind("Wrist Menu",							//SO FAR UNUSED
													"Enable One-Handed Wrist Menu",
													true,
													"Enabled/disables one-handed wrist menu (requires game restart)");

			verticalPointerOffset = Config.Bind("Wrist Menu",
												"Vertical Pointer Offset",
												-10f,
												"How far up or down (in degrees) the wrist menu pointer is from the center of your view");

			pointerColor = Config.Bind("Wrist Menu",
									   "Pointer Color",
									   Color.green,
									   "Color of the wrist menu pointer (RGBA)");

			pointerScale = Config.Bind("Wrist Menu",
									   "Pointer Scale",
									   0.01f,
									   "How large (in meters) the pointer is");

			//OPTION PANEL CONFIG
			lockPanelsAutomatically = Config.Bind("Option Panels",
												  "Automatically Lock Option Panels",
												  true,
												  "Enables/disables option panels locking automatically when spawned");

			autoLockPanelWhitelist = Config.Bind("Option Panels",
												 "(Advanced) Auto-Locking Panel Whitelist",
												 "OptionsPanel_Screenmanager AmmoSpawnerV2",
												 "Add a panel class name here (case-sensitive, separated by a space) to lock it automatically upon spawning");

			//WEAPON POSE LOCKING CONFIG-------------------------------------------------------------------------------------
			weaponPoseLockingEnabled = Config.Bind("Weapon Pose Locking",
												   "Enable Weapon Pose Locking",
												   true,
												   "Enables/disables locking weapons in mid-air by holding down the trigger on a safe or empty chamber");

			weaponPoseLockingTriggerDuration = Config.Bind("Weapon Pose Locking",
														 "Pose Locking Trigger Hold Duration",
														 0.5f,
														 "How long the trigger needs to be held down for the weapon to get locked");

			//SINGLE-ACTION REVOLVER CONFIG-------------------------------------------------------------------------------------
			oneHandedSAREnabled = Config.Bind("Single-Action Revolvers",
											  "Enable One-Handed Single-Action Revolvers",
											  true,
											  "Enables/disables the cylinder automatically advancing upon inserting a round");

			//GRENADE CONFIG-------------------------------------------------------------------------------------
			oneHandedGrenadesEnabled = Config.Bind("Grenades",
												   "Enable One-Handed Grenades",
												   true,
												   "Enables/disables pulling pins by pressing the touchpad and AX face buttons");

			pinnedGrenadePinPullDuration = Config.Bind("Grenades",
												 "Pinned Grenade Pin Pull Duration",
												 0.5f,
												 "How long (in seconds) the button needs to be held down to pull out a grenade pin (set to 0 for instant)");
		}

		OneHandedWristMenu oneHandedWristMenu;
		AutoLockingPanels autoLockingPanels;
		WeaponPoseLock weaponPoseLock;
		OneHandedSingleActionRevolver oneHandedSingleActionRevolver;

		void Awake()
        {
			//Wrist menu
			if (oneHandedWristMenuEnabled.Value)
			{
				oneHandedWristMenu = gameObject.AddComponent<OneHandedWristMenu>();
				oneHandedWristMenu.Hook(verticalPointerOffset.Value, pointerColor.Value, pointerScale.Value, bundle);
			}

			//Option panels
			if (lockPanelsAutomatically.Value)
            {
				autoLockingPanels = gameObject.AddComponent<AutoLockingPanels>();
				autoLockingPanels.Hook(autoLockPanelWhitelist.Value);
            }

			//Weapon pose locking
			if (weaponPoseLockingEnabled.Value)
            {
				weaponPoseLock = gameObject.AddComponent<WeaponPoseLock>();
				weaponPoseLock.Hook(weaponPoseLockingTriggerDuration.Value);
            }

			//Individual weapon tweaks
			if (oneHandedSAREnabled.Value)
			{
				oneHandedSingleActionRevolver = gameObject.AddComponent<OneHandedSingleActionRevolver>();
			}
			if (oneHandedGrenadesEnabled.Value)
            {
                On.FistVR.PinnedGrenade.Awake += PinnedGrenade_Awake;
                On.FistVR.PinnedGrenade.UpdateInteraction += PinnedGrenade_UpdateInteraction;
                On.FistVR.PinnedGrenade.IncreaseFuseSetting += PinnedGrenade_IncreaseFuseSetting;

                On.FistVR.FVRCappedGrenade.Start += FVRCappedGrenade_Start;
                On.FistVR.FVRCappedGrenade.FVRFixedUpdate += FVRCappedGrenade_FVRFixedUpdate;
			}

			//For debugging, remove before build
			//On.FistVR.AudioImpactController.ProcessCollision += AudioImpactController_ProcessCollision;
		}

		#region pinned grenades
		private void PinnedGrenade_Awake(On.FistVR.PinnedGrenade.orig_Awake orig, PinnedGrenade self)
		{
			self.gameObject.AddComponent<OneHandedPinnedGrenade>().Hook(pinnedGrenadePinPullDuration.Value);
			orig(self);
		}
		private void PinnedGrenade_UpdateInteraction(On.FistVR.PinnedGrenade.orig_UpdateInteraction orig, PinnedGrenade self, FVRViveHand hand)
		{
			self.GetComponent<OneHandedPinnedGrenade>().UpdateInteraction_Hooked(self, hand);
			orig(self, hand);
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
			self.gameObject.AddComponent<OneHandedCappedGrenade>().Hook();
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

		private void AudioImpactController_ProcessCollision(On.FistVR.AudioImpactController.orig_ProcessCollision orig, AudioImpactController self, Collision col)
		{
			orig(self, col);
			float impactMagnitude = col.relativeVelocity.magnitude;

			if (impactMagnitude < self.HitThreshold_Ignore)
            {
				Debug.Log("Impact magnitude is " + impactMagnitude + ", ignoring collision");
            }
			else if (impactMagnitude > self.HitThreshold_High)
            {
				Debug.Log("Impact magnitude is " + impactMagnitude + ", high impact");
			}
			else if (impactMagnitude > self.HitThreshold_Medium)
			{
				Debug.Log("Impact magnitude is " + impactMagnitude + ", medium impact");
			}
		}
	}
}