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
	/// --------------------------------TODO------------------------------
	/// - MISSING BASE METHOD CALLS:
	/// Derringer.FVRUpdate()
	/// 
	/// GBeamer.FVRUpdate() (Method missing entirely, would have to use FVRFixedUpdate())
	/// LeverActionFirearm.FVRUpdate() (ditto)
	/// 
	/// M72.FVRUpdate() & M72.FVRFixedUpdate() (both missing)
	/// PotatoGun.FVRUpdate() & PotatoGun.FVRFixedUpdate() (ditto)
	/// RPG7.FVRUpdate() & RPG7.FVRFixedUpdate() (ditto)
	/// SimpleLauncher.FVRUpdate() & SimpleLauncher.FVRFixedUpdate() (ditto)
	/// SingleActionRevolver.FVRFixedUpdate() (ditto)
	/// 
	/// FVRFireArms to convert:
	/// 
	/// HIGH PRIORITY
	/// - (Faster operating speed for) TubeFedShotgun
	/// - (Less clunky operation for) LeverActionFirearm
	/// - Derringer (Needs base method call in FVRUpdate)
	/// 
	/// LOW PRIORITY
	/// - SosigWeaponPlayerInterface
	/// - Airgun
	/// - SimpleLauncher (and Whizzbanger)
	/// - CarlGustaf
	/// - SimpleLauncher2
	/// - M72
	/// - RemoteMissileLauncher
	/// - Minigun
	/// - StingerLauncher
	/// - RPG7
	/// - Airgun
	/// - GBeamer
	/// - SRG
	/// - FlameThrower
	/// - sblp
	/// - MeatNailer
	/// - PotatoGun
	/// - RailTater
	/// - HCB
	/// - Girandoni
	/// - FlintlockWeapon
	/// - GrappleGun
	/// - LAPD2019
	/// - MF2_RL
	/// - RGM40
	/// - SRG 
	/// </summary>



	[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
	[BepInProcess("h3vr.exe")]
	public class AccessibilityOptionsBase : BaseUnityPlugin
	{
        #region config entries
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

		//Grip angle override
		public static ConfigEntry<bool> gripAngleOverrideEnabled;
		public static ConfigEntry<float> overrideGripAngle;

		//Quality of life
		public static ConfigEntry<bool> overrideRecoil;

		public static ConfigEntry<bool> oneHandedHoverBench;

		//Miscellaneous weapon tweaks
		public static ConfigEntry<bool> miscWeaponTweaksEnabled;

		public static ConfigEntry<bool> oneHandedSAREnabled;

		public static ConfigEntry<bool> oneHandedGrenadesEnabled;
		public static ConfigEntry<float> pinnedGrenadePinPullDuration;

		public static ConfigEntry<bool> oneHandedPumpReleaseEnabled;
        #endregion

        private const string ASSET_BUNDLE_NAME = "accessibilityoptions";
		public static AssetBundle pointerAssetBundle;

        public AccessibilityOptionsBase()
		{
			string pluginPath = Path.GetDirectoryName(Info.Location);
			pointerAssetBundle = AssetBundle.LoadFromFile(Path.Combine(pluginPath, ASSET_BUNDLE_NAME));

			//WRIST MENU CONFIG-------------------------------------------------------------------------------------
			oneHandedWristMenuEnabled = Config.Bind("Wrist Menu",
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

			//WEAPON POSE LOCKING CONFIG-------------------------------------------------------------------------------------
			weaponPoseLockingEnabled = Config.Bind("Weapon Pose Locking",
												   "Enable Weapon Pose Locking",
												   true,
												   "Lock weapons in mid-air by holding down the trigger on a safe or empty chamber");

			weaponPoseLockingTriggerDuration = Config.Bind("Weapon Pose Locking",
														   "Pose Locking Trigger Hold Duration",
														   0.35f,
														   "How long the trigger needs to be held down for the weapon to get locked");


			//QUALITY OF LIFE CONFIG-------------------------------------------------------------------------------------
			overrideRecoil = Config.Bind("Quality Of Life",
										 "Force Two-Handed Recoil",
										 true,
										 "Force weapons to always recoil like they're being two-handed");

            oneHandedHoverBench = Config.Bind("Quality Of Life",
                                              "Enable One-Handed Hoverbench",
                                              true,
                                              "Allow locking items into the Hoverbench without requiring two hands");

			gripAngleOverrideEnabled = Config.Bind("Quality Of Life",
												   "Enable Grip Angle Override",
												   true,
												   "Override the angle weapons are held at, to make one-handed aiming easier");

			overrideGripAngle = Config.Bind("Quality Of Life",
											"Override Grip Angle",
											-75f,
											"Determines the up/down angle of a weapon's hold pose");

			//OPTION PANEL CONFIG
			lockPanelsAutomatically = Config.Bind("Quality Of Life",
												  "Automatically Lock Option Panels",
												  true,
												  "Option panels lock automatically when spawned");

			autoLockPanelWhitelist = Config.Bind("Quality Of Life",
												 "(Advanced) Auto-Locking Panel Whitelist",
												 "OptionsPanel_Screenmanager AmmoSpawnerV2",
												 "Add a panel class name here (case-sensitive, separated by a space) to lock it automatically upon spawning");

			//MISCELLANEOUS WEAPON TWEAK CONFIG-------------------------------------------------------------------------------------
			miscWeaponTweaksEnabled = Config.Bind("Miscellaneous Weapon Tweaks",
												  "Enable Miscellaneous One-Handed Weapon Tweaks",
												  true,
												  "Toggle all miscellaneous weapon tweaks on or off");

			oneHandedSAREnabled = Config.Bind("Miscellaneous Weapon Tweaks",
											  "Enable One-Handed Single-Action Revolvers",
											  true,
											  "Automatically advance single-action revolver cylinders upon inserting a round");

			oneHandedGrenadesEnabled = Config.Bind("Miscellaneous Weapon Tweaks",
												   "Enable One-Handed Grenades",
												   true,
												   "Pull pins by holding the touchpad or AX face buttons");

			pinnedGrenadePinPullDuration = Config.Bind("Miscellaneous Weapon Tweaks",
												 "Pinned Grenade Pin Pull Duration",
												 0.5f,
												 "How long (in seconds) the button needs to be held down to pull out a grenade pin (set to 0 for instant)");

			oneHandedPumpReleaseEnabled = Config.Bind("Miscellaneous Weapon Tweaks",
													  "Enable One-Handed Pump Release",
													  true,
													  "Unlock pump-action weapon pumps by pulling the trigger while holding the pump");
		}

		OneHandedWristMenu oneHandedWristMenu;
		AutoLockingPanels autoLockingPanels;
		WeaponPoseLock weaponPoseLock;
		GripAngleOverride gripAngleOverride;
		OneHandedMiscWeaponTweaks oneHandedMiscWeaponTweaks;

		void Awake()
        {
			//Wrist menu
			if (oneHandedWristMenuEnabled.Value)
			{
				oneHandedWristMenu = gameObject.AddComponent<OneHandedWristMenu>();
			}

			//Option panels
			if (lockPanelsAutomatically.Value)
            {
				autoLockingPanels = gameObject.AddComponent<AutoLockingPanels>();
            }

			//Weapon pose locking
			if (weaponPoseLockingEnabled.Value)
            {
				weaponPoseLock = gameObject.AddComponent<WeaponPoseLock>();
            }

			//Grip angle override
			if (gripAngleOverrideEnabled.Value)
            {
				gripAngleOverride = gameObject.AddComponent<GripAngleOverride>();
            }

			//Quality of life
			if (overrideRecoil.Value)
			{
				On.FistVR.FVRFireArm.Recoil += FVRFireArm_Recoil;
			}

			if (oneHandedHoverBench.Value)
            {
                On.FistVR.FVRPivotLocker.TryToLockObject += FVRPivotLocker_TryToLockObject;
            }

			//Miscellaneous weapon tweaks
			if (miscWeaponTweaksEnabled.Value)
			{
				oneHandedMiscWeaponTweaks = gameObject.AddComponent<OneHandedMiscWeaponTweaks>();
			}
		}

        #region recoil override
        private void FVRFireArm_Recoil(On.FistVR.FVRFireArm.orig_Recoil orig, FVRFireArm self, bool twoHandStabilized, bool foregripStabilized, bool shoulderStabilized, FVRFireArmRecoilProfile overrideprofile, float VerticalRecoilMult)
		{
			orig(self, true, true, shoulderStabilized, overrideprofile, VerticalRecoilMult);
		}
		#endregion

		#region one-handed hoverbench
		private void FVRPivotLocker_TryToLockObject(On.FistVR.FVRPivotLocker.orig_TryToLockObject orig, FVRPivotLocker self)
		{
			orig(self);
			if (self.m_obj != null) return;

			Collider[] overlappingObjs = Physics.OverlapBox(self.TestingBox.position, self.TestingBox.localScale / 1.9f, self.TestingBox.rotation);
			for (int i = 0; i < overlappingObjs.Length; i++)
			{
				FVRPhysicalObject objectToLock = overlappingObjs[i].GetComponent<FVRPhysicalObject>();
				if (objectToLock != null)
				{
					if (weaponPoseLock.currentlyLockedWeapon != null && weaponPoseLock.currentlyLockedWeapon.thisFirearm == objectToLock) weaponPoseLock.currentlyLockedWeapon.UnlockWeapon();
					self.LockObject(objectToLock);
					return;
				}
			}
		}
		#endregion
	}
}