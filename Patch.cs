using System.Collections.Generic;

using HarmonyLib;

using PBCIViewCombatTimeline = CIViewCombatTimeline;
using PBWorldUICombat = WorldUICombat;

namespace EchKode.PBMods.WeaponRangeCircleOverlay
{
	[HarmonyPatch]
	static class Patch
	{
		[HarmonyPatch(typeof(PBCIViewCombatTimeline), "OnActionDrag")]
		[HarmonyPostfix]
		static void Civct_OnActionDragPostfix(object callbackAsObject)
		{
			CIViewCombatTimeline.OnActionDrag(callbackAsObject);
		}

		[HarmonyPatch(typeof(PBCIViewCombatTimeline), "OnActionDragEnd")]
		[HarmonyPostfix]
		static void Civct_OnActionDragEndPostfix(object callbackAsObject)
		{
			CIViewCombatTimeline.OnActionDragEnd(callbackAsObject);
		}

		[HarmonyPatch(typeof(PBWorldUICombat), "OnRangeEnd", new System.Type[] { typeof(HashSet<int>), typeof(int) })]
		[HarmonyPrefix]
		static void Wuic_OnRangeEndHashPrefix()
		{
			WorldUICombat.OnRangeEndPrefix();
		}

		[HarmonyPatch(typeof(PBWorldUICombat), "OnRangeEnd", new System.Type[] { typeof(int), typeof(int) })]
		[HarmonyPrefix]
		static void Wuic_OnRangeEndPrefix()
		{
			WorldUICombat.OnRangeEndPrefix();
		}

		[HarmonyPatch(typeof(PBWorldUICombat), "OnRangeEnd", new System.Type[] { typeof(HashSet<int>), typeof(int) })]
		[HarmonyPostfix]
		static void Wuic_OnRangeEndHashPostfix()
		{
			WorldUICombat.OnRangeEndPostfix();
		}

		[HarmonyPatch(typeof(PBWorldUICombat), "OnRangeEnd", new System.Type[] { typeof(int), typeof(int) })]
		[HarmonyPostfix]
		static void Wuic_OnRangeEndPostfix()
		{
			WorldUICombat.OnRangeEndPostfix();
		}
	}
}
