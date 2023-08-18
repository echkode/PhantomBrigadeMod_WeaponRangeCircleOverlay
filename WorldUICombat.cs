// Copyright (c) 2023 EchKode
// SPDX-License-Identifier: BSD-3-Clause

using HarmonyLib;

using PhantomBrigade;
using PBWorldUICombat = WorldUICombat;

using UnityEngine;

namespace EchKode.PBMods.WeaponRangeCircleOverlay
{
	static class WorldUICombat
	{
		private static PBWorldUICombat instance;
		private static WorldUICombatRangeHelper combatRangeHelper;

		internal static void OnRangeEndPrefix()
		{
			if (instance == null && !Initialize())
			{
				return;
			}
			if (CIViewCombatTimeline.DragActionID == IDUtility.invalidID)
			{
				return;
			}

			var rangeLink = instance.rangeLink;
			if (rangeLink?.instances == null)
			{
				return;
			}
			if (!rangeLink.instances.ContainsKey(CIViewCombatTimeline.CircleOverlayID))
			{
				return;
			}

			combatRangeHelper = rangeLink.instances[CIViewCombatTimeline.CircleOverlayID];
			rangeLink.instances.Remove(CIViewCombatTimeline.CircleOverlayID);
		}

		internal static void OnRangeEndPostfix()
		{
			if (instance == null && !Initialize())
			{
				return;
			}
			if (CIViewCombatTimeline.DragActionID == IDUtility.invalidID)
			{
				return;
			}
			if (combatRangeHelper == null)
			{
				return;
			}

			var rangeLink = instance.rangeLink;
			if (rangeLink?.instances == null)
			{
				return;
			}
			if (rangeLink.instances.ContainsKey(CIViewCombatTimeline.CircleOverlayID))
			{
				return;
			}

			rangeLink.instances.Add(CIViewCombatTimeline.CircleOverlayID, combatRangeHelper);
			combatRangeHelper = null;
		}

		static bool Initialize()
		{
			var t = Traverse.Create<PBWorldUICombat>();
			instance = t.Field<PBWorldUICombat>("ins").Value;

			if (instance == null)
			{
				Debug.LogFormat(
					"Mod {0} ({1}) Initialize | WorldUICombat instance is null",
					ModLink.modIndex,
					ModLink.modID);
				return false;
			}

			return true;
		}
	}
}
