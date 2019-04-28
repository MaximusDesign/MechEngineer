﻿using System;
using BattleTech.UI;
using Harmony;
using MechEngineer.Features.Engine.StaticHandler;

namespace MechEngineer.Features.Engine.Patches
{
    [HarmonyPatch(typeof(MechLabInventoryWidget), "RefreshJumpJetOptions")]
    public static class MechLabInventoryWidget_RefreshJumpJetOptions_Patch
    {
        // hide incompatible engines
        public static void Postfix(MechLabInventoryWidget __instance, float ___mechTonnage)
        {
            try
            {
                EngineMisc.RefreshAvailability(__instance, ___mechTonnage);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}