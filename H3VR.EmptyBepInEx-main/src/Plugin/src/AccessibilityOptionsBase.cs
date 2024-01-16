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
	/// - Fix wrist menu (hand check needs to check for OTHER hand)
	/// - Make grenade pins pullable with a button/touchpad hold
	/// </summary>

	[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
	[BepInProcess("h3vr.exe")]
	public class AccessibilityOptionsBase : BaseUnityPlugin
	{
		public static ConfigEntry<bool> oneHandedWristMenuEnabled;
		public static ConfigEntry<float> verticalPointerOffset;
		public static ConfigEntry<Color> pointerColor;
		public static ConfigEntry<float> pointerScale;

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
												 "How long (in seconds) the button needs to be held down to pull out a grenade pin");
		}

		OneHandedWristMenu oneHandedWristMenu;
		OneHandedSingleActionRevolver oneHandedSingleActionRevolver;

		void Awake()
        {
			if (oneHandedWristMenuEnabled.Value)
			{
				oneHandedWristMenu = gameObject.AddComponent<OneHandedWristMenu>();
				oneHandedWristMenu.Hook(verticalPointerOffset.Value, pointerColor.Value, pointerScale.Value, bundle);
			}

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
	}
}