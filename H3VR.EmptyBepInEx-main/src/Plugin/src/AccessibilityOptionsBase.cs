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
	/// - Upload to Git!
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
		public static ConfigEntry<float> grenadePinPullDuration;

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

			grenadePinPullDuration = Config.Bind("Grenades",
												 "(WIP) Grenade Pin Pull Duration",
												 1f,
												 "How long (in seconds) the button needs to be held down to pull out a grenade pin");
		}

		OneHandedWristMenu oneHandedWristMenu;
		OneHandedSingleActionRevolver oneHandedSingleActionRevolver;
		OneHandedGrenades oneHandedGrenades;

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
				oneHandedGrenades = gameObject.AddComponent<OneHandedGrenades>();
            }

        }
	}
}