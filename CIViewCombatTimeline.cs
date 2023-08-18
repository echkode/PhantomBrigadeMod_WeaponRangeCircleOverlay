// Copyright (c) 2023 EchKode
// SPDX-License-Identifier: BSD-3-Clause

using System.Collections.Generic;

using HarmonyLib;

using PhantomBrigade;
using PhantomBrigade.Combat.Systems;
using PhantomBrigade.Data;
using PhantomBrigade.Input.Components;
using PBCIViewCombatTimeline = CIViewCombatTimeline;
using PBWorldUICombat = WorldUICombat;

using UnityEngine;

namespace EchKode.PBMods.WeaponRangeCircleOverlay
{
	static class CIViewCombatTimeline
	{
		internal const int CircleOverlayID = -2;

		private static Dictionary<int, CIHelperTimelineAction> helpersActionsPlanned;

		internal static int DragActionID = IDUtility.invalidID;

		internal static void Initialize()
		{
			var t = Traverse.Create(PBCIViewCombatTimeline.ins);
			helpersActionsPlanned = t.Field<Dictionary<int, CIHelperTimelineAction>>(nameof(helpersActionsPlanned)).Value;
		}

		internal static void OnActionDrag(object callbackAsObject)
		{
			if (!CombatUIUtility.IsCombatUISafe())
			{
				return;
			}
			if (Contexts.sharedInstance.input.combatUIMode.e != CombatUIModes.Unit_Selection)
			{
				return;
			}
			if (!(callbackAsObject is UICallback uiCallback))
			{
				return;
			}

			int argumentInt = uiCallback.argumentInt;
			if (!helpersActionsPlanned.ContainsKey(argumentInt))
			{
				return;
			}
			var (ok, action) = IsAttackAction(argumentInt);
			if (!ok)
			{
				return;
			}

			if (ModLink.Settings.IsLoggingEnabled(ModLink.ModSettings.LoggingFlag.OnDrag))
			{
				Debug.LogFormat(
					"Mod {0} ({1}) Drag start on action | action ID: {2}",
					ModLink.modIndex,
					ModLink.modID,
					action.id.id);
			}

			ShowRangeOverlay(action);
		}

		internal static void OnActionDragEnd(object callbackAsObject)
		{
			if (!(callbackAsObject is UICallback uiCallback))
			{
				return;
			}

			int argumentInt = uiCallback.argumentInt;
			if (!helpersActionsPlanned.ContainsKey(argumentInt))
			{
				return;
			}
			var (ok, action) = IsAttackAction(argumentInt);
			if (!ok)
			{
				return;
			}

			if (ModLink.Settings.IsLoggingEnabled(ModLink.ModSettings.LoggingFlag.OnDrag))
			{
				Debug.LogFormat(
					"Mod {0} ({1}) Drag end on action | action ID: {2}",
					ModLink.modIndex,
					ModLink.modID,
					action.id.id);
			}

			HideRangeOverlay(action);
		}

		static void ShowRangeOverlay(ActionEntity action)
		{
			var (ok, combatEntity, partInUnit) = GetComponentsFromAction(action);
			if (!ok)
			{
				return;
			}

			PBWorldUICombat.OnRangeDisplay(CircleOverlayID, 0, combatEntity.projectedPosition.v, partInUnit);
			DragActionID = action.id.id;
		}

		static void HideRangeOverlay(ActionEntity action)
		{
			var (ok, _, _) = GetComponentsFromAction(action);
			if (!ok)
			{
				return;
			}

			DragActionID = IDUtility.invalidID;
			PBWorldUICombat.OnRangeEnd(CircleOverlayID);
			ActionProjectionSystem.ForceNextUpdate();
		}

		static (bool, ActionEntity) IsAttackAction(int actionID)
		{
			var action = IDUtility.GetActionEntity(actionID);
			if (action == null)
			{
				return (false, null);
			}
			if (action.isMovementExtrapolated)
			{
				return (false, null);
			}
			if (!action.hasStartTime)
			{
				return (false, null);
			}
			if (!action.hasDuration)
			{
				return (false, null);
			}
			if (!action.hasActiveEquipmentPart)
			{
				return (false, null);
			}

			var activePart = IDUtility.GetEquipmentEntity(action.activeEquipmentPart.equipmentID);
			if (activePart == null)
			{
				return (false, null);
			}
			if (!activePart.hasPrimaryActivationSubsystem)
			{
				return (false, null);
			}

			if (IsShieldAction(action))
			{
				return (false, null);
			}

			return (true, action);
		}

		static bool IsShieldAction(ActionEntity action)
		{
			var (dataOK, actionData) = GetActionData(action);
			if (!dataOK)
			{
				if (ModLink.Settings.IsLoggingEnabled(ModLink.ModSettings.LoggingFlag.ActionHook))
				{
					Debug.LogFormat(
						"Mod {0} ({1}) is shield action | action data not OK | action ID: {2}",
						ModLink.modIndex,
						ModLink.modID,
						action.id.id);
				}
				return false;
			}
			var (partOK, _, part) = GetPartInUnit(actionData);
			if (!partOK)
			{
				if (ModLink.Settings.IsLoggingEnabled(ModLink.ModSettings.LoggingFlag.ActionHook))
				{
					Debug.LogFormat(
						"Mod {0} ({1}) is shield action | part not found on unit | action ID: {2}",
						ModLink.modIndex,
						ModLink.modID,
						action.id.id);
				}
				return false;
			}

			if (ModLink.Settings.IsLoggingEnabled(ModLink.ModSettings.LoggingFlag.ActionHook))
			{
				Debug.LogFormat(
					"Mod {0} ({1}) is shield action | action ID: {2} | tags: {3}",
					ModLink.modIndex,
					ModLink.modID,
					action.id.id,
					string.Join(",", part.tagCache.tags));
			}

			return part.tagCache.tags.Contains("type_defensive");
		}

		static (bool, DataContainerAction) GetActionData(ActionEntity action)
		{
			if (!action.hasDataLinkAction)
			{
				return (false, null);
			}

			var actionData = action.dataLinkAction.data;
			if (actionData == null)
			{
				return (false, null);
			}
			if (actionData.dataEquipment == null)
			{
				return (false, null);
			}
			if (!actionData.dataEquipment.partUsed)
			{
				return (false, null);
			}
			if (actionData.dataEquipment.partSocket == "core")
			{
				return (false, null);
			}

			return (true, actionData);
		}

		static (bool, CombatEntity, EquipmentEntity)
			GetComponentsFromAction(ActionEntity action)
		{
			var (dataOK, actionData) = GetActionData(action);
			if (!dataOK)
			{
				return (false, null, null);
			}

			var (partOK, combatEntity, partInUnit) = GetPartInUnit(actionData);
			if (!partOK)
			{
				return (false, null, null);
			}

			return (true, combatEntity, partInUnit);
		}

		static (bool, CombatEntity, EquipmentEntity)
			GetPartInUnit(DataContainerAction actionData, CombatEntity actionOwner = null)
		{
			if (actionOwner == null)
			{
				var selectedCombatUnitID = Contexts.sharedInstance.combat.hasUnitSelected
					? Contexts.sharedInstance.combat.unitSelected.id
					: IDUtility.invalidID;
				actionOwner = IDUtility.GetCombatEntity(selectedCombatUnitID);
			}
			if (actionOwner == null)
			{
				return (false, null, null);
			}

			var unit = IDUtility.GetLinkedPersistentEntity(actionOwner);
			if (unit == null)
			{
				return (false, null, null);
			}

			var partInUnit = EquipmentUtility.GetPartInUnit(unit, actionData.dataEquipment.partSocket);
			if (partInUnit == null)
			{
				return (false, null, null);
			}

			return (true, actionOwner, partInUnit);
		}
	}
}
