﻿using System.IO;

using UnityEngine;

namespace EchKode.PBMods.WeaponRangeCircleOverlay
{
	partial class ModLink
	{
		internal sealed class ModSettings
		{
			[System.Flags]
			internal enum LoggingFlag
			{
				None = 0,
				System = 0x1,
				ActionHook = 0x2,
				OnDrag = 0x4,
				All = 0xFF,
			}

#pragma warning disable CS0649
			public LoggingFlag logging;
#pragma warning restore CS0649

			internal bool IsLoggingEnabled(LoggingFlag flag) => (logging & flag) == flag;
		}

		internal static ModSettings Settings;

		internal static void LoadSettings()
		{
			var settingsPath = Path.Combine(modPath, "settings.yaml");
			Settings = UtilitiesYAML.ReadFromFile<ModSettings>(settingsPath, false);
			if (Settings == null)
			{
				Settings = new ModSettings();

				Debug.LogFormat(
					"Mod {0} ({1}) no settings file found, using defaults | path: {2}",
					modIndex,
					modID,
					settingsPath);
			}
		}
	}
}
