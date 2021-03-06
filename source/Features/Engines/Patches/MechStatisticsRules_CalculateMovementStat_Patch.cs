﻿using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateMovementStat")]
    public static class MechStatisticsRules_CalculateMovementStat_Patch
    {
        private static MechDef def;

        // disable jump jet calculations
        private static readonly MechComponentRef[] Empty = { };

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(MovementCapabilitiesDef), "get_MaxSprintDistance"),
                    AccessTools.Method(typeof(MechStatisticsRules_CalculateMovementStat_Patch), "OverrideMaxSprintDistance")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(MechDef), "get_Inventory"),
                    AccessTools.Method(typeof(MechStatisticsRules_CalculateMovementStat_Patch), "OverrideInventory")
                );
        }

        public static void Prefix(MechDef mechDef)
        {
            try
            {
                def = mechDef;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        public static void Postfix()
        {
            try
            {
                def = null;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        public static float OverrideMaxSprintDistance(this MovementCapabilitiesDef @this)
        {
            try
            {
                var movement = def?.GetEngineMovement();
                if (movement != null)
                {
                    return movement.RunSpeed;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return @this.MaxSprintDistance;
        }

        public static MechComponentRef[] OverrideInventory(this MechDef @this)
        {
            return Empty;
        }
    }
}